using System;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("Coffee.NanoMonitor.Tests")]

namespace Coffee.NanoMonitor
{
    internal static class StringBuilderExtensions
    {
        public static bool IsEqual(this StringBuilder sb, string other)
        {
            if (sb == null || other == null) return false;
            if (sb.Length != other.Length) return false;

            for (var i = 0; i < sb.Length; i++)
            {
                if (sb[i] != other[i]) return false;
            }

            return true;
        }

        public static void AppendFormatNoAlloc(this StringBuilder sb, string format, double arg0 = 0, double arg1 = 0,
            double arg2 = 0, double arg3 = 0)
        {
            for (var i = 0; i < format.Length; i++)
            {
                var c = format[i];

                // Append formatted value
                if (c == '{')
                {
                    i = GetFormat(format, i, out var argIndex, out var padding, out var precision, out var alignment);

                    switch (argIndex)
                    {
                        case 0:
                            sb.AppendDouble(arg0, padding, precision, alignment);
                            break;
                        case 1:
                            sb.AppendDouble(arg1, padding, precision, alignment);
                            break;
                        case 2:
                            sb.AppendDouble(arg2, padding, precision, alignment);
                            break;
                        case 3:
                            sb.AppendDouble(arg3, padding, precision, alignment);
                            break;
                    }

                    continue;
                }

                // Append character
                sb.Append(c);
            }
        }

        private static void AppendInteger(this StringBuilder sb, double number, int padding, int alignment)
        {
            number = Math.Truncate(number);
            var sign = number < 0;
            number = sign ? -number : number;

            var startIndex = sb.Length;
            do
            {
                var n = Math.Truncate(number % 10);
                number /= 10;

                sb.Append((char)(n + 48));
            } while (1 <= number || sb.Length - startIndex < padding);

            if (sign)
            {
                sb.Append('-');
            }

            var endIndex = sb.Length - 1;

            sb.Reverse(startIndex, endIndex);
            sb.Alignment(alignment, startIndex, endIndex);
        }

        private static void AppendDouble(this StringBuilder sb, double number, int padding, int precision,
            int alignment)
        {
            var integer = Math.Truncate(number);
            var startIndex = sb.Length;
            sb.AppendInteger(integer, padding, 0);

            if (0 < precision)
            {
                sb.Append('.');
                number -= integer;
                number = Math.Round(number, precision);
                for (var p = 0; p < precision; p++)
                {
                    number *= 10;
                    integer = (long)number;
                    sb.Append((char)(integer + 48));
                    number -= integer;
                    number = Math.Round(number, precision);
                }
            }

            sb.Alignment(alignment, startIndex, sb.Length - 1);
        }

        private static void Reverse(this StringBuilder sb, int start, int end)
        {
            while (start < end)
            {
                // swap
                (sb[start], sb[end]) = (sb[end], sb[start]);
                start++;
                end--;
            }
        }

        private static void Alignment(this StringBuilder sb, int alignment, int start, int end)
        {
            if (alignment == 0) return;

            var len = end - start + 1;
            if (0 < alignment && len < alignment)
            {
                sb.Append(' ', alignment - len);
                for (var i = 0; i < len; i++)
                {
                    // swap
                    (sb[end - i], sb[start + alignment - i - 1]) = (sb[start + alignment - i - 1], sb[end - i]);
                }
            }
            else if (alignment < 0 && len < -alignment)
            {
                sb.Append(' ', -alignment - len);
            }
        }

        private static int GetFormat(string format, int i, out int argIndex, out int padding, out int precision,
            out int alignment)
        {
            argIndex = -1;
            padding = 0;
            precision = 0;
            alignment = 0;

            var alignmentSign = false;
            var readFlag = 0;

            for (; i < format.Length; i++)
            {
                var c = format[i];

                // End format
                if (c == '}')
                {
                    return i;
                }

                // Start format
                if (c == '{')
                {
                    readFlag = 1;
                }

                // After '{': Read argument index and format
                else if (readFlag == 1)
                {
                    if ('0' <= c && c <= '3')
                    {
                        argIndex = c - 48;
                    }
                    else if (c == ',')
                    {
                        readFlag = 2;
                    }
                    else if (c == ':')
                    {
                        readFlag = 3;
                    }
                }

                //After ',': Read alignment value
                else if (readFlag == 2)
                {
                    if ('0' <= c && c <= '9')
                    {
                        alignment = alignment * 10 + (alignmentSign ? 48 - c : c - 48);
                    }
                    else if (c == '-')
                    {
                        alignmentSign = true;
                    }
                    else if (c == ':')
                    {
                        readFlag = 3;
                    }
                }

                // After ':': Read format for integral
                else if (readFlag == 3)
                {
                    if (c == '.')
                    {
                        precision = 0;
                        readFlag = 4;
                    }
                    else if (c == '0')
                    {
                        padding++;
                    }
                    // Legacy mode
                    else if ('1' <= c && c <= '9')
                    {
                        precision = c - 48;
                    }
                }

                // After '.': Read decimal precision value
                else if (readFlag == 4)
                {
                    if (c == '0')
                    {
                        precision++;
                    }
                }
            }

            argIndex = -1;
            return i;
        }
    }
}
