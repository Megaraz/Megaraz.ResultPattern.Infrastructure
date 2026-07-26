# Contributing to Megaraz.ResultPattern.Infrastructure

Thank you for your interest in contributing. This package provides
provider-neutral infrastructure error extensions for `Megaraz.ResultPattern`.

## Development setup

Install the .NET 10 SDK, then clone the repository and run the test suite:

```bash
git clone https://github.com/Megaraz/Megaraz.ResultPattern.Infrastructure.git
cd Megaraz.ResultPattern.Infrastructure
dotnet test Megaraz.ResultPattern.Infrastructure.slnx --configuration Release
```

The solution targets .NET 8, .NET 9, and .NET 10. Tests run for each target
framework. GitHub Actions runs the same restore, build, test, and pack steps
for pushes and pull requests targeting `main`.

## Making changes

- Keep changes focused on the behavior being addressed.
- Preserve nullable reference type safety and existing public API behavior
  unless the change intentionally updates that contract.
- Keep provider-specific dependencies out of general abstractions.
- Add or update tests for new or changed public behavior.
- Update the README or XML documentation when public behavior changes.

Before opening a pull request, run:

```bash
dotnet test Megaraz.ResultPattern.Infrastructure.slnx --configuration Release
```

## License

By contributing, you agree that your contributions will be licensed under the
MIT License included in this repository.
