using Maincotech.Domain;
using Maincotech.Localization.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Maincotech.Localization
{
    public interface ILocalizationService
    {
        Task<T> GetLocalizedEntity<T>(T entity, string cultureCode) where T : IEntity;

        Task AddOrUpdateTerms(Terms terms);

        Task<IEnumerable<Terms>> GetAllTerms(string key);

        Task<Terms> GetTerms(string key, string cultureCode);
    }
}