using deuxsucres.iCalendar.Locales;
using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace deuxsucres.iCalendar.Structure
{
    /// <summary>
    /// Base of properties
    /// </summary>
    public abstract class CalProperty : ICalProperty
    {
        IDictionary<string, ICalPropertyParameter> _parameters = new Dictionary<string, ICalPropertyParameter>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Reset the property
        /// </summary>
        public virtual void Reset()
        {
            Name = null;
            _parameters.Clear();
        }

        #region Parameters

        /// <summary>
        /// Define a parameter
        /// </summary>
        public void SetParameter(ICalPropertyParameter parameter)
        {
            if (parameter != null)
                _parameters[parameter.Name] = parameter;
        }

        /// <summary>
        /// Define a parameter or remove it if null
        /// </summary>
        public void SetParameter(ICalPropertyParameter parameter, string name)
        {
            if (parameter != null)
            {
                parameter.Name = name;
                _parameters[name] = parameter;
            }
            else
                _parameters.Remove(name);
        }

        /// <summary>
        /// Find a parameter
        /// </summary>
        public ICalPropertyParameter FindParameter(string name)
        {
            if (_parameters.TryGetValue(name, out ICalPropertyParameter param))
                return param;
            return null;
        }

        /// <summary>
        /// Find a typed parameter
        /// </summary>
        public T FindParameter<T>(string name) where T : class, ICalPropertyParameter
        {
            return FindParameter(name) as T;
        }

        /// <summary>
        /// Find or create a parameter if not exists
        /// </summary>
        public T GetParameter<T>(string name) where T : class, ICalPropertyParameter, new()
        {
            var param = FindParameter<T>(name);
            if (param == null)
            {
                param = new T
                {
                    Name = name
                };
                SetParameter(param);
            }
            return param;
        }

        /// <summary>
        /// Remove a parameter
        /// </summary>
        public void RemoveParameter(string name)
        {
            _parameters.Remove(name);
        }

        /// <summary>
        /// List all parameters
        /// </summary>
        public IEnumerable<ICalPropertyParameter> GetParameters()
        {
            return _parameters.Values.AsEnumerable();
        }

        #endregion

        #region Serialization

        /// <summary>
        /// Serialize the value
        /// </summary>
        protected abstract string SerializeValue(ICalWriter writer, ContentLine line);

        /// <summary>
        /// Serialize a parameter
        /// </summary>
        protected virtual void SerializeParameter(ICalWriter writer, ContentLine line, ICalPropertyParameter parameter)
        {
            parameter.Serialize(writer, line);
        }

        /// <summary>
        /// Serialize the parameters
        /// </summary>
        protected virtual void SerializeParameters(ICalWriter writer, ContentLine line)
        {
            foreach (var prm in GetParameters())
            {
                SerializeParameter(writer, line, prm);
            }
        }

        /// <summary>
        /// Serialize the object
        /// </summary>
        public virtual ContentLine Serialize(ICalWriter writer)
        {
            ContentLine line = new ContentLine()
            {
                Name = Name?.ToUpper()
            };
            line.Value = SerializeValue(writer, line);
            SerializeParameters(writer, line);
            writer.WriteLine(line);
            return line;
        }

        /// <summary>
        /// Deserialize the value
        /// </summary>
        protected abstract bool DeserializeValue(ICalReader reader, ContentLine line);

        /// <summary>
        /// Deserialize a parameter
        /// </summary>
        protected virtual ICalPropertyParameter DeserializeParameter(ICalReader reader, ContentLine line, string name, string value)
        {
            return reader.MakePropertyParameter(name, value);
        }

        /// <summary>
        /// Deserialize the parameters
        /// </summary>
        protected virtual void DeserializeParameters(ICalReader reader, ContentLine line)
        {
            foreach (var prm in line.GetParams())
            {
                var param = DeserializeParameter(reader, line, prm.Key, prm.Value);
                reader.CheckSyntaxError(
                    () => param != null,
                    () => string.Format(SR.Err_FailToParseParameter, prm.Key, prm.Value, reader.CurrentLineNumber)
                    );
                if (param != null)
                    SetParameter(param);
            }
        }

        /// <summary>
        /// Deserialize the property
        /// </summary>
        /// <param name="reader">Reader</param>
        /// <param name="line">Line</param>
        public virtual void Deserialize(ICalReader reader, ContentLine line)
        {
            Reset();
            Name = line.Name;
            reader.CheckSyntaxError(
                () => DeserializeValue(reader, line),
                () => string.Format(SR.Err_FailToParseValue, line.Value, reader.CurrentLineNumber)
                );
            DeserializeParameters(reader, line);
        }

        #endregion

        /// <summary>
        /// Name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Count of parameters
        /// </summary>
        public int ParameterCount => _parameters.Count;
    }

    /// <summary>
    /// Typed property
    /// </summary>
    public class CalProperty<T> : CalProperty
    {
        #region Serialization

        /// <summary>
        /// Serialize value
        /// </summary>
        protected override string SerializeValue(ICalWriter writer, ContentLine line)
        {
            throw new NotImplementedException($"{GetType().Name}.{nameof(SerializeValue)}");
        }

        /// <summary>
        /// Deserialize value
        /// </summary>
        protected override bool DeserializeValue(ICalReader reader, ContentLine line)
        {
            throw new NotImplementedException($"{GetType().Name}.{nameof(DeserializeValue)}");
        }

        #endregion

        /// <summary>
        /// Value
        /// </summary>
        public virtual T Value { get; set; }

    }
}
