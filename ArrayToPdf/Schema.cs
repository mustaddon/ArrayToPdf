using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ArrayToPdf
{
    internal class Schema
    {
        public Schema(List<ColumnSchema> columns, IEnumerable items)
        {
            Columns = columns;
            Items = items;
        }

        public List<ColumnSchema> Columns { get; set; }
        public IEnumerable Items { get; set; }

        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Subject { get; set; }

        public PdfOrientations PageOrientation { get; set; } = DefaultPageOrientation;
        public PdfFormats PageFormat { get; set; } = DefaultPageFormat;

        public uint PageMarginTop { get; set; } = DefaultPageMargin;
        public uint PageMarginRight { get; set; } = DefaultPageMargin;
        public uint PageMarginBottom { get; set; } = DefaultPageMargin;
        public uint PageMarginLeft { get; set; } = DefaultPageMargin;

        public string? Header { get; set; } = DefaultHeader;
        public uint HeaderHeight { get; set; } = DefaultHeaderHeight;
        public uint HeaderFontSize { get; set; } = DefaultHeaderFontSize;
        public bool HeaderFontBold { get; set; } = DefaultHeaderFontBold;
        public PdfAlignments HeaderAlignment { get; set; } = DefaultHeaderAlignment;

        public string? Footer { get; set; }
        public uint FooterHeight { get; set; } = DefaultHeaderHeight;
        public uint FooterFontSize { get; set; } = DefaultHeaderFontSize;
        public bool FooterFontBold { get; set; } = DefaultHeaderFontBold;
        public PdfAlignments FooterAlignment { get; set; } = DefaultHeaderAlignment;
        public uint TableFontSize { get; set; } = DefaultTableFontSize;
        public PdfAlignments TableAlignment { get; set; } = DefaultTableAlignment;



        public const PdfOrientations DefaultPageOrientation = PdfOrientations.Landscape;
        public const PdfFormats DefaultPageFormat = PdfFormats.A4;
        public const string DefaultHeader = "{TITLE}\t{PAGE}/{PAGES}";
        public const uint DefaultPageMargin = 5;
        public const uint DefaultHeaderHeight = 7;
        public const uint DefaultHeaderFontSize = 10;
        public const bool DefaultHeaderFontBold = false;
        public const PdfAlignments DefaultHeaderAlignment = PdfAlignments.Left;
        public const uint DefaultTableFontSize = 8;
        public const PdfAlignments DefaultTableAlignment = PdfAlignments.Center;
    }

    internal class ColumnSchema
    {
        public MemberInfo? Member { get; set; }
        public uint? Width { get; set; }
        public string Name { get; set; } = string.Empty;
        public Func<object, object?>? Value { get; set; }
        public PdfAlignments? Alignment { get; set; }
    }
}
