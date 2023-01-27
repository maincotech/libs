using AutoMapper;
using Maincotech.Adapter;

namespace Maincotech.ModuleModel
{
    public class ModuleMapperProfile : Profile, IOrderedMapperProfile
    {
        public int Order => 1;

        public ModuleMapperProfile()
        {
            CreateMap<ModuleDescriptor, ModuleInfo>();
        }
    }
}