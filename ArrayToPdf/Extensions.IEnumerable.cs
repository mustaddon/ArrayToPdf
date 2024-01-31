using System;
using System.Collections.Generic;
using System.IO;

namespace ArrayToPdf;

public static partial class Extensions
{
    public static void ToPdf<T>(this IEnumerable<T> items, Stream stream, Action<SchemaBuilder<T>>? schema = null)
        => PdfBuilder.Build(stream, items, schema);

    public static byte[] ToPdf<T>(this IEnumerable<T> items, Action<SchemaBuilder<T>>? schema = null)
        => PdfBuilder.Build(items, schema).ToArray();

    public static MemoryStream ToPdfStream<T>(this IEnumerable<T> items, Action<SchemaBuilder<T>>? schema = null)
        => PdfBuilder.Build(items, schema);
}
