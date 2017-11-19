using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace deuxsucres.iCalendar.Parser
{
    /// <summary>
    /// Calendar file parser
    /// </summary>
    public class CalendarParser
    {
        #region Parsing

        /// <summary>
        /// Read the next content line
        /// </summary>
        public virtual ContentLine ReadContentLine(TextReader reader, IList<string> readden)
        {
            if (reader == null) return null;

            // Read the next line
            string line = reader.ReadLine();
            if (line == null)
                return null;
            line = line.Trim();
            readden?.Add(line);

            // Extract multiline 
            int peek;
            while (line == string.Empty || (peek = reader.Peek()) == 9 || peek == 32)
            {
                string sline = reader.ReadLine();
                readden?.Add(sline);
                if (line != string.Empty)
                    line += sline.Substring(1);
                else
                    line = sline;
            }

            var content = new ContentLine();

            // Read the name
            char[] partSeparators = new char[] { ':', ';' };
            int idx = line.IndexOfAny(partSeparators);
            if (idx < 0)
            {
                content.Name = line;
                return content;
            }
            else
            {
                content.Name = line.Substring(0, idx);
            }

            // Read the parameters
            char[] paramSeparators = new char[] { ':', '=', ';' };
            char[] paramListSeparators = new char[] { ':', ',', ';' };
            int pos = idx;
            while (pos < line.Length && line[pos] == ';')
            {
                pos++;  // Skip the separator
                idx = line.IndexOfAny(paramSeparators, pos);
                // End of line ?
                if (idx < 0)
                {
                    // Create an empty parameter
                    if (pos < line.Length)
                        content[line.Substring(pos)] = string.Empty;
                    return content;
                }
                // get the name
                string paramName = line.Substring(pos, idx - pos);
                string paramValue = string.Empty;
                // Whithout value ?
                if (line[idx] != '=')
                {
                    // Create an empty parameter
                    content[paramName] = paramValue;
                    // And continue
                    pos = idx;
                    continue;
                }
                // Check the value
                int sParam = idx + 1;
                pos = sParam - 1;
                do
                {
                    pos++;
                    // Encoded ?
                    if (pos < line.Length && line[pos] == '"')
                    {
                        idx = line.IndexOf('"', pos + 1);
                        if (idx < 0)
                        {
                            // Get the rest of the line as a parameter value
                            pos = line.Length;
                            break;
                        }
                        else
                        {
                            // Extract the parameter value
                            pos = idx + 1;
                        }
                    }
                    else
                    {
                        // Search a separator
                        idx = line.IndexOfAny(paramListSeparators, pos);
                        if (idx < 0)
                        {
                            // Get the rest of the line as a parameter value
                            pos = line.Length;
                            break;
                        }
                        else
                        {
                            // Extract the parameter value
                            paramValue = line.Substring(sParam, idx - pos);
                            pos = idx;
                        }
                    }
                } while (pos >= 0 && pos < line.Length && line[pos] == ',');
                if (pos >= 0)
                    paramValue = line.Substring(sParam, pos - sParam);
                // Create the parameter
                if (!string.IsNullOrEmpty(paramName) || !string.IsNullOrEmpty(paramValue))
                {
                    //paramValue = paramValue.Replace("=3D", "=").Replace("\\;", ";");
                    content[paramName] = paramValue;
                }
            }

            // Read the value
            content.Value = pos < line.Length ? line.Substring(pos + 1) : null;
            return content;
        }

        /// <summary>
        /// Read the next content line
        /// </summary>
        public ContentLine ReadContentLine(TextReader reader)
        {
            return ReadContentLine(reader, null);
        }

        #region Parse values

        protected readonly static Regex RxDate = new Regex(@"(\d{4})(\d{2})(\d{2})", RegexOptions.IgnoreCase);
        protected readonly static Regex RxDateTime = new Regex(@"(\d{4})(\d{2})(\d{2})T(\d{2})(\d{2})?(\d{2})?(Z?)", RegexOptions.IgnoreCase);
        protected readonly static Regex RxTime = new Regex(@"(\d{2})(\d{2})?(\d{2})?(Z?)", RegexOptions.IgnoreCase);
        protected readonly static Regex RxOffset = new Regex(@"(\+|\-)?(\d{1,2})\:?(\d{2})?\:?(\d{2})?");

        /// <summary>
        /// Split a list of values
        /// </summary>
        public virtual IEnumerable<T> SplitValue<T>(string value, Func<string, T> parse)
        {
            if (value == null) yield break;
            int ps = 0, pe = 0;
            int vl = value.Length;
            while (pe < vl)
            {
                char c = value[pe];
                if (c == '\\')
                {
                    pe++;
                }
                else if (c == ',')
                {
                    if (ps < pe)
                        yield return parse(value.Substring(ps, pe - ps));
                    ps = pe + 1;
                }
                pe++;
            }
            if (ps < vl)
                yield return parse(value.Substring(ps));
        }

        /// <summary>
        /// Parse a binary value
        /// </summary>
        public virtual byte[] ParseBinary(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            return Convert.FromBase64String(value);
        }

        /// <summary>
        /// Parse a boolean value
        /// </summary>
        public virtual bool? ParseBoolean(string value)
        {
            if (value.IsEqual("TRUE"))
                return true;
            else if (value.IsEqual("FALSE"))
                return false;
            return null;
        }

        /// <summary>
        /// Parse a calendar user address
        /// </summary>
        public virtual Uri ParseCalAddress(string value, bool encodedRequired)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            if (value.Length > 2 && value.StartsWith("\"") && value.EndsWith("\""))
                value = value.Substring(1, value.Length - 2);
            else if (encodedRequired)
                return null;
            if (!value.StartsWith("MAILTO:", StringComparison.OrdinalIgnoreCase))
                return null;
            return new Uri(value);
        }

        /// <summary>
        /// Parse a date
        /// </summary>
        public virtual DateTime? ParseDate(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            var match = RxDate.Match(value);
            if (match.Success)
                return new DateTime(
                    ParseInt(match.Groups[1].Value) ?? 0,
                    ParseInt(match.Groups[2].Value) ?? 0,
                    ParseInt(match.Groups[3].Value) ?? 0,
                    0, 0, 0, DateTimeKind.Local
                    );
            else if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime ret))
                return ret.Date;
            return null;

        }

        /// <summary>
        /// Parse as a datetime
        /// </summary>
        public virtual DateTime? ParseDateTime(string value, bool parseDate = true)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            var match = RxDateTime.Match(value);
            if (match.Success)
                return new DateTime(
                    ParseInt(match.Groups[1].Value) ?? 0,
                    ParseInt(match.Groups[2].Value) ?? 0,
                    ParseInt(match.Groups[3].Value) ?? 0,
                    ParseInt(match.Groups[4].Value) ?? 0,
                    ParseInt(match.Groups[5].Value) ?? 0,
                    ParseInt(match.Groups[6].Value) ?? 0,
                    match.Groups[match.Groups.Count - 1].Value.IsEqual("Z") ? DateTimeKind.Utc : DateTimeKind.Local);
            else if (parseDate && (match = RxDate.Match(value)).Success)
                return new DateTime(
                    ParseInt(match.Groups[1].Value) ?? 0,
                    ParseInt(match.Groups[2].Value) ?? 0,
                    ParseInt(match.Groups[3].Value) ?? 0,
                    0, 0, 0, DateTimeKind.Local
                    );
            else if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime ret))
                return ret;
            return null;
        }

        /// <summary>
        /// Parse as a time
        /// </summary>
        public virtual DateTime? ParseTime(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            var match = RxTime.Match(value);
            if (match.Success)
                return new DateTime(
                    1, 1, 1,
                    ParseInt(match.Groups[1].Value) ?? 0,
                    ParseInt(match.Groups[2].Value) ?? 0,
                    ParseInt(match.Groups[3].Value) ?? 0,
                    match.Groups[match.Groups.Count - 1].Value.IsEqual("Z") ? DateTimeKind.Utc : DateTimeKind.Local);
            return null;
        }

        /// <summary>
        /// Parse a duration
        /// </summary>
        public virtual TimeSpan? ParseDuration(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            value = value.Trim();
            bool neg = false;
            if (value.StartsWith("+"))
                value = value.Substring(1);
            else if (value.StartsWith("-"))
            {
                value = value.Substring(1);
                neg = true;
            }
            if (!value.StartsWith("P", StringComparison.OrdinalIgnoreCase))
                return null;
            value = value.Substring(1);

            Func<Tuple<int, string>> readNext = () =>
            {
                if (string.IsNullOrEmpty(value)) return null;
                string val = value;
                char c = val[0];
                val = val.Substring(1);
                if (!char.IsDigit(c)) return null;
                int rv = c - '0';
                while (val.Length > 0 && char.IsDigit(val[0]))
                {
                    c = val[0];
                    val = val.Substring(1);
                    rv = (rv * 10) + (c - '0');
                }
                if (val.Length == 0) return null;
                string rs = val.Substring(0, 1);
                val = val.Substring(1);
                value = val;
                return Tuple.Create(rv, rs);
            };

            var t = readNext();
            if (t == null && !value.StartsWith("T", StringComparison.OrdinalIgnoreCase))
                return null;

            if (t != null && t.Item2.IsEqual("W"))
            {
                int w = t.Item1 * 7;
                if (neg) w = -w;
                return TimeSpan.FromDays(w);
            }

            int d = 0, h = 0, m = 0, s = 0;

            if (t != null)
            {
                if (!t.Item2.IsEqual("D")) return null;
                d = t.Item1;
            }

            if (value != string.Empty && !value.StartsWith("T", StringComparison.OrdinalIgnoreCase))
                return null;

            if (value.StartsWith("T", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(1);
                t = readNext();
                if (t == null) return null;
                if (t.Item2.IsEqual("H"))
                {
                    h = t.Item1;
                    t = readNext();
                    if (t == null && value != string.Empty) return null;
                }
                if (t != null && t.Item2.IsEqual("M"))
                {
                    m = t.Item1;
                    t = readNext();
                    if (t == null && value != string.Empty) return null;
                }
                if (t != null && t.Item2.IsEqual("S"))
                {
                    s = t.Item1;
                }
                if (value != string.Empty) return null;
            }

            var result = new TimeSpan(d, h, m, s);
            if (neg) result = -result;
            return result;

            //dur-value  = (["+"] / "-") "P" (dur-date / dur-time / dur-week)

            //dur-date   = dur-day [dur-time]
            //dur-time   = "T" (dur-hour / dur-minute / dur-second)
            //dur-week   = 1*DIGIT "W"
            //dur-hour   = 1*DIGIT "H" [dur-minute]
            //dur-minute = 1*DIGIT "M" [dur-second]
            //dur-second = 1*DIGIT "S"
            //dur-day    = 1*DIGIT "D"            
        }

        /// <summary>
        /// Parse a float value
        /// </summary>
        public virtual double? ParseFloat(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            value = value.Trim();
            bool neg = false;
            if (value.StartsWith("+"))
                value = value.Substring(1);
            else if (value.StartsWith("-"))
            {
                value = value.Substring(1);
                neg = true;
            }
            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
            {
                if (neg) result = -result;
                return result;
            }
            return null;
        }

        /// <summary>
        /// Parse as a integer
        /// </summary>
        public virtual int? ParseInt(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            value = value.Trim();
            bool neg = false;
            if (value.StartsWith("+"))
                value = value.Substring(1);
            else if (value.StartsWith("-"))
            {
                value = value.Substring(1);
                neg = true;
            }
            if (int.TryParse(value, out int result))
            {
                if (neg) result = -result;
                return result;
            }
            return null;
        }

        /// <summary>
        /// Parse a period
        /// </summary>
        public virtual Period ParsePeriod(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            var parts = value.Split(new char[] { '/' }, 2);
            if (parts.Length != 2) return null;

            var dStart = ParseDateTime(parts[0]);
            if (!dStart.HasValue) return null;

            var dEnd = ParseDateTime(parts[1]);
            if (dEnd.HasValue)
                return new Period(dStart.Value, dEnd.Value);

            var duration = ParseDuration(parts[1]);
            if (duration.HasValue)
                return new Period(dStart.Value, duration.Value);
            return null;
        }

        /// <summary>
        /// Parse a list of integer with range check
        /// </summary>
        IList<int> ParseListInt(string value, int minRange, int maxRange, bool acceptNeg)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            var parts = value.Split(',');
            List<int> result = new List<int>();
            foreach (string part in parts)
            {
                int? v = ParseInt(part);
                if (v != null && (v >= minRange && v <= maxRange) || (acceptNeg && v >= -maxRange && v <= -minRange))
                    result.Add(v.Value);
                else
                    return null;
            }
            return result;
        }

        /// <summary>
        /// Parse a list of weekday recurrence
        /// </summary>
        IList<Recurrence.WeekdayNum> ParseListWeekdayNum(string value)
        {
            //weekdaynum = [([plus] ordwk / minus ordwk)] weekday
            //plus       = "+"
            //minus      = "-"
            //ordwk      = 1DIGIT / 2DIGIT       ; 1 à 53
            //weekday    = "SU" / "MO" / "TU" / "WE" / "TH" / "FR" / "SA"
            if (string.IsNullOrWhiteSpace(value)) return null;
            var parts = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<Recurrence.WeekdayNum> result = new List<Recurrence.WeekdayNum>();
            foreach (string part in parts.Select(p => p.Trim()))
            {
                if (string.IsNullOrEmpty(part)) return null;
                string source = part;
                int? v = null;
                bool? neg = null;
                if (source[0] == '+')
                {
                    source = source.Substring(1);
                    neg = false;
                }
                else if (source[0] == '-')
                {
                    source = source.Substring(1);
                    neg = true;
                }
                while (source.Length > 0 && char.IsDigit(source[0]))
                {
                    v = ((v ?? 0) * 10) + (source[0] - '0');
                    source = source.Substring(1);
                }
                if (neg.HasValue && !v.HasValue) return null;
                if (neg == true) v = -v;
                if (v.HasValue && !((v >= 1 && v <= 53) || (v >= -53 && v <= -1))) return null;

                if (!Enum.TryParse<Recurrence.Weekdays>(source, out Recurrence.Weekdays d))
                    return null;
                result.Add(new Recurrence.WeekdayNum
                {
                    Weekday = d,
                    Order = v
                });
            }
            return result;
        }

        /// <summary>
        /// Parse a recurrence
        /// </summary>
        public virtual Recurrence ParseRecur(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            var result = new Recurrence();

            var parts = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string part in parts)
            {
                var vparts = part.Split(new char[] { '=' }, 2);
                if (vparts.Length != 2) return null;
                switch (vparts[0].ToUpper())
                {
                    case "FREQ":
                        {
                            if (!Enum.TryParse<Recurrence.Frequencies>(vparts[1], true, out Recurrence.Frequencies fq))
                                return null;
                            result.Frequency = fq;
                            break;
                        }
                    case "UNTIL":
                        {
                            DateTime? dt = ParseDateTime(vparts[1]);
                            if (dt == null)
                                return null;
                            result.Until = dt;
                            result.Count = null;
                            break;
                        }
                    case "COUNT":
                        {
                            int? cnt = ParseInt(vparts[1]);
                            if (cnt == null)
                                return null;
                            result.Until = null;
                            result.Count = cnt;
                            break;
                        }
                    case "INTERVAL":
                        {
                            int? inter = ParseInt(vparts[1]);
                            if (inter == null)
                                return null;
                            result.Interval = inter;
                            break;
                        }
                    case "BYSECOND":
                        {
                            result.BySecond = ParseListInt(vparts[1], 0, 59, false);
                            if (result.BySecond == null)
                                return null;
                            break;
                        }
                    case "BYMINUTE":
                        {
                            result.ByMinute = ParseListInt(vparts[1], 0, 59, false);
                            if (result.ByMinute == null)
                                return null;
                            break;
                        }
                    case "BYHOUR":
                        {
                            result.ByHour = ParseListInt(vparts[1], 0, 23, false);
                            if (result.ByHour == null)
                                return null;
                            break;
                        }
                    case "BYDAY":
                        {
                            result.ByDay = ParseListWeekdayNum(vparts[1]);
                            if (result.ByDay == null)
                                return null;
                            break;
                        }
                    case "BYMONTHDAY":
                        {
                            result.ByMonthDay = ParseListInt(vparts[1], 1, 31, true);
                            if (result.ByMonthDay == null)
                                return null;
                            break;
                        }
                    case "BYYEARDAY":
                        {
                            result.ByYearDay = ParseListInt(vparts[1], 1, 366, true);
                            if (result.ByYearDay == null)
                                return null;
                            break;
                        }
                    case "BYWEEKNO":
                        {
                            result.ByWeekNo = ParseListInt(vparts[1], 1, 53, false);
                            if (result.ByWeekNo == null)
                                return null;
                            break;
                        }
                    case "BYMONTH":
                        {
                            result.ByMonth = ParseListInt(vparts[1], 1, 12, false);
                            if (result.ByMonth == null)
                                return null;
                            break;
                        }
                    case "BYSETPOS":
                        {
                            result.BySetPos = ParseListInt(vparts[1], 1, 366, true);
                            if (result.BySetPos == null)
                                return null;
                            break;
                        }
                    case "WKST":
                        {
                            if (!Enum.TryParse<Recurrence.Weekdays>(vparts[1], out Recurrence.Weekdays wd))
                                return null;
                            result.StartWeek = wd;
                            break;
                        }
                    default:
                        result.AddOther(vparts[0], vparts[1]);
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// Parse a text value
        /// </summary>
        public virtual string ParseText(string value)
        {
            if (value == null) return null;
            return value
                .Replace(@"\\", @"\")
                .Replace(@"\N", Environment.NewLine)
                .Replace(@"\n", Environment.NewLine)
                .Replace(@"\;", ";")
                .Replace(@"\,", ",")
                ;
        }

        /// <summary>
        /// Parse a list of value
        /// </summary>
        public virtual string[] ParseTextList(string value)
        {
            if (value == null) return null;
            return SplitValue(value, v => ParseText(v)).ToArray();
        }

        /// <summary>
        /// Parse an uri
        /// </summary>
        public virtual Uri ParseUri(string value, bool encodedRequired)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            try
            {
                if (value.Length > 2 && value.StartsWith("\"") && value.EndsWith("\""))
                    value = value.Substring(1, value.Length - 2);
                else if (encodedRequired)
                    return null;
                return new Uri(ParseText(value));
            }
            catch { return null; }
        }

        /// <summary>
        /// Parse an utc-offset
        /// </summary>
        public virtual TimeSpan? ParseUtcOffset(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            var match = RxOffset.Match(value);
            if (!match.Success) return null;
            bool neg = match.Groups[1].Value == "-";
            int h = ParseInt(match.Groups[2].Value) ?? 0;
            int m = ParseInt(match.Groups[3].Value) ?? 0;
            int s = ParseInt(match.Groups[4].Value) ?? 0;
            var off = new TimeSpan(h, m, s);
            return neg ? -off : off;
        }

        /// <summary>
        /// Parse an enum
        /// </summary>
        public virtual T? ParseEnum<T>(string value) where T : struct
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            if (Enum.TryParse<T>(value.Replace("-", "_"), true, out T e))
                return e;
            return null;
        }

        #endregion

        #endregion

        #region Encoding

        /// <summary>
        /// Write a splitted line
        /// </summary>
        /// <returns>Count of lines written</returns>
        public virtual int WriteLine(TextWriter writer, string line)
        {
            if (writer == null || string.IsNullOrEmpty(line)) return 0;
            string prefix = "";
            int count = 0;
            int lineSize = 75;
            while (true)
            {
                if (line.Length > lineSize)
                {
                    string tLine = line.Substring(0, lineSize);
                    line = line.Substring(lineSize);
                    writer.WriteLine(prefix + tLine);
                    prefix = " ";
                    lineSize = 74;
                    count++;
                }
                else
                {
                    writer.WriteLine(prefix + line);
                    count++;
                    break;
                }
            }
            return count;
        }

        /// <summary>
        /// Encode a content line
        /// </summary>
        public virtual string EncodeContentLine(ContentLine line)
        {
            if (line == null) return null;
            StringBuilder builder = new StringBuilder();
            builder.Append(line.Name);
            foreach (var prm in line.GetParams())
            {
                builder
                    .Append(";")
                    .Append(prm.Key)
                    .Append("=")
                    .Append(EncodeTextParameter(prm.Value))
                    ;
            }
            builder.Append(":").Append(line.Value);
            return builder.ToString();
        }

        /// <summary>
        /// Write a content line in a TextWriter
        /// </summary>
        /// <returns>Count of lines written</returns>
        public virtual int WriteContentLine(TextWriter writer, ContentLine line)
        {
            return WriteLine(writer, EncodeContentLine(line));
        }

        #region Encode values

        /// <summary>
        /// Encode a list
        /// </summary>
        public virtual string EncodeList<T>(IEnumerable<T> list, Func<T, string> encode)
        {
            if (list == null) return null;
            encode = encode ?? (v => v?.ToString());
            return string.Join(",", list.Select(v => EncodeText(encode(v))));
        }

        /// <summary>
        /// Encode a text value
        /// </summary>
        public virtual string EncodeText(string value)
        {
            if (value == null) return null;
            return value
                .Replace(@"\", @"\\")
                .Replace(";", @"\;")
                .Replace(",", @"\,")
                .Replace(Environment.NewLine, @"\n")
                .Replace("\n", @"\n")
                .Replace("\r", @"\n")
                ;
        }

        /// <summary>
        /// Encode a text parameter value
        /// </summary>
        public virtual string EncodeTextParameter(string value)
        {
            if (value == null) return null;
            if (value.IndexOfAny(new char[] { ',', ';', '=' }) >= 0)
                return $"\"{value}\"";
            return value;
        }

        /// <summary>
        /// Encode a date
        /// </summary>
        public virtual string EncodeDate(DateTime value)
        {
            return value.ToString("yyyyMMdd");
        }

        /// <summary>
        /// Encode a datetime
        /// </summary>
        public virtual string EncodeDateTime(DateTime value)
        {
            //string format = value.Second > 0 ? @"yyyyMMdd\THHmmss" : @"yyyyMMdd\THHmm";
            string format = @"yyyyMMdd\THHmmss";
            string result = value.ToString(format);
            if (value.Kind == DateTimeKind.Utc)
                result += "Z";
            return result;
        }

        /// <summary>
        /// Encode a time
        /// </summary>
        public virtual string EncodeTime(DateTime value)
        {
            string format = @"HHmmss";
            string result = value.ToString(format);
            if (value.Kind == DateTimeKind.Utc)
                result += "Z";
            return result;
        }

        /// <summary>
        /// Encode a duration
        /// </summary>
        public virtual string EncodeDuration(TimeSpan value)
        {
            if (value == TimeSpan.Zero) return null;
            StringBuilder builder = new StringBuilder();
            if (value < TimeSpan.Zero)
            {
                value = -value;
                builder.Append("-");
            }
            builder.Append("P");
            int days = value.Days;
            int hours = value.Hours;
            int min = value.Minutes;
            int sec = value.Seconds;
            if (days >= 7 && (days % 7 == 0))
            {
                builder.Append(days / 7).Append("W");
            }
            else
            {
                if (days > 0)
                    builder.Append(days).Append("D");
                if (hours > 0 || min > 0 || sec > 0)
                {
                    builder.Append("T");
                    if (hours > 0)
                        builder.Append(hours).Append("H");
                    if (min > 0)
                        builder.Append(min).Append("M");
                    if (sec > 0)
                        builder.Append(sec).Append("S");
                }
            }
            return builder.Length > 0 ? builder.ToString() : null;
        }

        /// <summary>
        /// Encode a cal-address
        /// </summary>
        public virtual string EncodeCalAddress(Uri value, bool encodeValue = true)
        {
            string val = value?.ToString();
            if (val == null) return null;
            if (encodeValue)
                val = $"\"{val}\"";
            return val;
        }

        /// <summary>
        /// Encode a Url
        /// </summary>
        public virtual string EncodeUri(Uri value, bool encodeValue = true)
        {
            string val = value?.ToString();
            if (val == null) return null;
            if (!value.Scheme.IsEqual("CID"))
                val = Uri.EscapeUriString(val);
            val = EncodeText(val);
            if (encodeValue)
                val = $"\"{val}\"";
            return val;
        }

        /// <summary>
        /// Encode an enum
        /// </summary>
        public virtual string EncodeEnum<T>(T? value) where T : struct
        {
            if (value == null) return null;
            return EncodeEnum(value.Value);
        }

        /// <summary>
        /// Encode an enum
        /// </summary>
        public virtual string EncodeEnum<T>(T value) where T : struct
        {
            return value.ToString().Replace("_", "-").ToUpper();
        }

        /// <summary>
        /// Encode a period
        /// </summary>
        public virtual string EncodePeriod(Period value)
        {
            if (value == null) return null;
            string p1 = EncodeDateTime(value.DateStart);
            string p2;
            if (value.DateEnd != null)
                p2 = EncodeDateTime(value.DateEnd.Value);
            else
                p2 = EncodeDuration(value.Duration.Value);
            if (p2 == null) return null;
            return $"{p1}/{p2}";
        }

        /// <summary>
        /// Encode an utc-offset
        /// </summary>
        public virtual string EncodeUtcOffset(TimeSpan value)
        {
            if (value == TimeSpan.Zero) return null;
            StringBuilder builder = new StringBuilder();
            if (value < TimeSpan.Zero)
            {
                value = -value;
                builder.Append("-");
            }
            else
            {
                builder.Append("+");
            }
            int hours = value.Hours;
            int min = value.Minutes;
            int sec = value.Seconds;
            builder.Append(hours.ToString("D2"));
            builder.Append(min.ToString("D2"));
            if (sec > 0)
                builder.Append(sec.ToString("D2"));
            return builder.Length > 0 ? builder.ToString() : null;
        }

        /// <summary>
        /// Encode a boolean value
        /// </summary>
        public virtual string EncodeBoolean(bool value)
        {
            return value ? "TRUE" : "FALSE";
        }

        /// <summary>
        /// Encode an integer value
        /// </summary>
        public virtual string EncodeInt(int value)
        {
            return value.ToString();
        }

        /// <summary>
        /// Encode a float value
        /// </summary>
        public virtual string EncodeFloat(double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Encode a binary value
        /// </summary>
        public virtual string EncodeBinary(byte[] value)
        {
            if (value == null) return null;
            return Convert.ToBase64String(value);
        }

        /// <summary>
        /// Encode a week day
        /// </summary>
        protected virtual string EncodeWeekDaynum(Recurrence.WeekdayNum value)
        {
            string o = string.Empty;
            if (value.Order.HasValue)
                o = EncodeInt(value.Order.Value);
            string d = EncodeEnum(value.Weekday);
            return o + d;
        }

        /// <summary>
        /// Encode a recurrence
        /// </summary>
        public virtual string EncodeRecur(Recurrence value)
        {
            if (value == null) return null;

            List<string> result = new List<string>
            {
                "FREQ=" + EncodeEnum(value.Frequency)
            };
            if (value.Until.HasValue)
                result.Add("UNTIL=" + EncodeDateTime(value.Until.Value));
            if (value.Count.HasValue)
                result.Add("COUNT=" + EncodeInt(value.Count.Value));
            if (value.Interval.HasValue)
                result.Add("INTERVAL=" + EncodeInt(value.Interval.Value));
            if (value.BySecond != null)
                result.Add("BYSECOND=" + EncodeList(value.BySecond, s => EncodeInt(s)));
            if (value.ByMinute != null)
                result.Add("BYMINUTE=" + EncodeList(value.ByMinute, s => EncodeInt(s)));
            if (value.ByHour != null)
                result.Add("BYHOUR=" + EncodeList(value.ByHour, s => EncodeInt(s)));
            if (value.ByDay != null)
                result.Add("BYDAY=" + EncodeList(value.ByDay, s => EncodeWeekDaynum(s)));
            if (value.ByMonthDay != null)
                result.Add("BYMONTHDAY=" + EncodeList(value.ByMonthDay, s => EncodeInt(s)));
            if (value.ByYearDay != null)
                result.Add("BYYEARDAY=" + EncodeList(value.ByYearDay, s => EncodeInt(s)));
            if (value.ByWeekNo != null)
                result.Add("BYWEEKNO=" + EncodeList(value.ByWeekNo, s => EncodeInt(s)));
            if (value.ByMonth != null)
                result.Add("BYMONTH=" + EncodeList(value.ByMonth, s => EncodeInt(s)));
            if (value.BySetPos != null)
                result.Add("BYSETPOS=" + EncodeList(value.BySetPos, s => EncodeInt(s)));
            if (value.StartWeek != Recurrence.Weekdays.MO)
                result.Add("WKST=" + EncodeEnum(value.StartWeek));
            if (value.Others != null)
            {
                foreach (var kvp in value.Others)
                    result.Add($"{kvp.Key}={kvp.Value}");
            }
            return string.Join(";", result);
        }

        #endregion

        #endregion
    }
}
