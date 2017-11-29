using DataExtractor.ETService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataExtractor
{
    class Program
    {

        static void Main(string[] args)
        {
            testTriggeredSendEmail();
            Console.ReadLine();
        }
        public static void MoveFileToDest()
        {
            string SourcePath = ConfigurationManager.AppSettings["JsonSource"].ToString();
            string DestPath = ConfigurationManager.AppSettings["JsonDest"].ToString();
            string SFileName = ConfigurationManager.AppSettings["SFileName"].ToString();

            string FilePath = Path.Combine(SourcePath, SFileName);
            string DFilePath = Path.Combine(DestPath, SFileName);

            if (!Directory.Exists(DestPath))
            {
                Directory.CreateDirectory(DestPath);
            }

            if (File.Exists(FilePath))
            {
                File.Move(SFileName, DFilePath);
            }
        }


        public static void testTriggeredSendEmail()
        {
            ETService.SoapClient frame = new ETService.SoapClient();
            Email email = new Email();
            frame.ClientCredentials.UserName.UserName = "webtech@pimco.com";
            frame.ClientCredentials.UserName.Password = "133171215054227028068033180158000111090232083231";

            TriggeredSendDefinition definition = new TriggeredSendDefinition();
            definition.CustomerKey = "Definition_Key";

            //subscriber to whom email will be sent
            Subscriber subscriber = new Subscriber();
            subscriber.EmailAddress = "sameer.mohammad@pimco.com";
            subscriber.SubscriberKey = "sameer.mohammad@pimco.com";
            TriggeredSend send = new TriggeredSend();

            send.TriggeredSendDefinition = definition;

            //If passing Full HTML_Body, pass value to HTML__Body (This attribute should exist in account)
            ETService.Attribute attribute1 = new ETService.Attribute();
            attribute1.Name = "HTML__BODY";
            //attribute2.Value = html; 
            //set HTML content to Email, Testing foriegn language.
            attribute1.Value = "????????????????????";
            subscriber.Attributes = new ETService.Attribute[] { attribute1 }; //set attribute value and assign that to subscriber
            send.Subscribers = new Subscriber[] { subscriber }; //set subscriber to Triggered send
            APIObject[] sends = { send };
            string requestId = null;
            string overAllStatus = null;
            CreateOptions test = new CreateOptions();
            //If you want to send email Asynchronous, permission should be enabled.
            test.RequestType = RequestType.Asynchronous;
           // CreateResult[] results = frame.Create(new CreateOptions(), sends, out requestId, out overAllStatus);
            Console.Write("Status ::: " + overAllStatus);
        }
    }
}
