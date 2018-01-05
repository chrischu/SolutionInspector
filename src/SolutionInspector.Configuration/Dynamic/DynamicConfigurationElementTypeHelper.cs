using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using SolutionInspector.Commons.Extensions;
using Wrapperator.Interfaces.Reflection;

namespace SolutionInspector.Configuration.Dynamic
{
  /// <summary>
  ///   Resolves the type for a given dynamic configuration element.
  /// </summary>
  public interface IDynamicConfigurationElementTypeHelper
  {
    Type ResolveElementType (XElement element);
    void ValidateElementCompatibility (XDocument document, ConfigurationElement element);
  }

  internal class DynamicConfigurationElementTypeHelper: IDynamicConfigurationElementTypeHelper
  {
    private readonly ISchemaLocationHelper _schemaLocationHelper;
    private readonly IAssemblyStatic _assembly;

    public DynamicConfigurationElementTypeHelper (ISchemaLocationHelper schemaLocationHelper, IAssemblyStatic assembly)
    {
      _schemaLocationHelper = schemaLocationHelper;
      _assembly = assembly;
    }

    /// <summary>
    ///   Resolves the element type for a given <paramref name="element" />.
    /// </summary>
    /// <exception cref="DynamicElementTypeResolutionException">Throw when type resolution fails.</exception>
    public Type ResolveElementType (XElement element)
    {
      var elementNamespace = element.Name.NamespaceName;
      var elementName = element.Name.LocalName;
      var document = element.Document;

      if (document == null)
        throw new ArgumentException("The given element is not attached to a document.", nameof(element));

      var schemaLocations = _schemaLocationHelper.GetSchemaLocations(document);

      if (!schemaLocations.TryGetValue(elementNamespace, out var schemaLocation))
        throw new DynamicElementTypeResolutionException(
            $"For the given element's namespace '{elementNamespace}' no schema location could be found. " +
            "Make sure that the document's root element contains an 'xsi:schemaLocation' attribute that contains a value pair for the namespace.");

      var assemblyLocation = Path.ChangeExtension(schemaLocation.AssertNotNull(), "dll");

      var assembly = LoadAssembly(assemblyLocation);

      var matchingTypes = assembly.GetTypes().Where(t => t.Name == elementName).ToList();

      if (matchingTypes.Count == 0)
      {
        throw new DynamicElementTypeResolutionException(
            $"Could not find type named '{elementName}' in assembly '{assembly.GetName().Name}' (loaded from '{assemblyLocation}').");
      }

      if (matchingTypes.Count > 1)
      {
        var matchingTypesString = matchingTypes.ConvertAndJoin(t => $"'{t.FullName}'", ", ");

        throw new DynamicElementTypeResolutionException(
            $"There are multiple types named '{elementName}' in assembly '{assembly.GetName().Name}' " +
            $"(loaded from '{assemblyLocation}'): {matchingTypesString}. This is not supported. " +
            "You need to rename one of the types so that the names become unique.");
      }

      return matchingTypes[0];
    }

    public void ValidateElementCompatibility (XDocument document, ConfigurationElement element)
    {
      var elementNamespace = element.Element.Name.NamespaceName;
      var schemaLocations = _schemaLocationHelper.GetSchemaLocations(document);

      if (!schemaLocations.TryGetValue(elementNamespace, out var schemaLocation))
      {
        throw new DynamicElementTypeCompatibilityException(
            $"The element belongs to a namespace ('{elementNamespace}') that is not contained in the schema locations of the containing document.");
      }

      var containingAssemblyName = element.GetType().Assembly.GetName().Name;
      var referencedAssemblyName = Path.GetFileNameWithoutExtension(schemaLocation);

      if (!schemaLocation.EndsWith($"{containingAssemblyName}.xsd"))
      {
        throw new DynamicElementTypeCompatibilityException(
            $"The element is of a type that does not seem to come from the assembly referenced in the schema location with namespace '{elementNamespace}'. " +
            $"The containing assembly's name is '{containingAssemblyName}', but the referenced assembly's name is {referencedAssemblyName}.");
      }
    }

    private IAssembly LoadAssembly (string assemblyLocation)
    {
      try
      {
        return _assembly.LoadFrom(assemblyLocation);
      }
      catch (Exception ex)
      {
        throw new DynamicElementTypeResolutionException(
            $"Error while loading assembly containing dynamic element type from '{assemblyLocation}'.",
            ex);
      }
    }
  }
}