using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace WordFinder;

public sealed partial class WordFinderApp
{
    public sealed class WordFinderConfig : CommandSettings
    {
        [Description("Path to word list file (default: words.txt)")]
        [CommandArgument(2, "[wordListPath]")]
        [DefaultValue("words.txt")]
        public string WordListPath { get; set; }

        [Description("The characters")]
        [CommandArgument(0, "[characters]")]
        public string? Characters { get; init; }

        [Description("Size of character box (WidthxHeight)")]
        [CommandArgument(1, "[size]")]
        public string? Size { get; init; }

        // [Description("Search for words along diagonals")]
        // [CommandOption("-d|--diagonal")]
        // [DefaultValue(false)]
        // public bool DiagonalSearch { get; init; }

        [CommandOption("-V|--verbose")]
        [DefaultValue(false)]
        public bool Verbose { get; init; }

        [Description("Minimum amount of letter a word needs to have in order to be parsed")]
        [CommandOption("-m|--min-letter")]
        [DefaultValue(2)]
        public int MinLetter { get; init; }

        [Description("Only show the first character on the result")]
        [CommandOption("-M|--minimal")]
        [DefaultValue(false)]
        public bool Minimal { get; init; }
        
        [Description("Only use one color to display the result")]
        [CommandOption("-s|--single-color")]
        [DefaultValue(false)]
        public bool SingleColor { get; init; }
        
        [CommandOption("-w|--wrap")]
        [DefaultValue(true)]
        public bool Wrap { get; init; }

        public override ValidationResult Validate()
        {
            if (!File.Exists(WordListPath))
                return ValidationResult.Error("Word file not found");

            return ValidationResult.Success();
        }
    }
}