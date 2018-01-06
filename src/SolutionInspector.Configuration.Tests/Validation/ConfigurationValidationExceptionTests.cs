using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Configuration.Validation;
using SolutionInspector.TestInfrastructure.AssertionExtensions;

namespace SolutionInspector.Configuration.Tests.Validation
{
  [TestFixture]
  public class ConfigurationValidationExceptionTests
  {
    [Test]
    public void Ctor ()
    {
      // ACT
      var result = new ConfigurationValidationException(
          new[] { "A", "B" },
          new Dictionary<string, IReadOnlyCollection<string>>
          {
              { "Prop1", new[] { "C", "D" } },
              { "Prop2", new[] { "E" } }
          });

      // ASSERT
      result.Message.Should().BeWithDiff(
          @"Validation failed because of the following errors:
  - For the document:
    - A
    - B
  - For property 'Prop1':
    - C
    - D
  - For property 'Prop2':
    - E");
    }
  }
}