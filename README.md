# ArrayToPdf [![NuGet version](https://badge.fury.io/nu/ArrayToPdf.svg)](http://badge.fury.io/nu/ArrayToPdf)
Create PDF from Array

### Example 1: Create with default settings
```C#
using ArrayToPdf;

var items = Enumerable.Range(1, 100).Select(x => new
{
    Prop1 = $"Text #{x}",
    Prop2 = x * 1000,
    Prop3 = DateTime.Now.AddDays(-x),
});

var pdf = items.ToPdf();
```
Result: 
[example1.pdf](https://github.com/mustaddon/ArrayToPdf/raw/master/Examples/example1.pdf)


### Example 2: Rename title and columns
```C#
var pdf = SomeItems.ToPdf(schema => schema
    .Title("Example name")
    .ColumnName(m => m.Name.Replace("Prop", "Column #")));
```
Result: 
[example2.pdf](https://github.com/mustaddon/ArrayToPdf/raw/master/Examples/example2.pdf)


### Example 3: Sort columns
```C#
var pdf = SomeItems.ToPdf(schema => schema
    .ColumnSort(m => m.Name, desc: true));
```
Result: 
[example3.pdf](https://github.com/mustaddon/ArrayToPdf/raw/master/Examples/example3.pdf)


### Example 4: Custom column's mapping
```C#
var pdf = SomeItems.ToPdf(schema => schema
    .PageOrientation(ArrayToPdfOrientations.Portrait)
    .PageMarginLeft(15)
    .AddColumn("MyColumnName #1", x => x.Prop1, 30)
    .AddColumn("MyColumnName #2", x => $"test:{x.Prop2}")
    .AddColumn("MyColumnName #3", x => x.Prop3));
```
Result: 
[example4.pdf](https://github.com/mustaddon/ArrayToPdf/raw/master/Examples/example4.pdf)


### Example 5: Filter columns
```C#
var pdf = SomeItems.ToPdf(schema => schema
    .ColumnFilter(m => m.Name != "Prop2"));
```
Result: 
[example5.pdf](https://github.com/mustaddon/ArrayToPdf/raw/master/Examples/example5.pdf)


[Example.ConsoleApp](https://github.com/mustaddon/ArrayToPdf/tree/master/Examples/Example.ConsoleApp/)
