# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
### Changed

- Changed tool command name from "spokesoft" to "lucy".

### Deprecated
### Removed
### Fixed

- Fixed issue with release workflow not pushing changes to master branch.

### Security

## [0.0.1-alpha] - 2025-10-28

### Added

- Change log file
- Configuration for version control and code formatting
- Initial project structure (clean architecture)
- Foundation for unit testing with xUnit and Moq
- Project entity and type configuration
- Support for localization and internationalization
- Entity framework core integration with sqlite
- Unit of work and repository patterns
- Database logging service and provider
- Initial database migrations (app data and logging)
- CQRS pattern implementation (enforced with separate read/write contexts)
- Request validation and handling with mediation pattern
- Initial command tree structure for CLI commands
- Command execution pipeline and integration with Spectre.Console.Cli
- Middleware for migrations, timing, validation, and error handling
- Basic error handling and user feedback mechanisms
- Initial CI/CD pipeline setup and build scripts
- Extension methods for common operations and DI registration
- MIT License

[unreleased]: https://github.com/spokesoft/lucy/compare/v0.0.1-alpha...HEAD
[0.0.1-alpha]: https://github.com/spokesoft/lucy/releases/tag/v0.0.1-alpha
