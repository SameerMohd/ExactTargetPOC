namespace DataExtractor.Core.RequestClients.TriggeredSendDefinition
{
    public interface ITriggeredSendDefinitionClient
    {
        int CreateTriggeredSendDefinition(string externalId,
            int emailId,
            string dataExtensionCustomerKey,
            string deliveryProfileCustomerKey,
            string name,
            string description, bool isCCNeed = false, bool isBccNeed = false, string CCmails = "", string BccEmails = "");

        bool DoesTriggeredSendDefinitionExist(string externalKey);

        void StartTriggeredSend(string externalKey);
        void UpdateTriggerSendDefinition(ETService.TriggeredSendDefinition tsd);
    }
}