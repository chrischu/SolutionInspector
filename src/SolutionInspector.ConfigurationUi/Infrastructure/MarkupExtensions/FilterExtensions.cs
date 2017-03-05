using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace SolutionInspector.ConfigurationUi.Infrastructure.MarkupExtensions
{
  [ContentProperty("Filters")]
  internal class FilterExtensions : MarkupExtension
  {
    private readonly Collection<IFilter> _filters = new Collection<IFilter>();
    public ICollection<IFilter> Filters => _filters;

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return new FilterEventHandler((s, e) => e.Accepted = Filters.All(f => f.Filter(e.Item)));
    }
  }

  internal interface IFilter
  {
    bool Filter(object item);
  }
}
