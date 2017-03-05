using System;
using System.Reflection;
using Microsoft.VisualStudio.Imaging;
using SolutionInspector.Commons;
using SolutionInspector.Commons.Extensions;

namespace SolutionInspector.ConfigurationUi
{
  internal partial class App
  {
    [STAThread]
    public static void Main ()
    {
      // TODO: BindingExceptionThrower.Attach();

      using (var resource = Assembly.GetEntryAssembly().GetManifestResourceStream($"{typeof(App).Namespace}.ImageCatalog.imagemanifest"))
      using (var tempFile = new TemporaryFile())
      {
        using (var tempFileStream = tempFile.GetStream())
          resource.AssertNotNull().CopyTo(tempFileStream);

        CrispImage.DefaultImageLibrary = ImageLibrary.Load(tempFile.Path, isDefault: true);
      }

      var app = new App();
      app.InitializeComponent();
      app.Run();
    }
  }
}