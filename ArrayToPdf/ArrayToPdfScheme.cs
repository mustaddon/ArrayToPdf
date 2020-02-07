using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RandomSolutions
{
    public class ArrayToPdfScheme<T>
    {
        internal ArrayToPdfScheme()
        {
            _initDefaultColumns();
        }


        public string Title;
        public string Author;
        public string Subject;

        public ArrayToPdfOrientations PageOrientation = ArrayToPdfOrientations.Landscape;
        public ArrayToPdfFormats PageFormat = ArrayToPdfFormats.A4;
        /// <summary>Margin in Millimeters</summary>
        public uint PageMarginTop = 5;
        /// <summary>Margin in Millimeters</summary>
        public uint PageMarginRight = 5;
        /// <summary>Margin in Millimeters</summary>
        public uint PageMarginBottom = 5;
        /// <summary>Margin in Millimeters</summary>
        public uint PageMarginLeft = 5;

        public string Header = "{TITLE}\t{PAGE}/{PAGES}";
        /// <summary>Height in Millimeters</summary>
        public uint HeaderHeight = 7;
        /// <summary>FontSize in Points</summary>
        public uint HeaderFontSize = 10;
        public bool HeaderFontBold = false;
        public ArrayToPdfAlignments HeaderAlignment = ArrayToPdfAlignments.Left;

        public string Footer;
        /// <summary>Height in Millimeters</summary>
        public uint FooterHeight = 7;
        /// <summary>FontSize in Points</summary>
        public uint FooterFontSize = 10;
        public bool FooterFontBold = false;
        public ArrayToPdfAlignments FooterAlignment = ArrayToPdfAlignments.Left;

        /// <summary>FontSize in Points</summary>
        public uint TableFontSize = 8;
        public ArrayToPdfAlignments TableAlignment = ArrayToPdfAlignments.Center;

        /// <param name="width">in Millimeters</param>
        public ArrayToPdfScheme<T> AddColumn(string name, Func<T, object> value, uint? width = null, ArrayToPdfAlignments? alignment = null)
        {
            (_columns ?? (_columns = new List<Column>())).Add(new Column
            {
                Index = _columns.Count,
                Name = name,
                ValueFn = value,
                Width = width,
                Alignment = alignment,
            });
            return this;
        }


        void _initDefaultColumns()
        {
            var members = typeof(T).GetMembers(BindingFlags.Instance | BindingFlags.Public)
                 .Where(x => x is PropertyInfo || x is FieldInfo);

            foreach (var member in members)
                _defaultColumns.Add(new Column
                {
                    Index = _defaultColumns.Count,
                    Name = member.Name,
                    ValueFn = new Func<T, object>(x => (member as PropertyInfo)?.GetValue(x) ?? (member as FieldInfo)?.GetValue(x))
                });
        }


        List<Column> _defaultColumns = new List<Column>();
        List<Column> _columns;

        internal List<Column> Columns => _columns ?? _defaultColumns;

        internal class Column
        {
            public int Index { get; set; }
            public string Name { get; set; }
            public Func<T, object> ValueFn { get; set; }
            public uint? Width { get; set; }
            public ArrayToPdfAlignments? Alignment { get; set; }
        }
    }
}
