using deuxsucres.iCalendar.Locales;
using deuxsucres.iCalendar.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace deuxsucres.iCalendar.Structure
{
    /// <summary>
    /// Base of calendar object
    /// </summary>
    public abstract class CalObject : ICalObject
    {
        StringComparer _nameComparer = StringComparer.OrdinalIgnoreCase;
        IList<ICalProperty> _properties = new List<ICalProperty>();

        /// <summary>
        /// Reset the object
        /// </summary>
        public virtual void Reset()
        {
            ClearProperties();
        }

        #region Properties management

        /// <summary>
        /// Clear all properties
        /// </summary>
        public void ClearProperties()
        {
            _properties.Clear();
        }

        /// <summary>
        /// Set a property
        /// </summary>
        /// <remarks>
        /// This method remove all properties existing in the same name
        /// </remarks>
        public void SetProperty(ICalProperty property)
        {
            if (property == null) return;
            RemoveProperty(property.Name);
            _properties.Add(property);
        }

        /// <summary>
        /// Set a property or remove it if property is null
        /// </summary>
        public void SetProperty(ICalProperty property, string name)
        {
            if (property == null)
            {
                RemoveProperty(name);
            }
            else
            {
                property.Name = name;
                SetProperty(property);
            }
        }

        /// <summary>
        /// Set or remove a property
        /// </summary>
        public T SetOrRemoveProperty<T>(string name, bool doset, Action<T> setter = null) where T : ICalProperty, new()
        {
            T result = default(T);
            if (doset)
            {
                result = GetProperty<T>(name);
                setter?.Invoke(result);
            }
            else
            {
                RemoveProperty(name);
            }
            return result;
        }

        /// <summary>
        /// Add a property
        /// </summary>
        public void AddProperty(ICalProperty property)
        {
            if (property != null && !_properties.Contains(property))
                _properties.Add(property);
        }

        /// <summary>
        /// Remove a property
        /// </summary>
        public void RemoveProperty(string name)
        {
            _properties = _properties
                .Where(p => !_nameComparer.Equals(p.Name, name))
                .ToList();
        }

        /// <summary>
        /// Find a property
        /// </summary>
        public ICalProperty FindProperty(string name)
        {
            return _properties.Where(p => _nameComparer.Equals(p.Name, name))
                .FirstOrDefault();
        }

        /// <summary>
        /// Find a typed property
        /// </summary>
        public T FindProperty<T>(string name) where T : ICalProperty
        {
            return _properties.Where(p => _nameComparer.Equals(p.Name, name))
                .OfType<T>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Find a multiple property
        /// </summary>
        public IEnumerable<ICalProperty> FindProperties(string name)
        {
            return _properties.Where(p => _nameComparer.Equals(p.Name, name));
        }

        /// <summary>
        /// Find a multiple property
        /// </summary>
        public IEnumerable<T> FindProperties<T>(string name) where T : ICalProperty
        {
            return _properties.Where(p => _nameComparer.Equals(p.Name, name))
                .OfType<T>();
        }

        /// <summary>
        /// Find or create a typed property
        /// </summary>
        public T GetProperty<T>(string name, Func<T> creator = null) where T : ICalProperty, new()
        {
            var prop = FindProperty<T>(name);
            if (prop == null)
            {
                if (creator != null)
                    prop = creator.Invoke();
                if (prop == null)
                    prop = new T();
                prop.Name = name;
                SetProperty(prop);
            }
            return prop;
        }

        /// <summary>
        /// List all properties
        /// </summary>
        public IEnumerable<ICalProperty> GetProperties()
        {
            return _properties.AsEnumerable();
        }

        #endregion

        #region Serialization

        /// <summary>
        /// Serialize the properties
        /// </summary>
        protected virtual void SerializeProperties(ICalWriter writer)
        {
            foreach (var prop in GetProperties())
                prop.Serialize(writer);
        }

        /// <summary>
        /// Internal serialization of the object
        /// </summary>
        protected virtual void InternalSerialize(ICalWriter writer)
        {
            SerializeProperties(writer);
        }

        /// <summary>
        /// Serialize the object
        /// </summary>
        public void Serialize(ICalWriter writer)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            writer.WriteBegin(Name);
            InternalSerialize(writer);
            writer.WriteEnd(Name);
        }

        /// <summary>
        /// Internal deserialization of the object
        /// </summary>
        protected abstract void InternalDeserialize(ICalReader reader);

        /// <summary>
        /// Check the begin line
        /// </summary>
        protected virtual bool CheckBeginLine(Parser.ContentLine line)
        {
            return line != null && line.Name.IsEqual(Constants.BEGIN) && line.Value.IsEqual(Name);
        }

        /// <summary>
        /// Check the end line
        /// </summary>
        protected virtual bool CheckEndLine(Parser.ContentLine line)
        {
            return line != null && line.Name.IsEqual(Constants.END) && line.Value.IsEqual(Name);
        }

        /// <summary>
        /// Deserialize the object
        /// </summary>
        public void Deserialize(ICalReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            var line = reader.CurrentLine;
            Reset();
            if (!reader.CheckSyntaxError(
                () => CheckBeginLine(line),
                string.Format(SR.Err_SyntaxExpected, $"{Constants.BEGIN}:{Name}")
            )) return;
            InternalDeserialize(reader);
            if (line == reader.CurrentLine)
                line = reader.ReadNextLine();
            if(!reader.CheckSyntaxError(
                () => CheckEndLine(line),
                string.Format(SR.Err_SyntaxExpectedAtLine, $"{Constants.END}:{Name}", reader.CurrentLineNumber)
            )) return;
            reader.ReadNextLine();
        }

        #endregion

        /// <summary>
        /// Name of the object
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Count of properties
        /// </summary>
        public int PropertyCount => _properties.Count;
    }
}
