using System.Xml.Linq;

namespace DotnetAddContent.Helpers;

public static class PropsFileEditor
{
    public static void CreateNew(FileInfo file, string include)
    {
        Directory.CreateDirectory(file.DirectoryName!);

        var doc = new XDocument(
            new XDeclaration("1.0", "utf-8", "yes"),
            new XElement("Project",
                new XElement("ItemGroup",
                    CreateContentElement(include)
                )
            )
        );

        doc.Save(file.FullName);
    }

    public static bool AppendInclude(FileInfo file, string include)
    {
        var doc = XDocument.Load(file.FullName);
        var ns = doc.Root?.Name.Namespace ?? "";

        var itemGroups = doc.Root?.Elements(ns + "ItemGroup");
        var targetGroup = itemGroups?.FirstOrDefault() ?? new XElement(ns + "ItemGroup");

        var alreadyExists = targetGroup.Elements(ns + "None")
            .Any(e => e.Attribute("Include")?.Value == include);

        if (alreadyExists) return false;

        targetGroup.Add(CreateContentElement(include));

        if (!itemGroups!.Any())
            doc.Root?.Add(targetGroup);

        doc.Save(file.FullName);
        return true;
    }

    private static XElement CreateContentElement(string include)
    {
        return new XElement("None",
            new XAttribute("Include", include),
            new XElement("CopyToOutputDirectory", "PreserveNewest"),
            new XElement("Link", "%(RecursiveDir)%(Filename)%(Extension)")
        );
    }
}
