using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Perfolizer.Metrology;

namespace HtmlCssToImage.Benchmarks;

class Program
{
    static void Main(string[] args)
    {
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new BaseConfig());
    }

     public class BaseConfig : ManualConfig
    {
        public Job Net9BaseJob { get; }
        public Job Net10BaseJob { get; }

        public BaseConfig()
        {
            _ = AddLogger(ConsoleLogger.Default);
            _ = AddExporter(DefaultExporters.Markdown);
            _ = AddExporter(MarkdownExporter.GitHub);
            _ = AddColumnProvider(DefaultColumnProviders.Instance);
            _ = WithSummaryStyle(SummaryStyle.Default);



            var exporter = new CsvExporter(
                CsvSeparator.CurrentCulture,
                new SummaryStyle(
                    cultureInfo: System.Globalization.CultureInfo.CurrentCulture,
                    printUnitsInHeader: true,
                    printUnitsInContent: false,
                    timeUnit: Perfolizer.Horology.TimeUnit.Nanosecond,
                    sizeUnit: SizeUnit.B
                ));
            _ = AddExporter(exporter);

            this.DontOverwriteResults();

            var baseJob = Job.Default.WithGcServer(true).WithMinWarmupCount(2).WithMaxWarmupCount(5).WithMaxIterationCount(20).WithEnvironmentVariable("DOTNET_TieredPGO","0");

            Net9BaseJob = baseJob
                .WithRuntime(CoreRuntime.Core90);
            Net10BaseJob = baseJob
                .WithRuntime(CoreRuntime.Core10_0);



            var bdnRunParam = Environment.GetEnvironmentVariable("BDNRUNPARAM");

            switch (bdnRunParam)
            {
                case "net9.0":
                    _ = AddJob(Net9BaseJob.WithId(".NET 9"));
                    break;
                case "net10.0":
                    _ = AddJob(Net10BaseJob.WithId(".NET 10"));
                    break;
                default:
                    _ = AddJob(
                        Net10BaseJob.WithId(".NET 10")
                    );
                    break;
            }
        }
    }
}