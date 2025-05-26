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
        var dir = Path.GetTempPath();
        var csprojPath = Path.Combine(dir, "test.csproj");
        File.WriteAllText(csprojPath, "<Project></Project>");

        var projectFile = new FileInfo(csprojPath);
        var propsFile = new FileInfo(Path.Combine(dir, "props/my.props"));

        CsprojEditor.AddImport(projectFile, propsFile);

        var content = File.ReadAllText(csprojPath);
        content.Should().Contain("<Import Project=\"props/my.props\"");
    }


    [Fact]
    public void AddImport_DoesNot_Duplicate()
    {
        var csprojPath = Path.Combine(Path.GetTempPath(), "test.csproj");
        File.WriteAllText(csprojPath, "<Project></Project>");
        var file = new FileInfo(csprojPath);

        CsprojEditor.AddImport(file, new FileInfo("tools/shared.props"));
        CsprojEditor.AddImport(file, new FileInfo("tools/shared.props"));

        var count = File.ReadAllText(csprojPath).Split("tools/shared.props").Length - 1;
        count.Should().Be(1);
    }
}
