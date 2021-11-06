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

        public ArrayToPdfOrientations PageOrientation { get; set; } = DefaultPageOrientation;
        public ArrayToPdfFormats PageFormat { get; set; } = DefaultPageFormat;

        public uint PageMarginTop { get; set; } = DefaultPageMargin;
        public uint PageMarginRight { get; set; } = DefaultPageMargin;
        public uint PageMarginBottom { get; set; } = DefaultPageMargin;
        public uint PageMarginLeft { get; set; } = DefaultPageMargin;

        public string? Header { get; set; } = DefaultHeader;
        public uint HeaderHeight { get; set; } = DefaultHeaderHeight;
        public uint HeaderFontSize { get; set; } = DefaultHeaderFontSize;
        public bool HeaderFontBold { get; set; } = DefaultHeaderFontBold;
        public ArrayToPdfAlignments HeaderAlignment { get; set; } = DefaultHeaderAlignment;

        public string? Footer { get; set; }
        public uint FooterHeight { get; set; } = DefaultHeaderHeight;
        public uint FooterFontSize { get; set; } = DefaultHeaderFontSize;
        public bool FooterFontBold { get; set; } = DefaultHeaderFontBold;
        public ArrayToPdfAlignments FooterAlignment { get; set; } = DefaultHeaderAlignment;
        public uint TableFontSize { get; set; } = DefaultTableFontSize;
        public ArrayToPdfAlignments TableAlignment { get; set; } = DefaultTableAlignment;



        public const ArrayToPdfOrientations DefaultPageOrientation = ArrayToPdfOrientations.Landscape;
        public const ArrayToPdfFormats DefaultPageFormat = ArrayToPdfFormats.A4;
        public const string DefaultHeader = "{TITLE}\t{PAGE}/{PAGES}";
        public const uint DefaultPageMargin = 5;
        public const uint DefaultHeaderHeight = 7;
        public const uint DefaultHeaderFontSize = 10;
        public const bool DefaultHeaderFontBold = false;
        public const ArrayToPdfAlignments DefaultHeaderAlignment = ArrayToPdfAlignments.Left;
        public const uint DefaultTableFontSize = 8;
        public const ArrayToPdfAlignments DefaultTableAlignment = ArrayToPdfAlignments.Center;
    }

    internal class ColumnSchema
    {
        public MemberInfo? Member { get; set; }
        public uint? Width { get; set; }
        public string Name { get; set; } = string.Empty;
        public Func<object, object?>? Value { get; set; }
        public ArrayToPdfAlignments? Alignment { get; set; }
    }
}
