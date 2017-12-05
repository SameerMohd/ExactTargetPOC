using System;
using System.Collections.Generic;
using DataExtractor.Core;
using DataExtractor.Core.Configuration;
using DataExtractor.Core.RequestClients.DataExtension;
using DataExtractor.Core.RequestClients.DeliveryProfile;
using DataExtractor.Core.RequestClients.Email;
using DataExtractor.Core.RequestClients.EmailTemplate;
using DataExtractor.Core.RequestClients.TriggeredSendDefinition;

namespace DataExtractor.Creation
{
    public class TriggeredEmailCreator : ITriggeredEmailCreator
    {
        private readonly ITriggeredSendDefinitionClient _triggeredSendDefinitionClient;
        private readonly IDataExtensionClient _dataExtensionClient;
        private readonly IEmailTemplateClient _emailTemplateClient;
        private readonly IEmailRequestClient _emailRequestClient;
        private readonly IDeliveryProfileClient _deliveryProfileClient;

        public TriggeredEmailCreator(IDataExtensionClient dataExtensionClient,
            ITriggeredSendDefinitionClient triggeredSendDefinitionClient,
            IEmailTemplateClient emailTemplateClient,
            IEmailRequestClient emailRequestClient,
            IDeliveryProfileClient deliveryProfileClient)
        {
            _dataExtensionClient = dataExtensionClient;
            _triggeredSendDefinitionClient = triggeredSendDefinitionClient;
            _emailTemplateClient = emailTemplateClient;
            _emailRequestClient = emailRequestClient;
            _deliveryProfileClient = deliveryProfileClient;
        }

        public TriggeredEmailCreator(IExactTargetConfiguration config)
        {
            _triggeredSendDefinitionClient = new TriggeredSendDefinitionClient(config);
            _dataExtensionClient = new DataExtensionClient(config);
            _emailTemplateClient = new EmailTemplateClient(config);
            _emailRequestClient = new EmailRequestClient(config);
            _deliveryProfileClient = new DeliveryProfileClient(config);
        }

        public int Create(string externalKey)
        {
            return CreateWithTemplate(externalKey,
                "<html>" +
                "<body>" +
                EmailContentHelper.GetContentAreaTag("dynamicArea") +
                EmailContentHelper.GetOpenTrackingTag() +
                EmailContentHelper.GetCompanyPhysicalMailingAddressTags() +
                "</body>" +
                "</html>");
        }

        public int CreateTriggeredSendDefinitionWithEmailTemplate(string externalKey, string layoutHtmlAboveBodyTag, string layoutHtmlBelowBodyTag, bool NeedCC = false, bool NeedBCC = false)
        {
            return CreateWithTemplate(externalKey,
                layoutHtmlAboveBodyTag +
                "<body>" +
                EmailContentHelper.GetContentAreaTag("dynamicArea") +
                EmailContentHelper.GetOpenTrackingTag() +
                EmailContentHelper.GetCompanyPhysicalMailingAddressTags() +
                "</body>" +
                layoutHtmlBelowBodyTag, NeedCC, NeedBCC);
        }

        public int CreateTriggeredSendDefinitionWithPasteHtml(string externalKey)
        {
            return CreateWithoutTemplate(externalKey);
        }

        public void StartTriggeredSend(string externalKey)
        {
            _triggeredSendDefinitionClient.StartTriggeredSend(externalKey);
        }

        private int CreateWithTemplate(string externalKey, string layoutHtml, bool NeedCC = false, bool NeedBcc = false)
        {
            return Create(externalKey, layoutHtml, NeedCC, NeedBcc);
        }

        private int CreateWithoutTemplate(string externalKey)
        {
            return Create(externalKey, null);
        }

        private int Create(string externalKey, string layoutHtml, bool NeedCC = false, bool NeedBCC = false)
        {
            var isTemplated = !string.IsNullOrWhiteSpace(layoutHtml);
            if (externalKey.Length > Guid.Empty.ToString().Length)
            {
                throw new ArgumentException("externalKey too long, should be max length of " + Guid.Empty.ToString().Length, "externalKey");
            }

            if (_triggeredSendDefinitionClient.DoesTriggeredSendDefinitionExist(externalKey))
            {
                throw new Exception(string.Format("A TriggeredSendDefinition with external key {0} already exsits", externalKey));
            }

            var dataExtensionExternalKey = ExternalKeyGenerator.GenerateExternalKey("data-extension-" + externalKey);
            if (!_dataExtensionClient.DoesDataExtensionExist(dataExtensionExternalKey))
            {
                var dataExtensionTemplateObjectId = _dataExtensionClient.RetrieveTriggeredSendDataExtensionTemplateObjectId();
                var dataExtensionFieldNames = new HashSet<string> { "Subject", "Body" };
                if (!isTemplated)
                {
                    dataExtensionFieldNames.Add("Head");
                }

                _dataExtensionClient.CreateDataExtension(dataExtensionTemplateObjectId,
                                    dataExtensionExternalKey,
                                    "triggeredsend-" + externalKey,
                                    dataExtensionFieldNames);

            }


            int emailId;
            var emailName = "email-" + externalKey;
            var emailExternalKey = ExternalKeyGenerator.GenerateExternalKey("email-" + externalKey);
            if (isTemplated)
            {
                var emailTempalteExternalKey = ExternalKeyGenerator.GenerateExternalKey("email-template" + externalKey);
                var emailTemplateId = _emailTemplateClient.RetrieveEmailTemplateId(emailTempalteExternalKey);
                if (emailTemplateId == 0)
                {
                    emailTemplateId = _emailTemplateClient.CreateEmailTemplate(emailTempalteExternalKey,
                        "template-" + externalKey,
                        layoutHtml);
                }

                emailId = _emailRequestClient.CreateEmailFromTemplate(emailTemplateId,
                    emailName,
                    "%%Subject%%",
                    new KeyValuePair<string, string>("dynamicArea", "%%Body%%"));
            }
            else
            {
                emailId = _emailRequestClient.CreateEmail(emailExternalKey, emailName, "%%Subject%%",
                    "<html>" +
                    "<head>%%Head%%</head>" +
                    "%%Body%%" +
                    EmailContentHelper.GetOpenTrackingTag() +
                    EmailContentHelper.GetCompanyPhysicalMailingAddressTags() +
                    "</html>");
            }


            var deliveryProfileExternalKey = ExternalKeyGenerator.GenerateExternalKey("blank-delivery-profile");
            _deliveryProfileClient.TryCreateBlankDeliveryProfile(deliveryProfileExternalKey);

            return _triggeredSendDefinitionClient.CreateTriggeredSendDefinition(externalKey,
                                                                                emailId,
                                                                                dataExtensionExternalKey,
                                                                                deliveryProfileExternalKey,
                                                                                externalKey,
                                                                                externalKey, NeedCC, NeedBCC);


        }
    }
}