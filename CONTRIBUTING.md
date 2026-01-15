# Contributing to AutoReacto

🎉 First off, thanks for taking the time to contribute!

## Project Structure

AutoReacto consists of two main projects:
- **AutoReacto** - The Discord bot (Console application)
- **AutoReacto.Dashboard** - WPF management interface (Windows desktop app)

## How Can I Contribute?

### Reporting Bugs

Before creating bug reports, please check the existing issues as you might find out that you don't need to create one. When you are creating a bug report, please include as many details as possible:

- **Use a clear and descriptive title**
- **Describe the exact steps to reproduce the problem**
- **Provide specific examples**
- **Describe the behavior you observed and what behavior you expected**
- **Include logs if possible** (found in the `logs/` directory)
- **Specify which component** (Bot or Dashboard)

### Suggesting Enhancements

Enhancement suggestions are tracked as GitHub issues. When creating an enhancement suggestion, please include:

- **Use a clear and descriptive title**
- **Provide a step-by-step description of the suggested enhancement**
- **Explain why this enhancement would be useful**
- **Specify if it applies to Bot, Dashboard, or both**

### Pull Requests

1. Fork the repo and create your branch from `main`
2. If you've added code, add tests if applicable
3. Ensure the code follows the existing style
4. Make sure your code lints
5. Test both Bot and Dashboard if your changes affect shared code
6. Issue that pull request!

## Development Setup

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Windows (required for WPF Dashboard development)
- Visual Studio 2022 or VS Code with C# extension

### Running Locally
```bash
# Clone the repository
git clone https://github.com/asuna48/C-Sharp-AutoReacto.git
cd C-Sharp-AutoReacto

# Restore dependencies
dotnet restore

# Run the bot
cd src/AutoReacto
dotnet run

# Or run the dashboard
cd src/AutoReacto.Dashboard
dotnet run
```

## Styleguides

### Git Commit Messages

- Use the present tense ("Add feature" not "Added feature")
- Use the imperative mood ("Move cursor to..." not "Moves cursor to...")
- Limit the first line to 72 characters or less
- Reference issues and pull requests liberally after the first line
- Use prefixes when applicable:
  - `[Bot]` for bot-only changes
  - `[Dashboard]` for dashboard-only changes
  - `[Core]` for shared/core changes

### C# Styleguide

- Follow Microsoft's [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use meaningful names for variables, methods, and classes
- Add XML documentation comments for public APIs
- Keep methods small and focused
- Use interfaces for dependency injection

### WPF/XAML Styleguide

- Use resource dictionaries for reusable styles
- Follow MVVM pattern where applicable
- Keep XAML clean and properly indented
- Use meaningful names for controls (x:Name)

## Localization

When adding new UI strings to the Dashboard:
1. Add the string to `LocalizationManager.cs`
2. Provide both Turkish and English translations
3. Use binding syntax: `{Binding Strings.YourNewString}`

## Code of Conduct

This project and everyone participating in it is governed by our Code of Conduct. By participating, you are expected to uphold this code.

## Questions?

Feel free to open an issue with your question or reach out to the maintainers.

Thank you! ❤️
