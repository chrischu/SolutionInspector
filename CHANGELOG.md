# Changelog

All notable changes to this project will be documented in this file.
This project adheres to [Semantic Versioning](http://semver.org/).

## [Unreleased](https://github.com/chrischu/SolutionInspector/compare/v0.1.0...HEAD)
### General
#### Added
* SolutionInspector.exe is now available on NuGet.org as well (<https://www.nuget.org/packages/SolutionInspector.exe/>).
* SolutionInspector's code now has 100% coverage by tests.
* SolutionInspector now has more options to define which configuration file is to be used:
  * `--configurationFile=AppConfig`: Use the SolutionInspector.exe.config file (is and was the default).
  * `--configurationFile=Solution`: Use the \<SolutionName>.sln.SolutionInspectorConfig file lying next to the inspected solution.
  * `--configurationFile=<configFilePath>`: Use the specified configuration file.
* SolutionInspector now has a new command: `initialize [-f|--force] <configFilePath>`
  * Creates a pristine copy of a SolutionInspector config file at the given path. The command will require confirmation (except when the force flag is used) when the file would be overwritten.

#### Changed
* Changed the way SolutionInspector includes MSBuild DLLs:
  * SolutionInspector no longer requires the installation of the MSBuild tools 2015, the DLLs are now directly included.

### ObjectModel
#### Added
* SolutionInspector now has better support for project items that contain wildcards (*) in their include/exclude paths or links (IProjectItem has new properties: IsLink, IsIncludedByWildcard, WildcardInclude, WildcardExclude).

### Rules
#### Added
* **ProjectReferencesMustBeValidRule**: Verifies that all ProjectReferences in the project are valid (i.e. point to existing csproj files included in the solution and have the correct project GUID).
* **ProjectGuidsMustBeUniqueRule**: Verifies that every project in the solution has a unique GUID.
* **NuGetReferenceHintPathsMustBeValidRule**: Verifies that all NuGet references in the project have correct hint paths (pointing to an actually existing file). 
* **NonDevelopmentNuGetReferencesShouldHaveIsPrivateSetToTrueRule**: Verifies that all non-development NuGet references in the project have their 'IsPrivate' flag (also referred to as 'Copy Local') set to true. 
* **ProjectItemMustBeIncludedByWildcardRule/ProjectItemMustNotBeIncludedByWildcardRule**: Verifies that project items are included/not included by wildcards.
* **RequiredResourceLanguagesRule**: Checks that all given resource files are localized in all given languages in the project.

#### Changed
* Renamed AllProjectItemsMustBePresentRule to ProjectItemsMustBePresentRule to be more in line with the newly added rules.

## [0.1.0](https://github.com/chrischu/SolutionInspector/compare/c02165e6caba2eaedc357a883ffdaf3663fce16c...v0.1.0) - 2016-06-05

* Initial release
