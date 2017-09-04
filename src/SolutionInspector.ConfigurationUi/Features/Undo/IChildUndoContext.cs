using System;

namespace SolutionInspector.ConfigurationUi.Features.Undo
{
  internal interface IChildUndoContext : IUndoContext, IDisposable
  {
  }
}