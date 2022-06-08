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
    [Route("api/c/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly ICommandRepository _repository;
        private readonly IMapper _mapper;

        public PlatformsController(ICommandRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(_repository.GetAllPlatforms()));
        }

        [HttpPost]
        public ActionResult TestInboundConnection()
        {
            Console.WriteLine("--> Inbound POST # Command Service");

            return Ok("Inbound test from Platforms Controller OK");
        }
    }
}