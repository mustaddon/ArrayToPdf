using RandomSolutions;
using System;
using System.IO;
using System.Linq;

namespace TestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Test();
        }

        static void Test()
        {
            var items = Enumerable.Range(1, 100).Select(x => new
            {
                Bool = true,
                Int = -1,
                Uint = 1u,
                Long = 1L,
                Double = 1.1d,
                Float = 1.1f,
                Decimal = 1.1m,
                DateTime = DateTime.Now,
                DateTimeOffset = DateTimeOffset.Now,
                String = $"Text text #{x}",
            });

            //var data = ArrayToPdf.CreatePdf(items, "Test");

            var data = items.ToPdf(scheme => 
                scheme.SetTitle("моя тестовая прога")
                //.setorientation(arraytopdforientations.portrait)
                //.SetMargin(0, 0, 0, 0)
                //.setfontsize(8)
                //.SetAlignment(ArrayToPdfAlignments.Left)
                .AddColumn("mycolumn#1", x => x.Int, 20)
                .AddColumn("mycolumn#2", x => x.Bool, 40)
                .AddColumn("mycolumn#3", x => x.String)
            );

            File.WriteAllBytes(@"..\test.pdf", data);
        }
    }
}
