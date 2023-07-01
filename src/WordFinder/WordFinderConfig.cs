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
        [CommandOption("-W|--words")]
        [DefaultValue("words.txt")]
        public string WordListPath { get; set; } = "words.txt";

        [CommandOption("-v|--verbose")]
        [DefaultValue(false)]
        public bool Verbose { get; init; }

        [Description("Minimum amount of letter a word needs to have in order to be searched for")]
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
        
        [Description("Allow words to be wrapped around the left and right side of the character box")]
        [CommandOption("-w|--wrap")]
        [DefaultValue(true)]
        public bool Wrap { get; set; }
        
        [Description("Turn off wrapping (This have higher priority than --wrap)")]
        [CommandOption("--no-wrap")]
        [DefaultValue(false)]
        public bool NoWrap { get; init; }
        
        [Description("Direction(s) to exclude searching from (Coma seperated list)\n(Options: Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight)")]
        [CommandOption("-e|--exclude")]
        [DefaultValue(null)]
        public string? Exclusion { get; init; }

        [Description("Don't search along diagonals\n(Shorthand to exclude UpLeft, UpRight, DownLeft, DownRight)")]
        [CommandOption("--no-diag|--no-diagonal")]
        [DefaultValue(false)]
        public bool NoDiag { get; init; }
        
        [Description("Don't search backward\n(Shorthand to exclude Left, UpLeft, Up, UpRight)")]
        [CommandOption("--no-backward|--no-back")]
        [DefaultValue(false)]
        public bool NoBack { get; init; }
        
        [Description("Color(s) to be used to print the result (Coma seperated list)\n(To see all supported colors, run the program with --print-colors")]
        [CommandOption("-c|--color")]
        [DefaultValue(null)]
        public string? Colors { get; init; }
        
        [Description("Prints all supported color")]
        [CommandOption("--print-colors")]
        [DefaultValue(false)]
        public bool PrintColors { get; init; }

        public override ValidationResult Validate()
        {
            if (!File.Exists(WordListPath))
                return ValidationResult.Error("Word file not found");

            return ValidationResult.Success();
        }
    }
}