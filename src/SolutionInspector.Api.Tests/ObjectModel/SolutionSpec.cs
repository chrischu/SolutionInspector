﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using FakeItEasy;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.Configuration.MsBuildParsing;
using SolutionInspector.Api.Extensions;
using SolutionInspector.Api.ObjectModel;

#region R# preamble for Machine.Specifications files

// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable NotAccessedField.Local
// ReSharper disable StaticMemberInGenericType
// ReSharper disable UnassignedField.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnassignedGetOnlyAutoProperty

#endregion

namespace SolutionInspector.Api.Tests.ObjectModel
{
  [Subject (typeof(Solution))]
  class SolutionSpec
  {
    static string SolutionPath;
    static IMsBuildParsingConfiguration MsBuildParsingConfiguration;

    Establish ctx = () =>
    {
      SolutionPath = Path.Combine(
          Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath).AssertNotNull(),
          @"ObjectModel\TestData\Solution\TestSolution.sln");

      MsBuildParsingConfiguration = A.Dummy<IMsBuildParsingConfiguration>();
    };

    class when_loading
    {
      Because of = () => Result = Solution.Load(SolutionPath, MsBuildParsingConfiguration);

      It parses_solution = () =>
      {
        Result.Name.Should().Be("TestSolution");
        Result.SolutionDirectory.FullName.Should().Be(Path.GetDirectoryName(SolutionPath));
        Result.BuildConfigurations.ShouldAllBeEquivalentTo(
            new[]
            {
                new BuildConfiguration("Debug", "Any CPU"),
                new BuildConfiguration("Release", "Any CPU")
            });
        Result.Projects.Single().Name.Should().Be("EmptyProject");
        Result.Identifier.Should().Be("TestSolution.sln");
        Result.FullPath.Should().Be(SolutionPath);
      };

      static ISolution Result;
    }
  }
}