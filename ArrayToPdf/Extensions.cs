using MigraDoc.DocumentObjectModel;
using System;
using System.Collections.Generic;

namespace ArrayToPdf
{
    public static class Extensions
    {
        public static byte[] ToPdf<T>(this IEnumerable<T> items, Action<SchemaBuilder<T>>? schema = null)
        {
            return ArrayToPdf.CreatePdf(items, schema);
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
