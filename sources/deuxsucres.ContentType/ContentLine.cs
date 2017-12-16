using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace deuxsucres.ContentType
{
    /// <summary>
    /// Content line
    /// </summary>
    public class ContentLine
    {
        IDictionary<string, ContentLineParameter> _parameters;

        /// <summary>
        /// Get a parameter or create it if not exists
        /// </summary>
        protected ContentLineParameter GetOrCreateParam(string name)
        {
            if (_parameters == null)
                _parameters = new Dictionary<string, ContentLineParameter>(StringComparer.OrdinalIgnoreCase);
            if(!_parameters.TryGetValue(name, out ContentLineParameter param))
            {
                param = new ContentLineParameter(name);
                _parameters[name] = param;
            }
            return param;
        }

        /// <summary>
        /// Define a parameter or add the value to an existing parameter
        /// </summary>
        public ContentLine AddParam(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name)) return this;
            GetOrCreateParam(name).Values.Add(value);
            return this;
        }

        /// <summary>
        /// Define a parameter
        /// </summary>
        public ContentLine SetParam(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name)) return this;
            if (value == null)
                RemoveParam(name);
            else
                GetOrCreateParam(name).Value = value;
            return this;
        }

        /// <summary>
        /// Remove a parameter
        /// </summary>
        public ContentLine RemoveParam(string name)
        {
            if (!string.IsNullOrWhiteSpace(name) && _parameters != null)
                _parameters.Remove(name);
            return this;
        }

        /// <summary>
        /// Get a parameter
        /// </summary>
        public ContentLineParameter GetParam(string name)
        {
            if (_parameters == null || string.IsNullOrWhiteSpace(name))
                return null;
            if (_parameters.TryGetValue(name, out ContentLineParameter result))
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
        public IEnumerable<ContentLineParameter> GetParams()
        {
            return _parameters != null ? _parameters.Values : Enumerable.Empty<ContentLineParameter>();
        }

        /// <summary>
        /// Group
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Full name
        /// </summary>
        public string FullName => string.IsNullOrEmpty(Group) ? Name : $"{Group}.{Name}";

        /// <summary>
        /// Value
        /// </summary>
        public string Value { get; set; }

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

    /// <summary>
    /// Content line parameter
    /// </summary>
    public class ContentLineParameter : IEnumerable<string>
    {
        /// <summary>
        /// Create a new parameter
        /// </summary>
        public ContentLineParameter(string name)
        {
            Name = name;
            Values = new List<string>();
        }

        #region Enumerable
        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            return ((IEnumerable<string>)Values).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<string>)Values).GetEnumerator();
        }
        #endregion

        /// <summary>
        /// Const to string
        /// </summary>
        public static implicit operator string(ContentLineParameter param) => param?.Value;

        /// <summary>
        /// Const to string array
        /// </summary>
        public static implicit operator string[] (ContentLineParameter param) => param?.Values?.ToArray();

        /// <summary>
        /// Const to string list
        /// </summary>
        public static implicit operator List<string>(ContentLineParameter param) => param.Values !=null ? new List<string>(param.Values) : null;

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Values
        /// </summary>
        public List<string> Values { get; private set; }

        /// <summary>
        /// Values as single string
        /// </summary>
        public string Value
        {
            get => string.Join(", ", Values);
            set
            {
                Values.Clear();
                if (!string.IsNullOrEmpty(value))
                    Values.Add(value);
            }
        }

    }
}