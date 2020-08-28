using AutoMapper;

namespace Maincotech.Adapter
{
    /// <summary>
    ///
    /// </summary>
    public class AutoMappingTypeAdapter : ITypeAdapter
    {
        public IMapper Mapper { get; private set; }

        public AutoMappingTypeAdapter(IMapper mapper)
        {
            Mapper = mapper;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public TTarget Adapt<TSource, TTarget>(TSource source) where TSource : class where TTarget : class, new()
        {
            return Mapper.Map<TTarget>(source);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public TTarget Adapt<TTarget>(object source) where TTarget : class, new()
        {
            return Mapper.Map<TTarget>(source);
        }
    }
}