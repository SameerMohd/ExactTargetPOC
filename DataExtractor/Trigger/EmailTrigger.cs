﻿using System;
using System.Collections.Generic;
using System.Linq;
using DataExtractor.Core;
using DataExtractor.Core.Configuration;
using DataExtractor.ETService;
using Attribute = DataExtractor.ETService.Attribute;

namespace DataExtractor.Trigger
{
    public enum RequestQueueing
    {
        No = 0,
        Yes,
    }

    public enum Priority
    {
        Normal = 0,
        High
    }

    public class EmailTrigger : IEmailTrigger
    {
        private readonly IExactTargetConfiguration _config;

        public EmailTrigger(IExactTargetConfiguration config)
        {
            _config = config;
        }

        public void Trigger(ExactTargetTriggeredEmail exactTargetTriggeredEmail, RequestQueueing requestQueueing = RequestQueueing.No, Priority priority = Priority.Normal)
        {
            var clientId = _config.ClientId;
            var client = SoapClientFactory.Manufacture(_config);

            var subscriber = new Subscriber
            {
                Addresses = new SubscriberAddress[] { new SubscriberAddress() { Address = "", AddressType = "" } },
                EmailAddress = exactTargetTriggeredEmail.EmailAddress,
                SubscriberKey = exactTargetTriggeredEmail.SubscriberKey ?? exactTargetTriggeredEmail.EmailAddress,
                Attributes =
                    exactTargetTriggeredEmail.ReplacementValues.Select(value => new Attribute
                    {
                        Name = value.Key,
                        Value = value.Value
                    }).ToArray()
            };

            // Add sender information if specified. This will send the email with FromAddress in the sender field.
            // Official doco here under the section "Determining the From Information at Send Time":
            // https://help.exacttarget.com/en/technical_library/web_service_guide/triggered_email_scenario_guide_for_developers/#Determining_the_From_Information_at_Send_Time
            if (!string.IsNullOrEmpty(exactTargetTriggeredEmail.FromAddress) && !string.IsNullOrEmpty(exactTargetTriggeredEmail.FromName))
            {
                subscriber.Owner = new Owner()
                {
                    FromAddress = exactTargetTriggeredEmail.FromAddress,
                    FromName = exactTargetTriggeredEmail.FromName
                };
            }

            var subscribers = new List<Subscriber> { subscriber };

            var tsd = new TriggeredSendDefinition
            {
                Client = clientId.HasValue ? new ClientID { ID = clientId.Value, IDSpecified = true } : null,
                CustomerKey = exactTargetTriggeredEmail.ExternalKey,
            };

            var ts = new TriggeredSend
            {
                Client = clientId.HasValue ? new ClientID { ID = clientId.Value, IDSpecified = true } : null,
                TriggeredSendDefinition = tsd,
                Subscribers = subscribers.ToArray()
            };

            var co = new CreateOptions
            {
                RequestType = requestQueueing == RequestQueueing.No ? RequestType.Synchronous : RequestType.Asynchronous,
                RequestTypeSpecified = true,
                QueuePriority = priority == Priority.High ? ETService.Priority.High : ETService.Priority.Medium,
                QueuePrioritySpecified = true
            };

            string requestId, status;
            var result = client.Create(
                co,
                new APIObject[] { ts },
                out requestId, out status);

            ExactTargetResultChecker.CheckResult(result.FirstOrDefault()); //we expect only one result because we've sent only one APIObject
        }

        public void TriggerCustom(TriggeredSendDataModel exactTargetTriggeredEmail, List<Subscriber> lst, RequestQueueing requestQueueing = RequestQueueing.No, Priority priority = Priority.Normal)
        {
            var clientId = _config.ClientId;
            var client = SoapClientFactory.Manufacture(_config);

            

            var tsd = new TriggeredSendDefinition
            {
                Client = clientId.HasValue ? new ClientID { ID = clientId.Value, IDSpecified = true } : null,
                CustomerKey = exactTargetTriggeredEmail.TriggerSendDefinitionExternalKey,
            };

            var ts = new TriggeredSend
            {
                Client = clientId.HasValue ? new ClientID { ID = clientId.Value, IDSpecified = true } : null,
                TriggeredSendDefinition = tsd,
                Subscribers = lst.ToArray()
            };

            var co = new CreateOptions
            {
                RequestType = requestQueueing == RequestQueueing.No ? RequestType.Synchronous : RequestType.Asynchronous,
                RequestTypeSpecified = true,
                QueuePriority = priority == Priority.High ? ETService.Priority.High : ETService.Priority.Medium,
                QueuePrioritySpecified = true
            };

            string requestId, status;
            var result = client.Create(
                co,
                new APIObject[] { ts },
                out requestId, out status);

            ExactTargetResultChecker.CheckResult(result.FirstOrDefault()); //we expect only one result because we've sent only one APIObject
        }
    }
}
