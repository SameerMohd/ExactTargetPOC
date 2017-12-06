using System.Linq;
using DataExtractor.Core.Configuration;
using DataExtractor.Core.RequestClients.Shared;
using DataExtractor.ETService;

namespace DataExtractor.Core.RequestClients.TriggeredSendDefinition
{
    public class TriggeredSendDefinitionClient : ITriggeredSendDefinitionClient
    {
        private readonly IExactTargetConfiguration _config;
        private readonly SoapClient _client;
        private readonly ISharedCoreRequestClient _sharedCoreRequestClient;

        public TriggeredSendDefinitionClient(IExactTargetConfiguration config)
        {
            _config = config;
            _client = SoapClientFactory.Manufacture(config);
            _sharedCoreRequestClient = new SharedCoreRequestClient(config);
        }

        public int CreateTriggeredSendDefinition(
            string externalId,
            int emailId,
            string dataExtensionCustomerKey,
            string deliveryProfileCustomerKey,
            string name,
            string description, bool NeedCC = false, bool NeedBcc = false)
        {
            var ts = new ETService.TriggeredSendDefinition
            {
                Client = _config.ClientId.HasValue ? new ClientID { ID = _config.ClientId.Value, IDSpecified = true } : null,
                Email = new ETService.Email { ID = emailId, IDSpecified = true },
                SendSourceDataExtension = new ETService.DataExtension { CustomerKey = dataExtensionCustomerKey },
                Name = name,
                Description = description,
                CustomerKey = externalId,
                TriggeredSendStatus = TriggeredSendStatusEnum.Active,
                SendClassification = new SendClassification
                {
                    CustomerKey = "Default Transactional"
                },
                IsMultipart = true,
                IsMultipartSpecified = true,
                //FromAddress="%%FromAddress%%",
                //FromName="%%FromName%%",
                DeliveryProfile = new ETService.DeliveryProfile
                {
                    CustomerKey = deliveryProfileCustomerKey
                },
                IsWrapped = true,
                IsWrappedSpecified = true

            };

            if (NeedCC)
                ts.CCEmail = "sam232b@gmail.com;kkmir09@gmail.com"; //"%%CCAddress%%"; //Multiple CCAddress
            // if (NeedBcc)
            //  ts.BccEmail = "kkmir09@gmail.com";// "%%BCCAddress%%";

            string requestId, status;
            var result = _client.Create(new CreateOptions(), new APIObject[] { ts }, out requestId, out status);

            ExactTargetResultChecker.CheckResult(result.FirstOrDefault());

            return result.First().NewID;

        }

        public bool DoesTriggeredSendDefinitionExist(string externalKey)
        {
            return _sharedCoreRequestClient.DoesObjectExist("CustomerKey", externalKey, "TriggeredSendDefinition");
        }

        public void StartTriggeredSend(string externalKey)
        {
            var ts = new ETService.TriggeredSendDefinition
            {
                Client = _config.ClientId.HasValue ? new ClientID { ID = _config.ClientId.Value, IDSpecified = true } : null,
                CustomerKey = externalKey,
                TriggeredSendStatus = TriggeredSendStatusEnum.Active,
                TriggeredSendStatusSpecified = true
            };

            string requestId, overallStatus;
            var result = _client.Update(new UpdateOptions(), new APIObject[] { ts }, out requestId, out overallStatus);
            ExactTargetResultChecker.CheckResult(result.FirstOrDefault());
        }

    }
}