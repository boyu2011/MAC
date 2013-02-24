using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using CommandLine.Text;

namespace MAC
{
    partial class Program
    {
        class Options
        {
        
            [Option("infile", Required = true,
                HelpText = "Input file to be processed.")]
            public string InputFile { get; set; }

            [Option("outfile",
                HelpText = "Output file to be processed.")]
            public string OutputFile { get; set; }

            [Option("create",
                HelpText = "Create a pocket")]
            public bool Create { get; set; }

            [Option("authenticate",
                HelpText = "Authenticate a pocket")]
            public bool Authenticate { get; set; }

            [Option("sha256",
                HelpText = "sha256")]
            public bool Sha256 { get; set; }

            [Option("sha512",
                HelpText = "sha512")]
            public bool Sha512 { get; set; }

            [Option("key", Required = true,
                HelpText = "Key to be processed.")]
            public string Key { get; set; }

            // omitting long name, default --verbose
            [Option(DefaultValue = true,
              HelpText = "Prints all messages to standard output.")]
            public bool Verbose { get; set; }

            [ParserState]
            public IParserState LastParserState { get; set; }

            [HelpOption]
            public string GetUsage()
            {
                return HelpText.AutoBuild(this,
                  (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
            }
        }
    }
}
