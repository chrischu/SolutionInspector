using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Schema;
using SolutionInspector.Commons.Extensions;

namespace SolutionInspector.Configuration.Dynamic
{
  internal interface ISchemaLocationHelper
  {
    IReadOnlyDictionary<string, string> GetSchemaLocations (XDocument document);
  }

  internal class SchemaLocationHelper : ISchemaLocationHelper
  {
    public IReadOnlyDictionary<string, string> GetSchemaLocations (XDocument document)
    {
      var root = document.Root.AssertNotNull();
      var schemaLocationAttribute = root.Attribute(XName.Get("schemaLocation", XmlSchema.InstanceNamespace));

      var result = new Dictionary<string, string>();

      if (schemaLocationAttribute == null)
        return result;

      var split = schemaLocationAttribute.Value.Split(new char[0] /* => whitespace */, StringSplitOptions.RemoveEmptyEntries);

      if (split.Length % 2 != 0)
        throw new ArgumentException(
            "The given document does not contain a valid schema location attribute of the form 'xsi:schemaLocation=\"(<Namespace> <Location>)*\"'.",
            nameof(document));

      for (int i = 0; i < split.Length; i += 2)
        result.Add(split[i], split[i + 1]);

      return result;
    }
  }
}