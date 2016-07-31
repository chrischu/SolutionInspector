using System;
using System.Collections.Generic;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.Extensions;

#region R# preamble for Machine.Specifications files

// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable NotAccessedField.Local
// ReSharper disable StaticMemberInGenericType
// ReSharper disable UnassignedField.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnassignedGetOnlyAutoProperty

#endregion

namespace SolutionInspector.Api.Tests.Extensions
{
  [Subject (typeof(DictionaryExtensions))]
  class DictionaryExtensionsSpec
  {
    class when_getting_value_or_default_and_value_exists
    {
      Establish ctx = () => { Dictionary = new Dictionary<string, int> { { "key", 7 } }; };

      Because of = () => Result = Dictionary.GetValueOrDefault("key");

      It returns_value = () =>
          Result.Should().Be(7);

      static int Result;

      static IReadOnlyDictionary<string, int> Dictionary;
    }

    class when_getting_value_or_default_and_value_does_not_exist
    {
      Establish ctx = () => { Dictionary = new Dictionary<string, int>(); };

      Because of = () => Result = Dictionary.GetValueOrDefault("key");

      It returns_value = () =>
          Result.Should().Be(default(int));

      static int Result;

      static IReadOnlyDictionary<string, int> Dictionary;
    }
  }
}