using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakePolicyFromApp
{
    [Verb("generate", HelpText = "Analyzes the installer and generates a report.")]
    class GenerateArguments
    {
        [Option('i', "input", Required = true, HelpText = "The installer that will be analyzed.")]
        public string InputFile { get; set; }

        [Option('n', "name", Required = false,
            HelpText = "Friendly name for policy: If not set, the name will be derived from the installer.")]
        public string? ContextName { get; set; }

        [Option('o', "output", Required = true,
            HelpText = "The output file path to the policy file. If not set, the content will be printed to stdout.")]
        public string OutputFile { get; set; }
    }
}