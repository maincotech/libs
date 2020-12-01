using Maincotech.Domain;
using Maincotech.Domain.Specifications;
using Maincotech.Localization.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maincotech.Localization.Specifications
{
  public static class TermsSpecifications
    {
        public static ISpecification<Terms> WithKey(string key)
        {
            return Specification<Terms>.Eval(entity => entity.Key == key );
        }
        public static ISpecification<Terms> WithKeyAndCulture(string key, string cultureCode)
        {
            return Specification<Terms>.Eval(entity => entity.Key == key && entity.CultureCode == cultureCode);
        }
    }
}
