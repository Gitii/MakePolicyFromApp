using System;
using System.CommandLine;

namespace MakePolicyFromApp;

class GenerateArguments
{
    public string InputFile { get; set; } = String.Empty;

    public string ContextName { get; set; } = String.Empty;

    public string OutputFile { get; set; } = String.Empty;

    public string Extractor { get; set; } = "universal";

    public static Command CreateCommand()
    {
        var c = new Command("generate", "Analyzes the installer and generates a WDAC policy file.");

        c.AddOption(
            new Option<string>(new[] { "-i", "--input", "--input-file" }, () => String.Empty)
            {
                Description = "The installer that will be analyzed.", IsRequired = true
            }
        );

        c.AddOption(
            new Option<string>(new[] { "-n", "--name", "--context-name" }, () => String.Empty)
            {
                Description =
                    "Friendly name for policy. If not set, the name will be derived from the installer.",
                IsRequired = false
            }
        );

        c.AddOption(
            new Option<string>(new[] { "-o", "--output", "--output-file" }, () => String.Empty)
            {
                Description =
                    "The output file path to the policy file. If not set, the content will be printed to stdout.",
                IsRequired = false
            }
        );

        c.AddOption(
            new Option<string>(new[] { "-e", "--extractor" }, () => "universal")
            {
                Description =
                    $"The extractor that is used to extract the input file. Options are 'universal' and 'innoextract'. If not specified, 'universal' is used.",
                IsRequired = false
            }
        );

        return c;
    }
}
