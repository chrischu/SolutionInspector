using System;
using JetBrains.Annotations;

namespace SolutionInspector.Configuration
{
  public interface IConfigurationValue
  {
    string Serialize();
    void Deserialize (string serialized);
  }

  public abstract class ConfigurationValue<T> : IConfigurationValue
      where T : ConfigurationValue<T>
  {
    private readonly Action<string> _updateValue;

    protected ConfigurationValue (Action<string> updateValue)
    {
      _updateValue = updateValue;
    }

    protected void Update ()
    {
      _updateValue(Serialize());
    }

    public abstract string Serialize ();

    [UsedImplicitly /* via Reflection */]
    public abstract void Deserialize (string serialized);

    public sealed override string ToString ()
    {
      return Serialize();
    }
  }
}