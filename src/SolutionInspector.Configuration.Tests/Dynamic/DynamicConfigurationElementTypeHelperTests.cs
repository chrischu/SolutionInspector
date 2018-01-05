using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using FakeItEasy;
using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;
using SolutionInspector.Configuration.Dynamic;
using SolutionInspector.TestInfrastructure;
using SolutionInspector.TestInfrastructure.AssertionExtensions;
using Wrapperator.Interfaces.Reflection;
using Wrapperator.Wrappers;

namespace SolutionInspector.Configuration.Tests.Dynamic
{
  [TestFixture]
  public class DynamicConfigurationElementTypeHelperTests
  {
    private ISchemaLocationHelper _schemaLocationHelper;
    private IAssemblyStatic _assembly;

    private IDynamicConfigurationElementTypeHelper _sut;

    private XDocument _document;
    private XElement _element;

    private string _namespace;
    private string _schemaLocation;
    private string _assemblyLocation;

    [SetUp]
    public void SetUp ()
    {
      _schemaLocationHelper = A.Fake<ISchemaLocationHelper>();
      _assembly = A.Fake<IAssemblyStatic>();

      _sut = new DynamicConfigurationElementTypeHelper(_schemaLocationHelper, _assembly);

      _namespace = "Namespace";
      _schemaLocation = $"{Assembly.GetExecutingAssembly().GetName().Name}.xsd";
      _assemblyLocation = $"{Assembly.GetExecutingAssembly().GetName().Name}.dll";

      _document = new XDocument();
      _document.Add(_element = new XElement(XName.Get(nameof(DynamicElementTypeResolverTestsElement), _namespace)));

      A.CallTo(() => _schemaLocationHelper.GetSchemaLocations(A<XDocument>._))
          .Returns(new Dictionary<string, string> { { _namespace, _schemaLocation } });

      A.CallTo(() => _assembly.LoadFrom(A<string>._)).Returns(Wrapper.Wrap(Assembly.GetExecutingAssembly()));
    }

    [Test]
    public void ResolveElementType ()
    {
      // ACT
      var result = _sut.ResolveElementType(_element);

      // ASSERT
      result.Should().Be(typeof(DynamicElementTypeResolverTestsElement));

      A.CallTo(() => _schemaLocationHelper.GetSchemaLocations(_document)).MustHaveHappened();
      A.CallTo(() => _assembly.LoadFrom(_assemblyLocation)).MustHaveHappened();
    }

    [Test]
    public void ResolveElementType_WithElementWithoutDocument_Throws ()
    {
      // ACT
      Action act = () => _sut.ResolveElementType(new XElement("element"));

      // ASSERT
      act.ShouldThrowArgumentException("The given element is not attached to a document.", "element");
    }

    [Test]
    public void ResolveElementType_WithElementWithoutSchemaLocation_Throws ()
    {
      A.CallTo(() => _schemaLocationHelper.GetSchemaLocations(A<XDocument>._)).Returns(new Dictionary<string, string>());

      // ACT
      Action act = () => _sut.ResolveElementType(_element);

      // ASSERT
      act.ShouldThrow<DynamicElementTypeResolutionException>().WithMessage(
          "For the given element's namespace 'Namespace' no schema location could be found. " +
          "Make sure that the document's root element contains an 'xsi:schemaLocation' attribute that contains a value pair for the namespace.",
          "element");
    }

    [Test]
    public void ResolveElementType_WhenAssemblyLoadingFails_Throws ()
    {
      var assemblyLoadException = Some.Exception;
      A.CallTo(() => _assembly.LoadFrom(A<string>._)).Throws(assemblyLoadException);

      // ACT
      Action act = () => _sut.ResolveElementType(_element);

      // ASSERT
      act.ShouldThrow<DynamicElementTypeResolutionException>()
          .WithMessage($"Error while loading assembly containing dynamic element type from '{_assemblyLocation}'.")
          .WithInnerException(assemblyLoadException);
    }

    [Test]
    public void ResolveElementType_WhenAssemblyDoesNotContainMatchingType_Throws ()
    {
      var element = new XElement(XName.Get("DOESNOTEXIST", _namespace));
      _element.Add(element);

      // ACT
      Action act = () => _sut.ResolveElementType(element);

      // ASSERT
      act.ShouldThrow<DynamicElementTypeResolutionException>()
          .WithMessage(
              $"Could not find type named 'DOESNOTEXIST' in assembly '{Assembly.GetExecutingAssembly().GetName().Name}' "
              + $"(loaded from '{_assemblyLocation}').");
    }

    [Test]
    public void ResolveElementType_WhenAssemblyDoesContainMultipleMatchingTypes_Throws ()
    {
      var duplicate1 = typeof(DynamicElementTypeResolverTestsDuplicateElement);
      var duplicate2 = typeof(Duplicate.DynamicElementTypeResolverTestsDuplicateElement);

      var element = new XElement(XName.Get(duplicate1.Name, _namespace));
      _element.Add(element);

      // ACT
      Action act = () => _sut.ResolveElementType(element);

      // ASSERT
      act.ShouldThrow<DynamicElementTypeResolutionException>()
          .WithMessage(
              $"There are multiple types named '{duplicate1.Name}' in assembly '{Assembly.GetExecutingAssembly().GetName().Name}' " +
              $"(loaded from '{_assemblyLocation}'): '{duplicate1.FullName}', '{duplicate2.FullName}'. This is not supported. " +
              "You need to rename one of the types so that the names become unique.");
    }

    [Test]
    public void ValidateElementCompatibility ()
    {
      var element = ConfigurationElement.Create<DynamicElementTypeResolverTestsElement>(XName.Get(nameof(DynamicElementTypeResolverTestsElement), _namespace));

      // ACT
      Action act = () => _sut.ValidateElementCompatibility(_document, element);

      // ASSERT
      act.ShouldNotThrow();
    }

    [Test]
    public void ValidateElementCompatibility_WithElementWithUnknownNamespace ()
    {
      var element = ConfigurationElement.Create<DynamicElementTypeResolverTestsElement>(XName.Get(nameof(DynamicElementTypeResolverTestsElement), "UNKNOWN"));
      
      // ACT
      Action act = () => _sut.ValidateElementCompatibility(_document, element);

      // ASSERT
      act.ShouldThrow<DynamicElementTypeCompatibilityException>().WithMessage(
          "The element belongs to a namespace ('UNKNOWN') that is not contained in the schema locations of the containing document.");
    }

    [Test]
    public void ValidateElementCompatibility_WithElementWithUnknownAssemblyName ()
    {
      var element = ConfigurationElement.Create<DynamicElementTypeResolverTestsElement>(XName.Get(nameof(DynamicElementTypeResolverTestsElement), _namespace));
      
      A.CallTo(() => _schemaLocationHelper.GetSchemaLocations(A<XDocument>._))
          .Returns(new Dictionary<string, string> { { _namespace, "otherSchemaLocation.xsd" } });

      // ACT
      Action act = () => _sut.ValidateElementCompatibility(_document, element);

      // ASSERT
      act.ShouldThrow<DynamicElementTypeCompatibilityException>().WithMessage(
          $"The element is of a type that does not seem to come from the assembly referenced in the schema location with namespace '{_namespace}'. " +
          $"The containing assembly's name is '{Assembly.GetExecutingAssembly().GetName().Name}', but the referenced assembly's name is otherSchemaLocation.");
    }
  }

  [UsedImplicitly]
  internal class DynamicElementTypeResolverTestsElement : ConfigurationElement
  {
  }

  [UsedImplicitly]
  internal class DynamicElementTypeResolverTestsDuplicateElement
  {
  }

  namespace Duplicate
  {
    [UsedImplicitly]
    internal class DynamicElementTypeResolverTestsDuplicateElement
    {
    }
  }
}