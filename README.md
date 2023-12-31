# WordFinder  

A *flexible - beautiful - simple* cli app to find words from letters  
## Installation  

1. Clone the repo `git clone https://github.com/LanLP0/WordFinder`
2. Run the project `dotnet run -c Release`
## Usage  
```
    WordFinder [characters] [size] [OPTIONS]

ARGUMENTS:
    [characters]    The characters
    [size]          Size of character box (WidthxHeight)

OPTIONS:
                          DEFAULT
    -h, --help                         Prints help information
    -W, --words           words.txt    Path to word list file (default: words.txt)
    -v, --verbose
    -m, --min-letter      2            Minimum amount of letter a word needs to have in order to be searched for
    -M, --minimal                      Only show the first character on the result
    -S, --single                       Only search for the first occurrence of a word
    -s, --single-color                 Only use one color to display the result
    -w, --wrap            True         Allow words to be wrapped around the left and right side of the character box
        --no-wrap                      Turn off wrapping (This have higher priority than --wrap)
    -e, --exclude                      Direction(s) to exclude searching from (Coma seperated list)
                                       (Options: Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight)
        --no-diag                      Don't search along diagonals
                                       (Shorthand to exclude UpLeft, UpRight, DownLeft, DownRight)
        --no-backward                  Don't search backward
                                       (Shorthand to exclude Left, UpLeft, Up, UpRight)
    -c, --color                        Color(s) to be used to print the result (Coma seperated list)
                                       (To see all supported colors, run the program with --print-colors
        --print-colors                 Prints all supported color
```
## Contributing  

Pull requests are welcome. For major changes, please open an issue first
to discuss what you would like to change.
## Screenshots  

<img src="https://raw.githubusercontent.com/LanLP0/WordFinder/main/screenshots/words_all.png"/>
<img src="https://raw.githubusercontent.com/LanLP0/WordFinder/main/screenshots/words_rendered.png"/>
<img src="https://raw.githubusercontent.com/LanLP0/WordFinder/main/screenshots/words_table.png"/>
<img src="https://raw.githubusercontent.com/LanLP0/WordFinder/main/screenshots/colors.png"/>