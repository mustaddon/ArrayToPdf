using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ArrayToPdf
{
    public class ArrayToPdf
    {
#if !NET45
        static ArrayToPdf()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
#endif

        public static MemoryStream CreatePdf<T>(IEnumerable<T> items, Action<SchemaBuilder<T>>? schema = null)
        {
            var builder = new SchemaBuilder<T>(items);
            schema?.Invoke(builder);
            return _createPdf(builder.Schema);
        }

        static MemoryStream _createPdf(Schema schema)
        {
            var ms = new MemoryStream();
            var renderer = new PdfDocumentRenderer(true);
            renderer.Document = _createDocument(schema);
            renderer.RenderDocument();
            renderer.PdfDocument.Save(ms);
            ms.Position = 0;
            return ms;
        }

        static readonly int _tableLeftBias = -1;

        static Document _createDocument(Schema schema)
        {
            var document = new Document();
            document.UseCmykColor = true;
            document.Info.Title = schema.Title;
            document.Info.Subject = schema.Subject;
            document.Info.Author = schema.Author;

            document.DefaultPageSetup.PageFormat = (PageFormat)schema.PageFormat;
            document.DefaultPageSetup.Orientation = (Orientation)schema.PageOrientation;
            document.DefaultPageSetup.HeaderDistance = Unit.FromMillimeter(schema.PageMarginTop);
            document.DefaultPageSetup.FooterDistance = Unit.FromMillimeter(schema.PageMarginBottom);
            document.DefaultPageSetup.TopMargin = Unit.FromMillimeter(schema.PageMarginTop + (string.IsNullOrWhiteSpace(schema.Header) ? 0 : schema.HeaderHeight));
            document.DefaultPageSetup.RightMargin = Unit.FromMillimeter(schema.PageMarginRight + _tableLeftBias);
            document.DefaultPageSetup.BottomMargin = Unit.FromMillimeter(schema.PageMarginBottom + (string.IsNullOrWhiteSpace(schema.Footer) ? 0 : schema.FooterHeight));
            document.DefaultPageSetup.LeftMargin = Unit.FromMillimeter(schema.PageMarginLeft - _tableLeftBias);

            Unit width, height;
            PageSetup.GetPageSize(document.DefaultPageSetup.PageFormat, out width, out height);
            document.DefaultPageSetup.PageWidth = width;
            document.DefaultPageSetup.PageHeight = height;

            var innerWidth = Unit.FromPoint(document.DefaultPageSetup.GetWidth() - document.DefaultPageSetup.LeftMargin - document.DefaultPageSetup.RightMargin);

            _addStyles(document, innerWidth);
            _addSection(document);
            _addHeader(document, innerWidth, schema);
            _addFooter(document, innerWidth, schema);
            _addTable(document, innerWidth, schema);

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

        static void _addHeader(Document document, Unit innerWidth, Schema schema)
        {
            if (string.IsNullOrWhiteSpace(schema.Header))
                return;

            var header = document.LastSection.Headers.Primary;
            var paragraph = header.AddParagraph();
            paragraph.Format.LeftIndent = Unit.FromMillimeter(_tableLeftBias);
            paragraph.Format.Alignment = (ParagraphAlignment)schema.HeaderAlignment;
            paragraph.Format.Font.Size = schema.HeaderFontSize;
            paragraph.Format.Font.Bold = schema.HeaderFontBold;
            _addTmpText(paragraph, schema.Header, schema);
        }

        static void _addFooter(Document document, Unit innerWidth, Schema schema)
        {
            if (string.IsNullOrWhiteSpace(schema.Footer))
                return;

            var footer = document.LastSection.Footers.Primary;
            var paragraph = footer.AddParagraph();
            paragraph.Format.LeftIndent = Unit.FromMillimeter(_tableLeftBias);
            paragraph.Format.Alignment = (ParagraphAlignment)schema.FooterAlignment;
            paragraph.Format.Font.Size = schema.FooterFontSize;
            paragraph.Format.Font.Bold = schema.FooterFontBold;
            _addTmpText(paragraph, schema.Footer, schema);
        }

        static void _addTmpText(Paragraph paragraph, string? template, Schema schema)
        {
            if(template != null)
                foreach (var part in template.Split(new[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries))
                    switch (part)
                    {
                        case "TITLE":
                            paragraph.AddText(schema.Title ?? string.Empty);
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

        static void _addTable(Document document, Unit innerWidth, Schema schema)
        {
            if (schema.Columns.Count == 0)
                return;

            var table = new Table();
            table.Format.Font.Size = Unit.FromPoint(schema.TableFontSize);
            table.Borders.Width = 0.5;


            var colWidth = Unit.FromPoint(innerWidth / schema.Columns.Count);
            var settedWidth = schema.Columns.Sum(x => x.Width ?? 0);
            var autoWidthCount = schema.Columns.Count(x => !x.Width.HasValue);
            var autoWidth = (innerWidth.Millimeter - settedWidth) / (autoWidthCount > 0 ? autoWidthCount : 1);

            // create columns
            schema.Columns.ForEach(x => table.AddColumn(Unit.FromMillimeter(x.Width ?? autoWidth)).Format.Alignment = (ParagraphAlignment)(x.Alignment ?? schema.TableAlignment));

            // add header
            var row = table.AddRow();
            row.TopPadding = 2;
            row.BottomPadding = 2;
            row.HeadingFormat = true;
            row.Format.Font.Bold = true;
            row.Shading.Color = Colors.LightGray;
            row.VerticalAlignment = VerticalAlignment.Center;

            var colIndex = 0;
            schema.Columns.ForEach(x => row.Cells[colIndex++].AddParagraph(x.Name ?? string.Empty));

            // add rows
            foreach (var item in schema.Items)
            {
                row = table.AddRow();
                row.TopPadding = 1;
                row.BottomPadding = 1;
                row.VerticalAlignment = VerticalAlignment.Center;

                colIndex = 0;
                foreach (var col in schema.Columns)
                {
                    var value = col.Value?.Invoke(item);
                    var cell = row.Cells[colIndex++];
                    cell.AddParagraph(value?.ToString() ?? string.Empty);
                }
            }

            //table.SetEdge(0, 0, scheme.Columns.Count, itemsCount+1, Edge.Box, BorderStyle.Single, 1, Colors.Black);
            document.LastSection.Add(table);
        }

    }
}
