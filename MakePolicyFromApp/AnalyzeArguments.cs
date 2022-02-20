using CommandLine;
using System;

namespace MakePolicyFromApp;

[Verb("analyze", HelpText = "Analyzes the installer and prints the content to stdout.")]
public class AnalyzeArguments
{
    [Option('i', "input", Required = true, HelpText = "The installer that will be analyzed.")]
    public string InputFile { get; set; } = String.Empty;

    [Option(
        'e',
        "extractor",
        Required = false,
        HelpText = $"The extractor that is used to extract the input file. Options are 'universal' and 'innoextract'. If not specified, 'universal' is used."
    )]
    public string Extractor { get; set; } = "universal";
}
