namespace SolutionInspector.ConfigurationUi.Features.Undo.Actions
{
  internal interface IUndoableAction
  {
    IDoableAction Undo ();
  }
}