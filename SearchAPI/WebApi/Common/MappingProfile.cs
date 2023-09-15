using AutoMapper;
using WebApi.Application.EventOperations.Queries.GetEvents;
using WebApi.Entities;

namespace WebApi.Common
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<Event, EventsViewModel>()
    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.description.ToString())); // Veya Description'ı uygun bir şekilde biçimlendirin


        }
    }
}