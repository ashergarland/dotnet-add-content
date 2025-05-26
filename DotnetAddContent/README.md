# dotnet-add-content

> View the source on GitHub: [github.com/ashergarland/dotnet-add-content](https://github.com/ashergarland/dotnet-add-content)

**`dotnet-add-content`** is a custom .NET CLI tool that helps you add content file includes to `.csproj` or `.props` files. It supports inline additions, reusable props files, and automatic import wiring.

Most `dotnet` commands let you create and configure projects from the command line. However, there is no built-in way to include content files like images, CSVs, or configuration files in your build without manually editing the `.csproj`.

This tool fills that gap by providing a clean and scriptable way to manage content includes using a simple CLI workflow.

You can use it to include static files like images, data files, and embedded assets in your project in a consistent and maintainable way.

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

## 🧠 Real-World Examples

### 🏗️ Simplified Project Setup: Setting Up a .NET Project With Content Files

Say you're bootstrapping a new .NET project with a test project and some data files in a `/data/` folder. You want those files copied to your test output directory automatically.

#### 🛠️ Without `dotnet-add-content`

You might run:

```bash
dotnet new sln -n MyApp
dotnet new console -n MyApp.App
dotnet new xunit -n MyApp.Tests

dotnet sln add MyApp.App
dotnet sln add MyApp.Tests

dotnet add MyApp.Tests reference MyApp.App
```

Then manually edit your `.csproj`:

```xml
<!-- Inside MyApp.Tests.csproj -->
<ItemGroup>
  <None Include="..\data\**\*.csv">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    <Link>data\%(RecursiveDir)%(Filename)%(Extension)</Link>
  </None>
</ItemGroup>
```

This works, but requires error-prone hand editing of project files.

---

#### ✅ With `dotnet-add-content`

Run the same setup:

```bash
dotnet new sln -n MyApp
dotnet new console -n MyApp.App
dotnet new xunit -n MyApp.Tests

dotnet sln add MyApp.App
dotnet sln add MyApp.Tests

dotnet add MyApp.Tests reference MyApp.App
```

Then instead of editing XML, just run:

```bash
dotnet add-content --project MyApp.Tests --include "../data/**/*.csv"
```

This safely injects the correct `<ItemGroup>` into `MyApp.Tests.csproj` and ensures:

* ✅ Files are copied to `bin/` at build time
* ✅ Paths are linked with folder structure intact
* ✅ Everything is reproducible in CI/CD or onboarding scripts

---

### 🔁 Reusable Content Patterns: Sharing Props Across Multiple Projects

When working in a multi-project solution, you might have multiple `.csproj` files that need to reference the same assets — for example, shared documentation, test resources, or config files.

#### 🛠️ Without `dotnet-add-content`

You’d typically:

1. Create a `.props` file manually:

   ```xml
   <!-- SharedContent.props -->
   <Project>
     <ItemGroup>
       <None Include="shared-assets/**/*.json">
         <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
         <Link>%(RecursiveDir)%(Filename)%(Extension)</Link>
       </None>
     </ItemGroup>
   </Project>
   ```

2. Then manually import it into each `.csproj`:

   ```xml
   <Import Project="SharedContent.props" />
   ```

This process is tedious and error-prone, especially across many projects.

#### ✅ With `dotnet-add-content`

You can do it all from the command line:

```bash
dotnet add-content --file SharedContent.props --include "shared-assets/**/*.json"
```

Then import it into any project:

```bash
dotnet add-content --project ProjectA --file SharedContent.props
dotnet add-content --project ProjectB --file SharedContent.props
```

Or do both in one step:

```bash
dotnet add-content --project ProjectA --file SharedContent.props --include "shared-assets/**/*.json"
```

This will:

* ✅ Create or update the shared `.props` file
* ✅ Append the new include if not already present
* ✅ Import it into the project if not already included

Using `.props` files this way keeps your content declarations DRY and your project files clean — perfect for scalable or enterprise-grade solutions with lots of shared data patterns.

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

MIT License © 2025 Asher Garland

---

🔗 **GitHub Repository:** [github.com/ashergarland/dotnet-add-content](https://github.com/ashergarland/dotnet-add-content)
