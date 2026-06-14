using AutoMapper;
using Almakaber.DAL.Entities;
using Almakaber.BLL.DTOs.Graves;
using Almakaber.BLL.DTOs.Deceased;
using Almakaber.BLL.DTOs.Supplications;

namespace Almakaber.BLL.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Grave, GraveDto>().ReverseMap();

            CreateMap<CreateGraveDto, Grave>();

            CreateMap<Deceased, DeceasedDto>()
                .ForMember(dest => dest.StreetNumber, opt => opt.MapFrom(src => src.Grave.StreetNumber))
                .ForMember(dest => dest.GraveNumber, opt => opt.MapFrom(src => src.Grave.GraveNumber));

            CreateMap<CreateDeceasedDto, Deceased>();

            CreateMap<Supplication, SupplicationDto>().ReverseMap();
            CreateMap<CreateSupplicationDto, Supplication>();
        }
    }
}