# dotnet-add-content

**`dotnet-add-content`** is a custom .NET CLI tool that makes it easy to add content file includes to `.csproj` or `.props` files — with support for inline adds, reusable props files, and automatic import wiring.

> Use it to include CSVs, assets, configs, docs, or other static files in your build output — cleanly and reproducibly.

---

## ✨ Features

- ✅ Add `<None Include="...">` entries directly to `.csproj` (inline)
- ✅ Generate or update `.props` files with shared content includes
- ✅ Automatically import `.props` files into `.csproj`
- ✅ Supports glob patterns (`data/**/*.csv`, `*.json`, `docs/file?.md`, etc.)
- ✅ Smart behavior: skip duplicates or use `--overwrite` to regenerate
- ✅ Fully validated CLI with rich error messages
- ✅ Friendly with MSBuild and NuGet packing

---

## 📦 Installation

Install globally from [NuGet](https://www.nuget.org/packages/dotnet-add-content):

```bash
dotnet tool install --global dotnet-add-content
````

> ✅ Requires [.NET 8 SDK or later](https://dotnet.microsoft.com/en-us/download)

---

## 🚀 Usage

### 1️⃣ Add inline content to a `.csproj`

```bash
dotnet add-content --project MyApp.csproj --include "assets/**/*.json"
```

Adds this:

```xml
<ItemGroup>
  <None Include="assets/**/*.json">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    <Link>%(RecursiveDir)%(Filename)%(Extension)</Link>
  </None>
</ItemGroup>
```

---

### 2️⃣ Add or update a `.props` file with includes

```bash
dotnet add-content --file Shared.props --include "docs/**/*.md"
```

Creates or updates:

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

Use `--overwrite` to regenerate the file:

```bash
dotnet add-content --file Shared.props --include "docs/**/*.md" --overwrite
```

---

### 3️⃣ Import a `.props` file into a `.csproj`

```bash
dotnet add-content --project MyLib.csproj --file Shared.props
```

Adds:

```xml
<Import Project="Shared.props" />
```

If the import already exists, it will not be duplicated.

---

### 4️⃣ Do both in one step

```bash
dotnet add-content --project MyApp.csproj --file Shared.props --include "resources/*.json"
```

* Adds or updates the `.props` file
* Adds `<Import>` to your `.csproj`

---

## 🧪 Testing

```bash
dotnet test
```

Includes full coverage for:

* 🔧 Inline and props editing
* 🧠 Smart CLI validation rules
* 🌀 Idempotent glob appends and import deduplication
* ⚠️ Error handling and invalid usage

---

## ✅ Develop and Run Locally

This is useful when developing or testing local changes before publishing.

### 🧼 1. Uninstall any old global version

```bash
dotnet tool uninstall -g dotnet-add-content
```

---

### 🔨 2. Build the NuGet package

```bash
dotnet pack -c Release
```

Generates:

```
./DotnetAddContent/bin/Release/dotnet-add-content.1.0.0.nupkg
```

---

### ➕ 3. Reinstall from local build

```bash
dotnet tool install --global --add-source ./DotnetAddContent/bin/Release dotnet-add-content
```

✅ Will run using .NET 9 if available, or fallback to .NET 8 (thanks to `<RollForward>Major</RollForward>`)

---

### 🚀 4. Run the CLI

```bash
dotnet add-content --help
```

🎉 See the full help output and usage examples.

---

## 🤝 Contributing

We welcome PRs, suggestions, and feedback!

To get started:

```bash
git clone https://github.com/ashergarland/dotnet-add-content.git
cd dotnet-add-content
dotnet build
dotnet test
```

---

## 📄 License

MIT License © 2024 Asher Garland
[View License](LICENSE)

---

## 📦 NuGet Package

📦 [nuget.org/packages/dotnet-add-content](https://www.nuget.org/packages/dotnet-add-content)

---

## 👏 Acknowledgements

Built with:

* [System.CommandLine](https://github.com/dotnet/command-line-api)
* `System.Xml.Linq` for safe `.csproj` and `.props` editing

---