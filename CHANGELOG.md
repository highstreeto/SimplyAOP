# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.2.0] - 2019-02-02

### Added

- Key-Value store added to `Invocation`
  - This allows for saving data specific to one `Invocation`
  - Example: Save a Stopwatch and print the elapsed time in `After...()` (see `MethodWatchAdvice` in the example project)

### Changed

- Lookup of Method now includes parameter types
  - Was only based on name
  - Now the parameters are determined by TParam (extracted if it is a `ValueTuple`)

## [0.1.0] - 2019-01-19

### Added

- Basic AOP functionality added in the form of `AspectWeaver`
  - Use `AspectConfiguration` to add functions which are run before and after the method (*Advices*)
- Basic example project found under `SimplyAOP.Example`