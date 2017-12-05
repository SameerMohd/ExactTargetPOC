﻿using System.Collections.Generic;

namespace DataExtractor.Core.RequestClients.Email
{
    public interface IEmailRequestClient
    {
        int CreateEmailFromTemplate(int emailTemplateId, string emailName, string subject, KeyValuePair<string, string> contentArea);
        int CreateEmail(string externalKey, string emailName, string subject, string htmlBody);
    }
}