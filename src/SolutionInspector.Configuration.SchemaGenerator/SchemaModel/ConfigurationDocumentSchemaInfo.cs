using System;
using SolutionInspector.Configuration;

namespace SolutionInspector.SchemaGenerator.SchemaModel
{
  /// <summary>
  ///   Schema info for a <see cref="ConfigurationDocument" />.
  /// </summary>
  public class ConfigurationDocumentSchemaInfo
  {
    public ConfigurationElementSchemaInfo RootElement { get; }

    public ConfigurationDocumentSchemaInfo (ConfigurationElementSchemaInfo rootElement)
    {
      RootElement = rootElement;
    }
  }
}