using System;
using SystemInterface.IO;
using FakeItEasy;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.Configuration.MsBuildParsing;
using SolutionInspector.Api.Utilities;
using SolutionInspector.TestInfrastructure.AssertionExtensions;

#region R# preamble for Machine.Specifications files

// ReSharper disable ArrangeTypeModifiers
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable NotAccessedField.Local
// ReSharper disable StaticMemberInGenericType
// ReSharper disable UnassignedField.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnassignedGetOnlyAutoProperty

#endregion

namespace SolutionInspector.Api.Tests.Utilities
{
  [Subject (typeof (SolutionLoader))]
  class SolutionLoaderSpec
  {
    static IFile File;
    static SolutionLoader SUT;

    static IMsBuildParsingConfiguration MsBuildParsingConfiguration;

    Establish ctx = () =>
    {
      File = A.Fake<IFile>();
      SUT = new SolutionLoader(File);

      MsBuildParsingConfiguration = A.Dummy<IMsBuildParsingConfiguration>();
    };

    // a positive test is not really possible here since the static Solution.Load method is not fakeable, but the positive case is covered by the
    // SolutionSpec (since the SolutionLoader does not have any special code for successful 

    class when_loading_a_non_existing_solution
    {
      Because of = () => Exception = Catch.Exception(() => SUT.Load("DOESNOTEXIST", MsBuildParsingConfiguration));

      It throws = () =>
          Exception.Should().Be<SolutionNotFoundException>().WithMessage("Could not find solution file at 'DOESNOTEXIST'.");

      static Exception Exception;
    }
  }
}