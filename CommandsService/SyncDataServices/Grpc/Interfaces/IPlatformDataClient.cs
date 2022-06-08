using System.Collections;
using System.Collections.Generic;
using CommandsService.Models;

namespace CommandsService.SyncDataServices.Grpc.Interfaces
{
    public interface IPlatformDataClient
    {
        IEnumerable<Platform> ReturnAllPlatforms();
    }
}