using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using SolutionInspector.Commons.Extensions;
using SolutionInspector.SolutionParsing.Tokenizing;

namespace SolutionInspector.SolutionParsing
{
  internal interface ISolutionParser
  {
    Solution Parse (string path);
  }

  internal class SolutionParser : ISolutionParser
  {
    private readonly ISolutionTokenizer _solutionTokenizer;

    public SolutionParser (ISolutionTokenizer solutionTokenizer)
    {
      _solutionTokenizer = solutionTokenizer;
    }

    public Solution Parse (string path)
    {
      var tokens = Tokenize(path);

      PreambleSolutionToken preamble = null;
      GlobalsSolutionToken globals = null;

      ProjectStartSolutionToken currentProjectStart = null;
      var projects = new List<Project>();

      ProjectSectionStartSolutionToken currentProjectSectionStart = null;
      ProjectSection currentProjectSection = null;
      var projectSectionItems = new List<ProjectSectionItemSolutionToken>();

      var state = ParserState.PrePreamble;

      foreach(var token in tokens)
      {
        if (state == ParserState.PostGlobals)
          ThrowTokenError(token, "No element is allowed after the globals section");

        switch (token)
        {
          case PreambleSolutionToken preambleToken:
            if (state != ParserState.PrePreamble)
              ThrowTokenError(preambleToken, "There can be only one preamble");

            preamble = preambleToken;
            state = ParserState.PostPreamble;
            break;

          case ProjectStartSolutionToken projectStart:
            if (state == ParserState.InProjectPreSection || state == ParserState.InProjectPostSection)
              ThrowTokenError(projectStart, "Project cannot be contained in another project");

            if (state == ParserState.InProjectSection)
              ThrowTokenError(projectStart, "Project cannot be contained in a project section");

            currentProjectStart = projectStart;
            state = ParserState.InProjectPreSection;

            break;

          case ProjectEndSolutionToken projectEnd:
            if (state != ParserState.InProjectPreSection && state != ParserState.InProjectPostSection)
              ThrowTokenError(projectEnd, "Unexpected project end");

            projects.Add(new Project(currentProjectStart.AssertNotNull("currentProjectStart != null"), projectEnd, currentProjectSection));
            currentProjectStart = null;

            state = ParserState.PostPreamble;

            break;

          case ProjectSectionStartSolutionToken sectionStart:
            if (state == ParserState.InProjectPostSection)
              ThrowTokenError(sectionStart, "There can only be one project section per project");

            if (state != ParserState.InProjectPreSection)
              ThrowTokenError(
                sectionStart,
                "Unexpected Project section start (a project section can only be contained in a project)");

            currentProjectSectionStart = sectionStart;
            state = ParserState.InProjectSection;

            break;

          case ProjectSectionEndSolutionToken sectionEnd:
            if(state != ParserState.InProjectSection)
              ThrowTokenError(sectionEnd, "Unexpected project section end");

            currentProjectSection = new ProjectSection(
              currentProjectSectionStart.AssertNotNull("currentProjectSectionStart != null"),
              sectionEnd,
              projectSectionItems.ToList());
            projectSectionItems = new List<ProjectSectionItemSolutionToken>();
            state = ParserState.InProjectPostSection;

            break;

          case ProjectSectionItemSolutionToken sectionItem:
            if(state != ParserState.InProjectSection)
              ThrowTokenError(sectionItem, "Unexpected project section item (can only be contained in a project section)");

            projectSectionItems.Add(sectionItem);

            break;

          case GlobalsSolutionToken globalsToken:
            if (state == ParserState.InProjectPreSection || state == ParserState.InProjectPostSection)
              ThrowTokenError(globalsToken, "The globals section cannot be contained inside a project");

            if(state == ParserState.InProjectSection)
              ThrowTokenError(globalsToken, "The globals section cannot be contained inside a project section");

            globals = globalsToken;
            state = ParserState.PostGlobals;
            break;

          default:
            throw new NotSupportedException($"Unexpected token type '{token.GetType().Name}");
        }
      }

      if (globals == null)
        throw new SolutionParsingException(-1, "Missing globals section");

      return new Solution(preamble.AssertNotNull("preamble != null"), projects, globals.AssertNotNull("globals != null"));
    }

    private void ThrowTokenError (SolutionToken token, string message) => throw new SolutionParsingException(token.LineNumber, message);

    private enum ParserState
    {
      PrePreamble,
      PostPreamble,
      InProjectPreSection,
      InProjectPostSection,
      InProjectSection,
      PostGlobals
    }

    private IEnumerable<SolutionToken> Tokenize(string path)
    {
      using (var lines = new LineEnumerator(File.ReadLines(path).GetEnumerator()))
        return _solutionTokenizer.Tokenize(lines);
    }
  }

  internal class Solution
  {
    private readonly PreambleSolutionToken _preamble;
    private readonly GlobalsSolutionToken _globals;

    public Solution (PreambleSolutionToken preamble, IReadOnlyList<Project> projects, GlobalsSolutionToken globals)
    {
      _preamble = preamble;
      Projects = projects;
      _globals = globals;
    }

    public IReadOnlyList<Project> Projects { get; }
  }

  internal class Project
  {
    private readonly ProjectStartSolutionToken _start;
    private readonly ProjectEndSolutionToken _end;
    [CanBeNull]
    private readonly ProjectSection _projectSection;

    public Project (ProjectStartSolutionToken start, ProjectEndSolutionToken end, [CanBeNull] ProjectSection projectSection)
    {
      _start = start;
      _end = end;
      _projectSection = projectSection;
    }

    public Guid Id => _start.Id;
    public Guid Type => _start.Type;
    public string Name => _start.Name;
    public string RelativePath => _start.RelativePath;

    public IEnumerable<string> Items => _projectSection != null ? _projectSection.Items : Enumerable.Empty<string>();
  }

  internal class ProjectSection
  {
    private readonly ProjectSectionStartSolutionToken _start;
    private readonly ProjectSectionEndSolutionToken _end;
    private readonly IReadOnlyList<ProjectSectionItemSolutionToken> _items;

    public ProjectSection (ProjectSectionStartSolutionToken start, ProjectSectionEndSolutionToken end, IReadOnlyList<ProjectSectionItemSolutionToken> items)
    {
      _start = start;
      _end = end;
      _items = items;
    }

    public IEnumerable<string> Items => _items.Select(t => t.Name);
  }
}