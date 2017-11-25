using deuxsucres.iCalendar.Serialization;
using deuxsucres.iCalendar.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.iCalendar.Tests.Serialization
{
    public class SerializationHelpersTest
    {
        public static IEnumerable<object[]> DefaultCreateParameterData()
        {
            yield return new object[] { "Test", typeof(TextParameter), "Test" };
            yield return new object[] { Constants.ALTREP, typeof(UriParameter), Constants.ALTREP };
            yield return new object[] { Constants.CN, typeof(TextParameter), Constants.CN };
            yield return new object[] { "CuType", typeof(EnumParameter<CalUserTypes>), Constants.CUTYPE };
            yield return new object[] { Constants.DELEGATED_FROM, typeof(CalAddressesParameter), Constants.DELEGATED_FROM };
            yield return new object[] { Constants.DELEGATED_TO, typeof(CalAddressesParameter), Constants.DELEGATED_TO };
            yield return new object[] { Constants.DIR, typeof(UriParameter), Constants.DIR };
            yield return new object[] { Constants.ENCODING, typeof(TextParameter), Constants.ENCODING };
            yield return new object[] { Constants.FMTTYPE, typeof(TextParameter), Constants.FMTTYPE };
            yield return new object[] { Constants.FBTYPE, typeof(EnumParameter<FreeBusyTypes>), Constants.FBTYPE };
            yield return new object[] { Constants.LANGUAGE, typeof(TextParameter), Constants.LANGUAGE };
            yield return new object[] { Constants.MEMBER, typeof(CalAddressesParameter), Constants.MEMBER };
            yield return new object[] { Constants.PARTSTAT, typeof(TextParameter), Constants.PARTSTAT };
            yield return new object[] { Constants.RANGE, typeof(EnumParameter<Ranges>), Constants.RANGE };
            yield return new object[] { Constants.RELATED, typeof(EnumParameter<Relateds>), Constants.RELATED };
            yield return new object[] { Constants.RELTYPE, typeof(EnumParameter<RelationTypes>), Constants.RELTYPE };
            yield return new object[] { Constants.ROLE, typeof(EnumParameter<Roles>), Constants.ROLE };
            yield return new object[] { Constants.RSVP, typeof(BooleanParameter), Constants.RSVP };
            yield return new object[] { Constants.SENT_BY, typeof(CalAddressParameter), Constants.SENT_BY };
            yield return new object[] { Constants.TZID, typeof(TextParameter), Constants.TZID };
            yield return new object[] { Constants.VALUE, typeof(EnumParameter<ValueTypes>), Constants.VALUE };
        }

        [Theory, MemberData(nameof(DefaultCreateParameterData))]
        public void DefaultCreateParameter(string name, Type paramTypeExpected, string nameExpected)
        {
            var p = SerializationHelpers.DefaultCreateParameter(name);
            Assert.NotNull(p);
            Assert.IsType(paramTypeExpected, p);
            Assert.Equal(nameExpected, p.Name);
        }

        public static IEnumerable<object[]> DefaultCreatePropertyData()
        {
            yield return new object[] { "Test", typeof(TextProperty), "Test", null, null };
            yield return new object[] { Constants.UID, null, null, typeof(NotImplementedException), "Property creation for Constants.UID not implemented" };
        }
        [Theory, MemberData(nameof(DefaultCreatePropertyData))]
        public void DefaultCreateProperty(string name, Type paramTypeExpected, string nameExpected, Type exceptionExpected, string errorExpected)
        {
            if (exceptionExpected == null)
            {
                var p = SerializationHelpers.DefaultCreateProperty(name);
                Assert.NotNull(p);
                Assert.IsType(paramTypeExpected, p);
                Assert.Equal(nameExpected, p.Name);
            }
            else
            {
                var ex = Assert.Throws(exceptionExpected, () => SerializationHelpers.DefaultCreateProperty(name));
                Assert.Equal(errorExpected, ex.Message);
            }
        }

        public static IEnumerable<object[]> DefaultCreateComponentData()
        {
            yield return new object[] { "Test", null, null, typeof(CalSyntaxError), "Unknown 'Test' component." };
            yield return new object[] { Constants.VEVENT, null, null, typeof(NotImplementedException), "Component creation for Constants.VEVENT not implemented" };
            yield return new object[] { Constants.VTODO, null, null, typeof(NotImplementedException), "Component creation for Constants.VTODO not implemented" };
            yield return new object[] { Constants.VJOURNAL, null, null, typeof(NotImplementedException), "Component creation for Constants.VJOURNAL not implemented" };
            yield return new object[] { Constants.VFREEBUSY, null, null, typeof(NotImplementedException), "Component creation for Constants.VFREEBUSY not implemented" };
            yield return new object[] { Constants.VTIMEZONE, null, null, typeof(NotImplementedException), "Component creation for Constants.VTIMEZONE not implemented" };
        }
        [Theory, MemberData(nameof(DefaultCreateComponentData))]
        public void DefaultCreateComponent(string name, Type paramTypeExpected, string nameExpected, Type exceptionExpected, string errorExpected)
        {
            if (exceptionExpected == null)
            {
                var p = SerializationHelpers.DefaultCreateComponent(name);
                Assert.NotNull(p);
                Assert.IsType(paramTypeExpected, p);
                Assert.Equal(nameExpected, p.Name);
            }
            else
            {
                var ex = Assert.Throws(exceptionExpected, () => SerializationHelpers.DefaultCreateComponent(name));
                Assert.Equal(errorExpected, ex.Message);
            }
        }
    }
}
