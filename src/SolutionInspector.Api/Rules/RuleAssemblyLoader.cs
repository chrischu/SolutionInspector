using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wrapperator.Interfaces.IO;
using Wrapperator.Interfaces.Reflection;

namespace SolutionInspector.Api.Rules
{
  internal interface IRuleAssemblyLoader
  {
    void LoadRuleAssemblies (IReadOnlyCollection<string> ruleAssemblyPaths);
  }

  internal class RuleAssemblyLoader : IRuleAssemblyLoader
  {
    private readonly IFile _file;
    private readonly IDirectory _directory;
    private readonly IAssembly _assembly;

    public RuleAssemblyLoader (IFile file, IDirectory directory, IAssembly assembly)
    {
      _file = file;
      _directory = directory;
      _assembly = assembly;
    }

    public void LoadRuleAssemblies (IReadOnlyCollection<string> ruleAssemblyPaths)
    {
      foreach (var ruleAssemblyPath in ruleAssemblyPaths)
      {
        try
        {
          if (IsDirectory(ruleAssemblyPath))
          {
            foreach (var file in _directory.GetFiles(ruleAssemblyPath, "*.dll"))
              LoadAssemblyFromFile(file);
          }
          else
          {
            LoadAssemblyFromFile(ruleAssemblyPath);
          }
        }
        catch (FileNotFoundException ex)
        {
          throw new RuleAssemblyNotFoundException(ruleAssemblyPath, ex);
        }
      }
    }

    private void LoadAssemblyFromFile (string ruleAssemblyPath)
    {
      IAssembly assembly;

      try
      {
        assembly = _assembly.LoadFrom(ruleAssemblyPath);
      }
      catch (Exception ex)
      {
        throw new InvalidRuleAssemblyException(ruleAssemblyPath, ex);
      }

      var ruleTypes = assembly.GetExportedTypes().Where(t => typeof(IRule).IsAssignableFrom(t));
      if (!ruleTypes.Any())
        throw new InvalidRuleAssemblyException(ruleAssemblyPath);
    }

    private bool IsDirectory (string path)
    {
      var attributes = _file.GetAttributes(path);
      return attributes.HasFlag(FileAttributes.Directory);
    }
  }
}