# Roadmap

Already planned features/fixes for SolutionInspector are documented here.

## Next Version (v0.2.0)

- [x] Release SolutionInspector.exe on NuGet as well
- [x] 100% code coverage
- [x] Wildcard support (see [#2](../issues/2))
- [ ] Change where the configuration file is located (next to solution instead of SolutionInspector.exe)
- [ ] Allow specifying a special configuration file via command line
- [ ] More rules
  - [x] See [#1](../issues/1)
  - [x] Project GUIDs should be unique
  - [x] NuGet references should have correct hint path
  - [x] non-development NuGet references should have Private (=CopyLocal) set to true
  - [x] Project item(s) should only be included with wildcard
  - [x] Project item(s) should only be included without wildcard

## Medium-Term

* Configuration UI (probably powered by WPF)

## Long-Term

* Fixes (violations allow automatical fixing)
* GUI reporting
