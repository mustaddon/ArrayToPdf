using MigraDoc.DocumentObjectModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace ArrayToPdf
{
    public static class Extensions
    {
        public static byte[] ToPdf<T>(this IEnumerable<T> items, Action<SchemaBuilder<T>>? schema = null)
        {
            using var ms = ToPdfStream(items, schema);
            return ms.ToArray();
        }

        public static byte[] ToPdf(this DataSet dataSet, Action<SchemaBuilder<DataRow>>? schema = null)
        {
            using var ms = ToPdfStream(dataSet, schema);
            return ms.ToArray();
        }

        public static byte[] ToPdf(this DataTable dataTable, Action<SchemaBuilder<DataRow>>? schema = null)
        {
            using var ms = ToPdfStream(dataTable, schema);
            return ms.ToArray();
        }

        public static MemoryStream ToPdfStream<T>(this IEnumerable<T> items, Action<SchemaBuilder<T>>? schema = null)
        {
            return ArrayToPdf.CreatePdf(items, schema);
        }

        public static MemoryStream ToPdfStream(this DataSet dataSet, Action<SchemaBuilder<DataRow>>? schema = null)
        {
            return ToPdfStream(dataSet.Tables[0], schema);
        }

        public static MemoryStream ToPdfStream(this DataTable dataTable, Action<SchemaBuilder<DataRow>>? schema = null)
        {
            return ArrayToPdf.CreatePdf(dataTable.Rows.AsEnumerable(), builder =>
            {
                if (!string.IsNullOrWhiteSpace(dataTable.TableName))
                    builder.Title(dataTable.TableName);

                foreach (DataColumn col in dataTable.Columns)
                    builder.AddColumn(col.ColumnName, x => x[col]);

                schema?.Invoke(builder);
            });
        }


        private static IEnumerable<DataRow> AsEnumerable(this DataRowCollection items)
        {
            foreach (DataRow item in items)
                yield return item;
        }

        private static IEnumerable<DataTable> AsEnumerable(this DataTableCollection items)
        {
            foreach (DataTable item in items)
                yield return item;
        }


        internal static Unit GetWidth(this PageSetup pageSetup)
        {
            return pageSetup.Orientation == Orientation.Landscape ? pageSetup.PageHeight : pageSetup.PageWidth;
        }

        internal static Unit GetHeight(this PageSetup pageSetup)
        {
            return pageSetup.Orientation != Orientation.Landscape ? pageSetup.PageHeight : pageSetup.PageWidth;
        }
    }
}
