using System;
using System.Xml.Linq;
using NUnit.Framework;
using SolutionInspector.Configuration.Dynamic;
using SolutionInspector.TestInfrastructure.AssertionExtensions;

namespace SolutionInspector.Configuration.Tests.Dynamic
{
  public class SchemaLocationHelperTests
  {
    private ISchemaLocationHelper _sut;

    [SetUp]
    public void SetUp ()
    {
      _sut = new SchemaLocationHelper();
    }

    [Test]
    public void GetSchemaLocations ()
    {
      var document = XDocument.Parse(
          @"<root xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""    Namespace1
  Location1 Namespace2
Location2""/>");

      // ACT
      var result = _sut.GetSchemaLocations(document);

      // ASSERT
      result.Should().HaveCount(2);
      result.Should().Contain("Namespace1", "Location1");
      result.Should().Contain("Namespace2", "Location2");
    }

    [Test]
    public void GetSchemaLocations_WithoutSchemaLocation_ReturnsEmptyDictionary ()
    {
      var document = XDocument.Parse("<root />");

      // ACT
      var result = _sut.GetSchemaLocations(document);

      // ASSERT
      result.Should().HaveCount(0);
    }

    [Test]
    public void GetSchemaLocations_WithInvalidSchemaLocation ()
    {
      var document = XDocument.Parse(
          @"<root xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""Namespace1""/>");

      // ACT
      Action act = () => _sut.GetSchemaLocations(document);

      // ASSERT
      act.ShouldThrowArgumentException(
          "The given document does not contain a valid schema location attribute of the form 'xsi:schemaLocation=\"(<Namespace> <Location>)*\"'.",
          "document");
    }
  }
}