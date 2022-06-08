using PlatformService.Dtos;

namespace PlatformService.AsyncDataServices.Http.Interfaces
{
    public interface IMessageBusClient
    {
        void PublishNewPlatform(PlatformPublishedDto platformPublishedDto);
    }
}