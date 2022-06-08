using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PlatformService.AsyncDataServices.Http.Interfaces;
using PlatformService.Dtos;
using PlatformService.Interfaces;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http.Interfaces;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepository _platformRepository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IConfiguration _configuration;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformsController(
            IPlatformRepository platformRepository, 
            IMapper mapper, 
            ICommandDataClient commandDataClient,
            IConfiguration configuration,
            IMessageBusClient messageBusClient)
        {
            _platformRepository = platformRepository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
            _configuration = configuration;
            _messageBusClient = messageBusClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            var platforms = _platformRepository.GetAllPlatforms();
            var platformReadDtos = _mapper.Map<IEnumerable<PlatformReadDto>>(platforms);
            
            return Ok(platformReadDtos);
        }

        [HttpGet("{id:int}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {
            var platform = _platformRepository.GetPlatformById(id);
            if (platform == null)
            {
                return NotFound();
            }
            
            var platformReadDto = _mapper.Map<PlatformReadDto>(platform);
            return Ok(platformReadDto);
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            if (platformCreateDto is null)
            {
                return BadRequest();
            }

            var platform = _mapper.Map<Platform>(platformCreateDto);
            _platformRepository.CreatePlatform(platform);
            _platformRepository.SaveChanges();

            var platformReadDto = _mapper.Map<PlatformReadDto>(platform);

            await SendMessageSync(platformReadDto);

            SendMessageAsync(platformReadDto);
            
            return CreatedAtRoute(nameof(GetPlatformById), new { Id = platformReadDto.Id }, platformReadDto);
        }

        private void SendMessageAsync(PlatformReadDto platformReadDto)
        {
            try
            {
                var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
                platformPublishedDto.Event = "Platform_Published";

                _messageBusClient.PublishNewPlatform(platformPublishedDto);

                Console.WriteLine($"--> Platform was published");
            }
            catch (Exception e)
            {
                Console.WriteLine($"--> Could not send asynchronously to CommandsService");
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task SendMessageSync(PlatformReadDto platformReadDto)
        {
            try
            {
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"--> Could not send synchronously to CommandsService with the URI of {_configuration["CommandService"]}");
                Console.WriteLine(ex);
            }
        }
    }
}