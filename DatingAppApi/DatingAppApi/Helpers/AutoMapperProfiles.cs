using System.Linq;
using AutoMapper;
using DatingAppApi.Data;
using DatingAppApi.ViewModels;

namespace DatingAppApi.Helpers
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
            .ForMember(z=>z.PhotoUrl, x=>x.MapFrom(d=>d.Photos.FirstOrDefault(c=>c.IsMain).Url))
            .ForMember(z=>z.Age,o=>o.MapFrom(x=>x.DateOfBirth.CalculateEdad()));
            CreateMap<User, UserForDetailsDto>()
            .ForMember(z => z.PhotoUrl, x => x.MapFrom(d => d.Photos.FirstOrDefault(c => c.IsMain).Url))
            .ForMember(z => z.Age, o => o.MapFrom(x => x.DateOfBirth.CalculateEdad()));  
            CreateMap<Photo, PhotosForDetailsDto>();
            CreateMap<UserForUpdateDto, User>();
            CreateMap<Photo, FotoForReturnDto>();
            CreateMap<FotosForCreationDto, Photo>();
            CreateMap<RegisterViewModels, User>();

        }
    }
}