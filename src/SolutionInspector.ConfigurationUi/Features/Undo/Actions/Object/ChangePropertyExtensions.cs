using System;
using System.Linq.Expressions;
using SolutionInspector.Commons.Utilities;

namespace SolutionInspector.ConfigurationUi.Features.Undo.Actions.Object
{
  internal static class ChangePropertyExtensions
  {
    public static IUndoableAction ChangeProperty<TObject, TProperty> (
      this IFutureUndoableObjectActionFactory<TObject> factory,
      Expression<Func<TObject, TProperty>> propertySelector,
      TProperty newValue)
    {
      return new ChangePropertyAction<TObject, TProperty>(factory.Object, propertySelector, newValue, propertySelector.Compile()(factory.Object));
    }

    public static IUndoableAction PropertyChanged<TObject, TProperty> (
      this IPastUndoableObjectActionFactory<TObject> factory,
      Expression<Func<TObject, TProperty>> propertySelector,
      TProperty oldValue)
    {
      return new ChangePropertyAction<TObject, TProperty>(factory.Object, propertySelector, propertySelector.Compile()(factory.Object), oldValue);
    }

    private class ChangePropertyAction<TObject, TProperty> : IUndoableAction
    {
      private readonly TObject _object;
      private readonly TProperty _newValue;
      private readonly TProperty _oldValue;
      private readonly Action<TObject, TProperty> _propertySet;

      public ChangePropertyAction (TObject @object, Expression<Func<TObject, TProperty>> propertySelector, TProperty newValue, TProperty oldValue)
      {
        _object = @object;
        _newValue = newValue;
        _oldValue = oldValue;
        _propertySet = ExpressionUtility.CreateSetterActionFromGetterExpression(propertySelector);
      }

      public void Redo ()
      {
        _propertySet(_object, _newValue);
      }

      public void Undo ()
      {
        _propertySet(_object, _oldValue);
      }
    }
  }
}