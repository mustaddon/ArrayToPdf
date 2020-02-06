using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp;

namespace RandomSolutions
{
    public class ArrayToPdf
    {
#if STANDARD
        static ArrayToPdf()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
#endif

        public static byte[] CreatePdf<T>(IEnumerable<T> items, string title = null)
        {
            return CreatePdf(items, scheme => scheme.SetTitle(title));
        }

        public static byte[] CreatePdf<T>(IEnumerable<T> items, Action<ArrayToPdfScheme<T>> schemeBuilder)
        {
            var scheme = new ArrayToPdfScheme<T>();
            schemeBuilder.Invoke(scheme);
            return _createPdf(items, scheme);
        }

        static byte[] _createPdf<T>(IEnumerable<T> items, ArrayToPdfScheme<T> scheme)
        {
            using (var ms = new MemoryStream())
            {
                var renderer = new PdfDocumentRenderer(true);
                renderer.Document = _createDocument(items, scheme);
                renderer.RenderDocument();
                renderer.PdfDocument.Save(ms);
                return ms.ToArray();
            }
        }

        static readonly int _tableLeftBias = -1;

        static Document _createDocument<T>(IEnumerable<T> items, ArrayToPdfScheme<T> scheme)
        {
            var document = new Document();
            document.UseCmykColor = true;
            document.Info.Title = scheme.Title;
            document.DefaultPageSetup.Orientation = (Orientation)scheme.Orientation;
            document.DefaultPageSetup.HeaderDistance = Unit.FromMillimeter(scheme.MarginTop);
            document.DefaultPageSetup.FooterDistance = Unit.FromMillimeter(scheme.MarginBottom);
            document.DefaultPageSetup.TopMargin = Unit.FromMillimeter(scheme.MarginTop + 7);
            document.DefaultPageSetup.RightMargin = Unit.FromMillimeter(scheme.MarginRight + _tableLeftBias);
            document.DefaultPageSetup.BottomMargin = Unit.FromMillimeter(scheme.MarginBottom);
            document.DefaultPageSetup.LeftMargin = Unit.FromMillimeter(scheme.MarginLeft - _tableLeftBias);

            var innerWidth = Unit.FromPoint(document.DefaultPageSetup.GetWidth() - document.DefaultPageSetup.LeftMargin - document.DefaultPageSetup.RightMargin);

            _addStyles(document, innerWidth);
            _addSection(document);
            _addHeader(document, innerWidth, scheme);
            _addTable(document, innerWidth, items, scheme);

            return document;
        }

        static void _addStyles(Document document, Unit innerWidth)
        {
            var style = document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop(Unit.FromMillimeter(innerWidth.Millimeter + _tableLeftBias), TabAlignment.Right);
        }

        static void _addSection(Document document)
        {
            var section = document.AddSection();
            section.PageSetup = document.DefaultPageSetup.Clone();
        }

        static void _addHeader<T>(Document document, Unit innerWidth, ArrayToPdfScheme<T> scheme)
        {
            var header = document.LastSection.Headers.Primary;
            var paragraph = header.AddParagraph(scheme.Title);
            paragraph.Format.LeftIndent = Unit.FromMillimeter(_tableLeftBias);
            paragraph.AddTab();
            paragraph.AddPageField();
            paragraph.AddText("/");
            paragraph.AddNumPagesField();
        }

        static void _addTable<T>(Document document, Unit innerWidth, IEnumerable<T> items, ArrayToPdfScheme<T> scheme)
        {
            if (scheme.Columns.Count == 0)
                return;

            var table = new Table();
            table.Format.Font.Size = Unit.FromPoint(scheme.FontSize);
            table.Borders.Width = 0.5;


            var colWidth = Unit.FromPoint(innerWidth / scheme.Columns.Count);

            // create columns
            scheme.Columns.ForEach(x => table.AddColumn(colWidth).Format.Alignment = (ParagraphAlignment)(x.Alignment ?? scheme.Alignment));

            // add header
            var row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Font.Bold = true;
            row.Shading.Color = Colors.LightGray;
            scheme.Columns.ForEach(x => row.Cells[x.Index].AddParagraph(x.Name));

            // add rows
            var itemsCount = 0;
            foreach (var item in items)
            {
                itemsCount++;
                row = table.AddRow();
                foreach(var col in scheme.Columns)
                {
                    var value = col.ValueFn(item);
                    var cell = row.Cells[col.Index];
                    cell.AddParagraph(value.ToString());
                }
            }

            //table.SetEdge(0, 0, scheme.Columns.Count, itemsCount+1, Edge.Box, BorderStyle.Single, 1, Colors.Black);
            document.LastSection.Add(table);
        }
    }
}
