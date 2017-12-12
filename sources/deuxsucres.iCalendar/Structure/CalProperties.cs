using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace deuxsucres.iCalendar.Structure
{

    /// <summary>
    /// Wrapper for list of properties in a object
    /// </summary>
    public class CalProperties<T> : IEnumerable<T> where T : ICalProperty
    {
        CalObject _source;

        /// <summary>
        /// Create a new collection
        /// </summary>
        public CalProperties(string name, CalObject source)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            Name = name;
        }

        /// <summary>
        /// Remove all properties of this name
        /// </summary>
        public void Clear()
        {
            _source.RemoveProperty(Name);
        }

        /// <summary>
        /// Add a property
        /// </summary>
        public void Add(T property)
        {
            property.Name = Name;
            _source.AddProperty(property);
        }

        /// <summary>
        /// Enumerate the properties
        /// </summary>
        protected IEnumerable<T> GetProperties()
        {
            return _source.FindProperties<T>(Name);
        }

        /// <summary>
        /// Get enumerator
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return GetProperties().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Name of the property
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Get the component at a position
        /// </summary>
        public T this[int idx] { get { return GetProperties().ElementAt(idx); } }

        /// <summary>
        /// Count the properties
        /// </summary>
        public int Count => GetProperties().Count();

    }

}
