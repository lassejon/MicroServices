using AutoMapper;
using CommandsService.Dtos;
using CommandsService.Models;
using PlatformService;

namespace CommandsService.Profiles
{
    public class CommandsProfile : Profile
    {
        public CommandsProfile()
        {
            // source -> target
            CreateMap<Command, CommandReadDto>();
            CreateMap<CommandCreateDto, Command>();
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<PlatformPublishedDto, Platform>()
                .ForMember(destination => 
                    destination.ExternalId, options => 
                        options.MapFrom(source => source.Id));
            CreateMap<GrpcPlatformModel, Platform>()
                .ForMember(destination => 
                    destination.ExternalId, options => 
                        options.MapFrom(source => source.PlatformId))
                .ForMember(destination => 
                    destination.Commands, options => 
                        options.Ignore());
        }
    }
}