using DataExtractor.Core.Configuration;
using DataExtractor.Core.RequestClients.Shared;
using DataExtractor.Core.RequestClients.TriggeredSendDefinition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataExtractor
{
    class SamProgram
    {
        public static ISharedCoreRequestClient _SharedClent { get; set; }
        //static void Main(string[] args)
        //{

        //}

        public static void SendUsingPreDefinedKeys(TriggeredSendDataModel TriggerData, List<SubscriberDataModel> Subscriberlist)
        {
            var config = GetConfig();
            GetClient(config);

            if (CheckIsExists(TriggerData))
            {

            }
        }

        private static void GetClient(IExactTargetConfiguration config)
        {
            _SharedClent = new SharedCoreRequestClient(config);
        }

        private static IExactTargetConfiguration GetConfig()
        {
            SimpleAES ObjAes = new SimpleAES();
            // Needs to get Loaded from Config File
            return new ExactTargetConfiguration
            {
           
            };
        }

        public static bool CheckIsExists(TriggeredSendDataModel TriggerData)
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
