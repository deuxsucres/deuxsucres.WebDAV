using deuxsucres.iCalendar.Structure;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar
{
    /// <summary>
    /// RRULE
    /// </summary>
    public class RecurRuleProperty : RecurrenceProperty
    {
        /// <summary>
        /// New property
        /// </summary>
        public RecurRuleProperty()
        {
            Name = Constants.RRULE;
        }

        /// <summary>
        /// Cast to recurrence
        /// </summary>
        public static implicit operator Recurrence(RecurRuleProperty prop) { return prop.Value; }

        /// <summary>
        /// Cast from recurrence
        /// </summary>
        public static implicit operator RecurRuleProperty(Recurrence value) { return value != null ? new RecurRuleProperty { Value = value } : null; }
    }
}
