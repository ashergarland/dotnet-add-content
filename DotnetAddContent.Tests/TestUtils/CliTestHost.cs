using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Text;

namespace DotnetAddContent.Tests.TestUtils;

public static class CliTestHost
{
    public static async Task<(int ExitCode, string Output)> RunAsync(Command command, params string[] args)
    {
        var output = new StringBuilder();
        using var writer = new StringWriter(output);
        var originalOut = Console.Out;
        var originalErr = Console.Error;

        try
        {
            Console.SetOut(writer);
            Console.SetError(writer);

            var parser = new CommandLineBuilder(command)
                .UseDefaults()
                .Build();

            var exitCode = await parser.InvokeAsync(args);

            Console.Out.Flush();
            Console.Error.Flush();
            writer.Flush();

            return (exitCode, output.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalErr);
        }
    }
}
