using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace MakePolicyFromApp;

public class AnalyzeArguments
{
    public string InputFile { get; set; } = String.Empty;

    public string Extractor { get; set; } = "universal";

    public static Command CreateCommand()
    {
        var c = new Command("analyze", "Analyzes the installer and prints the content to stdout.")
        {
            TreatUnmatchedTokensAsErrors = true,
        };

        c.AddOption(
            new Option<string>(new[] { "-i", "--input", "--input-file" }, () => string.Empty)
            {
                IsRequired = true, Description = "The installer that will be analyzed."
            }
        );

        c.AddOption(
            new Option<string>(new[] { "-e", "--extractor" }, () => "universal")
            {
                Description =
                    $"The extractor that is used to extract the input file. Options are 'universal' and 'innoextract'. If not specified, 'universal' is used.",
                IsRequired = false,
            }
        );

        return c;
    }
}
