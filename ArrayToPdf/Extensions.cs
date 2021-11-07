using MigraDoc.DocumentObjectModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace ArrayToPdf
{
    public static class Extensions
    {
        public static byte[] ToPdf<T>(this IEnumerable<T> items, Action<SchemaBuilder<T>>? schema = null)
        {
            return ArrayToPdf.CreatePdf(items, schema);
        }

        public static byte[] ToPdf(this DataSet dataSet, Action<SchemaBuilder<DataRow>>? schema = null)
        {
            return ToPdf(dataSet.Tables[0], schema);
        }

        public static byte[] ToPdf(this DataTable dataTable, Action<SchemaBuilder<DataRow>>? schema = null)
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
