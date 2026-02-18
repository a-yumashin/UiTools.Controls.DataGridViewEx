using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace UiTools.Controls.ExtendedDataGridView
{
    internal static class MiscExtensions
    {
        internal static void RemoveRightSideEmptySpace(this ContextMenuStrip ctxMenu)
        {
            // Inspired by https://stackoverflow.com/questions/23724662/how-to-remove-right-side-empty-space-in-toolstripmenuitem
            var padding = new Padding(0, 0, -12, 0);
            typeof(ContextMenuStrip).BaseType.GetField("ArrowPadding", BindingFlags.NonPublic | BindingFlags.Static)
                .SetValue(null, padding);
            typeof(ContextMenuStrip).BaseType.GetField("scaledArrowPadding", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(ctxMenu, padding);
        }

        internal static IEnumerable<EnumMemberDetails> GetEnumAttributes(this Type enumType)
        {
            if (!enumType.IsEnum)
                throw new ArgumentException($"{nameof(enumType)} must be an enumerated type");

            return Enum.GetValues(enumType)
                .Cast<object>()
                .Select(e =>
                {
                    var fieldInfo = e.GetType().GetField(e.ToString());
                    var attrDescr = fieldInfo.GetCustomAttribute<DescriptionAttribute>();
                    var attrHint = fieldInfo.GetCustomAttribute<HintAttribute>();
                    return new EnumMemberDetails(
                        (int)e,
                        e.ToString(),
                        attrDescr != null ? attrDescr.Description : e.ToString(),
                        attrHint != null ? attrHint.Hint : "");
                });
        }

        internal static string GetDescription(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var attrDescr = fieldInfo.GetCustomAttribute<DescriptionAttribute>(false);
            return attrDescr != null ? attrDescr.Description : value.ToString();
        }

        internal static string GetHint(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var attrHint = fieldInfo.GetCustomAttribute<HintAttribute>(false);
            return attrHint != null ? attrHint.Hint : string.Empty;
        }

        public static bool In(this object obj, IEnumerable<object> values)
        {
            return values.Contains(obj);
        }

        public static void MoveIntegerPartToTheRight(this MaskedTextBox mtb)
        {
            int sepPos = mtb.Mask.IndexOf('.');
            int maskIntPartLength = mtb.Mask.Length - sepPos;
            var textIntegerPart = mtb.Text.Substring(0, sepPos).Trim();
            if (textIntegerPart.Length < maskIntPartLength)
            {
                var textDecimalPart = mtb.Text.Substring(sepPos + 1).Trim();
                mtb.Text = new string('_', maskIntPartLength - textIntegerPart.Length + 1) + textIntegerPart + "." + textDecimalPart;
            }
        }

        public static void MoveIntegerNumberToTheRight(this MaskedTextBox mtb)
        {
            int maskIntPartLength = mtb.Mask.Length;
            var textIntegerPart = mtb.Text.Trim();
            if (textIntegerPart.Length < maskIntPartLength)
                mtb.Text = new string('_', maskIntPartLength - textIntegerPart.Length) + textIntegerPart;
        }

        private static NumberFormatInfo myDecimalNumberFormat = new NumberFormatInfo { NumberDecimalSeparator = "," };
        public static decimal ParseAsDecimal(this string value)
        {
            // Allows both decimal separators (dot and comma)
            if (value == null)
                throw new ArgumentNullException("value");
            return decimal.Parse(value.Replace(".", myDecimalNumberFormat.NumberDecimalSeparator),
                NumberStyles.AllowDecimalPoint, myDecimalNumberFormat);
        }

        public static string ToHtml(this Color color)
        {
            return ColorTranslator.ToHtml(Color.FromArgb(color.ToArgb()));
        }

        public static string PrepareForCsv(this string s)
        {
            return s.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
        }

        //internal static void SetFromDGVContentAlignment(this StringFormat stringFormat, DataGridViewContentAlignment dgvContentAlignment)
        //{
        //    switch (dgvContentAlignment)
        //    {
        //        case DataGridViewContentAlignment.TopLeft:
        //            stringFormat.LineAlignment = StringAlignment.Near;
        //            stringFormat.Alignment = StringAlignment.Near;
        //            break;
        //        case DataGridViewContentAlignment.TopCenter:
        //            stringFormat.LineAlignment = StringAlignment.Near;
        //            stringFormat.Alignment = StringAlignment.Center;
        //            break;
        //        case DataGridViewContentAlignment.TopRight:
        //            stringFormat.LineAlignment = StringAlignment.Near;
        //            stringFormat.Alignment = StringAlignment.Far;
        //            break;
        //        case DataGridViewContentAlignment.NotSet: // treating NotSet as MiddleLeft (why not?)
        //        case DataGridViewContentAlignment.MiddleLeft:
        //            stringFormat.LineAlignment = StringAlignment.Center;
        //            stringFormat.Alignment = StringAlignment.Near;
        //            break;
        //        case DataGridViewContentAlignment.MiddleCenter:
        //            stringFormat.LineAlignment = StringAlignment.Center;
        //            stringFormat.Alignment = StringAlignment.Center;
        //            break;
        //        case DataGridViewContentAlignment.MiddleRight:
        //            stringFormat.LineAlignment = StringAlignment.Center;
        //            stringFormat.Alignment = StringAlignment.Far;
        //            break;
        //        case DataGridViewContentAlignment.BottomLeft:
        //            stringFormat.LineAlignment = StringAlignment.Far;
        //            stringFormat.Alignment = StringAlignment.Near;
        //            break;
        //        case DataGridViewContentAlignment.BottomCenter:
        //            stringFormat.LineAlignment = StringAlignment.Far;
        //            stringFormat.Alignment = StringAlignment.Center;
        //            break;
        //        case DataGridViewContentAlignment.BottomRight:
        //            stringFormat.LineAlignment = StringAlignment.Far;
        //            stringFormat.Alignment = StringAlignment.Far;
        //            break;
        //    }
        //}
    }

    internal class EnumMemberDetails
    {
        public int Value { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Hint { get; private set; }

        public EnumMemberDetails(int value, string name, string description, string hint)
        {
            Value = value;
            Name = name;
            Description = description;
            Hint = hint;
        }
    }
}
