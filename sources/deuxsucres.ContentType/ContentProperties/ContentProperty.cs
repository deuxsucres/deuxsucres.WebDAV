using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace deuxsucres.ContentType
{

    /// <summary>
    /// Content property
    /// </summary>
    public abstract class ContentProperty
    {
        Dictionary<string, ContentParameter> _parameters;

        #region Parameters

        /// <summary>
        /// Remove all parameters
        /// </summary>
        public void ClearParameters()
        {
            _parameters = null;
        }

        /// <summary>
        /// Define a parameter
        /// </summary>
        public void SetParameter(ContentParameter parameter, string name = null)
        {
            if (parameter != null)
            {
                if (!string.IsNullOrEmpty(name))
                    parameter.Name = name;
                if (_parameters == null)
                    _parameters = new Dictionary<string, ContentParameter>(StringComparer.OrdinalIgnoreCase);
                _parameters[parameter.Name] = parameter;
            }
            else if (!string.IsNullOrEmpty(name))
                _parameters?.Remove(name);
        }

        /// <summary>
        /// Find a parameter
        /// </summary>
        public ContentParameter FindParameter(string name)
        {
            ContentParameter result = null;
            if (_parameters?.TryGetValue(name, out result) == true)
                return result;
            return null;
        }

        /// <summary>
        /// Find a typed parameter
        /// </summary>
        public T FindParameter<T>(string name) where T : ContentParameter
        {
            return FindParameter(name) as T;
        }

        /// <summary>
        /// Find or create a parameter if not exists
        /// </summary>
        public T GetParameter<T> (string name) where T : ContentParameter
        {
            T result = FindParameter<T>(name);
            if (result == null)
            {
                result = Activator.CreateInstance<T>();
                SetParameter(result, name);
            }
            return result;
        }

        /// <summary>
        /// Remove a parameter
        /// </summary>
        public void RemoveParameter(string name)
        {
            _parameters?.Remove(name);
            if (_parameters?.Count == 0)
                _parameters = null;
        }

        /// <summary>
        /// List all parameters
        /// </summary>
        public IEnumerable<ContentParameter> GetParameters() 
            => _parameters?.Values ?? Enumerable.Empty<ContentParameter>();

        #endregion

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Count of parameters
        /// </summary>
        public int ParameterCount => _parameters?.Count ?? 0;
    }

}
