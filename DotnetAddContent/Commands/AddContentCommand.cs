using System.CommandLine;
using DotnetAddContent.Helpers;

namespace DotnetAddContent.Commands;

public static class AddContentCommand
{
    public static Command Build()
    {
        var command = new Command("add-content", "Adds a content reference to the .csproj inline");

        var projectOption = new Option<FileInfo>("--project", "Path to the .csproj file") { IsRequired = true };
        var includeOption = new Option<string>("--include", "Glob pattern to include") { IsRequired = true };

        command.AddOption(projectOption);
        command.AddOption(includeOption);

        command.SetHandler((FileInfo project, string include) =>
        {
            CsprojEditor.AddInlineContent(project, include);
            Console.WriteLine($"✅ Added inline content to {project.Name}");
        }, projectOption, includeOption);

        return command;
    }
}
