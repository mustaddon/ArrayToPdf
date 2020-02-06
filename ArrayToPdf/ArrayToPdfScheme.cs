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

        public ArrayToPdfScheme<T> SetTitle(string title)
        {
            Title = title;
            return this;
        }

        public ArrayToPdfScheme<T> SetOrientation(ArrayToPdfOrientations orientation)
        {
            Orientation = orientation;
            return this;
        }

        public ArrayToPdfScheme<T> SetAlignment(ArrayToPdfAlignments alignment)
        {
            Alignment = alignment;
            return this;
        }

        /// <summary>
        /// Page Margins in Millimeters
        /// </summary>
        public ArrayToPdfScheme<T> SetMargin(uint top, uint right, uint bottom, uint left)
        {
            MarginTop = top;
            MarginRight = right;
            MarginBottom = bottom;
            MarginLeft = left;
            return this;
        }

        /// <summary>
        /// Table font size in Points
        /// </summary>
        public ArrayToPdfScheme<T> SetFontSize(uint value)
        {
            FontSize = value;
            return this;
        }

        /// <summary>
        /// Width in Millimeters
        /// </summary>
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
        internal ArrayToPdfOrientations Orientation = ArrayToPdfOrientations.Landscape;
        internal ArrayToPdfAlignments Alignment = ArrayToPdfAlignments.Center;
        internal string Title;
        internal uint MarginTop = 5;
        internal uint MarginRight = 5;
        internal uint MarginBottom = 5;
        internal uint MarginLeft = 5;
        internal uint FontSize = 10;

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
