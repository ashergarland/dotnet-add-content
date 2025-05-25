using System.CommandLine;
using DotnetAddContent.Helpers;

namespace DotnetAddContent.Commands;

public static class ContentTargetCommand
{
    public static Command Build()
    {
        var command = new Command("content-target", "Create or update a .props file and/or import it into a .csproj");

        // Options
        var fileOption = new Option<FileInfo>("--file", "Path to the .props file") { IsRequired = true };
        var includeOption = new Option<string?>("--include", "Glob pattern of files to include");
        var projectOption = new Option<FileInfo?>("--project", "Path to a .csproj file to import this props file into");
        var overwriteOption = new Option<bool>("--overwrite", "If set, overwrites the props file instead of appending");

        // Register options
        command.AddOption(fileOption);
        command.AddOption(includeOption);
        command.AddOption(projectOption);
        command.AddOption(overwriteOption);

        // Validation
        command.AddValidator(result =>
        {
            var include = result.GetValueForOption(includeOption);
            var project = result.GetValueForOption(projectOption);

            if (string.IsNullOrWhiteSpace(include) && project is null)
            {
                var errorMessaage = "You must specify at least one of --include or --project.";
                Console.Error.WriteLine(errorMessaage);
                result.ErrorMessage = errorMessaage;
            }

            if (!string.IsNullOrWhiteSpace(include) && !IsValidIncludePath(include!))
            {
                var errorMessaage = "Invalid include path. Must be a valid file path or glob pattern and not contain illegal characters.";
                Console.Error.WriteLine(errorMessaage);
                result.ErrorMessage = errorMessaage;
            }
        });

        command.SetHandler((FileInfo file, string? include, FileInfo? project, bool overwrite) =>
        {
            // include is either null or a valid glob
            if (include is not null)
            {
                if (overwrite || !file.Exists)
                {
                    PropsFileEditor.CreateNew(file, include);
                    Console.WriteLine($"✅ Created new props file at {file.FullName}");
                }
                else
                {
                    var added = PropsFileEditor.AppendInclude(file, include);
                    Console.WriteLine(added
                        ? $"✅ Appended to props file: {include}"
                        : $"ℹ️ Include already present: {include}");
                }
            }

            // project is either null or a valid .csproj path
            if (project is not null)
            {
                var imported = CsprojEditor.AddImport(project, file.FullName);
                Console.WriteLine(imported
                    ? $"✅ Imported {file.Name} into {project.Name}"
                    : $"ℹ️ Import already exists in {project.Name}");
            }
        }, fileOption, includeOption, projectOption, overwriteOption);

        return command;
    }

    private static bool IsValidIncludePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;

        // Check for any invalid characters (excluding wildcard characters *, ?, used in globs)
        var invalidChars = Path.GetInvalidPathChars().Where(c => c != '*' && c != '?').ToArray();
        return !path.Any(c => invalidChars.Contains(c));
    }
}
