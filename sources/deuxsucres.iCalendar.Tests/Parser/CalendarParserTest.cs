using deuxsucres.iCalendar.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.iCalendar.Tests.Parser
{
    public class CalendarParserTest
    {

        #region Read
        [Fact]
        public void ReadContentLine()
        {
            ContentLine content;
            var parser = new CalendarParser();

            // Text
            string test = "DESCRIPTION:This is a long description text in a only one line of text.\nDESCRIPTION:This is the second line.";
            using (var rdr = new StringReader(test))
            {
                Assert.NotNull((content = parser.ReadContentLine(rdr)));
                Assert.Equal("DESCRIPTION", content.Name);
                Assert.Equal("This is a long description text in a only one line of text.", content.Value);
                Assert.Equal(0, content.ParamCount);

                Assert.NotNull((content = parser.ReadContentLine(rdr)));
                Assert.Equal("DESCRIPTION", content.Name);
                Assert.Equal("This is the second line.", content.Value);
                Assert.Equal(0, content.ParamCount);

                Assert.Null((content = parser.ReadContentLine(rdr)));
            }

            // Null reader
            Assert.Null(parser.ReadContentLine(null));
        }

        [Fact]
        public void ReadContentLine_SimpleLine()
        {
            ContentLine content;
            var parser = new CalendarParser();

            // Simple line
            string test = "DESCRIPTION:This is a long description text in a only one line of text.";
            using (var rdr = new StringReader(test))
            {
                Assert.NotNull((content = parser.ReadContentLine(rdr)));
                Assert.Equal("DESCRIPTION", content.Name);
                Assert.Equal("This is a long description text in a only one line of text.", content.Value);
                Assert.Equal(0, content.ParamCount);

                Assert.Null((content = parser.ReadContentLine(rdr)));
            }

        }

        [Fact]
        public void ReadContentLine_FoldedLine()
        {
            ContentLine content;
            var parser = new CalendarParser();

            // Line folded
            string test = @"
DESCRIPTION:This is a lo
 ng description text
  in a only one line of text.";
            using (var rdr = new StringReader(test))
            {
                Assert.NotNull((content = parser.ReadContentLine(rdr)));
                Assert.Equal("DESCRIPTION", content.Name);
                Assert.Equal("This is a long description text in a only one line of text.", content.Value);
                Assert.Equal(0, content.ParamCount);
            }
        }

        [Fact]
        public void ReadContentLine_PropertyNameOnly()
        {
            ContentLine content;
            var parser = new CalendarParser();

            // Name only
            string test = "DESCRIPTION";
            using (var rdr = new StringReader(test))
            {
                Assert.NotNull((content = parser.ReadContentLine(rdr)));
                Assert.Equal("DESCRIPTION", content.Name);
                Assert.Null(content.Value);
                Assert.Equal(0, content.ParamCount);
            }

        }

        [Fact]
        public void ReadContentLine_PropertyParameters()
        {
            ContentLine content;
            var parser = new CalendarParser();

            // Parameters
            string test = "PROPERTY;PARAM1=value1;PARAM2=value2:Value";
            using (var rdr = new StringReader(test))
            {
                Assert.NotNull((content = parser.ReadContentLine(rdr)));
                Assert.Equal("PROPERTY", content.Name);
                Assert.Equal("Value", content.Value);
                Assert.Equal(2, content.ParamCount);
                Assert.Equal("value1", content["PARAM1"]);
                Assert.Equal("value2", content["PARAM2"]);

                Assert.Null((content = parser.ReadContentLine(rdr)));
            }
        }

        [Fact]
        public void ReadContentLine_PropertyEncodedParameters()
        {
            ContentLine content;
            var parser = new CalendarParser();

            // Encoded Parameters
            string test = "PROPERTY;PARAM1=\"va;l=u:e1\";PARAM2=value2:Value";
            using (var rdr = new StringReader(test))
            {
                Assert.NotNull((content = parser.ReadContentLine(rdr)));
                Assert.Equal("PROPERTY", content.Name);
                Assert.Equal("Value", content.Value);
                Assert.Equal(2, content.ParamCount);
                Assert.Equal("\"va;l=u:e1\"", content["PARAM1"]);
                Assert.Equal("value2", content["PARAM2"]);
            }

            test = "PROPERTY;PARAM1=value1;PARAM2=\"va;l=u:e,2\",\"Value3\",value 4,\"Value 5\":Value";
            using (var rdr = new StringReader(test))
            {
                Assert.NotNull((content = parser.ReadContentLine(rdr)));
                Assert.Equal("PROPERTY", content.Name);
                Assert.Equal("Value", content.Value);
                Assert.Equal(2, content.ParamCount);
                Assert.Equal("value1", content["PARAM1"]);
                Assert.Equal("\"va;l=u:e,2\",\"Value3\",value 4,\"Value 5\"", content["PARAM2"]);
            }
        }

        [Fact]
        public void ReadContentLine_PropertyOtherCases()
        {
            ContentLine content;
            var parser = new CalendarParser();

            // Other cases
            string test = @"ATTENDEE;RSVP=TRUE;ROLE=REQ-PARTICIPANT:MAILTO:jsmith@host.com";
            using (var rdr = new StringReader(test))
            {
                Assert.NotNull((content = parser.ReadContentLine(rdr)));
                Assert.Equal("ATTENDEE", content.Name);
                Assert.Equal("MAILTO:jsmith@host.com", content.Value);
                Assert.Equal(2, content.ParamCount);
                Assert.Equal("TRUE", content["RSVP"]);
                Assert.Equal("REQ-PARTICIPANT", content["ROLE"]);
            }

            test = @"RDATE;VALUE=DATE:19970304,19970504,19970704,19970904";
            using (var rdr = new StringReader(test))
            {
                Assert.NotNull((content = parser.ReadContentLine(rdr)));
                Assert.Equal("RDATE", content.Name);
                Assert.Equal("19970304,19970504,19970704,19970904", content.Value);
                Assert.Equal(1, content.ParamCount);
                Assert.Equal("DATE", content["VALUE"]);
            }

            test = @"DESCRIPTION;ALTREP=""http://www.wiz.org"":The Fall'98 Wild Wizards
  Conference - -Las Vegas\, NV\, USA";
            using (var rdr = new StringReader(test))
            {
                Assert.NotNull((content = parser.ReadContentLine(rdr)));
                Assert.Equal("DESCRIPTION", content.Name);
                Assert.Equal("The Fall'98 Wild Wizards Conference - -Las Vegas\\, NV\\, USA", content.Value);
                Assert.Equal(1, content.ParamCount);
                Assert.Equal("\"http://www.wiz.org\"", content["ALTREP"]);
            }
        }

        [Fact]
        public void ReadContentLine_PropertySpecialCases()
        {
            ContentLine content;
            var parser = new CalendarParser();

            // Special cases
            string test = "PROPERTY:Value1:Value2";
            using (var rdr = new StringReader(test))
            {
                Assert.NotNull((content = parser.ReadContentLine(rdr)));
                Assert.Equal("PROPERTY", content.Name);
                Assert.Equal("Value1:Value2", content.Value);
                Assert.Equal(0, content.ParamCount);
            }

            test = "PROPERTY;PARAM1=\"va;l=u:e1;PARAM2=value2:Value";
            using (var rdr = new StringReader(test))
            {
                Assert.NotNull((content = parser.ReadContentLine(rdr)));
                Assert.Equal("PROPERTY", content.Name);
                Assert.Null(content.Value);
                Assert.Equal(1, content.ParamCount);
                Assert.Equal("\"va;l=u:e1;PARAM2=value2:Value", content["PARAM1"]);
            }

            test = "PROPERTY;PARAM1=\"";
            using (var rdr = new StringReader(test))
            {
                Assert.NotNull((content = parser.ReadContentLine(rdr)));
                Assert.Equal("PROPERTY", content.Name);
                Assert.Null(content.Value);
                Assert.Equal(1, content.ParamCount);
                Assert.Equal("\"", content["PARAM1"]);
            }

            test = "PROPERTY=Value1:Value2";
            using (var rdr = new StringReader(test))
            {
                Assert.NotNull((content = parser.ReadContentLine(rdr)));
                Assert.Equal("PROPERTY=Value1", content.Name);
                Assert.Equal("Value2", content.Value);
                Assert.Equal(0, content.ParamCount);
            }

            test = "PROPERTY;Value1:Value2";
            using (var rdr = new StringReader(test))
            {
                Assert.NotNull((content = parser.ReadContentLine(rdr)));
                Assert.Equal("PROPERTY", content.Name);
                Assert.Equal("Value2", content.Value);
                Assert.Equal(1, content.ParamCount);
                Assert.Equal("", content["Value1"]);
            }

            test = "PROPERTY;Value1=:Value2";
            using (var rdr = new StringReader(test))
            {
                Assert.NotNull((content = parser.ReadContentLine(rdr)));
                Assert.Equal("PROPERTY", content.Name);
                Assert.Equal("Value2", content.Value);
                Assert.Equal(1, content.ParamCount);
                Assert.Equal("", content["Value1"]);
            }

            test = "PROPERTY;Value1=";
            using (var rdr = new StringReader(test))
            {
                Assert.NotNull((content = parser.ReadContentLine(rdr)));
                Assert.Equal("PROPERTY", content.Name);
                Assert.Null(content.Value);
                Assert.Equal(1, content.ParamCount);
                Assert.Equal("", content["Value1"]);
            }

            test = "PROPERTY;Value1";
            using (var rdr = new StringReader(test))
            {
                Assert.NotNull((content = parser.ReadContentLine(rdr)));
                Assert.Equal("PROPERTY", content.Name);
                Assert.Null(content.Value);
                Assert.Equal(1, content.ParamCount);
                Assert.Equal("", content["Value1"]);
            }

            test = "PROPERTY;";
            using (var rdr = new StringReader(test))
            {
                Assert.NotNull((content = parser.ReadContentLine(rdr)));
                Assert.Equal("PROPERTY", content.Name);
                Assert.Null(content.Value);
                Assert.Equal(0, content.ParamCount);
            }

            test = "PROPERTY=";
            using (var rdr = new StringReader(test))
            {
                Assert.NotNull((content = parser.ReadContentLine(rdr)));
                Assert.Equal("PROPERTY=", content.Name);
                Assert.Null(content.Value);
                Assert.Equal(0, content.ParamCount);
            }

            test = "PROPERTY:";
            using (var rdr = new StringReader(test))
            {
                Assert.NotNull((content = parser.ReadContentLine(rdr)));
                Assert.Equal("PROPERTY", content.Name);
                Assert.Equal(string.Empty, content.Value);
                Assert.Equal(0, content.ParamCount);
            }
        }

        #endregion

        #region Parse

        [Fact]
        public void ParseBinary()
        {
            var parser = new CalendarParser();

            byte[] binaryValue = Encoding.UTF8.GetBytes("Binary value Test");

            Assert.Equal(binaryValue, parser.ParseBinary("QmluYXJ5IHZhbHVlIFRlc3Q="));

            Assert.Null(parser.ParseBinary(null));
        }

        [Fact]
        public void ParseBoolean()
        {
            var parser = new CalendarParser();

            Assert.True(parser.ParseBoolean("True"));
            Assert.False(parser.ParseBoolean("False"));
            Assert.Null(parser.ParseBoolean("Test"));
            Assert.Null(parser.ParseBoolean(" "));

            Assert.Null(parser.ParseBoolean(null));
        }

        public static IEnumerable<object[]> ParseCalAddressData()
        {
            return new[]
            {
                // Encoded not required
                new object[] { "MAILTO:yan@deuxsucres.com", false, true, "mailto:yan@deuxsucres.com" },
                new object[] { "MailTo:yan@deuxsucres.com", false, true, "mailto:yan@deuxsucres.com" },
                new object[] { "MailTo:yan", false, true, "mailto:yan" },
                new object[] { "test", false, false, null },
                new object[] { "\"MAILTO:yan@deuxsucres.com\"", false, true, "mailto:yan@deuxsucres.com" },
                new object[] { "\"MailTo:yan@deuxsucres.com\"", false, true, "mailto:yan@deuxsucres.com" },
                new object[] { "\"MailTo:yan\"", false, true, "mailto:yan" },
                new object[] { "\"test\"", false, false, null },
                new object[] { null, false, false, null },

                // Encoded required
                new object[] { "MAILTO:yan@deuxsucres.com", true, false, null },
                new object[] { "MailTo:yan@deuxsucres.com", true, false, null },
                new object[] { "MailTo:yan", true, false, null },
                new object[] { "test", true, false, null },
                new object[] { "\"MAILTO:yan@deuxsucres.com\"", true, true, "mailto:yan@deuxsucres.com" },
                new object[] { "\"MailTo:yan@deuxsucres.com\"", true, true, "mailto:yan@deuxsucres.com" },
                new object[] { "\"MailTo:yan\"", true, true, "mailto:yan" },
                new object[] { "\"test\"", true, false, null },
                new object[] { null, true, false, null }
            };
        }

        [Theory, MemberData(nameof(ParseCalAddressData))]
        public void ParseCalAddress(string source, bool encoded, bool isNotNull, string expected)
        {
            var parser = new CalendarParser();

            Uri uri;
            uri = parser.ParseCalAddress(source, encoded);
            if (isNotNull)
            {
                Assert.NotNull(uri);
                Assert.Equal(expected, uri.AbsoluteUri.ToString());
            }
            else
            {
                Assert.Null(uri);
            }
        }

        [Fact]
        public void ParseDate()
        {
            var parser = new CalendarParser();

            DateTime? date;
            date = parser.ParseDate("20170116");
            Assert.NotNull(date);
            Assert.Equal(new DateTime(2017, 1, 16, 0, 0, 0, DateTimeKind.Local), date);
            Assert.Equal(DateTimeKind.Local, date.Value.Kind);

            date = parser.ParseDate("2017-01-16");
            Assert.NotNull(date);
            Assert.Equal(new DateTime(2017, 1, 16, 0, 0, 0, DateTimeKind.Local), date);
            Assert.Equal(DateTimeKind.Local, date.Value.Kind);

            Assert.Null(parser.ParseDate("test"));

            Assert.Null(parser.ParseDate(null));
        }

        [Fact]
        public void ParseDateTime()
        {
            var parser = new CalendarParser();

            DateTime? date;
            date = parser.ParseDateTime("20170116T230000");
            Assert.NotNull(date);
            Assert.Equal(new DateTime(2017, 1, 16, 23, 0, 0, DateTimeKind.Local), date);
            Assert.Equal(DateTimeKind.Local, date.Value.Kind);

            date = parser.ParseDateTime("20170116T070000Z");
            Assert.NotNull(date);
            Assert.Equal(new DateTime(2017, 1, 16, 7, 0, 0, DateTimeKind.Utc), date);
            Assert.Equal(DateTimeKind.Utc, date.Value.Kind);

            date = parser.ParseDateTime("20170116T0700Z");
            Assert.NotNull(date);
            Assert.Equal(new DateTime(2017, 1, 16, 7, 0, 0, DateTimeKind.Utc), date);
            Assert.Equal(DateTimeKind.Utc, date.Value.Kind);

            date = parser.ParseDateTime("20170116T07Z");
            Assert.NotNull(date);
            Assert.Equal(new DateTime(2017, 1, 16, 7, 0, 0, DateTimeKind.Utc), date);
            Assert.Equal(DateTimeKind.Utc, date.Value.Kind);

            date = parser.ParseDateTime("20170116T070000");
            Assert.NotNull(date);
            Assert.Equal(new DateTime(2017, 1, 16, 7, 0, 0, DateTimeKind.Utc), date);
            Assert.Equal(DateTimeKind.Local, date.Value.Kind);

            date = parser.ParseDateTime("20170116T0700");
            Assert.NotNull(date);
            Assert.Equal(new DateTime(2017, 1, 16, 7, 0, 0, DateTimeKind.Utc), date);
            Assert.Equal(DateTimeKind.Local, date.Value.Kind);

            date = parser.ParseDateTime("20170116T07");
            Assert.NotNull(date);
            Assert.Equal(new DateTime(2017, 1, 16, 7, 0, 0, DateTimeKind.Utc), date);
            Assert.Equal(DateTimeKind.Local, date.Value.Kind);

            date = parser.ParseDateTime("20170116");
            Assert.NotNull(date);
            Assert.Equal(new DateTime(2017, 1, 16, 0, 0, 0, DateTimeKind.Local), date);
            Assert.Equal(DateTimeKind.Local, date.Value.Kind);

            date = parser.ParseDateTime("2017-01-16 07:00:00");
            Assert.NotNull(date);
            Assert.Equal(new DateTime(2017, 1, 16, 7, 0, 0, DateTimeKind.Local), date);
            Assert.Equal(DateTimeKind.Local, date.Value.Kind);

            Assert.Null(parser.ParseDateTime("test"));

            Assert.Null(parser.ParseDateTime(null));
        }

        [Fact]
        public void ParseTime()
        {
            var parser = new CalendarParser();

            DateTime? date;
            date = parser.ParseTime("230000");
            Assert.NotNull(date);
            Assert.Equal(new DateTime(1, 1, 1, 23, 0, 0, DateTimeKind.Local), date);
            Assert.Equal(DateTimeKind.Local, date.Value.Kind);

            date = parser.ParseTime("070000Z");
            Assert.NotNull(date);
            Assert.Equal(new DateTime(1, 1, 1, 7, 0, 0, DateTimeKind.Utc), date);
            Assert.Equal(DateTimeKind.Utc, date.Value.Kind);

            Assert.Null(parser.ParseTime("test"));

            Assert.Null(parser.ParseTime(null));
        }

        [Fact]
        public void ParseDuration()
        {
            var parser = new CalendarParser();

            TimeSpan? duration;

            duration = parser.ParseDuration("P15DT5H0M20S");
            Assert.NotNull(duration);
            Assert.Equal(new TimeSpan(15, 5, 0, 20), duration);

            duration = parser.ParseDuration("P7W");
            Assert.NotNull(duration);
            Assert.Equal(new TimeSpan(7 * 7, 0, 0, 0), duration);

            duration = parser.ParseDuration("+p15dt5h0m20s");
            Assert.NotNull(duration);
            Assert.Equal(new TimeSpan(15, 5, 0, 20), duration);

            duration = parser.ParseDuration("-P15DT5H0M20S");
            Assert.NotNull(duration);
            Assert.Equal(-new TimeSpan(15, 5, 0, 20), duration);

            duration = parser.ParseDuration("-P7W");
            Assert.NotNull(duration);
            Assert.Equal(-new TimeSpan(7 * 7, 0, 0, 0), duration);

            duration = parser.ParseDuration("PT5H30M");
            Assert.NotNull(duration);
            Assert.Equal(new TimeSpan(0, 5, 30, 0), duration);

            Assert.Null(parser.ParseDuration("test"));
            Assert.Null(parser.ParseDuration("pz"));
            Assert.Null(parser.ParseDuration("p5H"));
            Assert.Null(parser.ParseDuration("p5D7H"));
            Assert.Null(parser.ParseDuration("p5DT"));
            Assert.Null(parser.ParseDuration("p5DT7HM"));
            Assert.Null(parser.ParseDuration("p5DT7H6MS"));
            Assert.Null(parser.ParseDuration("p5DT7H6M5S0"));

            Assert.Null(parser.ParseDuration(null));
        }

        [Fact]
        public void ParseFloat()
        {
            var parser = new CalendarParser();

            double? value;

            value = parser.ParseFloat("12.34");
            Assert.NotNull(value);
            Assert.Equal(12.34, value);

            value = parser.ParseFloat("+12.34");
            Assert.NotNull(value);
            Assert.Equal(12.34, value);

            value = parser.ParseFloat("-12.34");
            Assert.NotNull(value);
            Assert.Equal(-12.34, value);

            Assert.Null(parser.ParseFloat("test"));

            Assert.Null(parser.ParseFloat(null));
        }

        [Fact]
        public void ParseInt()
        {
            var parser = new CalendarParser();

            int? value;

            value = parser.ParseInt("12");
            Assert.NotNull(value);
            Assert.Equal(12, value);

            value = parser.ParseInt("+12");
            Assert.NotNull(value);
            Assert.Equal(12, value);

            value = parser.ParseInt("-12");
            Assert.NotNull(value);
            Assert.Equal(-12, value);

            Assert.Null(parser.ParseInt("test"));

            Assert.Null(parser.ParseInt(null));
        }

        [Fact]
        public void ParsePeriod()
        {
            var parser = new CalendarParser();

            Period value;

            value = parser.ParsePeriod("19970101T180000Z/19970102T070000Z");
            Assert.NotNull(value);
            Assert.Equal(new DateTime(1997, 1, 1, 18, 0, 0), value.DateStart);
            Assert.NotNull(value.DateEnd);
            Assert.Equal(new DateTime(1997, 1, 2, 7, 0, 0), value.DateEnd);
            Assert.Null(value.Duration);

            value = parser.ParsePeriod("19970101T180000Z/PT5H30M");
            Assert.NotNull(value);
            Assert.Equal(new DateTime(1997, 1, 1, 18, 0, 0), value.DateStart);
            Assert.Null(value.DateEnd);
            Assert.NotNull(value.Duration);
            Assert.Equal(new TimeSpan(5, 30, 0), value.Duration);

            Assert.Null(parser.ParsePeriod("test"));
            Assert.Null(parser.ParsePeriod("test/PT5H30M"));
            Assert.Null(parser.ParsePeriod("19970101T180000Z/test"));

            Assert.Null(parser.ParsePeriod(null));
        }

        [Fact]
        public void ParseRecur()
        {
            var parser = new CalendarParser();

            var recur = parser.ParseRecur("FREQ=MONTHLY;BYDAY=MO,2TU,+6WE,TH,-3FR;BYSETPOS=-1");
            Assert.NotNull(recur);
            Assert.Equal(Recurrence.Frequencies.Monthly, recur.Frequency);
            Assert.Equal(Recurrence.Weekdays.MO, recur.StartWeek);
            Assert.Null(recur.Others);
            Assert.Null(recur.Interval);
            Assert.NotNull(recur.ByDay);
            Assert.Equal(5, recur.ByDay.Count);
            Assert.Null(recur.ByDay[0].Order);
            Assert.Equal(Recurrence.Weekdays.MO, recur.ByDay[0].Weekday);
            Assert.Equal(2, recur.ByDay[1].Order);
            Assert.Equal(Recurrence.Weekdays.TU, recur.ByDay[1].Weekday);
            Assert.Equal(6, recur.ByDay[2].Order);
            Assert.Equal(Recurrence.Weekdays.WE, recur.ByDay[2].Weekday);
            Assert.Null(recur.ByDay[3].Order);
            Assert.Equal(Recurrence.Weekdays.TH, recur.ByDay[3].Weekday);
            Assert.Equal(-3, recur.ByDay[4].Order);
            Assert.Equal(Recurrence.Weekdays.FR, recur.ByDay[4].Weekday);
            Assert.Equal(new int[] { -1 }, recur.BySetPos?.ToArray());

            recur = parser.ParseRecur("FREQ=YEARLY;INTERVAL=2;BYMONTH=1;BYDAY=SU;BYHOUR=8,9;BYMINUTE=30");
            Assert.NotNull(recur);
            Assert.Equal(Recurrence.Frequencies.Yearly, recur.Frequency);
            Assert.Equal(2, recur.Interval);
            Assert.NotNull(recur.ByDay);
            Assert.Equal(1, recur.ByDay.Count);
            Assert.Null(recur.ByDay[0].Order);
            Assert.Equal(Recurrence.Weekdays.SU, recur.ByDay[0].Weekday);
            Assert.Equal(new int[] { 8, 9 }, recur.ByHour?.ToArray());
            Assert.Equal(new int[] { 1 }, recur.ByMonth?.ToArray());
            Assert.Equal(new int[] { 30 }, recur.ByMinute?.ToArray());

            recur = parser.ParseRecur("freq=daily;count=10;interval=2");
            Assert.NotNull(recur);
            Assert.NotNull(recur);
            Assert.Equal(Recurrence.Frequencies.Daily, recur.Frequency);
            Assert.Equal(2, recur.Interval);
            Assert.Equal(10, recur.Count);
            Assert.Null(recur.Until);

            recur = parser.ParseRecur("FREQ=DAILY;COUNT=10;INTERVAL=2;UNTIL=20100201");
            Assert.NotNull(recur);
            Assert.NotNull(recur);
            Assert.Equal(Recurrence.Frequencies.Daily, recur.Frequency);
            Assert.Equal(2, recur.Interval);
            Assert.Null(recur.Count);
            Assert.Equal(new DateTime(2010, 2, 1), recur.Until);

            recur = parser.ParseRecur("FREQ=DAILY;;UNTIL=20100201;COUNT=10;INTERVAL=2;BYSECOND=1,2;BYMINUTE=3;BYHOUR=4,10;BYMONTH=1,6;BYMONTHDAY=1;BYYEARDAY=1;BYWEEKNO=1;WKST=SU;yanos=test;");
            Assert.NotNull(recur);
            Assert.Equal(Recurrence.Weekdays.SU, recur.StartWeek);
            Assert.Equal(Recurrence.Frequencies.Daily, recur.Frequency);
            Assert.Equal(2, recur.Interval);
            Assert.Equal(10, recur.Count);
            Assert.Null(recur.Until);
            Assert.Equal(new int[] { 1, 2 }, recur.BySecond?.ToArray());
            Assert.Equal(new int[] { 3 }, recur.ByMinute?.ToArray());
            Assert.Equal(new int[] { 4, 10 }, recur.ByHour?.ToArray());
            Assert.Equal(new int[] { 1, 6 }, recur.ByMonth?.ToArray());
            Assert.Equal(new int[] { 1 }, recur.ByMonthDay?.ToArray());
            Assert.Equal(new int[] { 1 }, recur.ByYearDay?.ToArray());
            Assert.Equal(new int[] { 1 }, recur.ByWeekNo?.ToArray());
            Assert.NotNull(recur.Others);
            Assert.Equal(new Dictionary<string, string>
            {
                { "yanos", "test" }
            }, recur.Others);

            Assert.Null(parser.ParseRecur(""));
            Assert.Null(parser.ParseRecur("FREQ=DAILY;COUNT;INTERVAL=2"));
            Assert.Null(parser.ParseRecur("FREQ=Test"));
            Assert.Null(parser.ParseRecur("UNTIL=Test"));
            Assert.Null(parser.ParseRecur("COUNT=Test"));
            Assert.Null(parser.ParseRecur("INTERVAL=Test"));
            Assert.Null(parser.ParseRecur("BYSECOND=Test"));
            Assert.Null(parser.ParseRecur("BYMINUTE=500"));
            Assert.Null(parser.ParseRecur("BYHOUR=-5"));
            Assert.Null(parser.ParseRecur("BYMONTH="));
            Assert.Null(parser.ParseRecur("BYMONTHDAY=0"));
            Assert.Null(parser.ParseRecur("BYYEARDAY=0"));
            Assert.Null(parser.ParseRecur("BYWEEKNO=0"));
            Assert.Null(parser.ParseRecur("BYSETPOS=0"));
            Assert.Null(parser.ParseRecur("WKST=test"));
            Assert.Null(parser.ParseRecur("BYDAY="));
            Assert.Null(parser.ParseRecur("BYDAY=MO, ,FR"));
            Assert.Null(parser.ParseRecur("BYDAY=test"));
            Assert.Null(parser.ParseRecur("BYDAY=-MO"));
            Assert.Null(parser.ParseRecur("BYDAY=0MO"));
            Assert.Null(parser.ParseRecur("BYDAY=+100MO"));
            Assert.Null(parser.ParseRecur("BYDAY=-100MO"));
        }

        [Fact]
        public void ParseText()
        {
            var parser = new CalendarParser();
            string nl = Environment.NewLine;
            Assert.Equal($"a\\b{nl}c{nl}d;e,f", parser.ParseText(@"a\\b\Nc\nd\;e\,f"));

            Assert.Null(parser.ParseText(null));
        }

        public static IEnumerable<object[]> ParseUriData()
        {
            return new[]
            {
                // Encoded not required
                new object[] { "http://www.deuxsucres.com", false, true, "http://www.deuxsucres.com/" },
                new object[] { "test", false, false, null },
                new object[] { "\"http://www.deuxsucres.com\"", false, true, "http://www.deuxsucres.com/" },
                new object[] { "\"test\"", false, false, null },
                new object[] { null, false, false, null },

                // Encoded required
                new object[] { "http://www.deuxsucres.com", true, false, null },
                new object[] { "test", true, false, null },
                new object[] { "\"http://www.deuxsucres.com\"", true, true, "http://www.deuxsucres.com/" },
                new object[] { "\"test\"", true, false, null },
                new object[] { null, true, false, null }
            };
        }

        [Theory, MemberData(nameof(ParseUriData))]
        public void ParseUri(string source, bool encoded, bool isNotNull, string expected)
        {
            var parser = new CalendarParser();

            Uri uri;
            uri = parser.ParseUri(source, encoded);
            if (isNotNull)
            {
                Assert.NotNull(uri);
                Assert.Equal(expected, uri.AbsoluteUri.ToString());
            }
            else
            {
                Assert.Null(uri);
            }
        }

        [Fact]
        public void ParseUtcOffset()
        {
            var parser = new CalendarParser();

            TimeSpan? offset;

            offset = parser.ParseUtcOffset("-0500");
            Assert.NotNull(offset);
            Assert.Equal(-new TimeSpan(5, 0, 0), offset);

            offset = parser.ParseUtcOffset("+0100");
            Assert.NotNull(offset);
            Assert.Equal(new TimeSpan(1, 0, 0), offset);

            offset = parser.ParseUtcOffset("010203");
            Assert.NotNull(offset);
            Assert.Equal(new TimeSpan(1, 2, 3), offset);

            Assert.Null(parser.ParseUtcOffset("test"));

            Assert.Null(parser.ParseUtcOffset(null));
        }

        public enum EnumTest
        {
            Value,
            SecondValue,
            Third_Value
        }

        [Fact]
        public void ParseEnum()
        {
            var parser = new CalendarParser();

            Assert.Equal(EnumTest.Value, parser.ParseEnum<EnumTest>("value"));
            Assert.Equal(EnumTest.SecondValue, parser.ParseEnum<EnumTest>("SECONDVALUE"));
            Assert.Equal(EnumTest.Third_Value, parser.ParseEnum<EnumTest>("THIRD-Value"));
            Assert.Equal(EnumTest.Third_Value, parser.ParseEnum<EnumTest>("third_value"));

            Assert.Null(parser.ParseEnum<EnumTest>("Test"));
            Assert.Null(parser.ParseEnum<EnumTest>(null));
        }

        [Fact]
        public void ParseTextList()
        {
            var parser = new CalendarParser();

            Assert.Equal(new string[] { }, parser.ParseTextList(""));
            Assert.Equal(new string[] { "test" }, parser.ParseTextList("test"));
            Assert.Equal(new string[] { "test1", "test2" }, parser.ParseTextList("test1,test2"));
            Assert.Equal(new string[] { "test1", "test2" }, parser.ParseTextList("test1,test2,,"));
            Assert.Equal(new string[] { "test1,test2" }, parser.ParseTextList(@"test1\,test2"));
            Assert.Equal(new string[] { "test1,test2", "test3", "test4;test5" }, parser.ParseTextList(@"test1\,test2,test3,test4\;test5"));

            Assert.Null(parser.ParseTextList(null));
        }

        #endregion

        #region Write

        [Fact]
        public void WriteLine()
        {
            var parser = new CalendarParser();

            StringBuilder result = new StringBuilder();
            using (var wrt = new StringWriter(result))
            {
                Assert.Equal(0, parser.WriteLine(wrt, null));
            }
            Assert.Equal(string.Empty, result.ToString());

            result = new StringBuilder();
            using (var wrt = new StringWriter(result))
            {
                Assert.Equal(1, parser.WriteLine(wrt, "Test of line"));
            }
            Assert.Equal("Test of line" + Environment.NewLine, result.ToString());

            string test = "This is a very long line of text to test multiline write behavior. For this test the line need to be larger than 75 characters. So for the test we use a large text for having 3 lines of text.";
            result = new StringBuilder();
            using (var wrt = new StringWriter(result))
            {
                Assert.Equal(3, parser.WriteLine(wrt, test));
            }
            string expected =
                  "This is a very long line of text to test multiline write behavior. For this" + Environment.NewLine
                + "  test the line need to be larger than 75 characters. So for the test we us" + Environment.NewLine
                + " e a large text for having 3 lines of text." + Environment.NewLine;
            Assert.Equal(expected, result.ToString());
        }

        [Fact]
        public void EncodeContentLine()
        {
            var parser = new CalendarParser();

            ContentLine line = null;
            Assert.Null(parser.EncodeContentLine(line));

            line = new ContentLine();
            Assert.Equal(":", parser.EncodeContentLine(line));

            line = new ContentLine
            {
                Name = "Name"
            };
            Assert.Equal("Name:", parser.EncodeContentLine(line));

            line = new ContentLine
            {
                Name = "Name",
                Value = "Value"
            };
            Assert.Equal("Name:Value", parser.EncodeContentLine(line));

            line = new ContentLine
            {
                Value = "Value"
            };
            Assert.Equal(":Value", parser.EncodeContentLine(line));

            line = new ContentLine
            {
                Name = "Name",
                Value = "Value"
            };
            line
                .SetParam("p1", null)
                .SetParam("p2", string.Empty)
                .SetParam("p3", "val")
                .SetParam("p4", "p=val")
                ;
            Assert.Equal("Name;p1=;p2=;p3=val;p4=\"p=val\":Value", parser.EncodeContentLine(line));
        }

        [Fact]
        public void WriteContentLine()
        {
            var parser = new CalendarParser();

            ContentLine line = new ContentLine
            {
                Name = "Name",
                Value = "This is a very long line of text to test multiline write behavior. For this test the line need to be larger than 75 characters. So for the test we use a large text for having 3 lines of text."
            };
            line
                .SetParam("p1", null)
                .SetParam("p2", string.Empty)
                .SetParam("p3", "val")
                .SetParam("p4", "p=val")
                ;
            StringBuilder result = new StringBuilder();
            using (var wrt = new StringWriter(result))
            {
                Assert.Equal(0, parser.WriteContentLine(wrt, null));
            }
            Assert.Equal(string.Empty, result.ToString());

            result = new StringBuilder();
            using (var wrt = new StringWriter(result))
            {
                Assert.Equal(3, parser.WriteContentLine(wrt, line));
            }
            string expected =
                  "Name;p1=;p2=;p3=val;p4=\"p=val\":This is a very long line of text to test mul" + Environment.NewLine
                + " tiline write behavior. For this test the line need to be larger than 75 ch" + Environment.NewLine
                + " aracters. So for the test we use a large text for having 3 lines of text." + Environment.NewLine;
            Assert.Equal(expected, result.ToString());
        }

        #endregion

        #region Encode

        [Fact]
        public void EncodeBinary()
        {
            var parser = new CalendarParser();

            byte[] binaryValue = Encoding.UTF8.GetBytes("Binary value Test");

            Assert.Equal("QmluYXJ5IHZhbHVlIFRlc3Q=", parser.EncodeBinary(binaryValue));

            Assert.Null(parser.EncodeBinary(null));
        }

        [Fact]
        public void EncodeBoolean()
        {
            var parser = new CalendarParser();

            Assert.Equal("TRUE", parser.EncodeBoolean(true));
            Assert.Equal("FALSE", parser.EncodeBoolean(false));
        }

        public static IEnumerable<object[]> EncodeCalAddressData()
        {
            return new[]
            {
                // Not encoded
                new object[] { new Uri("MAILTO:yan@deuxsucres.com"), false, true, "mailto:yan@deuxsucres.com" },
                new object[] { new Uri("MailTo:yan@deuxsucres.com"), false, true, "mailto:yan@deuxsucres.com" },
                new object[] { new Uri("mailto:yan"), false, true, "mailto:yan" },
                new object[] { null, false, false, null },

                // Encoded
                new object[] { new Uri("MAILTO:yan@deuxsucres.com"), true, true, "\"mailto:yan@deuxsucres.com\"" },
                new object[] { new Uri("MailTo:yan@deuxsucres.com"), true, true, "\"mailto:yan@deuxsucres.com\"" },
                new object[] { new Uri("mailto:yan"), true, true, "\"mailto:yan\"" },
                new object[] { null, true, false, null }
            };
        }

        [Theory, MemberData(nameof(EncodeCalAddressData))]
        public void EncodeCalAddress(Uri uri, bool encode, bool isNotNull, string expected)
        {
            var parser = new CalendarParser();

            string result = parser.EncodeCalAddress(uri, encode);
            if (isNotNull)
            {
                Assert.NotNull(result);
                Assert.Equal(expected, result);
            }
            else
            {
                Assert.Null(result);
            }
        }

        [Fact]
        public void EncodeDate()
        {
            var parser = new CalendarParser();

            Assert.Equal("20170116", parser.EncodeDate(new DateTime(2017, 1, 16, 0, 0, 0, DateTimeKind.Local)));
            Assert.Equal("20170116", parser.EncodeDate(new DateTime(2017, 1, 16, 23, 12, 5, DateTimeKind.Local)));
            Assert.Equal("20170116", parser.EncodeDate(new DateTime(2017, 1, 16, 0, 0, 0, DateTimeKind.Utc)));
            Assert.Equal("20170116", parser.EncodeDate(new DateTime(2017, 1, 16, 23, 12, 5, DateTimeKind.Utc)));
        }

        [Fact]
        public void EncodeDateTime()
        {
            var parser = new CalendarParser();

            Assert.Equal("20170116T000000", parser.EncodeDateTime(new DateTime(2017, 1, 16, 0, 0, 0, DateTimeKind.Local)));
            Assert.Equal("20170116T000000Z", parser.EncodeDateTime(new DateTime(2017, 1, 16, 0, 0, 0, DateTimeKind.Utc)));
            Assert.Equal("20170116T231205", parser.EncodeDateTime(new DateTime(2017, 1, 16, 23, 12, 5, DateTimeKind.Local)));
            Assert.Equal("20170116T231205Z", parser.EncodeDateTime(new DateTime(2017, 1, 16, 23, 12, 5, DateTimeKind.Utc)));
            Assert.Equal("20170116T231205", parser.EncodeDateTime(new DateTime(2017, 1, 16, 23, 12, 5, 987, DateTimeKind.Local)));
            Assert.Equal("20170116T231205Z", parser.EncodeDateTime(new DateTime(2017, 1, 16, 23, 12, 5, 987, DateTimeKind.Utc)));
        }

        [Fact]
        public void EncodeTime()
        {
            var parser = new CalendarParser();

            Assert.Equal("000000", parser.EncodeTime(new DateTime(2017, 1, 16, 0, 0, 0, DateTimeKind.Local)));
            Assert.Equal("000000Z", parser.EncodeTime(new DateTime(2017, 1, 16, 0, 0, 0, DateTimeKind.Utc)));
            Assert.Equal("231205", parser.EncodeTime(new DateTime(2017, 1, 16, 23, 12, 5, DateTimeKind.Local)));
            Assert.Equal("231205Z", parser.EncodeTime(new DateTime(2017, 1, 16, 23, 12, 5, DateTimeKind.Utc)));
            Assert.Equal("231205", parser.EncodeTime(new DateTime(2017, 1, 16, 23, 12, 5, 987, DateTimeKind.Local)));
            Assert.Equal("231205Z", parser.EncodeTime(new DateTime(2017, 1, 16, 23, 12, 5, 987, DateTimeKind.Utc)));
        }

        [Fact]
        public void EncodeDuration()
        {
            var parser = new CalendarParser();

            Assert.Null(parser.EncodeDuration(TimeSpan.Zero));
            Assert.Null(parser.EncodeDuration(new TimeSpan(0, 0, 0, 0)));
            Assert.Equal("P15D", parser.EncodeDuration(new TimeSpan(15, 0, 0, 0)));
            Assert.Equal("P6W", parser.EncodeDuration(new TimeSpan(7 * 6, 5, 0, 0)));
            Assert.Equal("P15DT5H", parser.EncodeDuration(new TimeSpan(15, 5, 0, 0)));
            Assert.Equal("P15DT5H12M", parser.EncodeDuration(new TimeSpan(15, 5, 12, 0)));
            Assert.Equal("P15DT5H12M20S", parser.EncodeDuration(new TimeSpan(15, 5, 12, 20)));
            Assert.Equal("P15DT5H20S", parser.EncodeDuration(new TimeSpan(15, 5, 0, 20)));
            Assert.Equal("P15DT20S", parser.EncodeDuration(new TimeSpan(15, 0, 0, 20)));
            Assert.Equal("PT5H12M20S", parser.EncodeDuration(new TimeSpan(0, 5, 12, 20)));
            Assert.Equal("PT12M20S", parser.EncodeDuration(new TimeSpan(0, 0, 12, 20)));
            Assert.Equal("PT20S", parser.EncodeDuration(new TimeSpan(0, 0, 0, 20)));
            Assert.Equal("-P15DT5H12M20S", parser.EncodeDuration(-new TimeSpan(15, 5, 12, 20)));
        }

        [Fact]
        public void EncodeFloat()
        {
            var parser = new CalendarParser();

            Assert.Equal("1234.56", parser.EncodeFloat(1234.56));
            Assert.Equal("-1234.56", parser.EncodeFloat(-1234.56));
        }

        [Fact]
        public void EncodeInt()
        {
            var parser = new CalendarParser();

            Assert.Equal("1234", parser.EncodeInt(1234));
            Assert.Equal("-1234", parser.EncodeInt(-1234));
        }

        [Fact]
        public void EncodePeriod()
        {
            var parser = new CalendarParser();

            Assert.Null(parser.EncodePeriod(null));

            Period period = new Period(new DateTime(1997, 1, 1, 18, 0, 0, DateTimeKind.Utc), new DateTime(1997, 1, 2, 7, 0, 0, DateTimeKind.Utc));
            Assert.Equal("19970101T180000Z/19970102T070000Z", parser.EncodePeriod(period));

            period = new Period(new DateTime(1997, 1, 1, 18, 0, 0, DateTimeKind.Utc), new TimeSpan(5, 30, 0));
            Assert.Equal("19970101T180000Z/PT5H30M", parser.EncodePeriod(period));

            period = new Period(new DateTime(1997, 1, 1, 18, 0, 0, DateTimeKind.Utc), TimeSpan.Zero);
            Assert.Null(parser.EncodePeriod(period));
        }

        [Fact]
        public void EncodeRecur()
        {
            var parser = new CalendarParser();

            Assert.Null(parser.EncodeRecur(null));

            Recurrence rec = new Recurrence();
            Assert.Equal("FREQ=SECONDLY", parser.EncodeRecur(rec));
            rec.Frequency = Recurrence.Frequencies.Daily;
            rec.Until = new DateTime(2017, 11, 18, 17, 46, 0);
            rec.Count = 23;
            rec.Interval = 2;
            rec.BySecond = new int[] { 3, 6, 9 };
            rec.ByMinute = new int[] { 7, 8, 9 };
            rec.ByHour = new int[] { 1, 2, 4 };
            rec.ByDay = new Recurrence.WeekdayNum[] {
                new Recurrence.WeekdayNum { Weekday = Recurrence.Weekdays.MO, Order = 1 },
                new Recurrence.WeekdayNum { Weekday = Recurrence.Weekdays.TH, Order = 3 }
            };
            rec.ByMonthDay = new int[] { 3, 2, 1 };
            rec.ByYearDay = new int[] { 9, 8, 7 };
            rec.ByWeekNo = new int[] { 9, 7, 5 };
            rec.ByMonth = new int[] { 5, 6 };
            rec.BySetPos = new int[] { 1, 9 };
            rec.StartWeek = Recurrence.Weekdays.TU;
            rec.Others = new Dictionary<string, string> {
                {"p1", "val1" },
                {"p2", "val2" }
            };
            Assert.Equal("FREQ=DAILY;UNTIL=20171118T174600;COUNT=23;INTERVAL=2;BYSECOND=3,6,9;BYMINUTE=7,8,9;BYHOUR=1,2,4;BYDAY=1MO,3TH;BYMONTHDAY=3,2,1;BYYEARDAY=9,8,7;BYWEEKNO=9,7,5;BYMONTH=5,6;BYSETPOS=1,9;WKST=TU;p1=val1;p2=val2", parser.EncodeRecur(rec));
        }

        [Fact]
        public void EncodeText()
        {
            var parser = new CalendarParser();
            string nl = Environment.NewLine;
            Assert.Equal(@"a\\b\nc\nd\;e\,f", parser.EncodeText($"a\\b{nl}c{nl}d;e,f"));

            Assert.Null(parser.EncodeText(null));
        }

        public static IEnumerable<object[]> EncodeUriData()
        {
            return new[]
            {
                // Not encoded
                new object[] { new Uri("http://www.deuxsucres.com"), false, true, "http://www.deuxsucres.com/" },
                new object[] { null, false, false, null },

                // Encoded
                new object[] { new Uri("http://www.deuxsucres.com"), true, true, "\"http://www.deuxsucres.com/\"" },
                new object[] { null, true, false, null }
            };
        }

        [Theory, MemberData(nameof(EncodeUriData))]
        public void EncodeUri(Uri source, bool encode, bool isNotNull, string expected)
        {
            var parser = new CalendarParser();

            string result = parser.EncodeUri(source, encode);
            if (isNotNull)
            {
                Assert.NotNull(result);
                Assert.Equal(expected, result);
            }
            else
            {
                Assert.Null(result);
            }
        }

        [Fact]
        public void EncodeUtcOffset()
        {
            var parser = new CalendarParser();

            Assert.Null(parser.EncodeUtcOffset(TimeSpan.Zero));
            Assert.Equal("-0500", parser.EncodeUtcOffset(-new TimeSpan(5, 0, 0)));
            Assert.Equal("+0100", parser.EncodeUtcOffset(new TimeSpan(1, 0, 0)));
            Assert.Equal("+010203", parser.EncodeUtcOffset(new TimeSpan(1, 2, 3)));
        }

        [Fact]
        public void EncodeEnum()
        {
            var parser = new CalendarParser();

            Assert.Equal("VALUE", parser.EncodeEnum(EnumTest.Value));
            Assert.Equal("SECONDVALUE", parser.EncodeEnum(EnumTest.SecondValue));
            Assert.Equal("THIRD-VALUE", parser.EncodeEnum(EnumTest.Third_Value));
            Assert.Null(parser.EncodeEnum<EnumTest>(null));
            Assert.Equal("THIRD-VALUE", parser.EncodeEnum((EnumTest?)EnumTest.Third_Value));
        }

        [Fact]
        public void EncodeList()
        {
            var parser = new CalendarParser();

            Assert.Equal("test", parser.EncodeList(new string[] { "test" }, s => s));
            Assert.Equal("test1,test2", parser.EncodeList(new string[] { "test1", "test2" }, s => s));
            Assert.Equal(@"test1\,test2,test3,test4\;test5", parser.EncodeList(new string[] { "test1,test2", "test3", "test4;test5" }, s => s));
            Assert.Equal("test1,test2", parser.EncodeList(new string[] { "test1", "test2" }, null));
            Assert.Equal("4,5", parser.EncodeList(new int[] { 4, 5 }, null));
            Assert.Equal("", parser.EncodeList(new string[] { }, s => s));
            Assert.Null(parser.EncodeList<string>(null, s => s));
        }

        [Fact]
        public void EncodeTextParameter()
        {
            var parser = new CalendarParser();

            Assert.Equal("", parser.EncodeTextParameter(""));
            Assert.Equal("test", parser.EncodeTextParameter("test"));
            Assert.Equal("\"test=value\"", parser.EncodeTextParameter("test=value"));
            Assert.Null(parser.EncodeTextParameter(null));
        }

        #endregion

    }
}
