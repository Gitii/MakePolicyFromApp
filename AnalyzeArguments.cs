using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakePolicyFromApp
{
    [Verb("analyze", HelpText = "Analyzes the installer and prints the content to stdout.")]
    public class AnalyzeArguments
    {
        [Option('i', "input", Required = true, HelpText = "The installer that will be analyzed.")]
        public string InputFile { get; set; }
    }
}