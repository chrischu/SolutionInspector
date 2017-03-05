using JetBrains.Annotations;

namespace SolutionInspector.ConfigurationUi.Features.Dialogs
{
  internal class DialogResult<T>
  {
    public bool WasCancelled { get; }
    public T Value { get; }

    private DialogResult ([CanBeNull] T value, bool wasCancelled)
    {
      Value = value;
      WasCancelled = wasCancelled;
    }

    public static DialogResult<T> Accept(T value)
    {
      return new DialogResult<T>(value, false);
    }

    public static DialogResult<T> Cancel()
    {
      return new DialogResult<T>(default(T), true);
    }
  }
}