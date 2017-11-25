using System;
using System.Collections.Generic;
using System.Text;
using deuxsucres.iCalendar.Serialization;

namespace deuxsucres.iCalendar.Structure
{
    /// <summary>
    /// Component of a calendar
    /// </summary>
    public abstract class CalComponent : CalObject
    {

        /// <summary>
        /// Reset
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            ExtraComponents.Clear();
        }

        /// <summary>
        /// Serialize a list of components
        /// </summary>
        protected virtual void SerializeComponents(IEnumerable<CalComponent> components, ICalWriter writer)
        {
            foreach (var comp in components)
            {
                comp.Calendar = Calendar;
                comp.Serialize(writer);
            }
        }

        /// <summary>
        /// Serialize the extra components
        /// </summary>
        protected virtual void SerializeExtraComponents(ICalWriter writer)
        {
            SerializeComponents(ExtraComponents, writer);
        }

        /// <summary>
        /// Serialize
        /// </summary>
        protected override void InternalSerialize(ICalWriter writer)
        {
            base.InternalSerialize(writer);
            SerializeExtraComponents(writer);
        }

        ///// <summary>
        ///// Process a component
        ///// </summary>
        ///// <returns>
        ///// Returns true if the component is processed by the object. If false, the component will
        ///// be added in the extra components.
        ///// </returns>
        //protected virtual bool ProcessComponent(CalComponent component)
        //{
        //    return false;
        //}

        ///// <summary>
        ///// Process a property
        ///// </summary>
        ///// <returns>
        ///// Returns true if the property is processed by the object. If false a default property
        ///// will created and added in the componenent.
        ///// </returns>
        //protected virtual bool ProcessProperty(ICalReader reader, ContentLine line)
        //{
        //    return false;
        //}

        ///// <summary>
        ///// Read a component
        ///// </summary>
        //protected virtual CalComponent ReadComponent(ICalReader reader, ContentLine line)
        //{
        //    return reader.ReadComponent(line);
        //}

        ///// <summary>
        ///// Internal deserialization
        ///// </summary>
        //protected override void InternalDeserialize(ICalReader reader)
        //{
        //    ContentLine line;
        //    while ((line = reader.ReadNextLine()) != null)
        //    {
        //        // Component
        //        if (line.Name.IsEqual(Constants.BEGIN))
        //        {
        //            var comp = ReadComponent(reader, line);
        //            if (comp != null && ProcessComponent(comp))
        //                ExtraComponents.Add(comp);
        //        }
        //        // End
        //        else if (line.Name.IsEqual(Constants.END))
        //        {
        //            if (line.Value.IsEqual(Name))
        //                return;
        //        }
        //        else
        //        {
        //            // Property
        //            if (!ProcessProperty(reader, line))
        //                AddProperty(reader.MakeProperty(line));
        //        }
        //    }
        //}

        /// <summary>
        /// Calendar
        /// </summary>
        public Calendar Calendar { get; set; }

        /// <summary>
        /// Extra components
        /// </summary>
        public List<CalComponent> ExtraComponents { get; private set; } = new List<CalComponent>();

    }
}
