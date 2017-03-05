using System;
using System.Collections;
using System.Xml;
using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;
using SolutionInspector.Internals.ObjectModel;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.Internals.Tests.ObjectModel
{
  public class NuGetPackageTests
  {
    [Test]
    public void CreateFromXmlElement ()
    {
      var packageElement = CreateXmlElement(@"<package id=""Id"" version=""1.2.3.4"" targetFramework=""TargetFramework"" />");

      // ACT
      var result = NuGetPackage.FromXmlElement(packageElement);

      // ASSERT
      result.ShouldBeEquivalentTo(new NuGetPackage("Id", new Version(1, 2, 3, 4), false, null, "TargetFramework", false));
    }

    [Test]
    public void CreateFromXmlElement_WithPreReleaseTagAndDevelopmentDependency ()
    {
      var developmentDependency = Some.Boolean;
      var developmentDependencyString = developmentDependency ? "true" : "false";
      var packageElement =
          CreateXmlElement(
            $@"<package 
id=""Id"" 
version=""1.2.3.4-pre"" 
targetFramework=""TargetFramework"" 
developmentDependency=""{developmentDependencyString}"" />");

      // ACT
      var result = NuGetPackage.FromXmlElement(packageElement);

      // ASSERT
      result.ShouldBeEquivalentTo(new NuGetPackage("Id", new Version(1, 2, 3, 4), true, "-pre", "TargetFramework", developmentDependency));
    }

    [Test]
    [TestCaseSource (nameof(EqualsTestData))]
    public bool Equals (NuGetPackage a, [CanBeNull] NuGetPackage b)
    {
      // ACT & ASSERT
      return a.Equals(b);
    }

    private static IEnumerable EqualsTestData ()
    {
      var a = new NuGetPackage(Some.String(), Some.Version, Some.Boolean, Some.String(), Some.String(), Some.Boolean);
      var equalToA = new NuGetPackage(a.Id, a.Version, a.IsPreRelease, a.PreReleaseTag, a.TargetFramework, a.IsDevelopmentDependency);
      var differentFromA = new NuGetPackage(Some.String(), Some.Version, Some.Boolean, Some.String(), Some.String(), Some.Boolean);

      yield return new TestCaseData(a, a) { ExpectedResult = true, TestName = "Equals_ReferenceEquality" };
      yield return new TestCaseData(a, equalToA) { ExpectedResult = true };
      yield return new TestCaseData(a, differentFromA) { ExpectedResult = false };

      yield return new TestCaseData(a, null) { ExpectedResult = false };
    }

    [Test]
    [TestCaseSource (nameof(EqualsWithObjectsTestData))]
    public bool Equals_WithObjects (object a, [CanBeNull] object b)
    {
      // ACT & ASSERT
      return a.Equals(b);
    }

    private static IEnumerable EqualsWithObjectsTestData ()
    {
      var a = new NuGetPackage(Some.String(), Some.Version, Some.Boolean, Some.String(), Some.String(), Some.Boolean);

      yield return new TestCaseData(a, default(object)) { ExpectedResult = false };
      yield return new TestCaseData(a, a) { ExpectedResult = true };
    }

    [Test]
    public void EqualityOperators ()
    {
      var a = new NuGetPackage(Some.String(), Some.Version, Some.Boolean, Some.String(), Some.String(), Some.Boolean);
      var equalToA = new NuGetPackage(a.Id, a.Version, a.IsPreRelease, a.PreReleaseTag, a.TargetFramework, a.IsDevelopmentDependency);
      var differentFromA = new NuGetPackage(Some.String(), Some.Version, Some.Boolean, Some.String(), Some.String(), Some.Boolean);

      // ACT & ASSERT
      (a == equalToA).Should().BeTrue();
      (a != equalToA).Should().BeFalse();

      (a == differentFromA).Should().BeFalse();
      (a != differentFromA).Should().BeTrue();
    }

    private XmlElement CreateXmlElement (string xml)
    {
      var doc = new XmlDocument();
      doc.LoadXml(xml);
      return doc.DocumentElement;
    }
  }
}