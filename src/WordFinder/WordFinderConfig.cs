using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace WordFinder;

public sealed partial class WordFinderApp
{
    public sealed class WordFinderConfig : CommandSettings
    {
        [Description("The characters")]
        [CommandArgument(0, "[characters]")]
        public string? Characters { get; init; }

        [Description("Size of character box (WidthxHeight)")]
        [CommandArgument(1, "[size]")]
        public string? Size { get; set; }
        
        [Description("Path to word list file (default: words.txt)")]
        [CommandArgument(2, "[wordListPath]")]
        [DefaultValue("words.txt")]
        public string WordListPath { get; set; }

        [CommandOption("-v|--verbose")]
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
        
        [Description("Only search for the first occurrence of a word")]
        [CommandOption("-S|--single")]
        [DefaultValue(false)]
        public bool Single { get; init; }
        
        [Description("Only use one color to display the result")]
        [CommandOption("-s|--single-color")]
        [DefaultValue(false)]
        public bool SingleColor { get; init; }
        
        [CommandOption("-w|--wrap")]
        [DefaultValue(true)]
        public bool Wrap { get; init; }
        
        [Description("Direction(s) to exclude searching from (coma seperated list)")]
        [CommandOption("-e|--exclude")]
        [DefaultValue(null)]
        public string? Exclusion { get; init; }

        [Description("Don't search along diagonals (quick option)")]
        [CommandOption("--no-diag|--no-diagonal")]
        [DefaultValue(false)]
        public bool NoDiag { get; init; }
        
        [Description("Don't search backward (quick option)")]
        [CommandOption("--no-backward|--no-back")]
        [DefaultValue(false)]
        public bool NoBack { get; init; }

        public override ValidationResult Validate()
        {
            if (!File.Exists(WordListPath))
                return ValidationResult.Error("Word file not found");

            return ValidationResult.Success();
        }
    }
}