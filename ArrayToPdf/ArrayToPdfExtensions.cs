using MigraDoc.DocumentObjectModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace RandomSolutions
{
    public static class ArrayToPdfExtensions
    {
        public static byte[] ToPdf<T>(this IEnumerable<T> items, string title = null)
            => ArrayToPdf.CreatePdf(items, title);

        public static byte[] ToPdf<T>(this IEnumerable<T> items, Action<ArrayToPdfScheme<T>> schemeBuilder)
            => ArrayToPdf.CreatePdf(items, schemeBuilder);


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
