using System.Reflection;
using NullGuard;

#if DISABLE_IMPLICIT_NULLABILITY
[assembly: NullGuard(ValidationFlags.None)]
#else
[assembly: NullGuard (ValidationFlags.Arguments | ValidationFlags.NonPublic)]
[assembly: AssemblyMetadata ("ImplicitNullability.AppliesTo", "InputParameters, RefParameters")]
#endif