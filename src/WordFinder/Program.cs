using Spectre.Console.Cli;
using WordFinder;

Console.CancelKeyPress += (_, _) => Environment.Exit(0);

var app = new CommandApp<WordFinderApp>();

return app.Run(args);