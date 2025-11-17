# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
### Changed
### Deprecated
### Removed
### Fixed
### Security

## [0.0.5] - 2025-11-17

### Added

- Added comment entity and related migrations.
- Added comment related commands.
- Added comprehensive unit tests for comment related features.
- Added 'show board' command to display project board.

### Changed

- Changed 'show project' command to include ticket counts by status and comments.
- Changed 'show ticket' command to include comments.

## [0.0.4] - 2025-11-16

### Added

- Added ticket entity and related migrations.
- Added ticket related commands.
- Added comprehensive unit tests for ticket related features.
- Added status filtering to the 'list tickets' command.
- Added case insensitive handling for project, ticket and status keys.
- Added unit tests for queries and repositories.

## [0.0.3] - 2025-11-15

### Added

- Added status entity and related migrations.
- Added status related commands.
- Added comprehensive unit tests status related features.

### Changed

- Refactored test file structure and improved test comments.

### Fixed

- Adds missing migration for sequence entity.
- Fixes issue with request validation in mediation layer.

## [0.0.2] - 2025-11-11

### Added

- Added sequences entity for ticket and iteration numbering.
- Added comprehensive unit tests for project sequences.

### Changed

- Updated project entity to include sequences collection.

## [0.0.1]

### Fixed

- Fixed issue with change log generation script not appending the unreleased link correctly.

## [0.0.1-beta] - 2025-11-10

### Added

- Added unit tests for project entity
- Added unit tests for project command handlers and validators
- Added unit tests for project repository
- Added unit tests for mediation service
- Added unit tests for database logging service
- Added unit tests for command execution pipeline
- Added unit tests for project commands

### Changed

- Refactored project entity is now immutable with private setters and update methods

## [0.0.1-alpha.1] - 2025-10-28

### Changed

- Changed tool command name from "spokesoft" to "lucy".

### Fixed

- Fixed issue with release workflow not pushing changes to master branch.

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

[unreleased]: https://github.com/spokesoft/lucy/compare/v0.0.5...HEAD
[0.0.5]: https://github.com/spokesoft/lucy/compare/v0.0.4...v0.0.5
[0.0.4]: https://github.com/spokesoft/lucy/compare/v0.0.3...v0.0.4
[0.0.3]: https://github.com/spokesoft/lucy/compare/v0.0.2...v0.0.3
[0.0.2]: https://github.com/spokesoft/lucy/compare/v0.0.1...v0.0.2
[0.0.1]: https://github.com/spokesoft/lucy/compare/v0.0.1-beta...v0.0.1
[0.0.1-beta]: https://github.com/spokesoft/lucy/compare/v0.0.1-alpha.1...v0.0.1-beta
[0.0.1-alpha.1]: https://github.com/spokesoft/lucy/compare/v0.0.1-alpha...v0.0.1-alpha.1
[0.0.1-alpha]: https://github.com/spokesoft/lucy/releases/tag/v0.0.1-alpha.1
