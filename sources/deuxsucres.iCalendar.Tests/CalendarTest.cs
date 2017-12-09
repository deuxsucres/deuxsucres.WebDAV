using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using deuxsucres.iCalendar.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.iCalendar.Tests
{
    public class CalendarTest
    {

        [Fact]
        public void Create()
        {
            var cal = new Calendar();
            Assert.NotNull(cal);
            Assert.Equal("VCALENDAR", cal.Name);
            Assert.Equal(0, cal.ComponentCount);
            Assert.Equal(3, cal.PropertyCount);
            Assert.Equal(0, cal.Events.Count);
            Assert.Equal(0, cal.Todos.Count);
            Assert.Equal(0, cal.Journals.Count);
            Assert.Equal(0, cal.FreeBusies.Count);
            Assert.Equal(0, cal.Timezones.Count);

            Assert.Equal(new string[] { "PRODID", "VERSION", "CALSCALE" }, cal.GetProperties().Select(p => p.Name));

            Assert.Equal("-//deuxsucres//deuxsucres.iCalendar v1.0//EN", cal.ProdID);
            Assert.Equal("2.0", cal.Version);
            Assert.Equal("GREGORIAN", cal.Scale);
            Assert.Null(cal.Method);

            Assert.Equal(3, cal.PropertyCount);

            Assert.Equal(new string[] { "PRODID", "VERSION", "CALSCALE" }, cal.GetProperties().Select(p => p.Name));
        }

        [Fact]
        public void Properties()
        {
            var cal = new Calendar();
            Assert.Equal("VCALENDAR", cal.Name);
            Assert.Equal(0, cal.ComponentCount);
            Assert.Equal(3, cal.PropertyCount);

            Assert.Equal(new string[] { "PRODID", "VERSION", "CALSCALE" }, cal.GetProperties().Select(p => p.Name));

            cal.ProdID = "Test 1";
            cal.Version = "Test 2";
            cal.Scale = "Test 3";
            cal.Method = "Test 4";

            Assert.Equal("Test 1", cal.ProdID);
            Assert.Equal("Test 2", cal.Version);
            Assert.Equal("Test 3", cal.Scale);
            Assert.Equal("Test 4", cal.Method);

            Assert.Equal(4, cal.PropertyCount);

            Assert.Equal(new string[] { "CALSCALE", "METHOD", "PRODID", "VERSION" }, cal.GetProperties().Select(p => p.Name).OrderBy(n => n));
        }

        [Fact]
        public void Components()
        {
            var cal = new Calendar();
            Assert.Equal(0, cal.ComponentCount);

            var comp1 = new Event();
            var comp2 = new Event();

            cal.AddComponent(comp1);
            cal.AddComponent(comp2);
            cal.AddComponent(comp1);
            Assert.Equal(2, cal.ComponentCount);
            Assert.Equal(2, cal.Events.Count);
            Assert.Equal(0, cal.Todos.Count);
            Assert.Equal(0, cal.Journals.Count);
            Assert.Equal(0, cal.FreeBusies.Count);
            Assert.Equal(0, cal.Timezones.Count);

            Assert.Equal(new CalComponent[] { comp1, comp2 }, cal.GetComponents());

            cal.RemoveComponent(comp1);
            Assert.Equal(1, cal.ComponentCount);
            cal.RemoveComponent(comp1);
            Assert.Equal(1, cal.ComponentCount);
            Assert.Equal(new CalComponent[] { comp2 }, cal.GetComponents());

            cal.ClearComponents();
            Assert.Equal(0, cal.ComponentCount);
        }

        [Fact]
        public void Deserialize_Properties()
        {
            var parser = new CalendarParser();
            string input = @"BEGIN:VCALENDAR
VERSION:2.0
PRODID:-//hacksw/handcal//NONSGML v1.0//EN
CALSCALE:JULIAN
METHOD:REQUEST
Other:Value
END:VCALENDAR";
            using (var source = new StringReader(input))
            {
                var reader = new CalTextReader(parser, source, false);
                reader.ReadNextLine();

                var cal = new Calendar();
                cal.Deserialize(reader);
                Assert.Equal("VCALENDAR", cal.Name);
                Assert.Equal(0, cal.ComponentCount);
                Assert.Equal(5, cal.PropertyCount);

                Assert.Equal("-//hacksw/handcal//NONSGML v1.0//EN", cal.ProdID);
                Assert.Equal("2.0", cal.Version);
                Assert.Equal("JULIAN", cal.Scale);
                Assert.Equal("REQUEST", cal.Method);

                Assert.NotNull(cal.FindProperty("other"));
                Assert.IsType<TextProperty>(cal.FindProperty("other"));

                Assert.Equal(new string[] { "VERSION", "PRODID", "CALSCALE", "METHOD", "Other" }, cal.GetProperties().Select(p => p.Name));
            }
        }

        [Fact]
        public void Deserialize_Components()
        {
            var parser = new CalendarParser();
            string input = @"BEGIN:VCALENDAR
VERSION:2.0
PRODID:-//hacksw/handcal//NONSGML v1.0//EN
BEGIN:VEVENT
END:VEVENT
BEGIN:VEVENT
END:VEVENT
END:VCALENDAR";
            using (var source = new StringReader(input))
            {
                var reader = new CalTextReader(parser, source, false);
                reader.ReadNextLine();

                var cal = new Calendar();
                cal.Deserialize(reader);
                Assert.Equal("VCALENDAR", cal.Name);
                Assert.Equal(2, cal.ComponentCount);
                Assert.Equal(2, cal.Events.Count);
            }
        }

        [Fact]
        public void Deserialize_Sample1()
        {
            string resName = "sample-1.ics";
            var parser = new CalendarParser();
            var cal = new Calendar();
            using (var source = new StreamReader(Utils.OpenResourceStream(resName)))
            {
                var reader = new CalTextReader(parser, source, false);
                reader.ReadNextLine();

                cal.Deserialize(reader);
                Assert.Equal("2.0", cal.Version);
                Assert.Equal("-//hacksw/handcal//NONSGML v1.0//EN", cal.ProdID);
                Assert.Equal(1, cal.ComponentCount);
                Assert.Equal(1, cal.Events.Count);
                var evnt = cal.Events[0];
                Assert.NotNull(evnt);
                Assert.Equal(new DateTime(1997, 7, 14, 17, 0, 0), (DateTime?)evnt.DateStart);
                Assert.Equal(new DateTime(1997, 7, 15, 3, 59, 59), (DateTime?)evnt.DateEnd);
                Assert.False(evnt.IsAllDay);
                Assert.Equal("Bastille Day Party", evnt.Summary);
            }

            using (var source = new StringWriter())
            {
                var writer = new CalTextWriter(parser, source);

                cal.Serialize(writer);
                Assert.Equal(Utils.ReadResource(resName) + Environment.NewLine, source.GetStringBuilder().ToString());
            }
        }

        [Fact]
        public void Deserialize_Sample2()
        {
            string resName = "sample-2.ics";
            var parser = new CalendarParser();
            var cal = new Calendar();
            using (var source = new StreamReader(Utils.OpenResourceStream(resName)))
            {
                var reader = new CalTextReader(parser, source, false);
                reader.ReadNextLine();

                cal.Deserialize(reader);
                Assert.Equal("2.0", cal.Version);
                Assert.Equal("-//xyz Corp//NONSGML PDA Calendar Verson 1.0//EN", cal.ProdID);
                Assert.Equal(1, cal.ComponentCount);
                Assert.Equal(2, cal.PropertyCount);

                var evnt = cal.Events[0];
                Assert.Equal(new DateTime(1996, 7, 4, 12, 0, 0), (DateTime?)evnt.DtStamp);
                Assert.Equal("uid1@host.com", evnt.UID);
                Assert.Equal(new Uri("MAILTO:jsmith@host.com"), (Uri)evnt.Organizer);
                Assert.Equal(new DateTime(1996, 9, 18, 14, 30, 0), (DateTime?)evnt.DateStart);
                Assert.Equal(new DateTime(1996, 9, 20, 22, 0, 0), (DateTime?)evnt.DateEnd);
                Assert.False(evnt.IsAllDay);
                Assert.Equal(EventStatuses.Confirmed, (EventStatuses)evnt.Status);
                Assert.Equal(new string[] { "CONFERENCE" }, evnt.Categories.SelectMany(c => c.Value));
                Assert.Equal("Networld+Interop Conference", evnt.Summary);
                Assert.Equal($"Networld+Interop Conference and Exhibit{Environment.NewLine}Atlanta World Congress Center{Environment.NewLine}Atlanta, Georgia", evnt.Description);
            }

            resName = "sample-2-ser.ics";
            using (var source = new StringWriter())
            {
                var writer = new CalTextWriter(parser, source);

                cal.Serialize(writer);
                Assert.Equal(Utils.ReadResource(resName), source.GetStringBuilder().ToString());
            }
        }

        [Fact]
        public void Deserialize_Sample3()
        {
            string resName = "sample-3.ics";
            var parser = new CalendarParser();
            var cal = new Calendar();
            using (var source = new StreamReader(Utils.OpenResourceStream(resName)))
            {
                var reader = new CalTextReader(parser, source, false);
                reader.ReadNextLine();

                cal.Deserialize(reader);
                Assert.Equal("2.0", cal.Version);
                Assert.Equal("-//RDU Software//NONSGML HandCal//EN", cal.ProdID);
                Assert.Equal(2, cal.ComponentCount);
                Assert.Equal(2, cal.PropertyCount);

                var tz = cal.Timezones[0];
                Assert.Equal("US-Eastern", tz.TzId);
                Assert.Equal(2, tz.ZoneComponents.Count);
                var tzc = tz.ZoneComponents[0];
                Assert.Equal(Constants.STANDARD, tzc.Name);
                Assert.False(tzc.IsDayLight);
                Assert.Equal(new DateTime(1998, 10, 25, 2, 0, 0), (DateTime?)tzc.DateStart);
                Assert.Equal(new DateTime[] {
                    new DateTime(1998, 10, 25, 2, 0, 0)
                }, tzc.RecurDates.SelectMany(r => r.DatesValue));
                Assert.Equal(TimeSpan.FromHours(-4), (TimeSpan?)tzc.OffsetFrom);
                Assert.Equal(TimeSpan.FromHours(-5), (TimeSpan?)tzc.OffsetTo);
                Assert.Equal(new string[] { "EST" }, tzc.TimeZoneNames.Select(n => n.Value));

                tzc = tz.ZoneComponents[1];
                Assert.Equal(Constants.DAYLIGHT, tzc.Name);
                Assert.True(tzc.IsDayLight);
                Assert.Equal(new DateTime(1999, 4, 4, 2, 0, 0), (DateTime?)tzc.DateStart);
                Assert.Equal(new DateTime[] {
                    new DateTime(1999, 4, 4, 2, 0, 0)
                }, tzc.RecurDates.SelectMany(r => r.DatesValue));
                Assert.Equal(TimeSpan.FromHours(-5), (TimeSpan?)tzc.OffsetFrom);
                Assert.Equal(TimeSpan.FromHours(-4), (TimeSpan?)tzc.OffsetTo);
                Assert.Equal(new string[] { "EDT" }, tzc.TimeZoneNames.Select(n => n.Value));

                var evnt = cal.Events[0];
                Assert.Equal(new DateTime(1998, 3, 9, 23, 10, 0), (DateTime?)evnt.DtStamp);
                Assert.Equal("guid-1.host1.com", evnt.UID);
                Assert.Equal(new Uri("MAILTO:mrbig@host.com").ToString(), ((Uri)evnt.Organizer).ToString());
                Assert.Equal(Roles.Chair, (Roles?)evnt.Organizer.Role);
                var attend = evnt.Attendees.FirstOrDefault();
                Assert.NotNull(attend);
                Assert.Equal(new Uri("mailto:employee-A@host.com").ToString(), ((Uri)attend.Value).ToString());
                Assert.Equal(Roles.Req_Participant, (Roles?)attend.Role);
                Assert.Equal(CalUserTypes.Group, (CalUserTypes?)attend.CuType);
                Assert.True((bool?)attend.Rsvp);

                Assert.Equal("Project XYZ Review Meeting", evnt.Description);
                Assert.Equal(new string[] { "MEETING" }, evnt.Categories.SelectMany(c => c.Value));
                Assert.Equal(Classes.Public, (Classes?)evnt.Class);
                Assert.Equal(new DateTime(1998, 3, 9, 13, 0, 0), (DateTime?)evnt.Created);
                Assert.Equal("XYZ Project Review", evnt.Summary);
                Assert.Equal(new DateTime(1998, 3, 12, 8, 30, 0), (DateTime?)evnt.DateStart);
                Assert.Equal("US-Eastern", evnt.DateStart?.TimeZoneID);
                Assert.Equal(new DateTime(1998, 3, 12, 9, 30, 0), (DateTime?)evnt.DateEnd);
                Assert.Equal("US-Eastern", evnt.DateEnd?.TimeZoneID);
                Assert.False(evnt.IsAllDay);
                Assert.Equal("1CP Conference Room 4350", evnt.Location);

            }

            resName = "sample-3-ser.ics";
            using (var source = new StringWriter())
            {
                var writer = new CalTextWriter(parser, source);

                cal.Serialize(writer);
                Assert.Equal(Utils.ReadResource(resName), source.GetStringBuilder().ToString());
            }
        }

        [Fact]
        public void Deserialize_Sample4()
        {
            string resName = "sample-4.ics";
            var parser = new CalendarParser();
            var cal = new Calendar();
            using (var source = new StreamReader(Utils.OpenResourceStream(resName)))
            {
                var reader = new CalTextReader(parser, source, false);
                reader.ReadNextLine();

                cal.Deserialize(reader);
                Assert.Equal("2.0", cal.Version);
                Assert.Equal("-//ABC Corporation//NONSGML My Product//EN", cal.ProdID);
                Assert.Equal("xyz", cal.Method);
                Assert.Equal(1, cal.ComponentCount);
                Assert.Equal(3, cal.PropertyCount);

                var evnt = cal.Events[0];
                Assert.Equal(new DateTime(1997, 3, 24, 12, 00, 0), (DateTime?)evnt.DtStamp);
                Assert.Equal(0, (int?)evnt.Sequence);
                Assert.Equal("uid3@host1.com", evnt.UID);
                Assert.Equal(new Uri("mailto:jdoe@host1.com").ToString(), ((Uri)evnt.Organizer).ToString());
                var attend = evnt.Attendees.FirstOrDefault();
                Assert.NotNull(attend);
                Assert.Equal(new Uri("mailto:jsmith@host1.com").ToString(), ((Uri)attend.Value).ToString());
                Assert.True((bool?)attend.Rsvp);
                Assert.Equal(new DateTime(1997, 3, 24, 12, 30, 0), (DateTime?)evnt.DateStart);
                Assert.Equal(new DateTime(1997, 3, 24, 21, 0, 0), (DateTime?)evnt.DateEnd);
                Assert.False(evnt.IsAllDay);
                Assert.Equal(new string[] { "MEETING", "PROJECT" }, evnt.Categories.SelectMany(c => c.Value));
                Assert.Equal(Classes.Public, (Classes?)evnt.Class);
                Assert.Equal("Calendaring Interoperability Planning Meeting", evnt.Summary);
                Assert.Equal($"Discuss how we can test c&s interoperability{Environment.NewLine}using iCalendar and other IETF standards.", evnt.Description);
                Assert.Equal("LDB Lobby", evnt.Location);

                Assert.Equal(1, evnt.Attachments.Count);
                var attach = evnt.Attachments.First();
                Assert.Equal("application/postscript", attach.FormatType);
                Assert.Equal("ftp://xyzcorp.com/pub/conf/bkgrnd.ps", attach.UriValue?.ToString());
            }

            resName = "sample-4-ser.ics";
            using (var source = new StringWriter())
            {
                var writer = new CalTextWriter(parser, source);

                cal.Serialize(writer);
                Assert.Equal(Utils.ReadResource(resName), source.GetStringBuilder().ToString());
            }
        }

        [Fact]
        public void Deserialize_Sample5()
        {
            string resName = "sample-5.ics";
            var parser = new CalendarParser();
            var cal = new Calendar();
            using (var source = new StreamReader(Utils.OpenResourceStream(resName)))
            {
                var reader = new CalTextReader(parser, source, false);
                reader.ReadNextLine();

                cal.Deserialize(reader);
                Assert.Equal("2.0", cal.Version);
                Assert.Equal("-//ABC Corporation//NONSGML My Product//EN", cal.ProdID);
                Assert.Equal(1, cal.ComponentCount);
                Assert.Equal(2, cal.PropertyCount);

                Assert.Equal(1, cal.Todos.Count);

                var todo = cal.Todos.First();
                Assert.Equal(new DateTime(1998, 1, 30, 13, 45, 0), (DateTime?)todo.DtStamp);
                Assert.Equal(2, (int?)todo.Sequence);
                Assert.Equal("uid4@host1.com", todo.UID);
                Assert.Equal(new Uri("mailto:unclesam@us.gov").ToString(), ((Uri)todo.Organizer).ToString());
                var attend = todo.Attendees.FirstOrDefault();
                Assert.NotNull(attend);
                Assert.Equal(new Uri("mailto:jqpublic@host.com").ToString(), ((Uri)attend.Value).ToString());
                Assert.Equal(TodoPartStatuses.Accepted, (TodoPartStatuses?)attend.PartStat);
                Assert.Equal(new DateTime(1998, 4, 15, 23, 59, 59), (DateTime?)todo.Due);
                Assert.Equal(TodoStatuses.Needs_Action, (TodoStatuses?)todo.Status);
                Assert.Equal("Submit Income Taxes", todo.Summary);

                Assert.Single(todo.Alarms);
                var alarm = todo.Alarms.First();
                Assert.Equal(AlarmActions.Audio, (AlarmActions?)alarm.Action);
                Assert.Equal(new DateTime(1998, 4, 3, 12, 0, 0), (DateTime?)alarm.Trigger.AbsoluteValue);
                Assert.Equal(4, (int?)alarm.Repeat);
                Assert.Equal(TimeSpan.FromHours(1), (TimeSpan?)alarm.Duration);

                Assert.Equal(1, alarm.Attachments.Count);
                var attach = alarm.Attachments.First();
                Assert.Equal("audio/basic", attach.FormatType);
                Assert.Equal("http://host.com/pub/audio-files/ssbanner.aud", attach.UriValue?.ToString());
            }

            resName = "sample-5-ser.ics";
            using (var source = new StringWriter())
            {
                var writer = new CalTextWriter(parser, source);

                cal.Serialize(writer);
                Assert.Equal(Utils.ReadResource(resName), source.GetStringBuilder().ToString());
            }
        }

        [Fact]
        public void Deserialize_Sample6()
        {
            string resName = "sample-6.ics";
            var parser = new CalendarParser();
            var cal = new Calendar();
            using (var source = new StreamReader(Utils.OpenResourceStream(resName)))
            {
                var reader = new CalTextReader(parser, source, false);
                reader.ReadNextLine();

                cal.Deserialize(reader);
                Assert.Equal("2.0", cal.Version);
                Assert.Equal("-//ABC Corporation//NONSGML My Product//EN", cal.ProdID);
                Assert.Equal(1, cal.ComponentCount);
                Assert.Equal(2, cal.PropertyCount);

                var journal = cal.Journals[0];
                Assert.Equal(new DateTime(1997, 3, 24, 12, 00, 0), (DateTime?)journal.DtStamp);
                Assert.Equal("uid5@host1.com", journal.UID);
                Assert.Equal(new Uri("mailto:jsmith@host.com").ToString(), ((Uri)journal.Organizer).ToString());
                Assert.Equal(JournalStatuses.Draft, (JournalStatuses?)journal.Status);
                Assert.Equal(Classes.Public, (Classes?)journal.Class);
                Assert.Equal(new string[] { "Project Report", " XYZ", " Weekly Meeting" }, journal.Categories.SelectMany(c => c.Value));
                var sb = new StringBuilder()
                    .AppendLine("Project xyz Review Meeting Minutes")
                    .AppendLine("Agenda")
                    .AppendLine("1. Review of project version 1.0 requirements.")
                    .AppendLine("2. Definition of project processes.")
                    .AppendLine("3. Review of project schedule.")
                    .AppendLine("Participants: John Smith, Jane Doe, Jim Dandy")
                    .AppendLine("-It was decided that the requirements need to be signed off by product marketing.")
                    .AppendLine("-Project processes were accepted.")
                    .AppendLine("-Project schedule needs to account for scheduled holidays and employee vacation time. Check with HR for specific dates.")
                    .AppendLine("-New schedule will be distributed by Friday.")
                    .Append("-Next weeks meeting is cancelled. No meeting until 3/23.")
                    ;
                Assert.Equal(sb.ToString(), journal.Description);
            }

            resName = "sample-6-ser.ics";
            using (var source = new StringWriter())
            {
                var writer = new CalTextWriter(parser, source);

                cal.Serialize(writer);
                Assert.Equal(Utils.ReadResource(resName), source.GetStringBuilder().ToString());
            }
        }

        [Fact]
        public void Deserialize_Sample7()
        {
            string resName = "sample-7.ics";
            var parser = new CalendarParser();
            var cal = new Calendar();
            using (var source = new StreamReader(Utils.OpenResourceStream(resName)))
            {
                var reader = new CalTextReader(parser, source, false);
                reader.ReadNextLine();

                cal.Deserialize(reader);
                Assert.Equal("2.0", cal.Version);
                Assert.Equal("-//RDU Software//NONSGML HandCal//EN", cal.ProdID);
                Assert.Equal(1, cal.ComponentCount);
                Assert.Equal(2, cal.PropertyCount);

                Assert.Equal(1, cal.FreeBusies.Count);
                var fb = cal.FreeBusies[0];
                Assert.NotNull(fb);

                Assert.Equal(new Uri("mailto:jsmith@host.com").ToString(), ((Uri)fb.Organizer).ToString());
                Assert.Equal(new DateTime(1998, 3, 13, 14, 17, 11), (DateTime?)fb.DateStart);
                Assert.Equal(new DateTime(1998, 4, 10, 14, 17, 11), (DateTime?)fb.DateEnd);
                Assert.Equal(new Uri("http://www.host.com/calendar/busytime/jsmith.ifb").ToString(), ((Uri)fb.Url).ToString());
                Assert.Equal(3, fb.FreeBusies.Count);

                var f = fb.FreeBusies[0];
                Assert.Equal(new DateTime(1998, 3, 14, 23, 30, 0), (DateTime?)f.Periods.Single().DateStart);
                Assert.Equal(new DateTime(1998, 3, 15, 00, 30, 0), (DateTime?)f.Periods.Single().DateEnd);

                f = fb.FreeBusies[1];
                Assert.Equal(new DateTime(1998, 3, 16, 15, 30, 0), (DateTime?)f.Periods.Single().DateStart);
                Assert.Equal(new DateTime(1998, 3, 16, 16, 30, 0), (DateTime?)f.Periods.Single().DateEnd);

                f = fb.FreeBusies[2];
                Assert.Equal(new DateTime(1998, 3, 18, 03, 00, 00), (DateTime?)f.Periods.Single().DateStart);
                Assert.Equal(new DateTime(1998, 3, 18, 04, 00, 00), (DateTime?)f.Periods.Single().DateEnd);
            }

            resName = "sample-7-ser.ics";
            using (var source = new StringWriter())
            {
                var writer = new CalTextWriter(parser, source);

                cal.Serialize(writer);
                Assert.Equal(Utils.ReadResource(resName), source.GetStringBuilder().ToString());
            }
        }

        [Fact]
        public void Deserialize_Sample8()
        {
            string resName = "sample-8.ics";
            var parser = new CalendarParser();
            var cal = new Calendar();
            using (var source = new StreamReader(Utils.OpenResourceStream(resName)))
            {
                var reader = new CalTextReader(parser, source, false);
                reader.ReadNextLine();

                cal.Deserialize(reader);
                Assert.Equal("2.0", cal.Version);
                Assert.Equal("-//RDU Software//NONSGML HandCal//EN", cal.ProdID);
                Assert.Equal(13, cal.ComponentCount);
                Assert.Equal(2, cal.PropertyCount);

                Assert.Equal(4, cal.Events.Count);
                Assert.Equal(1, cal.Todos.Count);
                Assert.Equal(4, cal.Timezones.Count);
                Assert.Equal(1, cal.Journals.Count);
                Assert.Equal(3, cal.FreeBusies.Count);

                //var fb = cal.FreeBusies[0];
                //Assert.NotNull(fb);

                //Assert.Equal(new Uri("mailto:jsmith@host.com").ToString(), ((Uri)fb.Organizer).ToString());
                //Assert.Equal(new DateTime(1998, 3, 13, 14, 17, 11), (DateTime?)fb.DateStart);
                //Assert.Equal(new DateTime(1998, 4, 10, 14, 17, 11), (DateTime?)fb.DateEnd);
                //Assert.Equal(new Uri("http://www.host.com/calendar/busytime/jsmith.ifb").ToString(), ((Uri)fb.Url).ToString());
                //Assert.Equal(3, fb.FreeBusies.Count);

                //var f = fb.FreeBusies[0];
                //Assert.Equal(new DateTime(1998, 3, 14, 23, 30, 0), (DateTime?)f.Periods.Single().DateStart);
                //Assert.Equal(new DateTime(1998, 3, 15, 00, 30, 0), (DateTime?)f.Periods.Single().DateEnd);

                //f = fb.FreeBusies[1];
                //Assert.Equal(new DateTime(1998, 3, 16, 15, 30, 0), (DateTime?)f.Periods.Single().DateStart);
                //Assert.Equal(new DateTime(1998, 3, 16, 16, 30, 0), (DateTime?)f.Periods.Single().DateEnd);

                //f = fb.FreeBusies[2];
                //Assert.Equal(new DateTime(1998, 3, 18, 03, 00, 00), (DateTime?)f.Periods.Single().DateStart);
                //Assert.Equal(new DateTime(1998, 3, 18, 04, 00, 00), (DateTime?)f.Periods.Single().DateEnd);

                var evnt = cal.Events[3];
                Assert.Equal("Our Blissful Anniversary", evnt.Summary);
                Assert.Equal(new DateTime(1997, 11, 2), (DateTime?)evnt.DateStart);
                Assert.True(evnt.IsAllDay);
            }

            resName = "sample-8-ser.ics";
            using (var source = new StringWriter())
            {
                var writer = new CalTextWriter(parser, source);

                cal.Serialize(writer);
                Assert.Equal(Utils.ReadResource(resName), source.GetStringBuilder().ToString());
            }
        }

    }
}
