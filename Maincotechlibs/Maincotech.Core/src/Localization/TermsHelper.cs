using System.Reflection;

namespace Maincotech.Domain
{
    public static class TermsHelper
    {
        /// <summary>
        /// return class.properyName.entityId
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static string GenerateTermsKey(IEntity entity, PropertyInfo propertyInfo)
        {
            return $"{entity.GetType().Name}.{propertyInfo.Name}.{entity.Id}";
        }

        public static string GenerateTermsKey(IEntity entity, string propertyName)
        {
            return $"{entity.GetType().Name}.{propertyName}.{entity.Id}";
        }
    }
}