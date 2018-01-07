using System;
using System.Diagnostics.CodeAnalysis;
using Autofac;
using SolutionInspector.Commons.Console;
using Wrapperator.Interfaces;
using Wrapperator.Interfaces.IO;
using Wrapperator.Interfaces.Reflection;
using Wrapperator.Interfaces.Xml;
using Wrapperator.Interfaces.Xml.Schema;
using Wrapperator.Wrappers;

namespace SolutionInspector.BuildTool
{
  [ExcludeFromCodeCoverage]
  internal class BuildToolProgram : ConsoleProgramBase
  {
    internal const string DefaultBaseSchemaVersion = "1";
    internal const string BaseSchemaNamespaceTemplate = "http://chrischu.github.io/SolutionInspector/schema/base_v{0}.xsd";

    public static int Main (string[] args)
    {
      return new BuildToolProgram().Run(args);
    }

    protected override void RegisterServices (ContainerBuilder builder)
    {
      builder.Register(ctx => Wrapper.Assembly).As<IAssemblyStatic>();
      builder.Register(ctx => Wrapper.File).As<IFileStatic>();
      builder.Register(ctx => Wrapper.Console).As<IConsoleStatic>();
      builder.Register(ctx => Wrapper.XmlWriter).As<IXmlWriterStatic>();
      builder.Register(ctx => Wrapper.XmlSchema).As<IXmlSchemaStatic>();

      builder.RegisterType<RuleAssemblySchemaCreator>().As<IRuleAssemblySchemaCreator>();
      builder.RegisterType<SchemaInfoRetriever>().As<ISchemaInfoRetriever>();
    }
  }
}