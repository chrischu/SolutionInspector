using System.Xml.Linq;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Wrapperator.Interfaces.Xml.Linq;

namespace SolutionInspector.Configuration.Tests
{
  public class ConfigurationManagerTests
  {
    private IXDocumentStatic _xDocumentStatic;

    private IConfigurationManager _sut;

    [SetUp]
    public void SetUp ()
    {
      _xDocumentStatic = A.Fake<IXDocumentStatic>();

      _sut = new ConfigurationManager(_xDocumentStatic);
    }

    [Test]
    public void LoadSection ()
    {
      var path = "path";

      var xDocument = XDocument.Parse("<element />");

      var xDocumentWrapper = A.Fake<IXDocument>();
      A.CallTo(() => xDocumentWrapper._XDocument).Returns(xDocument);

      A.CallTo(() => _xDocumentStatic.Load(A<string>._)).Returns(xDocumentWrapper);

      // ACT
      var result = _sut.LoadDocument<DummyConfigurationDocument>("path");

      // ASSERT
      result.Element.Should().BeSameAs(xDocument.Root);

      A.CallTo(() => _xDocumentStatic.Load(path)).MustHaveHappened();
    }

    private class DummyConfigurationDocument : ConfigurationDocument
    {
    }
  }
}
