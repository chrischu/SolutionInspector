using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using SolutionInspector.Commons.Attributes;

namespace SolutionInspector.TestInfrastructure
{
  /// <summary>
  ///   Returns random data for common types.
  /// </summary>
  [PublicApi]
  public static class Some
  {
    private const int c_SomeStringDefaultMaxLength = 100;

    // Thread-local Random instance provider
    private static int s_Seed = Environment.TickCount;

    private static readonly ThreadLocal<Random> s_threadLocalRandomProvider =
        new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref s_Seed)));

    private static readonly Type[] s_possibleTypes = typeof(int).Assembly.GetExportedTypes();

    private static Random Random => s_threadLocalRandomProvider.Value;

    [SuppressMessage(
      "Microsoft.Design",
      "CA1065:DoNotRaiseExceptionsInUnexpectedLocations",
      Justification = "We do not raise an exception in this property, it simply returns a do-not-care exception instance for specs.")]
    public static Exception Exception => new SomeException(String);

    private static int NextRandomBetweenInclusive (int minValue, int maxValue)
    {
      return Random.Next(minValue, checked(maxValue + 1));
    }

    /// <summary>
    ///   Some exception.
    /// </summary>
    [Serializable]
    private class SomeException : Exception
    {
      public SomeException (string message)
        : base(message)
      {
      }

      [ExcludeFromCodeCoverage /* Serialization ctor */]
      protected SomeException (SerializationInfo info, StreamingContext context)
        : base(info, context)
      {
      }
    }

    // ReSharper disable UnusedMember.Global - maybe used in the future
    public static bool Boolean => Random.Next(2) == 1;

    public static string String => StringBetween(1, c_SomeStringDefaultMaxLength);

    public static Version Version => new Version(PositiveInteger, PositiveInteger, PositiveInteger, PositiveInteger);

    private static string StringBetween (int minLength, int maxLength)
    {
      return new string(Enumerable.Range(1, NextRandomBetweenInclusive(minLength, maxLength)).Select(x => Char).ToArray());
    }

    private const string c_AcceptableCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    private static char Char => c_AcceptableCharacters[NextRandomBetweenInclusive(0, c_AcceptableCharacters.Length - 1)];

    public static int Integer => Random.Next(int.MinValue, int.MaxValue /* yes, this doesn't include int.MaxValue itself */);

    public static int PositiveInteger => NextRandomBetweenInclusive(1, 100);

    public static byte[] ByteArray => Encoding.UTF8.GetBytes(String);

    public static T Enum<T> ()
    {
      var values = System.Enum.GetValues(typeof(T));
      return (T) values.GetValue(Random.Next(values.Length));
    }

    public static Guid Guid => Guid.NewGuid();

    public static XElement XElement => new XElement(String);

    public static XAttribute XAttribute => new XAttribute(String, String);


    public static Type Type => s_possibleTypes[Random.Next(s_possibleTypes.Length)];

    // ReSharper restore UnusedMember.Global
  }
}