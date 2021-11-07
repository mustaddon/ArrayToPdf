using ArrayToPdf;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Example1();
            Example2();
            Example3();
            Example4();
            Example5();
            Example6();
            Example7();
            TestTypes();
        }

        static IEnumerable<SomeItem> SomeItems = Enumerable.Range(1, 100).Select(x => new SomeItem
        {
            Prop1 = $"Text #{x}",
            Prop2 = x * 1000,
            Prop3 = DateTime.Now.AddDays(-x),
        });

        // default settings
        static void Example1()
        {
            var pdf = SomeItems.ToPdf();

            File.WriteAllBytes($@"..\..\..\..\{nameof(Example1)}.pdf".ToLower(), pdf);
        }

        // rename title and columns
        static void Example2()
        {
            var pdf = SomeItems.ToPdf(schema => schema
                .Title("Example name")
                .ColumnName(m => m.Name.Replace("Prop", "Column #")));

            File.WriteAllBytes($@"..\..\..\..\{nameof(Example2)}.pdf".ToLower(), pdf);
        }

        // sort columns
        static void Example3()
        {
            var pdf = SomeItems.ToPdf(schema => schema
                .ColumnSort(m => m.Name, desc: true));

            File.WriteAllBytes($@"..\..\..\..\{nameof(Example3)}.pdf".ToLower(), pdf);
        }

        // custom column's mapping
        static void Example4()
        {
            var pdf = SomeItems.ToPdf(schema => schema
                .PageOrientation(PdfOrientations.Portrait)
                .PageMarginLeft(15)
                .AddColumn("MyColumnName #1", x => x.Prop1, 30)
                .AddColumn("MyColumnName #2", x => $"test:{x.Prop2}")
                .AddColumn("MyColumnName #3", x => x.Prop3));

            File.WriteAllBytes($@"..\..\..\..\{nameof(Example4)}.pdf".ToLower(), pdf);
        }

        // filter columns
        static void Example5()
        {
            var pdf = SomeItems.ToPdf(schema => schema
                .ColumnFilter(m => m.Name != "Prop2"));

            File.WriteAllBytes($@"..\..\..\..\{nameof(Example5)}.pdf".ToLower(), pdf);
        }

        // list of dictionaries 
        static void Example6()
        {
            var items = Enumerable.Range(1, 100).Select(x => new Dictionary<object, object>
            {
                { "Column #1", $"Text #{x}" },
                { "Column #2", x * 1000 },
                { "Column #3", DateTime.Now.AddDays(-x) },
            });

            var pdf = items.ToPdf();

            File.WriteAllBytes($@"..\..\..\..\{nameof(Example6)}.pdf".ToLower(), pdf);
        }

        // list of expandos 
        static void Example7()
        {
            var items = Enumerable.Range(1, 100).Select(x =>
            {
                var item = new ExpandoObject();
                var itemDict = item as IDictionary<string, object>;
                itemDict.Add("Column #1", $"Text #{x}");
                itemDict.Add("Column #2", x * 1000);
                itemDict.Add("Column #3", DateTime.Now.AddDays(-x));
                return item;
            });

            var pdf = items.ToPdf();

            File.WriteAllBytes($@"..\..\..\..\{nameof(Example7)}.pdf".ToLower(), pdf);
        }

        static void TestTypes()
        {
            var items = Enumerable.Range(1, 1000).Select(x => new
            {
                Bool = x % 2 == 0,
                NullableBool = x % 2 == 0 ? true : (bool?)null,
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

            var data = items.ToPdf();

            File.WriteAllBytes($@"..\{nameof(TestTypes)}.pdf", data);
        }
    }
}
