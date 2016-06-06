using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using JetBrains.Annotations;
using NLog;

namespace SolutionInspector.Api
{
  internal interface IMsBuildInstallationChecker
  {
    bool IsMsBuildInstalled ();
    void SuggestInstallation ();
  }

  [UsedImplicitly /* by Autofac container */]
  [ExcludeFromCodeCoverage]
  internal class MsBuildInstallationChecker : IMsBuildInstallationChecker
  {
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public bool IsMsBuildInstalled ()
    {
      _logger.Info("Checking for 'MSBuild Tools 2015'...");

      try
      {
        Assembly.Load("Microsoft.Build, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
        _logger.Info("Check successful, 'MSBuild Tools 2015' are installed.");
        return true;
      }
      catch (FileNotFoundException)
      {
        _logger.Error(@"Could not find MSBuild assemblies in version 14.0. This most likely means that 'MSBuild Tools 2015' was not installed.");
        return false;
      }
    }

    public void SuggestInstallation ()
    {
      Console.WriteLine(
          "Just press the RETURN key to open a browser with the download page of the 'MSBuild Tools 2015' " +
          "or press any other key to cancel...");

      var key = Console.ReadKey();
      if (key.Key == ConsoleKey.Enter)
        Process.Start("https://www.microsoft.com/en-us/download/details.aspx?id=48159");
    }
  }
}