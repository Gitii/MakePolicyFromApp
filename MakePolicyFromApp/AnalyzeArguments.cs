using CommandLine;
using System;

namespace MakePolicyFromApp;

[Verb("analyze", HelpText = "Analyzes the installer and prints the content to stdout.")]
public class AnalyzeArguments
{
    [Option('i', "input", Required = true, HelpText = "The installer that will be analyzed.")]
    public string InputFile { get; set; } = String.Empty;
}
