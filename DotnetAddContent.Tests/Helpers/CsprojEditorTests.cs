using DotnetAddContent.Helpers;
using FluentAssertions;

namespace DotnetAddContent.Tests.Helpers;
public class CsprojEditorTests
{
    [Fact]
    public void AddInlineContent_Appends_New_ItemGroup()
    {
        var csprojPath = Path.Combine(Path.GetTempPath(), "test.csproj");
        File.WriteAllText(csprojPath, "<Project></Project>");

        CsprojEditor.AddInlineContent(new FileInfo(csprojPath), "resources/*.json");

        var content = File.ReadAllText(csprojPath);
        content.Should().Contain("resources/*.json");
    }

    [Fact]
    public void AddImport_Adds_Import_If_Not_Already_Present()
    {
        var csprojPath = Path.Combine(Path.GetTempPath(), "test.csproj");
        File.WriteAllText(csprojPath, "<Project></Project>");

        CsprojEditor.AddImport(new FileInfo(csprojPath), "props/my.props");

        var content = File.ReadAllText(csprojPath);
        content.Should().Contain("<Import Project=\"props/my.props\"");
    }

    [Fact]
    public void AddImport_DoesNot_Duplicate()
    {
        var csprojPath = Path.Combine(Path.GetTempPath(), "test.csproj");
        File.WriteAllText(csprojPath, "<Project></Project>");
        var file = new FileInfo(csprojPath);

        CsprojEditor.AddImport(file, "tools/shared.props");
        CsprojEditor.AddImport(file, "tools/shared.props");

        var count = File.ReadAllText(csprojPath).Split("tools/shared.props").Length - 1;
        count.Should().Be(1);
    }
}
