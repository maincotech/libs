using Maincotech.Caching;
using Maincotech.Domain;
using Maincotech.Domain.Repositories;
using Maincotech.EF;
using Maincotech.Localization.Models;
using Maincotech.Localization.Specifications;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Maincotech.Localization
{
    public class LocalizationService : ILocalizationService
    {
        private readonly ICacheManager _cacheManager;
        private readonly DbContext _dbContext;
        private readonly IRepository<Terms> _termsRepository;

        public LocalizationService(DbContext localizationDbContext, ICacheManager cacheManager = null)
        {
            _dbContext = localizationDbContext;
            _termsRepository = new EntityFrameworkRepository<Terms>(new EntityFrameworkRepositoryContext(_dbContext));
            _cacheManager = cacheManager ?? new RuntimeCacheProvider();
        }

        public async Task AddOrUpdateTerms(Terms terms)
        {
            var entity = _termsRepository.Find(TermsSpecifications.WithKeyAndCulture(terms.Key, terms.CultureCode));
            if (entity == null)
            {
                _termsRepository.Add(terms);
            }
            else
            {
                entity.Value = terms.Value;
                _termsRepository.Update(entity);
            }
            _termsRepository.Context.Commit();
        }

        public async Task<IEnumerable<Terms>> GetAllTerms(string key)
        {
            return _termsRepository.FindAll(TermsSpecifications.WithKey(key));
        }

        public async Task<T> GetLocalizedEntity<T>(T entity, string cultureCode) where T : IEntity
        {
            var properties = entity.GetType().GetPropertiesWithAttribute<LocalizableAttribute>();

            foreach (var property in properties)
            {
                var key = TermsHelper.GenerateTermsKey(entity, property);
                var defaultValue = (string)property.FastGetValue(entity);
                var cachedValue = _cacheManager.Get<string>(key, () =>
                 {
                     var terms = _termsRepository.Find(TermsSpecifications.WithKeyAndCulture(key, cultureCode));
                     if (terms != null)
                     {
                         return terms.Value;
                     }
                     else
                     {
                         return defaultValue;
                     }
                 });
                if (cachedValue != null && cachedValue.Equals(defaultValue, System.StringComparison.OrdinalIgnoreCase) == false)
                {
                    property.FastSetValue(entity, cachedValue);
                }
            }

            return entity;
        }

        public async Task<Terms> GetTerms(string key, string cultureCode)
        {
            return _termsRepository.Find(TermsSpecifications.WithKeyAndCulture(key, cultureCode));
        }
    }
}