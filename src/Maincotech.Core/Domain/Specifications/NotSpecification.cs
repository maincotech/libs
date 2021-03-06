﻿using System;
using System.Linq.Expressions;

namespace Maincotech.Domain.Specifications
{
    /// <summary>

    /// Represents the specification which indicates the semantics opposite to the given specification.
    /// </summary>
    /// <typeparam name="T">The type of the object to which the specification is applied.</typeparam>
    public class NotSpecification<T> : Specification<T>
    {
        #region Private Fields

        private ISpecification<T> spec;

        #endregion Private Fields

        #region Ctor

        /// <summary>
        /// Initializes a new instance of <c>NotSpecification&lt;T&gt;</c> class.
        /// </summary>
        /// <param name="specification">The specification to be reversed.</param>
        public NotSpecification(ISpecification<T> specification)
        {
            spec = specification;
        }

        #endregion Ctor

        #region Public Methods

        /// <summary>
        /// Gets the LINQ expression which represents the current specification.
        /// </summary>
        /// <returns>The LINQ expression.</returns>
        public override Expression<Func<T, bool>> GetExpression()
        {
            var body = Expression.Not(spec.GetExpression().Body);
            return Expression.Lambda<Func<T, bool>>(body, spec.GetExpression().Parameters);
        }

        #endregion Public Methods
    }
}