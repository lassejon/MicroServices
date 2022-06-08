using System;
using System.Collections;
using System.Collections.Generic;
using AutoMapper;
using CommandsService.Dtos;
using CommandsService.Interfaces;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepository _repository;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
        {
            if (!_repository.PlatformExists(platformId))
            {
                return NotFound();
            }

            var commands = _repository.GetCommandsForPlatform(platformId);

            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
        }

        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
        {
            var command = _repository.GetCommand(platformId, commandId);
            
            if (!_repository.PlatformExists(platformId) || command == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CommandReadDto>(command));
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandCreateDto)
        {
            if (!_repository.PlatformExists(platformId))
            {
                return NotFound();
            }

            var command = _mapper.Map<Command>(commandCreateDto);

            _repository.CreateCommand(platformId, command);
            _repository.SaveChanges();

            var commandReadDto = _mapper.Map<CommandReadDto>(command);

            return CreatedAtRoute
            (
                nameof(GetCommandForPlatform),
                new
                {
                    platformId = platformId,
                    commandId = commandReadDto.Id,
                },
                commandReadDto
            );
        }
    }
}