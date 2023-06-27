# WordFinder  

A *flexible - beautiful - simple* cli app to find words from letters  
## Installation  

1. Clone the repo `git clone https://github.com/LanLP0/WordFinder`
2. Run the project `dotnet run -c Release`
## Usage  
```
    WordFinder [characters] [size] [wordListPath] [OPTIONS]

ARGUMENTS:
    [characters]      The characters
    [size]            Size of character box (WidthxHeight)
    [wordListPath]    Path to word list file (default: words.txt)

OPTIONS:
                          DEFAULT
    -h, --help                       Prints help information
    -v, --verbose
    -m, --min-letter      2          Minimum amount of letter a word needs to have in order to be searched for
    -M, --minimal                    Only show the first character on the result
    -S, --single                     Only search for the first occurrence of a word
    -s, --single-color               Only use one color to display the result
    -w, --wrap            True       Allow words to be wrapped around the left and right side of the character box
        --no-wrap                    Turn off wrapping (This have higher priority than --wrap)
    -e, --exclude                    Direction(s) to exclude searching from (coma seperated list)
                                     (Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight)
        --no-diag                    Don't search along diagonals (shorthand)
        --no-backward                Don't search backward (shorthand)
```
## Contributing  

Pull requests are welcome. For major changes, please open an issue first
to discuss what you would like to change.
## Screenshots  

<img src="https://raw.githubusercontent.com/LanLP0/WordFinder/main/screenshots/words_all.png"/>
<img src="https://raw.githubusercontent.com/LanLP0/WordFinder/main/screenshots/words_rendered.png"/>
<img src="https://raw.githubusercontent.com/LanLP0/WordFinder/main/screenshots/words_table.png"/>
<img src="https://raw.githubusercontent.com/LanLP0/WordFinder/main/screenshots/colors.png"/>