using System;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.Configuration.Tests
{
  public class ConfigurationValueTests
  {
    private Action<string> _onDeserialize;

    private IConfigurationValue _sut;
    private Action<string> _updateValue;

    [SetUp]
    public void SetUp ()
    {
      _updateValue = A.Fake<Action<string>>();
      _onDeserialize = A.Fake<Action<string>>();

      _sut = new DummyConfigurationValue(_updateValue, _onDeserialize);
    }

    [Test]
    public void Serialize ()
    {
      // ACT
      var result = _sut.Serialize();

      // ASSERT
      result.Should().Be("Serialize");
    }

    [Test]
    public void Deserialize ()
    {
      var serialized = Some.String;

      // ACT
      _sut.Deserialize(serialized);

      // ASSERT
      A.CallTo(() => _onDeserialize(serialized)).MustHaveHappened();
    }

    [Test]
    public void ToString_CallsSerialize ()
    {
      // ACT
      var result = _sut.ToString();

      // ASSERT
      result.Should().Be("Serialize");
    }

    private class DummyConfigurationValue : ConfigurationValue<DummyConfigurationValue>
    {
      private readonly Action<string> _onDeserialize;

      public DummyConfigurationValue (Action<string> updateValue, Action<string> onDeserialize)
        : base(updateValue)
      {
        _onDeserialize = onDeserialize;
      }

      public override string Serialize ()
      {
        return "Serialize";
      }

      public override void Deserialize (string serialized)
      {
        _onDeserialize(serialized);
      }
    }
  }
}