using System;
using System.Collections;
using System.Collections.Generic;
using AutoMapper;
using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc.Interfaces;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using PlatformService;

namespace CommandsService.SyncDataServices.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        
        public PlatformDataClient(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }
        
        public IEnumerable<Platform> ReturnAllPlatforms()
        {
            Console.WriteLine($"--> Calling GRPC Service {_configuration["GrpcPlatform"]}");
            var channel = GrpcChannel.ForAddress(_configuration["GrpcPlatform"]);
            var client = new GrpcPlatform.GrpcPlatformClient(channel);
            var request = new GetAllRequest();

            try
            {
                var reply = client.GetAllPlatforms(request);
                return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
            }
            catch (Exception e)
            {
                Console.WriteLine($"--> Could not call GRPC Server {e.Message}");
                return null;
            }
        }
    }
}