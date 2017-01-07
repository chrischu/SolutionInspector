using System;
using System.Windows;
using MahApps.Metro;

namespace SolutionInspector.ConfigurationUi
{
  public partial class App
  {
    protected override void OnStartup (StartupEventArgs e)
    {
      base.OnStartup(e);

      TestStuff();

      //BindingExceptionThrower.Attach();
      ChangeTheme();
    }

    private void TestStuff ()
    {
      //var xdoc = XDocument.Load(@"D:\Development\SolutionInspector\src\SolutionInspector.DefaultRules\Template.SolutionInspectorRuleset");
      //var element = xdoc.XPathSelectElement("//solutionInspectorRuleset");

      //var ruleset = new SolutionInspectorRulesetConfigurationSection(element);

      //var xdoc = new XDocument();
      //xdoc.Add(new XElement("element"));

      //var someConfiguration = new SomeConfigurationElement(xdoc.Root);
      //someConfiguration.String = "ASDF";
      //someConfiguration.Int = 1203928;
      //someConfiguration.Commas.Add("A");
      //someConfiguration.Commas.Add("B");

      //someConfiguration.Other.String = "QWER";

      //var added = someConfiguration.Others.AddNew();
      //added.String = "YXCV";

      //var someConfiguration2 = new SomeConfigurationElement(xdoc.Root);

      //var xdocString = xdoc.ToString();

      //someConfiguration2.Others.Remove(added);

      //xdocString = xdoc.ToString();
    }

    private void ChangeTheme ()
    {
      ThemeManager.AddAccent(
        "VisualStudioPurple",
        new Uri("pack://application:,,,/SolutionInspector.ConfigurationUi;component/Resources/VisualStudioPurple.xaml"));

      var theme = ThemeManager.DetectAppStyle(Current);
      ThemeManager.ChangeAppStyle(Current, ThemeManager.GetAccent("VisualStudioPurple"), theme.Item1);
    }
  }
}