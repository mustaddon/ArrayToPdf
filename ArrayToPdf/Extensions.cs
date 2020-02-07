using MigraDoc.DocumentObjectModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace RandomSolutions
{
    public static class ArrayToPdfExtensions
    {
        public static byte[] ToPdf<T>(this IEnumerable<T> items, string title = null, 
            ArrayToPdfFormats format = ArrayToPdfFormats.A4,
            ArrayToPdfOrientations orientation = ArrayToPdfOrientations.Landscape)
        {
            return ArrayToPdf.CreatePdf(items, scheme =>
            {
                scheme.Title = title;
                scheme.PageFormat = format;
                scheme.PageOrientation = orientation;
            });
        }
        
        public static byte[] ToPdf<T>(this IEnumerable<T> items, Action<ArrayToPdfScheme<T>> schemeBuilder)
        {
            return ArrayToPdf.CreatePdf(items, schemeBuilder);
        }

        public static ArrayToPdfScheme<T> SetInfo<T>(this ArrayToPdfScheme<T> scheme, string title, string subject, string author)
        {
            scheme.Title = title;
            scheme.Subject = subject;
            scheme.Author = author;
            return scheme;
        }

        /// <summary>Margin in Millimeters</summary>
        public static ArrayToPdfScheme<T> SetPageMargin<T>(this ArrayToPdfScheme<T> scheme, uint top, uint right, uint bottom, uint left)
        {
            scheme.PageMarginTop = top;
            scheme.PageMarginRight = right;
            scheme.PageMarginBottom = bottom;
            scheme.PageMarginLeft = left;
            return scheme;
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
