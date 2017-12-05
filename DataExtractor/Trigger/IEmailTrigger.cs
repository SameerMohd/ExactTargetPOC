namespace DataExtractor.Trigger
{
    public interface IEmailTrigger
    {
        void Trigger(ExactTargetTriggeredEmail exactTargetTriggeredEmail, RequestQueueing requestQueueing, Priority priority);
    }
}