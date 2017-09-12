using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace deuxsucres.iCalendar.Parser
{
    /// <summary>
    /// Content line
    /// </summary>
    public class ContentLine
    {
        IDictionary<string, string> _parameters;

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Define a parameter
        /// </summary>
        public ContentLine SetParam(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name)) return this;
            if (_parameters == null)
                _parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _parameters[name] = value ?? string.Empty;
            return this;
        }

        /// <summary>
        /// Get a parameter
        /// </summary>
        public string GetParam(string name)
        {
            if (_parameters == null || string.IsNullOrWhiteSpace(name))
                return null;
            string result;
            if (_parameters.TryGetValue(name, out result))
                return result;
            return null;
        }

        /// <summary>
        /// Indicates if the parameter exists
        /// </summary>
        public bool HavingParam(string name)
        {
            if (_parameters == null || string.IsNullOrWhiteSpace(name))
                return false;
            return _parameters.ContainsKey(name);
        }

        /// <summary>
        /// Get all the names
        /// </summary>
        public IEnumerable<string> GetNames()
        {
            return _parameters != null ? _parameters.Keys : Enumerable.Empty<string>();
        }

        /// <summary>
        /// Get all parameters
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> GetParams()
        {
            return _parameters != null ? _parameters : Enumerable.Empty<KeyValuePair<string, string>>();
        }

        /// <summary>
        /// Parameters access
        /// </summary>
        public string this[string name]
        {
            get { return GetParam(name); }
            set { SetParam(name, value); }
        }

        /// <summary>
        /// Parameters count
        /// </summary>
        public int ParamCount => _parameters?.Count ?? 0;
    }
}
