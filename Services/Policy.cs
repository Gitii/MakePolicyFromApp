using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;

namespace MakePolicyFromApp.Services
{
    public interface IPolicy
    {
        public Task<string> Generate(string directory);

        Task<string> MakePolicyHumanReadable(string policyContent, string contextDirectory,
            string contextName);
    }

    class Policy : IPolicy
    {
        private IPowershell powershell;
        private ILogger<Policy> logger;

        private static Regex NOT_ID_REGEX = new Regex("[^_A-Z0-9]+");

        public Policy(IPowershell powershell, ILogger<Policy> logger)
        {
            this.powershell = powershell;
            this.logger = logger;
        }

        public async Task<string> Generate(string directory)
        {
            var policyFile = Path.GetTempFileName();

            try
            {
                logger.LogInformation("Generating policy from " + directory);

                await powershell.ExecuteCommand("New-CIPolicy",
                    new Dictionary<string, object>()
                    {
                        { "ScanPath", directory },
                        { "Level", "Publisher" },
                        { "Fallback", "Hash" },
                        { "FilePath", policyFile },
                        { "UserPEs", IPowershell.VOID }
                    }, new[] { "ConfigCI" });

                return await File.ReadAllTextAsync(policyFile);
            }
            finally
            {
                File.Delete(policyFile);
            }
        }

        public async Task<string> MakePolicyHumanReadable(string policyContent, string contextDirectory,
            string contextName)
        {
            XDocument policyXml = XDocument.Parse(policyContent);

            var siPolicy = policyXml.Root!;
            var ns = siPolicy.Name.NamespaceName;
            var fileRules = siPolicy.Element(XName.Get("FileRules", ns))!;
            var fileRuleRefs = siPolicy.Descendants(XName.Get("FileRuleRef", ns));
            var signingScenarios = siPolicy.Descendants(XName.Get("SigningScenario", ns));

            var ids = GetBetterIds(fileRules, contextDirectory);

            UpdateFriendlyNames(fileRules, contextDirectory, contextName);
            UpdateIds(ids, fileRules, fileRuleRefs);
            UpdateScenarios(signingScenarios, contextName);

            return policyXml.ToString();
        }

        private void UpdateScenarios(IEnumerable<XElement> signingScenarios, string contextName)
        {
            foreach (var friendlyNameAttribute in signingScenarios.Attributes()
                .Where((a) => a.Name.LocalName == "FriendlyName"))
            {
                friendlyNameAttribute.Value =
                    $"Generated policy for \"{contextName}\" ({DateTime.Today.ToShortDateString()})";
            }
        }

        private Dictionary<string, string> GetBetterIds(XElement fileRuleRoot, string contextDirectory)
        {
            var ns = fileRuleRoot.Name.NamespaceName;

            var allowFileRule = fileRuleRoot.Descendants(XName.Get("Allow", ns)).ToArray();

            return allowFileRule.Attributes(XName.Get("ID")).ToDictionary(
                (a) => a.Value,
                (a) => GetBetterId(a.Value,
                    a.Parent!.Attribute(XName.Get("FriendlyName"))?.Value));

            string GetBetterId(string origValue, string? friendlyNameValue)
            {
                if (friendlyNameValue == null)
                {
                    return origValue;
                }

                friendlyNameValue = MakeRelative(contextDirectory, friendlyNameValue).ToUpper();

                var id = "ID_ALLOW_" + NOT_ID_REGEX.Replace(friendlyNameValue, "_");
                if (id.Length > 100)
                {
                    return id.Substring(0, 100);
                }
                else
                {
                    return id;
                }
            }
        }

        private void UpdateIds(Dictionary<string, string> idMap, XElement fileRuleRoot,
            IEnumerable<XElement> fileRuleRefs)
        {
            var ns = fileRuleRoot.Name.NamespaceName;

            var allowFileRule = fileRuleRoot.Descendants(XName.Get("Allow", ns)).ToArray();

            foreach (XAttribute attribute in allowFileRule.Attributes(XName.Get("ID")))
            {
                if (idMap.ContainsKey(attribute.Value))
                {
                    attribute.Value = idMap[attribute.Value];
                }
            }

            foreach (XAttribute attribute in fileRuleRefs.Attributes(XName.Get("RuleID")))
            {
                if (idMap.ContainsKey(attribute.Value))
                {
                    attribute.Value = idMap[attribute.Value];
                }
            }
        }

        private void UpdateFriendlyNames(XElement fileRuleRoot, string contextDirectory, string contextName)
        {
            logger.LogInformation("Updating rule names...");

            foreach (var allowFileRule in fileRuleRoot.Descendants(XName.Get("Allow", fileRuleRoot.Name.NamespaceName)))
            {
                var friendlyNameAttr =
                    allowFileRule.Attribute(XName.Get("FriendlyName"));
                if (friendlyNameAttr != null &&
                    friendlyNameAttr.Value.StartsWith(contextDirectory, StringComparison.OrdinalIgnoreCase))
                {
                    var relativePath = MakeRelative(contextDirectory, friendlyNameAttr.Value);
                    var friendlyName = $"[{contextName}]/{relativePath.Replace(Path.DirectorySeparatorChar, '/')}";

                    logger.LogDebug($"Changing name from \"{friendlyNameAttr.Value}\" to \"{friendlyName}\"");

                    friendlyNameAttr.Value = friendlyName;
                }
                else
                {
                    logger.LogDebug($"Skipping attribute {allowFileRule.ToString(SaveOptions.DisableFormatting)}");
                }
            }
        }

        private string MakeRelative(string contextDirectory, string absolutePath)
        {
            return Path.GetRelativePath(contextDirectory, absolutePath)
                .Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
    }
}