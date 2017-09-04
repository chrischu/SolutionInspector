using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SolutionInspector.Commons.Utilities
{
  /// <summary>
  ///   Utility that provides various helper methods for dealing with <see cref="Expression" />s.
  /// </summary>
  public static class ExpressionUtility
  {
    private static readonly Dictionary<Tuple<Type, MemberInfo>, Delegate> s_createActionFromGetterExpressionCache =
        new Dictionary<Tuple<Type, MemberInfo>, Delegate>();

    /// <summary>
    ///   Creates an <see cref="Action{TObject,TProperty}" /> that can be used to set the property referenced by <paramref name="propertyGet" />.
    /// </summary>
    public static Action<TObject, TProperty> CreateSetterActionFromGetterExpression<TObject, TProperty> (
      Expression<Func<TObject, TProperty>> propertyGet)
    {
      var member = propertyGet.Body as MemberExpression;

      if (member == null)
        throw new ArgumentException("The given expression is not a valid property get expression.", nameof(propertyGet));

      var cacheKey = Tuple.Create(typeof(TObject), member.Member);

      Delegate action;
      if (s_createActionFromGetterExpressionCache.TryGetValue(cacheKey, out action))
        return (Action<TObject, TProperty>) action;

      var param = Expression.Parameter(typeof(TProperty), "value");
      var assign = Expression.Assign(member, param);
      var set = Expression.Lambda<Action<TObject, TProperty>>(assign, propertyGet.Parameters[0], param).Compile();

      s_createActionFromGetterExpressionCache.Add(cacheKey, set);
      return set;
    }
  }
}