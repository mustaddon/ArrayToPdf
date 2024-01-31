using System;
using System.Collections.Generic;
using System.IO;

namespace ArrayToPdf;


[Obsolete("Class is deprecated, please use PdfBuilder instead.")]
public class ArrayToPdf
{
    [Obsolete("Method is deprecated, please use PdfBuilder.Build instead.")]
    public static MemoryStream CreatePdf<T>(IEnumerable<T> items, Action<SchemaBuilder<T>>? schema = null)
        => PdfBuilder.Build(items, schema);

    [Obsolete("Method is deprecated, please use PdfBuilder.Build instead.")]
    public static void CreatePdf<T>(Stream stream, IEnumerable<T> items, Action<SchemaBuilder<T>>? schema = null)
        => PdfBuilder.Build(stream, items, schema);
}