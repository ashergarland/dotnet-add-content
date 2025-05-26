using System.Xml.Linq;

namespace DotnetAddContent.Helpers;

public static class CsprojEditor
{
    public static void AddInlineContent(FileInfo project, string include)
    {
        var doc = XDocument.Load(project.FullName);
        var ns = doc.Root?.Name.Namespace ?? "";

        var itemGroup = new XElement(ns + "ItemGroup",
            new XElement(ns + "None",
                new XAttribute("Include", include),
                new XElement(ns + "CopyToOutputDirectory", "PreserveNewest"),
                new XElement(ns + "Link", "data\\%(RecursiveDir)%(Filename)%(Extension)")
            )
        );

        doc.Root?.Add(itemGroup);
        doc.Save(project.FullName);
    }

    public static bool AddImport(FileInfo project, FileInfo propsFile)
    {
        var doc = XDocument.Load(project.FullName);
        var ns = doc.Root?.Name.Namespace ?? "";

        var normalizedPath = Path
            .GetRelativePath(project.DirectoryName!, propsFile.FullName)
            .Replace("\\", "/");

        bool alreadyImported = doc.Root?
            .Elements(ns + "Import")
            .Any(e => string.Equals(e.Attribute("Project")?.Value.ToLower(), normalizedPath, StringComparison.OrdinalIgnoreCase)) ?? false;

        if (!alreadyImported)
        {
            var import = new XElement(ns + "Import", new XAttribute("Project", normalizedPath));
            doc.Root?.Add(import);
            doc.Save(project.FullName);
            return true;
        }

        return false;
    }
}
