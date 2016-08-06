using System;
using System.Xml;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.ObjectModel;
using SolutionInspector.TestInfrastructure;

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

namespace SolutionInspector.Tests.ObjectModel
{
  [Subject (typeof(NuGetPackage))]
  class NuGetPackageSpec
  {
    Establish ctx = () => { };

    class when_creating_from_xml
    {
      Establish ctx = () => { PackageElement = CreateXmlElement(@"<package id=""Id"" version=""1.2.3.4"" targetFramework=""TargetFramework"" />"); };

      Because of = () => Result = NuGetPackage.FromXmlElement(PackageElement);

      It parses_xml = () =>
      {
        Result.Id.Should().Be("Id");
        Result.Version.Should().Be(new Version(1, 2, 3, 4));
        Result.FullVersionString.Should().Be("1.2.3.4");
        Result.IsDevelopmentDependency.Should().BeFalse();
        Result.IsPreRelease.Should().BeFalse();
        Result.PreReleaseTag.Should().BeNull();
        Result.TargetFramework.Should().Be("TargetFramework");
      };

      static XmlElement PackageElement;
      static NuGetPackage Result;
    }

    class when_creating_from_xml_with_pre_release_tag_and_development_dependency
    {
      Establish ctx = () =>
      {
        PackageElement =
            CreateXmlElement(@"<package id=""Id"" version=""1.2.3.4-pre"" targetFramework=""TargetFramework"" developmentDependency=""true"" />");
      };

      Because of = () => Result = NuGetPackage.FromXmlElement(PackageElement);

      It parses_xml = () =>
      {
        Result.Id.Should().Be("Id");
        Result.Version.Should().Be(new Version(1, 2, 3, 4));
        Result.FullVersionString.Should().Be("1.2.3.4-pre");
        Result.IsDevelopmentDependency.Should().BeTrue();
        Result.IsPreRelease.Should().BeTrue();
        Result.PreReleaseTag.Should().Be("-pre");
        Result.TargetFramework.Should().Be("TargetFramework");
      };

      static XmlElement PackageElement;
      static NuGetPackage Result;
    }

    class when_creating_from_xml_with_non_true_development_dependency
    {
      It works_with_false = () =>
          NuGetPackage.FromXmlElement(
              CreateXmlElement(@"<package id=""Id"" version=""1.2.3.4-pre"" targetFramework=""TargetFramework"" developmentDependency=""false"" />"))
              .IsDevelopmentDependency.Should()
              .BeFalse();

      It works_with_any_other_value = () =>
          NuGetPackage.FromXmlElement(
              CreateXmlElement(
                  $@"<package id=""Id"" version=""1.2.3.4-pre"" targetFramework=""TargetFramework"" developmentDependency=""{Some.String()}"" />"))
              .IsDevelopmentDependency.Should()
              .BeFalse();
    }

    class when_comparing_equality
    {
      Establish ctx = () =>
      {
        A = new NuGetPackage(Some.String(), Some.Version, Some.Boolean, Some.String(), Some.String(), Some.Boolean);
        EqualToA = new NuGetPackage(A.Id, A.Version, A.IsPreRelease, A.PreReleaseTag, A.TargetFramework, A.IsDevelopmentDependency);
        DifferentFromA = new NuGetPackage(Some.String(), Some.Version, Some.Boolean, Some.String(), Some.String(), Some.Boolean);
      };

      Because of = () =>
      {
        /* Actual tests are in the its. */
      };

      It works_with_same_reference = () =>
          A.Equals(A).Should().BeTrue();

      It works_with_null = () =>
          A.Equals(null).Should().BeFalse();

      It works_with_equal_instances = () =>
          A.Equals(EqualToA).Should().BeTrue();

      It works_with_differing_instances = () =>
          A.Equals(DifferentFromA).Should().BeFalse();

      It works_with_same_reference_as_object = () =>
          A.Equals((object) A).Should().BeTrue();

      It works_with_null_as_object = () =>
          A.Equals((object) null).Should().BeFalse();

      It works_with_equality_operator = () =>
          (A == EqualToA).Should().BeTrue();

      It works_with_inequality_operator = () =>
          (A != EqualToA).Should().BeFalse();

      static NuGetPackage A;
      static NuGetPackage EqualToA;
      static NuGetPackage DifferentFromA;
    }

    static XmlElement CreateXmlElement (string xml)
    {
      var doc = new XmlDocument();
      doc.LoadXml(xml);
      return doc.DocumentElement;
    }
  }
}