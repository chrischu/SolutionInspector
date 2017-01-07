using System;
using NUnit.Framework;
using SolutionInspector.TestInfrastructure;
using SolutionInspector.TestInfrastructure.AssertionExtensions;

namespace SolutionInspector.Configuration.Tests
{
  public class ConfigurationConverterAttributeTests
  {
    [Test]
    public void Ctor_WithNonIConfigurationType_Throws ()
    {
      // ACT
      Action act = () => Dev.Null = new ConfigurationConverterAttribute(typeof(string));

      // ASSERT
      act.ShouldThrowArgumentException($"The given type must derive from '{typeof(IConfigurationConverter)}'.", "configurationConverterType");
    }

    [Test]
    public void Ctor_WithTypeWithoutDefaultCtor_Throws ()
    {
      // ACT
      Action act = () => Dev.Null = new ConfigurationConverterAttribute(typeof(DummyConverter));

      // ASSERT
      act.ShouldThrowArgumentException("The given type must provide a public default constructor.", "configurationConverterType");
    }

    private class DummyConverter : IConfigurationConverter
    {
      // ReSharper disable once UnusedParameter.Local
      public DummyConverter (int x)
      {
      }
    }
  }
}