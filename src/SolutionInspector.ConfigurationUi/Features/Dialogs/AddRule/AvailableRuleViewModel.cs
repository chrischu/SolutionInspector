using GalaSoft.MvvmLight;
using SolutionInspector.Internals;

namespace SolutionInspector.ConfigurationUi.Features.Dialogs.AddRule
{
  internal class AvailableRuleViewModel : ViewModelBase
  {
    public AvailableRuleViewModel (RuleTypeInfo ruleTypeInfo, string documentation)
    {
      RuleTypeInfo = ruleTypeInfo;
      Documentation = documentation;
    }

    public string AssemblyName => RuleTypeInfo.RuleType.Assembly.GetName().Name;
    public string Name => RuleTypeInfo.RuleType.Name;
    public RuleTypeInfo RuleTypeInfo { get; }
    public string Documentation { get; }
  }
}