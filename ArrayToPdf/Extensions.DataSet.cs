using MigraDoc.DocumentObjectModel;
using System;
using System.Data;
using System.IO;

namespace ArrayToPdf;

public static partial class Extensions
{
    public static void ToPdf(this DataSet dataSet, Stream stream, Action<SchemaBuilder<DataRow>>? schema = null)
        => dataSet.Tables[0].ToPdf(stream, schema);

    public static byte[] ToPdf(this DataSet dataSet, Action<SchemaBuilder<DataRow>>? schema = null)
        => dataSet.Tables[0].ToPdf(schema);

    public static MemoryStream ToPdfStream(this DataSet dataSet, Action<SchemaBuilder<DataRow>>? schema = null)
        => dataSet.Tables[0].ToPdfStream(schema);

}
