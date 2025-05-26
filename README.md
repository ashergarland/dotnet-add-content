# dotnet-add-content

![NuGet](https://img.shields.io/nuget/v/dotnet-add-content?label=nuget)

**`dotnet-add-content`** is a custom .NET CLI tool that helps you add content file includes to `.csproj` or `.props` files. It supports inline additions, reusable props files, and automatic import wiring.

Most `dotnet` commands let you create and configure projects from the command line. However, there is no built-in way to include content files like images, CSVs, or configuration files in your build without manually editing the `.csproj`.

This tool fills that gap by providing a clean and scriptable way to manage content includes using a simple CLI workflow.

You can use it to include static files like images, data files, and embedded assets in your project in a consistent and maintainable way.

## ✨ Features

- ✅ Add `<None Include="...">` entries directly to `.csproj` (inline)
- ✅ Generate or update `.props` files with shared content includes
- ✅ Automatically import `.props` files into `.csproj`
- ✅ Supports glob patterns (`data/**/*.csv`, `*.json`, `docs/file?.md`, etc.)
- ✅ Smart behavior: skip duplicates or use `--overwrite` to regenerate
- ✅ Fully validated CLI with rich error messages
- ✅ Friendly with MSBuild and NuGet packing

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

## 📦 Installation

Install globally from [NuGet](https://www.nuget.org/packages/dotnet-add-content):

```bash
dotnet tool install --global dotnet-add-content
````

> ✅ Requires [.NET 8 SDK or later](https://dotnet.microsoft.com/en-us/download)

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

### 3️⃣ Import a `.props` file into a `.csproj`

```bash
dotnet add-content --project MyLib.csproj --file Shared.props
```

Adds:

```xml
<Import Project="Shared.props" />
```

If the import already exists, it will not be duplicated.

### 4️⃣ Do both in one step

```bash
dotnet add-content --project MyApp.csproj --file Shared.props --include "resources/*.json"
```

* Adds or updates the `.props` file
* Adds `<Import>` to your `.csproj`

## 🧪 Testing

```bash
dotnet test
```

Includes full coverage for:

* 🔧 Inline and props editing
* 🧠 Smart CLI validation rules
* 🌀 Idempotent glob appends and import deduplication
* ⚠️ Error handling and invalid usage

## ✅ Develop and Run Locally

This is useful when developing or testing local changes before publishing.

### 🧼 1. Uninstall any old global version

```bash
dotnet tool uninstall -g dotnet-add-content
```

### 🔨 2. Build the NuGet package

```bash
dotnet pack -c Release
```

Generates:

```
./DotnetAddContent/bin/Release/dotnet-add-content.1.0.0.nupkg
```

### ➕ 3. Reinstall from local build

```bash
dotnet tool install --global --add-source ./DotnetAddContent/bin/Release dotnet-add-content
```

✅ Will run using .NET 9 if available, or fallback to .NET 8 (thanks to `<RollForward>Major</RollForward>`)

### 🚀 4. Run the CLI

```bash
dotnet add-content --help
```

🎉 See the full help output and usage examples.

## 🤝 Contributing

We welcome PRs, suggestions, and feedback!

To get started:

```bash
git clone https://github.com/ashergarland/dotnet-add-content.git
cd dotnet-add-content
dotnet build
dotnet test
```

## 📄 License

MIT License © 2025 Asher Garland
[View License](LICENSE)

## 📦 NuGet Package

📦 [nuget.org/packages/dotnet-add-content](https://www.nuget.org/packages/dotnet-add-content)


### 📦 Releasing a New Version to NuGet

This project uses GitHub Actions to automatically publish to [nuget.org](https://www.nuget.org/packages/dotnet-add-content) when a new version is tagged.

#### 🔑 1. Prerequisites

* A GitHub Actions secret `NUGET_API_KEY` must be configured in the repo:

  * [Create an API key on nuget.org](https://www.nuget.org/account/apikeys)
  * Scope it to `dotnet-add-content` only
  * Add it as a [GitHub secret](https://github.com/{your-org}/{your-repo}/settings/secrets/actions) with name: `NUGET_API_KEY`

#### 🚀 2. Publish a Release

Bump your version in the `.csproj`, then:

```bash
git commit -am "release: v1.0.1"
git tag v1.0.1
git push origin main --tags
```

This will:

* Run tests
* Build and pack the tool
* Push to NuGet using your API key

### 📌 Notes

* `dotnet tool install -g dotnet-add-content` will get the latest published version
* The CLI targets `.NET 8+` and uses `<RollForward>Major</RollForward>` so it works on .NET 9+

## 👏 Acknowledgements

Built with:

* [System.CommandLine](https://github.com/dotnet/command-line-api)
* `System.Xml.Linq` for safe `.csproj` and `.props` editing

---