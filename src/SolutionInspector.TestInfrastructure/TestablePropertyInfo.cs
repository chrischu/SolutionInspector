using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using JetBrains.Annotations;

namespace SolutionInspector.TestInfrastructure
{
  /// <summary>
  /// <see cref="PropertyInfo"/> that can be faked.
  /// </summary>
  [UsedImplicitly]
  public class TestablePropertyInfo : PropertyInfo
  {
    public override MemberTypes MemberType => MemberTypes.Property;

    // ReSharper disable UnassignedGetOnlyAutoProperty
    public override Type PropertyType { get; }
    public override PropertyAttributes Attributes { get; }
    public override bool CanRead { get; }
    public override bool CanWrite { get; }
    public override MethodInfo GetMethod { get; }
    public override MethodInfo SetMethod { get; }
    public override string Name { get; }
    public override Type DeclaringType { get; }
    public override Type ReflectedType { get; }
    public override IEnumerable<CustomAttributeData> CustomAttributes { get; }
    public override int MetadataToken { get; }
    public override Module Module { get; }
    // ReSharper restore UnassignedGetOnlyAutoProperty

    public override void SetValue (object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    public override MethodInfo[] GetAccessors (bool nonPublic)
    {
      throw new NotImplementedException();
    }

    public override MethodInfo GetGetMethod (bool nonPublic)
    {
      throw new NotImplementedException();
    }

    public override MethodInfo GetSetMethod (bool nonPublic)
    {
      throw new NotImplementedException();
    }

    public override ParameterInfo[] GetIndexParameters ()
    {
      throw new NotImplementedException();
    }

    public override object GetValue (object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    public override object[] GetCustomAttributes (bool inherit)
    {
      throw new NotImplementedException();
    }

    public override object[] GetCustomAttributes (Type attributeType, bool inherit)
    {
      throw new NotImplementedException();
    }

    public override bool IsDefined (Type attributeType, bool inherit)
    {
      throw new NotImplementedException();
    }
  }
}