namespace CommandsService.EventProcessing.Interfaces
{
    public interface IEventProcessor
    {
        void ProcessEvent(string message);
    }
}