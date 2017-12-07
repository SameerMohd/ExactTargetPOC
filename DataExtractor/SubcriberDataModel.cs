using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataExtractor
{
    public class SubscriberDataModel
    {
        public string SubcriberEmail { get; set; }
        public int? SubscriberKey { get; set; }
        public List<KeyValuePair<string, string>> ReplacementValues { get; set; }
    }
    public class TriggeredSendDataModel
    {
        public string TriggerSendDefinitionExternalKey { get; set; }
        public string EmailTemplateExternalKey { get; set; }
        public string DataExtensionExternalKey { get; set; }
        public string EmailExternalKey { get; set; }
    }
}
