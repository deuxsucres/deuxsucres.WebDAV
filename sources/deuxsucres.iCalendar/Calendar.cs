using deuxsucres.iCalendar.Locales;
using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using deuxsucres.iCalendar.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace deuxsucres.iCalendar
{
    /// <summary>
    /// Calendar object
    /// </summary>
    public class Calendar : CalObject
    {
        List<CalComponent> _components = new List<CalComponent>();

        /// <summary>
        /// Create a new calendar
        /// </summary>
        public Calendar()
        {
            Events = new CalComponents<Event>(_components);
            Todos = new CalComponents<Todo>(_components);
            Journals = new CalComponents<Journal>(_components);
            Timezones = new CalComponents<TimeZone>(_components);
            FreeBusies = new CalComponents<FreeBusy>(_components);
            ProdID = Constants.DefaultProdID;
            Version = "2.0";
            Scale = Constants.GREGORIAN;
        }

        /// <summary>
        /// Reset
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            ProdID = Constants.DefaultProdID;
            Version = "2.0";
            //Scale = Constants.GREGORIAN;
            ClearComponents();
        }

        /// <summary>
        /// Serialize
        /// </summary>
        protected override void InternalSerialize(ICalWriter writer)
        {
            SerializeProperties(writer);
            foreach (var comp in GetComponents())
            {
                comp.Calendar = this;
                comp.Serialize(writer);
            }
        }

        /// <summary>
        /// Internal deserialization of the object
        /// </summary>
        protected override void InternalDeserialize(ICalReader reader)
        {
            ContentLine line;
            while ((line = reader.ReadNextLine()) != null)
            {
                switch (line.Name.ToUpper())
                {
                    // Component
                    case Constants.BEGIN:
                        AddComponent(reader.ReadComponent(line));
                        break;
                    // End
                    case Constants.END:
                        reader.CheckSyntaxError(
                            () => line.Value.IsEqual(Constants.VCALENDAR),
                            string.Format(SR.Err_UnexpectedEnd, line.Value, Constants.VCALENDAR, reader.CurrentLineNumber)
                            );
                        return;
                    // PRODID
                    case Constants.PRODID: SetProperty(reader.MakeProperty<TextProperty>(line)); break;
                    // VERSION
                    case Constants.VERSION: SetProperty(reader.MakeProperty<VersionProperty>(line)); break;
                    // CALSCALE
                    case Constants.CALSCALE: SetProperty(reader.MakeProperty<TextProperty>(line)); break;
                    // METHOD
                    case Constants.METHOD: SetProperty(reader.MakeProperty<TextProperty>(line)); break;
                    default:
                        AddProperty(reader.MakeProperty(line));
                        break;
                }
            }
        }

        #region Components

        /// <summary>
        /// Clear all components
        /// </summary>
        public void ClearComponents()
        {
            _components.Clear();
        }

        /// <summary>
        /// Add a component
        /// </summary>
        public void AddComponent(CalComponent component)
        {
            if (component != null && !_components.Contains(component))
            {
                component.Calendar = this;
                _components.Add(component);
            }
        }

        /// <summary>
        /// Remove a component
        /// </summary>
        public void RemoveComponent(CalComponent component)
        {
            if (component != null)
                _components.Remove(component);
        }

        /// <summary>
        /// List the components
        /// </summary>
        public IEnumerable<CalComponent> GetComponents()
        {
            return _components.AsEnumerable();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Name of the object
        /// </summary>
        public override string Name => Constants.VCALENDAR;

        /// <summary>
        /// Prod ID
        /// </summary>
        public TextProperty ProdID
        {
            get { return GetProperty<TextProperty>(Constants.PRODID); }
            set { SetProperty(value, Constants.PRODID); }
        }

        /// <summary>
        /// Version
        /// </summary>
        public VersionProperty Version
        {
            get { return GetProperty<VersionProperty>(Constants.VERSION); }
            set { SetProperty(value, Constants.VERSION); }
        }

        /// <summary>
        /// Scale
        /// </summary>
        public TextProperty Scale
        {
            get { return GetProperty<TextProperty>(Constants.CALSCALE); }
            set { SetProperty(value, Constants.CALSCALE); }
        }

        /// <summary>
        /// Method
        /// </summary>
        public TextProperty Method
        {
            get { return FindProperty<TextProperty>(Constants.METHOD); }
            set { SetProperty(value, Constants.METHOD); }
        }

        /// <summary>
        /// Count of components
        /// </summary>
        public int ComponentCount => _components.Count;

        /// <summary>
        /// List of events
        /// </summary>
        public CalComponents<Event> Events { get; private set; }

        /// <summary>
        /// List of todos
        /// </summary>
        public CalComponents<Todo> Todos { get; private set; }

        /// <summary>
        /// List of journal entry
        /// </summary>
        public CalComponents<Journal> Journals { get; private set; }

        /// <summary>
        /// List of timezones
        /// </summary>
        public CalComponents<TimeZone> Timezones { get; private set; }

        /// <summary>
        /// List of free busies
        /// </summary>
        public CalComponents<FreeBusy> FreeBusies { get; private set; }

        #endregion

    }
}
