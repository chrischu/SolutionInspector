using System;
using System.Collections;
using System.Xml.Linq;
using NUnit.Framework;
using SolutionInspector.Configuration.Collections;

namespace SolutionInspector.Configuration.Tests.Collections
{
  public abstract class ConfigurationElementCollectionTestsBase
  {
    protected XDocument Document { get; private set; }
    protected XElement CollectionElement { get; private set; }

    [SetUp]
    public void SetUp ()
    {
      Document = new XDocument();
      CollectionElement = new XElement("collection");
      Document.Add(CollectionElement);
    }

    protected static IEnumerable ValidateNewElementTestCaseSource ()
    {
      yield return new ValidateNewElementTestCase((c, e) => c.Add(e), "Add");
      yield return new ValidateNewElementTestCase((c, e) => c.Insert(0, e), "Insert");
      yield return new ValidateNewElementTestCase((c, e) => c[0] = e, "Set");
    }

    public class ValidateNewElementTestCase
    {
      private readonly Action<IConfigurationElementCollectionBase<DummyConfigurationElement>, DummyConfigurationElement> _action;
      private readonly string _name;

      public ValidateNewElementTestCase (Action<IConfigurationElementCollectionBase<DummyConfigurationElement>, DummyConfigurationElement> action, string name)
      {
        _action = action;
        _name = name;
      }

      public void Execute (IConfigurationElementCollectionBase<DummyConfigurationElement> collection, DummyConfigurationElement element)
      {
        _action(collection, element);
      }

      public override string ToString ()
      {
        return _name;
      }
    }

    public class DummyConfigurationElement : ConfigurationElement
    {
    }
  }
}