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
            document.Info.Subject = scheme.Subject;
            document.Info.Author = scheme.Author;

            document.DefaultPageSetup.PageFormat = (PageFormat)scheme.PageFormat;
            document.DefaultPageSetup.Orientation = (Orientation)scheme.PageOrientation;
            document.DefaultPageSetup.HeaderDistance = Unit.FromMillimeter(scheme.PageMarginTop);
            document.DefaultPageSetup.FooterDistance = Unit.FromMillimeter(scheme.PageMarginBottom);
            document.DefaultPageSetup.TopMargin = Unit.FromMillimeter(scheme.PageMarginTop + (string.IsNullOrWhiteSpace(scheme.Header) ? 0 : scheme.HeaderHeight));
            document.DefaultPageSetup.RightMargin = Unit.FromMillimeter(scheme.PageMarginRight + _tableLeftBias);
            document.DefaultPageSetup.BottomMargin = Unit.FromMillimeter(scheme.PageMarginBottom + (string.IsNullOrWhiteSpace(scheme.Footer) ? 0 : scheme.FooterHeight));
            document.DefaultPageSetup.LeftMargin = Unit.FromMillimeter(scheme.PageMarginLeft - _tableLeftBias);

            Unit width, height;
            PageSetup.GetPageSize(document.DefaultPageSetup.PageFormat, out width, out height);
            document.DefaultPageSetup.PageWidth = width;
            document.DefaultPageSetup.PageHeight = height;

            var innerWidth = Unit.FromPoint(document.DefaultPageSetup.GetWidth() - document.DefaultPageSetup.LeftMargin - document.DefaultPageSetup.RightMargin);

            _addStyles(document, innerWidth);
            _addSection(document);
            _addHeader(document, innerWidth, scheme);
            _addFooter(document, innerWidth, scheme);
            _addTable(document, innerWidth, items, scheme);

            return document;
        }

        static void _addStyles(Document document, Unit innerWidth)
        {
            var style = document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop(Unit.FromMillimeter(innerWidth.Millimeter + _tableLeftBias), TabAlignment.Right);

            style = document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop(Unit.FromMillimeter(innerWidth.Millimeter + _tableLeftBias), TabAlignment.Right);
        }

        static void _addSection(Document document)
        {
            var section = document.AddSection();
            section.PageSetup = document.DefaultPageSetup.Clone();
        }

        static void _addHeader<T>(Document document, Unit innerWidth, ArrayToPdfScheme<T> scheme)
        {
            if (string.IsNullOrWhiteSpace(scheme.Header))
                return;

            var header = document.LastSection.Headers.Primary;
            var paragraph = header.AddParagraph();
            paragraph.Format.LeftIndent = Unit.FromMillimeter(_tableLeftBias);
            paragraph.Format.Alignment = (ParagraphAlignment)scheme.HeaderAlignment;
            paragraph.Format.Font.Size = scheme.HeaderFontSize;
            paragraph.Format.Font.Bold = scheme.HeaderFontBold;
            _addTmpText(paragraph, scheme.Header, scheme);
        }

        static void _addFooter<T>(Document document, Unit innerWidth, ArrayToPdfScheme<T> scheme)
        {
            if (string.IsNullOrWhiteSpace(scheme.Footer))
                return;

            var footer = document.LastSection.Footers.Primary;
            var paragraph = footer.AddParagraph();
            paragraph.Format.LeftIndent = Unit.FromMillimeter(_tableLeftBias);
            paragraph.Format.Alignment = (ParagraphAlignment)scheme.FooterAlignment;
            paragraph.Format.Font.Size = scheme.FooterFontSize;
            paragraph.Format.Font.Bold = scheme.FooterFontBold;
            _addTmpText(paragraph, scheme.Footer, scheme);
        }

        static void _addTmpText<T>(Paragraph paragraph, string template, ArrayToPdfScheme<T> scheme)
        {
            foreach (var part in template.Split(new[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries))
                switch (part)
                {
                    case "TITLE":
                        paragraph.AddText(scheme.Title ?? string.Empty);
                        break;
                    case "PAGE":
                        paragraph.AddPageField();
                        break;
                    case "PAGES":
                        paragraph.AddNumPagesField();
                        break;
                    default:
                        paragraph.AddText(part);
                        break;
                }
        }

        static void _addTable<T>(Document document, Unit innerWidth, IEnumerable<T> items, ArrayToPdfScheme<T> scheme)
        {
            if (scheme.Columns.Count == 0)
                return;

            var table = new Table();
            table.Format.Font.Size = Unit.FromPoint(scheme.TableFontSize);
            table.Borders.Width = 0.5;


            var colWidth = Unit.FromPoint(innerWidth / scheme.Columns.Count);
            var settedWidth = scheme.Columns.Sum(x => x.Width ?? 0);
            var autoWidthCount = scheme.Columns.Count(x => !x.Width.HasValue);
            var autoWidth = (innerWidth.Millimeter - settedWidth) / (autoWidthCount > 0 ? autoWidthCount : 1);

            // create columns
            scheme.Columns.ForEach(x => table.AddColumn(Unit.FromMillimeter(x.Width ?? autoWidth)).Format.Alignment = (ParagraphAlignment)(x.Alignment ?? scheme.TableAlignment));

            // add header
            var row = table.AddRow();
            row.TopPadding = 2;
            row.BottomPadding = 2;
            row.HeadingFormat = true;
            row.Format.Font.Bold = true;
            row.Shading.Color = Colors.LightGray;
            row.VerticalAlignment = VerticalAlignment.Center;
            scheme.Columns.ForEach(x => row.Cells[x.Index].AddParagraph(x.Name ?? string.Empty));

            // add rows
            //var itemsCount = 0;
            foreach (var item in items)
            {
                //itemsCount++;
                row = table.AddRow();
                row.TopPadding = 1;
                row.BottomPadding = 1;
                row.VerticalAlignment = VerticalAlignment.Center;
                foreach (var col in scheme.Columns)
                {
                    var value = col.ValueFn(item);
                    var cell = row.Cells[col.Index];
                    cell.AddParagraph(value?.ToString() ?? string.Empty);
                }
            }

            //table.SetEdge(0, 0, scheme.Columns.Count, itemsCount+1, Edge.Box, BorderStyle.Single, 1, Colors.Black);
            document.LastSection.Add(table);
        }

    }
}
