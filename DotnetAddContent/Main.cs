using System.CommandLine;
using DotnetAddContent.Commands;

var root = new RootCommand("dotnet add-content CLI");

root.AddCommand(AddContentCommand.Build());
root.AddCommand(ContentTargetCommand.Build());

return await root.InvokeAsync(args);
