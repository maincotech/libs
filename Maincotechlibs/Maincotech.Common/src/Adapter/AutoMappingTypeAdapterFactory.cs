using AutoMapper;

namespace Maincotech.Adapter
{
    /// <summary>
    /// AutoMappingTypeAdapterFactory
    /// </summary>
    public class AutoMappingTypeAdapterFactory : ITypeAdapterFactory
    {
        /// <summary>
        /// Mapper
        /// </summary>
        public IMapper Mapper { get; private set; }

        /// <summary>
        /// Mapper configuration
        /// </summary>
        public MapperConfiguration MapperConfiguration { get; private set; }

        /// <summary>
        /// Initialize mapper
        /// </summary>
        /// <param name="config">Mapper configuration</param>
        public void Init(MapperConfiguration config)
        {
            MapperConfiguration = config;
            Mapper = config.CreateMapper();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public ITypeAdapter Create()
        {
            return new AutoMappingTypeAdapter(Mapper);
        }
    }
}