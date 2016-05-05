using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SolutionInspector.Api.Extensions;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.Api.Reporting
{
  internal class XmlViolationReporter : IViolationReporter
  {
    private readonly TextWriter _outWriter;
    private readonly IRuleViolationViewModelConverter _ruleViolationViewModelConverter;

    public XmlViolationReporter(TextWriter outWriter, IRuleViolationViewModelConverter ruleViolationViewModelConverter)
    {
      _outWriter = outWriter;
      _ruleViolationViewModelConverter = ruleViolationViewModelConverter;
    }

    public void Report(IEnumerable<IRuleViolation> violations)
    {
      var violationModels = _ruleViolationViewModelConverter.Convert(violations);

      XmlDocument doc = new XmlDocument();
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

      doc.WriteTo(new XmlTextWriter(_outWriter) { Formatting = Formatting.Indented });
    }
  }
}