using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace deuxsucres.ContentType
{
    /// <summary>
    /// Entity of content
    /// </summary>
    public abstract class ContentEntity : IContentComponent
    {
        List<IContentComponent> _components = new List<IContentComponent>();

        #region IContentComponent
        ContentEntity IContentComponent.Parent { get; set; }
        #endregion

        #region Components

        /// <summary>
        /// Clear the components
        /// </summary>
        public void ClearComponents()
        {
            _components.Clear();
        }

        /// <summary>
        /// Add a component
        /// </summary>
        public void AddComponent(IContentComponent component)
        {
            if (component != null && !_components.Contains(component))
                _components.Add(component);
        }

        /// <summary>
        /// Define a component
        /// </summary>
        public void SetComponent(IContentComponent component)
        {
            if (component == null) return;
            RemoveComponents(component.Name);
            AddComponent(component);
        }

        /// <summary>
        /// Define a property
        /// </summary>
        public void SetProperty(ContentProperty property, string name = null)
        {
            if (property != null)
            {
                if (!string.IsNullOrEmpty(name))
                    property.Name = name;
                SetComponent(property);
            }
            else if (!string.IsNullOrEmpty(name))
                RemoveComponents(name);
        }

        /// <summary>
        /// Find or create a property if not exists
        /// </summary>
        public T GetProperty<T>(string name) where T : ContentProperty
        {
            T result = FindComponents<T>(name).FirstOrDefault();
            if (result == null)
            {
                result = Activator.CreateInstance<T>();
                SetProperty(result, name);
            }
            return result;
        }

        /// <summary>
        /// Find components from a name
        /// </summary>
        public IEnumerable<IContentComponent> FindComponents(string name)
            => _components.Where(c => string.Equals(name, c.Name, StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Find typed components from a name
        /// </summary>
        public IEnumerable<T> FindComponents<T>(string name) where T : IContentComponent
            => FindComponents(name).OfType<T>();

        /// <summary>
        /// List the components
        /// </summary>
        public IEnumerable<IContentComponent> GetComponents() => _components;

        /// <summary>
        /// Remove a component
        /// </summary>
        public void RemoveComponent(IContentComponent component)
        {
            _components.Remove(component);
        }

        /// <summary>
        /// Remove all components from a name
        /// </summary>
        public void RemoveComponents(string name)
        {
            for (int i = _components.Count - 1; i >= 0; i--)
            {
                if (string.Equals(name, _components[i].Name, StringComparison.OrdinalIgnoreCase))
                    _components.RemoveAt(i);
            }
        }

        #endregion

        /// <summary>
        /// Name
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Component access
        /// </summary>
        public IContentComponent this[int idx] { get => _components[idx]; }

        /// <summary>
        /// Count of components
        /// </summary>
        public int ComponentCount => _components.Count;
    }
}
