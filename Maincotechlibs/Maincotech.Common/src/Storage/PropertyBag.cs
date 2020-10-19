using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Maincotech.Storage
{
    /// <summary>
    /// Represents the property bag that contains a list of the mapping between the properties and their values
    /// for a specific object.
    /// </summary>
    public class PropertyBag : IEnumerable<KeyValuePair<string, object>>
    {
        #region Private Fields

        private readonly Dictionary<string, object> _propertyValues = new Dictionary<string, object>();

        #endregion Private Fields

        #region Public Static Fields

        /// <summary>
        /// The binding flags for getting properties on a given object.
        /// </summary>
        public static readonly BindingFlags PropertyBagBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.GetProperty;

        #endregion Public Static Fields

        #region Ctor

        /// <summary>
        /// Initializes a new instance of <c>PropertyBag</c> class.
        /// </summary>
        public PropertyBag()
        {
        }

        /// <summary>
        /// Initializes a new instance of <c>PropertyBag</c> class and populates the content by using the given object.
        /// </summary>
        /// <param name="target">The target object used for initializing the property bag.</param>
        public PropertyBag(object target)
        {
            target
                .GetType()
                .GetProperties(PropertyBagBindingFlags)
                .ToList()
                .ForEach(pi => _propertyValues.Add(pi.Name, pi.GetValue(target, null)));
        }

        #endregion Ctor

        #region Public Properties

        /// <summary>
        /// Gets the number of elements in the property bag.
        /// </summary>
        public int Count
        {
            get { return _propertyValues.Count; }
        }

        /// <summary>
        /// Gets the property value by using the index.
        /// </summary>
        /// <param name="idx">The index.</param>
        /// <returns>The property value.</returns>
        public object this[string idx]
        {
            get
            {
                return _propertyValues[idx];
            }
            set
            {
                _propertyValues[idx] = value;
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Adds a property and its value to the property bag.
        /// </summary>
        /// <param name="propertyName">The name of the property to be added.</param>
        /// <param name="propertyValue">The value of the property.</param>
        /// <returns>The instance with the added property.</returns>
        public PropertyBag Add(string propertyName, object propertyValue)
        {
            _propertyValues.Add(propertyName, propertyValue);
            return this;
        }

        /// <summary>
        /// Adds a property to property bag, to be used as the sort field.
        /// </summary>
        /// <typeparam name="T">The type of the field.</typeparam>
        /// <param name="propertyName">The name of the property to be added.</param>
        /// <returns>The instance with the added sort field.</returns>
        public PropertyBag AddSort<T>(string propertyName)
        {
            _propertyValues.Add(propertyName, default(T));
            return this;
        }

        /// <summary>
        /// Clears the property bag.
        /// </summary>
        public void Clear()
        {
            _propertyValues.Clear();
        }

        /// <summary>
        /// Gets the <see cref="System.String"/> value which represents the current property bag.
        /// </summary>
        /// <returns>The <see cref="System.String"/> value which represents the current property bag.</returns>
        public override string ToString()
        {
            return string.Format("[{0}]", string.Join(", ", _propertyValues.Keys.Select(p => p)));
        }

        #endregion Public Methods

        #region IEnumerator<KeyValuePair<string, object>> Members

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _propertyValues.GetEnumerator();
        }

        #endregion IEnumerator<KeyValuePair<string, object>> Members

        #region IEnumerable Members

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _propertyValues.GetEnumerator();
        }

        #endregion IEnumerable Members
    }
}
