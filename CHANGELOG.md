# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Key-Value storage added to `Invocation`
  - This allows for saving data specific to one `Invocation`
  - Example: Save a Stopwatch and print the elapsed time in `After...()` (see `MethodWatchAdvice` in the example project)

## [0.1.0] - 2019-01-19

### Added

- Basic AOP functionality added in the form of `AspectWeaver`
  - Use `AspectConfiguration` to add functions which are run before and after the method (*Advices*)
- Basic example project found under `SimplyAOP.Example`