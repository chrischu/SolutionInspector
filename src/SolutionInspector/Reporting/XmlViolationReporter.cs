using System.Collections.Generic;
using System.IO;
using System.Xml;
using SolutionInspector.Api.Rules;
using SolutionInspector.Commons.Extensions;

namespace SolutionInspector.Reporting
{
  internal class XmlViolationReporter : ViolationReporterBase
  {
    private readonly IRuleViolationViewModelConverter _ruleViolationViewModelConverter;

    public XmlViolationReporter (TextWriter writer, IRuleViolationViewModelConverter ruleViolationViewModelConverter)
      : base(writer)
    {
      _ruleViolationViewModelConverter = ruleViolationViewModelConverter;
    }

    protected override void Report (TextWriter writer, IEnumerable<IRuleViolation> violations)
    {
      var violationModels = _ruleViolationViewModelConverter.Convert(violations);

      var doc = new XmlDocument();
      var vios = doc.CreateElement("violations");
      doc.AppendChild(vios);

      foreach (var violation in violationModels)
      {
        var vio = doc.CreateElement("violation");
        vios.AppendChild(vio);

        vio.AddAttribute("index", violation.Index.ToString());
        vio.AddAttribute("rule", violation.Rule);
        vio.AddAttribute("target", violation.Target);
        vio.AddElement("message", violation.Message);
      }

      doc.WriteTo(new XmlTextWriter(writer) { Formatting = Formatting.Indented });
    }
  }
}