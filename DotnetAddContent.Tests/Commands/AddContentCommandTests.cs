using DotnetAddContent.Commands;
using DotnetAddContent.Tests.TestUtils;
using FluentAssertions;

namespace DotnetAddContent.Tests.Commands;
public class AddContentCommandTests
{
    [Fact]
    public async Task Adds_Inline_Content_To_Csproj()
    {
        var path = Path.Combine(Path.GetTempPath(), "inline-test.csproj");
        File.WriteAllText(path, "<Project></Project>");

        var command = AddContentCommand.Build();

        var (exitCode, output) = await CliTestHost.RunAsync(command,
            "--project", path,
            "--include", "assets/**/*.json");

        exitCode.Should().Be(0);
        output.Should().Contain("✅ Added inline content");

        var content = File.ReadAllText(path);
        content.Should().Contain("assets/**/*.json");
    }
}
