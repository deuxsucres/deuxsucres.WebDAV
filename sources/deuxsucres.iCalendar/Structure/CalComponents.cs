using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace deuxsucres.iCalendar.Structure
{
    /// <summary>
    /// Wrapper for list of components in a object
    /// </summary>
    public class CalComponents<T> : IEnumerable<T> where T : CalComponent
    {
        IList<CalComponent> _source;

        /// <summary>
        /// Create a new list
        /// </summary>
        public CalComponents(IList<CalComponent> source)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
        }

        /// <summary>
        /// Enumerate the components
        /// </summary>
        protected IEnumerable<T> GetComponents()
        {
            return _source.OfType<T>();
        }

        /// <summary>
        /// Get enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return GetComponents().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Remove all components of this list
        /// </summary>
        public void Clear()
        {
            var comps = this.ToList();
            foreach (var comp in comps)
                _source.Remove(comp);
        }

        /// <summary>
        /// Add a component
        /// </summary>
        public void Add(T component)
        {
            if (component != null && !_source.Contains(component))
                _source.Add(component);
        }

        /// <summary>
        /// Create and add a new component
        /// </summary>
        public T CreateNew()
        {
            var comp = (T)Activator.CreateInstance(typeof(T));
            _source.Add(comp);
            return comp;
        }

        /// <summary>
        /// Remove a component
        /// </summary>
        public bool Remove(T component)
        {
            return _source.Remove(component);
        }

        /// <summary>
        /// Get the component at a position
        /// </summary>
        public T this[int idx] { get { return GetComponents().ElementAt(idx); } }

        /// <summary>
        /// Count the components
        /// </summary>
        public int Count => GetComponents().Count();

    }
}
