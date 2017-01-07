using System.Linq;
using GalaSoft.MvvmLight;
using SolutionInspector.ConfigurationUi.Infrastructure;

namespace SolutionInspector.ConfigurationUi.ViewModel
{
  internal class ImportsViewModel : ViewModelBase
  {
    //private readonly IRuleAssemblyImportsConfiguration _ruleAssemblyImports;

    //public ImportsViewModel (IRuleAssemblyImportsConfiguration ruleAssemblyImports)
    //{
    //  _ruleAssemblyImports = ruleAssemblyImports;
    //  // TODO
    //  //Paths = new AdvancedObservableCollection<ImportViewModel>(ruleAssemblyImports.Imports.Select(i => new ImportViewModel(i)));
    //  //Paths.ElementAdded += element => _ruleAssemblyImports.Imports.Add(element.Path);
    //  //Paths.ElementRemoved += element => _ruleAssemblyImports.Imports.Remove(element.Path);
    //}

    public AdvancedObservableCollection<ImportViewModel> Paths { get; }
  }

  public enum ImportType
  {
    File,
    Directory
  }
}