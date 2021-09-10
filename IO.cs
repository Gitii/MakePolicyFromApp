using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakePolicyFromApp
{
    static class IO
    {
        public static async Task CopyToDirectoryAsync(string inputFile, string outputDirectory)
        {
            var outputFile = Path.Join(outputDirectory, Path.GetFileName(inputFile));
            await CopyFileAsync(inputFile, outputFile);
        }

        public static async Task CopyFileAsync(string sourcePath, string destinationPath)
        {
            await using Stream source = File.OpenRead(sourcePath);
            await using Stream destination = File.Create(destinationPath);
            await source.CopyToAsync(destination);
        }
    }
}