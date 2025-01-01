# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [3.5.0] - 2025-01-01

### Added

- Added getter function to pass a custom delta time to delays (Thanks to @Sebsii)

## [3.4.0] - 2024-01-30

### Changed

- Moved `Stop` and `Complete` methods to extension methods with additional null checks

## [3.3.0] - 2023-03-29

### Added

- Added method to reset the internal state

## [3.2.1] - 2023-03-12

### Added

- Added null check before trying to stop a coroutine

## [3.2.0] - 2023-01-17

### Added

- Added pause and resume methods for delays

## [3.1.1] - 2022-12-30

### Fixed

- Fixed package version

## [3.1.0] - 2022-12-30

### Added

- Added method to complete a delay

## [3.0.1] - 2022-12-07

### Fixed

- Fixed package version

## [3.0.0] - 2022-12-07

### Changed

- `DelayMonoBehaviour` will no longer be destroyed after a scene change
- Rewrote the `Delay` class to use `IEnumerator` directly to stop the coroutine

## [2.1.1] - 2022-09-28

### Added

- Added Unity `.meta` files

## [2.1.0] - 2022-09-26

### Added

- Conditions for `WaitUntil` are now evaluated before starting a coroutine

### Changed

- Negative delays do not start coroutines anymore

## [2.0.0] - 2022-09-10

### Changed

- `Coroutine` is now returned from methods

## [1.0.1] - 2022-08-11

### Changed

- Updated README

## [1.0.0] - 2022-08-11

### Added

- Initial release
