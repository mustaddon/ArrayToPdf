using RandomSolutions;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace TestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Example1();
            Example2();
            Test();
        }

        static void Example1()
        {
            var items = Enumerable.Range(1, 100).Select(x => new
            {
                Prop1 = $"Text #{x}",
                Prop2 = x * 1000,
                Prop3 = DateTime.Now.AddDays(-x),
            });

            var pdf = items.ToPdf("Example1");

            File.WriteAllBytes(@"..\..\..\..\Examples\example1.pdf", pdf);
        }

        static void Example2()
        {
            var items = Enumerable.Range(1, 100).Select(x => new
            {
                Prop1 = $"Text #{x}",
                Prop2 = x * 1000,
                Prop3 = DateTime.Now.AddDays(-x),
            });

            var pdf = items.ToPdf(scheme =>
            {
                scheme.Title = "Example2";
                scheme.PageOrientation = ArrayToPdfOrientations.Portrait;
                scheme.PageMarginLeft = 15;
                scheme.AddColumn("MyColumnName #1", x => x.Prop1, 50);
                scheme.AddColumn("MyColumnName #2", x => $"test:{x.Prop2}");
                scheme.AddColumn("MyColumnName #3", x => x.Prop3);
            });

            File.WriteAllBytes(@"..\..\..\..\Examples\example2.pdf", pdf);
        }


        static void Test()
        {
            var items = Enumerable.Range(1, 1000).Select(x => new
            {
                Bool = x % 2 == 0,
                Int = -x * 100,
                Uint = (uint)x * 100,
                Long = (long)x * 100,
                Double = 1.1d + x,
                Float = 1.1f + x,
                Decimal = 1.1m + x,
                DateTime = DateTime.Now.AddDays(-x),
                DateTimeOffset = DateTimeOffset.Now.AddDays(-x),
                String = $"text text text #{x}",
            });

            var data = items.ToPdf(scheme =>
            {
                scheme.Title = "Test";

                //scheme.PageFormat = ArrayToPdfFormats.A3;
                //scheme.PageOrientation = ArrayToPdfOrientations.Portrait;
                //scheme.SetPageMargin(0, 0, 0, 0);
                //scheme.Header = "Page:{PAGE}";
                //scheme.HeaderAlignment = ArrayToPdfAlignments.Center;
                //scheme.HeaderFontSize = 12;
                //scheme.HeaderFontBold = true;
                //scheme.HeaderHeight = 8;
                //scheme.Footer = "\tPage:{PAGE}";
                //scheme.AddColumn("mycolumn#1", x => x.Int);
                //scheme.AddColumn("mycolumn#2", x => x.Bool);
                //scheme.AddColumn("mycolumn#3", x => x.String);
                //scheme.AddColumn("mycolumn#4", x => x.DateTime, 80);
            });

            File.WriteAllBytes(@"..\test.pdf", data);
        }
    }
}
