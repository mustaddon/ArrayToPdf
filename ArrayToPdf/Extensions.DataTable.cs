using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace ArrayToPdf;

public static partial class Extensions
{
    public static void ToPdf(this DataTable dataTable, Stream stream, Action<SchemaBuilder<DataRow>>? schema = null)
    {
        PdfBuilder.Build(stream, dataTable.Rows.AsEnumerable(), builder =>
        {
            if (!string.IsNullOrWhiteSpace(dataTable.TableName))
                builder.Title(dataTable.TableName);

            foreach (DataColumn col in dataTable.Columns)
                builder.AddColumn(col.ColumnName, x => x[col]);

            schema?.Invoke(builder);
        });
    }

    public static byte[] ToPdf(this DataTable dataTable, Action<SchemaBuilder<DataRow>>? schema = null)
        => dataTable.ToPdfStream(schema).ToArray();

    public static MemoryStream ToPdfStream(this DataTable dataTable, Action<SchemaBuilder<DataRow>>? schema = null)
    {
        var ms = new MemoryStream();
        dataTable.ToPdf(ms, schema);
        ms.Position = 0;
        return ms;
    }


    private static IEnumerable<DataRow> AsEnumerable(this DataRowCollection items)
    {
        foreach (DataRow item in items)
            yield return item;
    }
}
