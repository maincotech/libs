using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Maincotech.Data
{
    public class ValueComparer<T> : IComparer<T>, IEqualityComparer<T>
    {
        public ValueComparer(params Func<T, object>[] props)
        {
            Properties = new ReadOnlyCollection<Func<T, object>>(props);
        }

        public ReadOnlyCollection<Func<T, object>> Properties { get; }

        public int Compare(T x, T y)
        {
            return
                Properties.Select(prop => Comparer.DefaultInvariant.Compare(prop(x), prop(y)))
                    .FirstOrDefault(comp => comp != 0);
        }

        public bool Equals(T x, T y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x == null || y == null) return false;
            //Object.Equals handles strings and nulls correctly
            return Properties.All(f => Equals(f(x), f(y)));
        }

        //http://stackoverflow.com/questions/263400/263416#263416
        public int GetHashCode(T obj)
        {
            if (obj == null) return -42;
            unchecked
            {
                var hash = 17;
                foreach (var value in Properties.Select(prop => prop(obj)))
                {
                    if (value == null)
                        hash = hash * 23 - 1;
                    else
                        hash = hash * 23 + value.GetHashCode();
                }
                return hash;
            }
        }
    }
}