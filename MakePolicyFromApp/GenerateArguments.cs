﻿using CommandLine;
using System;

namespace MakePolicyFromApp;

[Verb("generate", HelpText = "Analyzes the installer and generates a WDAC policy file.")]
class GenerateArguments
{
    [Option('i', "input", Required = true, HelpText = "The installer that will be analyzed.")]
    public string InputFile { get; set; } = String.Empty;

    [Option(
        'n',
        "name",
        Required = false,
        HelpText = "Friendly name for policy. If not set, the name will be derived from the installer."
    )]
    public string? ContextName { get; set; }

    [Option(
        'o',
        "output",
        Required = false,
        HelpText = "The output file path to the policy file. If not set, the content will be printed to stdout."
    )]
    public string OutputFile { get; set; } = String.Empty;

    [Option(
        'e',
        "extractor",
        Required = false,
        HelpText = $"The extractor that is used to extract the input file. Options are 'universal' and 'innoextract'. If not specified, 'universal' is used."
    )]
    public string Extractor { get; set; } = "universal";
}
