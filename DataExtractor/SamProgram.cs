using DataExtractor.Core.Configuration;
using DataExtractor.Core.RequestClients.DataExtension;
using DataExtractor.Core.RequestClients.DeliveryProfile;
using DataExtractor.Core.RequestClients.Email;
using DataExtractor.Core.RequestClients.EmailTemplate;
using DataExtractor.Core.RequestClients.Shared;
using DataExtractor.Core.RequestClients.TriggeredSendDefinition;
using DataExtractor.ETService;
using DataExtractor.Trigger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataExtractor
{
    class SamProgram
    {
        #region"constants                   "
        public static ISharedCoreRequestClient _SharedClent { get; set; }
        public static ITriggeredSendDefinitionClient triggeredSendDefinitionClient { get; set; }
        public static IDataExtensionClient _dataExtensionClient { get; set; }
        public static IEmailTemplateClient _emailTemplateClient { get; set; }
        public static IEmailRequestClient _emailRequestClient { get; set; }
        public static IDeliveryProfileClient _deliveryProfileClient { get; set; }
        #endregion

        //static void Main(string[] args)
        //{

        //}


        private static void SendUsingPreDefinedKeys(TriggeredSendDataModel TriggerData, List<SubscriberDataModel> Subscriberlist)
        {
            var config = GetConfig();
            GetClient(config);

            if (!CheckIsExists(TriggerData))
            {
                throw new Exception("Dependent object not Exists");
            }
            StartTriggerSend(TriggerData);
            SendMail(TriggerData, Subscriberlist, config);
        }

        private static void SendMail(TriggeredSendDataModel triggerData, List<SubscriberDataModel> subscriberlist, IExactTargetConfiguration config)
        {
            var emailTrigger = new EmailTrigger(config);
            var lst = GetSubscriberList(subscriberlist, triggerData);
            emailTrigger.TriggerCustom(triggerData, lst);

        }

        private static List<Subscriber> GetSubscriberList(List<SubscriberDataModel> subscriberlist, TriggeredSendDataModel triggerData)
        {
            List<Subscriber> lst = new List<Subscriber>();
            if (subscriberlist != null)
            {
                foreach (var sub in subscriberlist)
                {
                    var subscriber = new Subscriber
                    {
                        Addresses = new SubscriberAddress[] { new SubscriberAddress() { Address = "", AddressType = "" } },
                        EmailAddress = sub.SubscriberEmail,
                        SubscriberKey = sub.SubscriberKey ?? sub.SubscriberEmail,
                        Attributes =
                            sub.ReplacementValues.Select(value => new ETService.Attribute
                            {
                                Name = value.Key,
                                Value = value.Value
                            }).ToArray()
                    };
                    subscriber.Owner = new Owner()
                    {
                        FromAddress = sub.FromEmail ?? triggerData.FromEmail,
                        FromName = sub.FromName ?? triggerData.FromName,
                    };

                    lst.Add(subscriber);
                }
            }
            return lst;
        }

        private static void StartTriggerSend(TriggeredSendDataModel TriggerData)
        {
            var TS = _SharedClent.RetrieveObject<TriggeredSendDefinition>("CustomerKey", TriggerData.TriggerSendDefinitionExternalKey, "TriggeredSendDefinition");
            if (TS != null)
            {
                if (TS.TriggeredSendStatus != TriggeredSendStatusEnum.Active)
                {
                    triggeredSendDefinitionClient.StartTriggeredSend(TS.CustomerKey);
                }
            }
        }

        private static void GetClient(IExactTargetConfiguration config)
        {
            _SharedClent = new SharedCoreRequestClient(config);
            triggeredSendDefinitionClient = new TriggeredSendDefinitionClient(config);
            _dataExtensionClient = new DataExtensionClient(config);
            _emailTemplateClient = new EmailTemplateClient(config);
            _emailRequestClient = new EmailRequestClient(config);
            _deliveryProfileClient = new DeliveryProfileClient(config);
        }

        private static IExactTargetConfiguration GetConfig()
        {
            SimpleAES ObjAes = new SimpleAES();
            // Needs to get Loaded from Config File
            return new ExactTargetConfiguration
            {

            };
        }

        private static bool CheckIsExists(TriggeredSendDataModel TriggerData)
        {
            if (TriggerData != null)
            {
                var isEmailTemplateExternalKey = _SharedClent.DoesObjectExist("CustomerKey", TriggerData.EmailTemplateExternalKey, "Template");
                var isDataExtension = _SharedClent.DoesObjectExist("CustomerKey", TriggerData.DataExtensionExternalKey, "DataExtension");
                var isTriggeredSendDefinition = _SharedClent.DoesObjectExist("CustomerKey", TriggerData.TriggerSendDefinitionExternalKey, "TriggeredSendDefinition");
                var isEmail = _SharedClent.DoesObjectExist("CustomerKey", TriggerData.EmailExternalKey, "Email");

                if (isEmailTemplateExternalKey && isDataExtension && isTriggeredSendDefinition && isEmail)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
