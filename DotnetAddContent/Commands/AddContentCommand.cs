namespace DotnetAddContent.Commands;

using System.CommandLine;
using DotnetAddContent.Helpers;

public static class AddContentCommand
{
    public static Command Build()
    {
        var command = new Command("add-content", "Adds content includes inline or via props file, and optionally imports props into a .csproj");

        var projectOption = CliOptions.ProjectOption;
        var fileOption = CliOptions.FileOption;
        var includeOption = CliOptions.IncludeOption;
        var overwriteOption = CliOptions.OverwriteOption;

        command.AddOption(projectOption);
        command.AddOption(fileOption);
        command.AddOption(includeOption);
        command.AddOption(overwriteOption);

        command.AddValidator(result =>
        {
            var project = result.GetValueForOption(projectOption);
            var file = result.GetValueForOption(fileOption);
            var include = result.GetValueForOption(includeOption);
            var overwrite = result.GetValueForOption(overwriteOption);

            bool hasProject = project != null;
            bool hasFile = file != null;
            bool hasInclude = !string.IsNullOrWhiteSpace(include);

            if (!hasInclude && !hasFile)
            {
                var errorMessaage = "You must specify at least --include or --file.";
                Console.Error.WriteLine(errorMessaage);
                result.ErrorMessage = errorMessaage;
                return;
            }

            if (hasInclude && !IsValidIncludePath(include!))
            {
                var errorMessaage = "Invalid include path. Must be a valid glob or file pattern.";
                Console.Error.WriteLine(errorMessaage);
                result.ErrorMessage = errorMessaage;
                return;
            }

            if (overwrite && !hasFile)
            {
                var errorMessaage = "--overwrite requires --file.";
                Console.Error.WriteLine(errorMessaage);
                result.ErrorMessage = errorMessaage;
                return;
            }

            // Prevent ambiguous input: --project + --include + no --file
            if (hasProject && hasInclude && !hasFile)
            {
                // valid scenario: Scenario 1: Add content inline to .csproj
                return;
            }

            if (hasInclude && !hasProject && !hasFile)
            {
                var errorMessaage = "--include must be used with --project or --file.";
                Console.Error.WriteLine(errorMessaage);
                result.ErrorMessage = errorMessaage;
                return;
            }
        });

        command.SetHandler((FileInfo? project, FileInfo? file, string? include, bool overwrite) =>
        {
            // Scenario 1: Add content inline to .csproj
            if (project != null && file == null && include != null)
            {
                CsprojEditor.AddInlineContent(project, include);
                Console.WriteLine($"✅ Added inline content to {project.Name}");
            }

            // Scenario 2: Add content to props file only
            if (file != null && include != null && project == null)
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

            // Scenario 3: Add props import only
            if (file != null && project != null && include == null)
            {
                var imported = CsprojEditor.AddImport(project, file.FullName);
                Console.WriteLine(imported
                    ? $"✅ Imported {file.Name} into {project.Name}"
                    : $"ℹ️ Import already exists in {project.Name}");
            }

            // Scenario 4: Add content to props and import into .csproj
            if (file != null && project != null && include != null)
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

                var imported = CsprojEditor.AddImport(project, file.FullName);
                Console.WriteLine(imported
                    ? $"✅ Imported {file.Name} into {project.Name}"
                    : $"ℹ️ Import already exists in {project.Name}");
            }
        }, projectOption, fileOption, includeOption, overwriteOption);

        return command;
    }

    private static readonly char[] ExtraInvalidPathChars = ['<', '>', ':', '"', '|'];

    private static bool IsValidIncludePath(string path)
    {
        var invalidChars = Path.GetInvalidPathChars().Concat(ExtraInvalidPathChars).Distinct();
        return !path.Any(c => invalidChars.Contains(c));
    }
}
