using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ArrayToPdf
{
    public sealed class SchemaBuilder<T>
    {
        internal SchemaBuilder(IEnumerable items)
        {
            Schema = new(DefaultColumns(items), items);
        }

        private bool _defaultCols = true;

        internal Schema Schema { get; }

        public SchemaBuilder<T> Title(string? title) { Schema.Title = title; return this; }
        public SchemaBuilder<T> Author(string? author) { Schema.Author = author; return this; }
        public SchemaBuilder<T> Subject(string? subject) { Schema.Subject = subject; return this; }
        public SchemaBuilder<T> PageOrientation(PdfOrientations value = Schema.DefaultPageOrientation) { Schema.PageOrientation = value; return this; }
        public SchemaBuilder<T> PageFormat(PdfFormats value = Schema.DefaultPageFormat) { Schema.PageFormat = value; return this; }

        /// <summary>Margin in Millimeters</summary>
        public SchemaBuilder<T> PageMarginTop(uint value = Schema.DefaultPageMargin) { Schema.PageMarginTop = value; return this; }

        /// <summary>Margin in Millimeters</summary>
        public SchemaBuilder<T> PageMarginRight(uint value = Schema.DefaultPageMargin) { Schema.PageMarginRight = value; return this; }

        /// <summary>Margin in Millimeters</summary>
        public SchemaBuilder<T> PageMarginBottom(uint value = Schema.DefaultPageMargin) { Schema.PageMarginBottom = value; return this; }

        /// <summary>Margin in Millimeters</summary>
        public SchemaBuilder<T> PageMarginLeft(uint value = Schema.DefaultPageMargin) { Schema.PageMarginLeft = value; return this; }

        public SchemaBuilder<T> Header(string value = Schema.DefaultHeader) { Schema.Header = value; return this; }

        /// <summary>Height in Millimeters</summary>
        public SchemaBuilder<T> HeaderHeight(uint value = Schema.DefaultHeaderHeight) { Schema.HeaderHeight = value; return this; }

        /// <summary>FontSize in Points</summary>
        public SchemaBuilder<T> HeaderFontSize(uint value = Schema.DefaultHeaderFontSize) { Schema.HeaderFontSize = value; return this; }
        public SchemaBuilder<T> HeaderFontBold(bool value = Schema.DefaultHeaderFontBold) { Schema.HeaderFontBold = value; return this; }
        public SchemaBuilder<T> HeaderAlignment(PdfAlignments value = Schema.DefaultHeaderAlignment) { Schema.HeaderAlignment = value; return this; }

        public SchemaBuilder<T> Footer(string? value) { Schema.Footer = value; return this; }

        /// <summary>Height in Millimeters</summary>
        public SchemaBuilder<T> FooterHeight(uint value = Schema.DefaultHeaderHeight) { Schema.FooterHeight = value; return this; }

        /// <summary>FontSize in Points</summary>
        public SchemaBuilder<T> FooterFontSize(uint value = Schema.DefaultHeaderFontSize) { Schema.FooterFontSize = value; return this; }
        public SchemaBuilder<T> FooterFontBold(bool value = Schema.DefaultHeaderFontBold) { Schema.FooterFontBold = value; return this; }
        public SchemaBuilder<T> FooterAlignment(PdfAlignments value = Schema.DefaultHeaderAlignment) { Schema.FooterAlignment = value; return this; }

        /// <summary>FontSize in Points</summary>
        public SchemaBuilder<T> TableFontSize(uint value = Schema.DefaultTableFontSize) { Schema.TableFontSize = value; return this; }
        public SchemaBuilder<T> TableAlignment(PdfAlignments value = Schema.DefaultTableAlignment) { Schema.TableAlignment = value; return this; }


        public SchemaBuilder<T> ColumnName(Func<ColumnInfo, string> name)
        {
            foreach (var col in Schema.Columns.Select((x, i) => new ColumnInfo(i, x)))
                col.Schema.Name = name(col);
            return this;
        }

        public SchemaBuilder<T> ColumnWidth(Func<ColumnInfo, uint?> width)
        {
            foreach (var col in Schema.Columns.Select((x, i) => new ColumnInfo(i, x)))
                col.Schema.Width = width(col);
            return this;
        }

        public SchemaBuilder<T> ColumnAlignment(Func<ColumnInfo, PdfAlignments?> alignment)
        {
            foreach (var col in Schema.Columns.Select((x, i) => new ColumnInfo(i, x)))
                col.Schema.Alignment = alignment(col);
            return this;
        }

        public SchemaBuilder<T> ColumnFilter(Func<ColumnInfo, bool> filter)
        {
            Schema.Columns = Schema.Columns
                .Select((x, i) => new ColumnInfo(i, x))
                .Where(x => filter(x))
                .Select(x => x.Schema)
                .ToList();
            return this;
        }

        public SchemaBuilder<T> ColumnSort<TKey>(Func<ColumnInfo, TKey> sort, bool desc = false)
        {
            var colInfos = Schema.Columns.Select((x, i) => new ColumnInfo(i, x)).ToList();

            Schema.Columns = (desc
                ? colInfos.OrderByDescending(sort)
                : colInfos.OrderBy(sort)
            ).Select(x => x.Schema).ToList();

            return this;
        }

        public SchemaBuilder<T> ColumnValue(Func<ColumnInfo, T, object?> value)
        {
            foreach (var col in Schema.Columns.Select((x, i) => new ColumnInfo(i, x)))
                col.Schema.Value = x => value(col, (T)x);
            return this;
        }

        public SchemaBuilder<T> AddColumn(string name, Func<T, object?> value, uint? width = null)
        {
            if (_defaultCols)
            {
                Schema.Columns.Clear();
                _defaultCols = false;
            }

            Schema.Columns.Add(new()
            {
                Name = name,
                Value = x => value((T)x),
                Width = width,
            });

            return this;
        }

        private List<ColumnSchema> DefaultColumns(IEnumerable items)
        {
            var type = typeof(T);

            if (type == typeof(object))
            {
                var enumerator = items.GetEnumerator();

                if (!enumerator.MoveNext())
                    return new List<ColumnSchema>();

                type = enumerator.Current.GetType();
            }

            if (typeof(IDictionary<string, object?>).IsAssignableFrom(type))
            {
                var enumerator = items.GetEnumerator();
                enumerator.MoveNext();
                return (enumerator.Current as IDictionary<string, object?>)
                    ?.Select(kvp => new ColumnSchema()
                    {
                        Name = kvp.Key,
                        Value = new(x => (x as IDictionary<string, object?>)?[kvp.Key]),
                    })
                    .ToList() ?? new List<ColumnSchema>();
            }

            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                var enumerator = items.GetEnumerator();
                enumerator.MoveNext();
                var dict = (enumerator.Current as IDictionary)?.GetEnumerator();

                var result = new List<ColumnSchema>();

                while (dict?.MoveNext() == true)
                {
                    var key = dict.Key;
                    result.Add(new()
                    {
                        Name = key.ToString(),
                        Value = new(x => (x as IDictionary)?[key]),
                    });
                }

                return result;
            }

            return type.GetMembers(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x is PropertyInfo || x is FieldInfo)
                .Select(member => new ColumnSchema
                {
                    Member = member,
                    Name = member.Name,
                    Value = new(x => (member as PropertyInfo)?.GetValue(x) ?? (member as FieldInfo)?.GetValue(x)),
                })
                .ToList();
        }

    }
}
