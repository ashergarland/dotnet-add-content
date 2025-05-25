namespace DotnetAddContent.Tests.Commands;

using DotnetAddContent.Commands;
using DotnetAddContent.Tests.TestUtils;
using FluentAssertions;

public class AddContentCommandTests
{
    private static System.CommandLine.Command Command => AddContentCommand.Build();

    [Fact]
    public async Task Adds_Inline_Content_To_Csproj()
    {
        var csproj = TempFile("inline.csproj", "<Project></Project>");

        var (code, output) = await CliTestHost.RunAsync(Command,
            "--project", csproj,
            "--include", "assets/**/*.json");

        code.Should().Be(0);
        output.Should().Contain("✅ Added inline content");
        File.ReadAllText(csproj).Should().Contain("assets/**/*.json");
    }

    [Fact]
    public async Task Creates_New_Props_File_With_Include()
    {
        var props = TempFile("content.props", create: false);

        var (code, output) = await CliTestHost.RunAsync(Command,
            "--file", props,
            "--include", "docs/**/*.md");

        code.Should().Be(0);
        output.Should().Contain("✅ Created new props file");
        File.ReadAllText(props).Should().Contain("docs/**/*.md");
    }

    [Fact]
    public async Task Adds_Props_Import_To_Csproj()
    {
        var csproj = TempFile("import.csproj", "<Project></Project>");
        var props = TempFile("import.props", "<Project></Project>");

        var (code, output) = await CliTestHost.RunAsync(Command,
            "--project", csproj,
            "--file", props);

        code.Should().Be(0);
        output.Should().Contain("✅ Imported");
        File.ReadAllText(csproj).Should().Contain("Import");
    }

    [Fact]
    public async Task Adds_Props_And_Imports_It()
    {
        var csproj = TempFile("combo.csproj", "<Project></Project>");
        var props = TempFile("combo.props", create: false);

        var (code, output) = await CliTestHost.RunAsync(Command,
            "--project", csproj,
            "--file", props,
            "--include", "content/*.txt");

        code.Should().Be(0);
        output.Should().Contain("✅ Created new props file");
        output.Should().Contain("✅ Imported");
        File.ReadAllText(csproj).Should().Contain("Import");
        File.ReadAllText(props).Should().Contain("content/*.txt");
    }

    [Fact]
    public async Task Fails_With_No_Required_Options()
    {
        var (code, output) = await CliTestHost.RunAsync(Command);

        code.Should().NotBe(0);
        output.Should().Contain("You must specify at least");
    }

    [Fact]
    public async Task Fails_If_Include_Is_Invalid_Glob()
    {
        var props = TempFile("bad-glob.props");

        var (code, output) = await CliTestHost.RunAsync(Command,
            "--file", props,
            "--include", "data/<illegal>name.csv");

        code.Should().NotBe(0);
        output.Should().Contain("Invalid include path");
    }

    [Fact]
    public async Task Fails_If_Include_Provided_With_Neither_Project_Nor_File()
    {
        var (code, output) = await CliTestHost.RunAsync(Command,
            "--include", "data/file.csv");

        code.Should().NotBe(0);
        output.Should().Contain("--include must be used with --project or --file");
    }

    [Fact]
    public async Task Fails_If_Overwrite_Specified_Without_File()
    {
        var csproj = TempFile("overwrite.csproj", "<Project></Project>");

        var (code, output) = await CliTestHost.RunAsync(Command,
            "--project", csproj,
            "--include", "content.txt",
            "--overwrite");

        code.Should().NotBe(0);
        output.Should().Contain("--overwrite requires --file");
    }

    [Fact]
    public async Task Allows_Directory_For_Project()
    {
        var dir = Path.Combine(Path.GetTempPath(), "project-dir");
        Directory.CreateDirectory(dir);

        var projectPath = Path.Combine(dir, "Proj.csproj");
        File.WriteAllText(projectPath, "<Project></Project>");

        var (code, output) = await CliTestHost.RunAsync(Command,
            "--project", dir,
            "--include", "img/**");

        code.Should().Be(0);
        output.Should().Contain("✅ Added inline content");
    }

    [Fact]
    public async Task Fails_If_Project_Directory_Contains_Multiple_Csproj()
    {
        var dir = Path.Combine(Path.GetTempPath(), $"multi-csproj-{Guid.NewGuid()}");
        Directory.CreateDirectory(dir);

        File.WriteAllText(Path.Combine(dir, "A.csproj"), "<Project></Project>");
        File.WriteAllText(Path.Combine(dir, "B.csproj"), "<Project></Project>");

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            CliTestHost.RunAsync(Command,
                "--project", dir,
                "--include", "img/**"));

        ex.Message.Should().Contain("does not contain exactly one .csproj file");
    }

    [Fact]
    public async Task Appends_To_Existing_Props_File_If_Not_Overwritten()
    {
        var props = TempFile("append-existing.props", """
        <Project>
          <ItemGroup>
            <None Include="docs/intro.md" />
          </ItemGroup>
        </Project>
        """);

        var (code, output) = await CliTestHost.RunAsync(Command,
            "--file", props,
            "--include", "docs/guide.md");

        code.Should().Be(0);
        output.Should().Contain("✅ Appended to props file");
        File.ReadAllText(props).Should().Contain("docs/guide.md");
    }

    [Fact]
    public async Task Skips_Appending_If_Include_Already_Exists()
    {
        var props = TempFile("no-dup.props", """
        <Project>
          <ItemGroup>
            <None Include="docs/readme.md" />
          </ItemGroup>
        </Project>
        """);

        var (code, output) = await CliTestHost.RunAsync(Command,
            "--file", props,
            "--include", "docs/readme.md");

        code.Should().Be(0);
        output.Should().Contain("ℹ️ Include already present");
    }

    private static string TempFile(string name, string? contents = null, bool create = true)
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}-{name}");

        if (create)
        {
            File.WriteAllText(path, contents ?? string.Empty);
        }

        return path;
    }
}
