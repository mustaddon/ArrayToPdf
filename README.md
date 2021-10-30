# ArrayToPdf [![NuGet version](https://badge.fury.io/nu/ArrayToPdf.svg)](http://badge.fury.io/nu/ArrayToPdf)
Create PDF from Array

### Example #1

```C#
var items = Enumerable.Range(1, 100).Select(x => new
{
    Prop1 = $"Text #{x}",
    Prop2 = x * 1000,
    Prop3 = DateTime.Now.AddDays(-x),
});

var pdf = items.ToPdf("Example1");
```

Result: 
[example1.pdf](https://github.com/mustaddon/ArrayToPdf/raw/master/Examples/example1.pdf)


### Example #2

```C#
var pdf = items.ToPdf(scheme =>
{
    scheme.Title = "Example2";
    scheme.PageOrientation = ArrayToPdfOrientations.Portrait;
    scheme.PageMarginLeft = 15;
    scheme.AddColumn("MyColumnName #1", x => x.Prop1, 50);
    scheme.AddColumn("MyColumnName #2", x => $"test:{x.Prop2}");
    scheme.AddColumn("MyColumnName #3", x => x.Prop3);
});
```

Result: 
[example2.pdf](https://github.com/mustaddon/ArrayToPdf/raw/master/Examples/example2.pdf)


[More info in the test console application...](https://github.com/mustaddon/ArrayToPdf/tree/master/TestConsoleApp/)
