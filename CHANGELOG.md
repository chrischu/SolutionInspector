# Changelog

All notable changes to this project will be documented in this file.
This project adheres to [Semantic Versioning](http://semver.org/).

## [Unreleased](https://github.com/chrischu/SolutionInspector/compare/v0.1.0...HEAD)
### General
#### Added
* SolutionInspector.exe is now available on NuGet.org as well (<https://www.nuget.org/packages/SolutionInspector.Console/>).
* SolutionInspector's code now has 100% coverage by tests.

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

#### Changed
* Renamed AllProjectItemsMustBePresentRule to ProjectItemsMustBePresentRule to be more in line with the newly added rules.

## 0.1.0 - 2016-06-05

* Initial release
