using Spectre.Console.Cli;
using WordFinder;

Console.CancelKeyPress += (_, _) => Environment.Exit(0);

return new CommandApp<WordFinderApp>().Run(args);