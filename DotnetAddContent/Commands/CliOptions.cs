namespace DotnetAddContent.Commands;

using System.CommandLine;
using System.CommandLine.Parsing;

public static class CliOptions
{
    public static Option<FileInfo?> ProjectOption => new(
        aliases: ["--project", "-p"],
        description: "Path to a .csproj file or a directory containing one",
        parseArgument: ParseProjectOption);

    public static Option<FileInfo?> FileOption => new(
        aliases: ["--file", "-f"],
        description: "Path to the .props file");

    public static Option<string?> IncludeOption => new(
        aliases: ["--include", "-i"],
        description: "Glob or file pattern to include");

    public static Option<bool> OverwriteOption => new(
        aliases: ["--overwrite", "-o"],
        description: "If set, overwrites the props file instead of appending");

    private static FileInfo? ParseProjectOption(ArgumentResult result)
    {
        var token = result.Tokens.FirstOrDefault()?.Value;
        if (token == null) return null;

        try
        {
            return ResolveCsprojPath(token);
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.Message;
            return null;
        }
    }

    public static FileInfo ResolveCsprojPath(string input)
    {
        var path = Path.GetFullPath(input);

        if (File.Exists(path) && Path.GetExtension(path) == ".csproj")
            return new FileInfo(path);

        if (Directory.Exists(path))
        {
            var matches = Directory.GetFiles(path, "*.csproj");
            if (matches.Length == 1)
                return new FileInfo(matches[0]);

            throw new ArgumentException($"Directory '{path}' does not contain exactly one .csproj file.");
        }

        throw new FileNotFoundException($"Could not resolve a .csproj file from: {input}");
    }
}
