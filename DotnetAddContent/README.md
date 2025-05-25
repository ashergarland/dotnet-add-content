# dotnet-add-content

> View the source on GitHub: [github.com/ashergarland/dotnet-add-content](https://github.com/ashergarland/dotnet-add-content)

**`dotnet-add-content`** is a custom .NET CLI tool that makes it easy to add content file includes to `.csproj` or `.props` files — with support for inline adds, reusable props files, and automatic import wiring.

> Use it to include CSVs, assets, configs, docs, or other static files in your build output — cleanly and reproducibly.

## ✨ Features

- ✅ Add `<None Include="...">` entries directly to `.csproj` (inline)
- ✅ Generate or update `.props` files with shared content includes
- ✅ Automatically import `.props` files into `.csproj`
- ✅ Supports glob patterns (`data/**/*.csv`, `*.json`, `docs/file?.md`, etc.)
- ✅ Smart behavior: skip duplicates or use `--overwrite` to regenerate
- ✅ Fully validated CLI with rich error messages
- ✅ Friendly with MSBuild and NuGet packing

---

## 🚀 Usage

### ➕ Inline add to a `.csproj`

```bash
dotnet add-content --project MyApp.csproj --include "assets/**/*.json"
````

### 🏗️ Create or update a `.props` file

```bash
dotnet add-content --file Shared.props --include "docs/**/*.md"
```

### 📥 Import `.props` into a project

```bash
dotnet add-content --file Shared.props --project MyLib.csproj
```

### 🔁 Combine both

```bash
dotnet add-content --file Shared.props --include "data/**/*.csv" --project App.csproj
```

---

## 📝 License

MIT License © 2024 Asher Garland

---

🔗 **GitHub Repository:** [github.com/ashergarland/dotnet-add-content](https://github.com/ashergarland/dotnet-add-content)
