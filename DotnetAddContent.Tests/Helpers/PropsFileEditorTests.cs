using DotnetAddContent.Helpers;
using FluentAssertions;

namespace DotnetAddContent.Tests.Helpers;
public class PropsFileEditorTests
{
    [Fact]
    public void CreateNew_Creates_Valid_Props_File()
    {
        var temp = Path.GetTempFileName().Replace(".tmp", ".props");
        File.Delete(temp);

        PropsFileEditor.CreateNew(new FileInfo(temp), "data/**/*.csv");

        File.Exists(temp).Should().BeTrue();
        var contents = File.ReadAllText(temp);
        contents.Should().Contain("<None Include=\"data/**/*.csv\"");
    }

    [Fact]
    public void AppendInclude_Adds_New_Entry()
    {
        var path = Path.Combine(Path.GetTempPath(), "test.props");
        File.Delete(path);
        PropsFileEditor.CreateNew(new FileInfo(path), "data/a.csv");

        var result = PropsFileEditor.AppendInclude(new FileInfo(path), "data/b.csv");
        result.Should().BeTrue();

        var xml = File.ReadAllText(path);
        xml.Should().Contain("data/a.csv").And.Contain("data/b.csv");
    }

    [Fact]
    public void AppendInclude_DoesNot_Duplicate_Entries()
    {
        var path = Path.Combine(Path.GetTempPath(), "test.props");
        File.Delete(path);
        PropsFileEditor.CreateNew(new FileInfo(path), "data/a.csv");

        var result = PropsFileEditor.AppendInclude(new FileInfo(path), "data/a.csv");
        result.Should().BeFalse();

        var count = File.ReadAllText(path).Split("Include=\"data/a.csv\"").Length - 1;
        count.Should().Be(1);
    }
}
