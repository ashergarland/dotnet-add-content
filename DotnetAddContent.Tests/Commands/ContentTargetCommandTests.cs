using DotnetAddContent.Commands;
using DotnetAddContent.Tests.TestUtils;
using FluentAssertions;

namespace DotnetAddContent.Tests.Commands;
public class ContentTargetCommandTests
{
    private System.CommandLine.Command command => ContentTargetCommand.Build();

    [Fact]
    public async Task Creates_New_Props_File_With_Include()
    {
        var temp = Path.Combine(Path.GetTempPath(), "content-target.props");
        File.Delete(temp);

        var args = new[]
        {
            "--file", temp,
            "--include", "data/**/*.csv"
        };

        var (exitCode, output) = await CliTestHost.RunAsync(command, args);

        exitCode.Should().Be(0);
        output.Should().Contain("✅ Created new props file");
        File.Exists(temp).Should().BeTrue();
        File.ReadAllText(temp).Should().Contain("data/**/*.csv");
    }

    [Fact]
    public async Task Appends_Only_Once_When_Rerun()
    {
        var temp = Path.Combine(Path.GetTempPath(), "props-reuse.props");
        File.Delete(temp);

        // Run twice with same include
        await CliTestHost.RunAsync(command, "--file", temp, "--include", "data/single.csv");
        var (exitCode2, output2) = await CliTestHost.RunAsync(command, "--file", temp, "--include", "data/single.csv");

        exitCode2.Should().Be(0);
        output2.Should().Contain("ℹ️ Include already present");
    }

    [Fact]
    public async Task Fails_If_No_Options()
    {
        var (exitCode, output) = await CliTestHost.RunAsync(command, "--file", "some.props");

        exitCode.Should().NotBe(0);
        output.Should().Contain("You must specify at least one of");
    }
}
