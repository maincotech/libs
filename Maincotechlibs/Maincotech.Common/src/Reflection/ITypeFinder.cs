using System;
using System.Collections.Generic;
using System.Reflection;

namespace Maincotech.Reflection
{
    /// <summary>
    /// Classes implementing this interface provide information about types
    /// to various services in the  engine.
    /// </summary>
    public interface ITypeFinder
    {
        /// <summary>
        /// Find classes of type
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes</param>
        /// <returns>Result</returns>
        IEnumerable<Type> ClassesOfType<T>(bool onlyConcreteClasses = true);

        /// <summary>
        /// Find classes of type
        /// </summary>
        /// <param name="assignTypeFrom">Assign type from</param>
        /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes</param>
        /// <returns>Result</returns>
        /// <returns></returns>
        IEnumerable<Type> ClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true);

        /// <summary>
        /// Find classed with specific attribute
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="onlyConcreteClasses"></param>
        /// <returns></returns>
        IEnumerable<Type> ClassesWithAttribute<TAttribute>(bool onlyConcreteClasses = true) where TAttribute : Attribute;

        /// <summary>
        /// Gets the assemblies related to the current implementation.
        /// </summary>
        /// <returns>A list of assemblies</returns>
        IList<Assembly> GetAssemblies();
    }
}