# Repository guidelines

## Project purpose

This repository contains `Megaraz.ResultPattern.Infrastructure`, a reusable
.NET extension package for infrastructure-oriented errors and integrations
built on `Megaraz.ResultPattern`. The current extraction includes database
error concepts and file-based error logging and is still being separated from
its original monolith. The intended result is a self-contained, documented,
tested, public OSS NuGet package rather than a home for application-specific
infrastructure code.

## Extraction and package boundaries

- This package may depend on `Megaraz.ResultPattern`; the core package must
  never depend on this package.
- Do not depend on the original monolith, `Rasmus.SharedKernel`, application
  projects, or application-owned interfaces. Move genuinely reusable
  contracts into this package or use suitable BCL abstractions.
- Do not depend on `Megaraz.ResultPattern.AspNetCore` or introduce HTTP,
  endpoint, presentation, authentication, or localization policy here.
- Keep persistence-provider-specific behavior isolated. Do not add Entity
  Framework Core or another provider dependency to general abstractions unless
  the integration explicitly requires and documents it.
- Remove placeholders and stale extraction artifacts rather than preserving
  them as accidental public API.
- Before the first public release, distinguish intentional public contracts
  from incomplete monolith-era code. After release, preserve those contracts
  according to semantic versioning.

## Architecture and behavior

- Infrastructure-specific errors should build on the core `Error` contract and
  use `ErrorType.External`; keep their detailed classification in this
  package.
- Use the current public API of the referenced core package. Do not recreate
  removed core members or extend legacy enums merely to make extracted code
  compile.
- Keep error codes stable and machine-readable. Do not derive user-facing
  messages from exception text or expose database, filesystem, stack-trace,
  connection, credential, or path details to consumers by default.
- Logging must be explicit and policy-driven. Avoid hidden global side effects,
  unbounded reads, silent data loss, and unsafe concurrent access.
- Honor cancellation for asynchronous I/O. Keep file ownership, encoding,
  retention, corruption handling, and concurrency behavior deterministic and
  documented.
- Prefer small package-owned abstractions and composition. Application-specific
  logging destinations, retention rules, and operational policies belong in
  consuming applications.
- Avoid dependencies unless they provide substantial value for a clearly
  defined integration.

## Technical and release constraints

- Nullable reference types and implicit usings are enabled and must remain
  effective. Treat compiler and analyzer warnings seriously.
- The project currently targets `net10.0`. When productionizing the package,
  choose and document supported target frameworks deliberately, considering
  alignment with the `net8.0`, `net9.0`, and `net10.0` support matrix of the
  sibling packages.
- Keep the minimum compatible `Megaraz.ResultPattern` version deliberate and
  test against the package contract consumers will actually restore.
- Preserve semantic versioning once a public version exists. Explicitly report
  public API, behavior, dependency, target-framework, and serialized-format
  compatibility risks.
- Public APIs require XML documentation. Public behavior, configuration,
  security considerations, and examples belong in the README.
- Before publishing, ensure the project has intentional NuGet metadata,
  license, repository links, Source Link, packaged README, release notes, CI,
  and a release workflow consistent with the sibling packages.

## Testing and packaging

New or changed behavior requires tests. If the solution still lacks a test
project, adding one is part of productionizing public behavior. Cover error
construction and invariants, serialization compatibility, malformed log
entries, retention boundaries, cancellation, concurrent readers/writers,
filesystem failures, and configuration validation as relevant.

Before completing implementation work, run:

1. `dotnet restore Megaraz.ResultPattern.Infrastructure.slnx`
2. `dotnet build Megaraz.ResultPattern.Infrastructure.slnx --configuration Release --no-restore`
3. `dotnet test Megaraz.ResultPattern.Infrastructure.slnx --configuration Release --no-build`
4. `dotnet pack Megaraz.ResultPattern.Infrastructure/Megaraz.ResultPattern.Infrastructure.csproj --configuration Release --no-build --no-restore`

Report failures, warnings, absent test coverage, skipped validation, and public
API, serialized-format, dependency, or package changes.

## Working style

- Start non-trivial tasks by inspecting the extracted code, the current core
  package API, sibling-package conventions, and existing uncommitted work,
  then present a short implementation plan.
- Make focused changes and separate extraction cleanup from behavioral
  redesign where practical.
- Do not reformat unrelated files or overwrite unrelated work in a dirty
  worktree.
- Do not copy a monolith abstraction into the package without first checking
  that it is generally useful and belongs within this package's boundary.
- Summarize changed files, design decisions, verification performed, remaining
  extraction work, and compatibility or security considerations.
