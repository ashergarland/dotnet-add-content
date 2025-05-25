# dotnet-add-content

**`dotnet-add-content`** is a custom .NET CLI tool that makes it easy to add content file includes to `.csproj` or `.props` files — with support for inline adds or reusable imports.

> Easily reference content like CSVs, configs, assets, or documentation from your build output.

---

## ✨ Features

- ✅ Add `<None Include="...">` entries directly to your `.csproj`
- ✅ Generate or update `.props` files with shared content references
- ✅ Automatically import `.props` files into `.csproj`
- ✅ Supports globs like `data/**/*.csv`
- ✅ Smart behavior: skip duplicates or use `--overwrite` to regenerate
- ✅ Fully tested with clean CLI UX

---

## 📦 Installation

Install from [NuGet](https://www.nuget.org/packages/dotnet-add-content):

```bash
dotnet tool install --global dotnet-add-content
````

> Requires [.NET 8 SDK or later](https://dotnet.microsoft.com/en-us/download)

---

## 🚀 Usage

### ➕ Add inline content to `.csproj`

```bash
dotnet add-content --project MyApp.csproj --include "data/**/*.csv"
```

This adds:

```xml
<ItemGroup>
  <None Include="data/**/*.csv">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    <Link>%(RecursiveDir)%(Filename)%(Extension)</Link>
  </None>
</ItemGroup>
```

---

### 📁 Add or update a `.props` file

```bash
dotnet content-target --file Shared.props --include "docs/**/*.md"
```

This creates (or appends) a shared props file like:

```xml
<Project>
  <ItemGroup>
    <None Include="docs/**/*.md">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
      <Link>%(RecursiveDir)%(Filename)%(Extension)</Link>
    </None>
  </ItemGroup>
</Project>
```

---

### 📥 Import props into a `.csproj`

```bash
dotnet content-target --file Shared.props --project MyLib.csproj
```

This adds:

```xml
<Import Project="Shared.props" />
```

---

### 🔁 Combine actions

```bash
dotnet content-target --file Shared.props --include "resources/**/*.json" --project App.csproj
```

---

## 🧪 Testing

```bash
dotnet test
```

Includes full coverage of:

* Project and props editing
* CLI argument parsing and validation
* Idempotency and edge cases

---

## 🤝 Contributing

PRs welcome! To get started:

```bash
git clone https://github.com/ashergarland/dotnet-add-content.git
cd dotnet-add-content
dotnet build
```

---

## 📄 License

MIT License © 2024 Asher Garland
[View License](LICENSE)

---

## 📦 NuGet Package

* 📦 [nuget.org/packages/dotnet-add-content](https://www.nuget.org/packages/dotnet-add-content)

---

## 👏 Acknowledgements

Built using:

* [System.CommandLine](https://github.com/dotnet/command-line-api)
* `.csproj` XML editing via `System.Xml.Linq`

````