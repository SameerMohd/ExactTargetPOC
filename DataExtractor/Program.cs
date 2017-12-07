using DataExtractor.Core.Configuration;
using DataExtractor.Creation;
using DataExtractor.ETService;
using DataExtractor.Trigger;
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
            // TestWithPasteHtml("SamTest12012017");
            // createSenderProfile();
            TestWithTemplate("SamTestTemplate12052017_21", true, true);
            Console.WriteLine("Done");
            Console.ReadKey();
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

        private static void TestWithPasteHtml(string externalKey)
        {
            try
            {
                CreateTriggeredSendWithPasteHtml(externalKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            try
            {
                StartTriggeredSend(externalKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            try
            {
                Send(externalKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed for Paste Html");
                Console.WriteLine(ex);
            }
        }


        private static void TestWithTemplate(string externalKey, bool NeedCC = false, bool NeedBCC = false)
        {
            try
            {
                CreateTriggeredSendWithTemplate(externalKey, NeedCC, NeedBCC);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            try
            {
                StartTriggeredSend(externalKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            try
            {
                Send(externalKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed for Template");
                Console.WriteLine(ex);
            }
        }

        private static void CreateTriggeredSendWithTemplate(string externalKey, bool NeedCC = false, bool NeedBCC = false)
        {
            var triggeredEmailCreator = new TriggeredEmailCreator(GetConfig());
            triggeredEmailCreator.CreateTriggeredSendDefinitionWithEmailTemplate(externalKey, "<html><head><style>table{font-family:arial,sans-serif;border-collapse:collapse;width:100%}td,th{border:1px solid #ddd;text-align:left;padding:8px}tr:nth-child(even){background-color:#ddd}</style></head>", "</html>", NeedCC, NeedBCC);
            Console.WriteLine("Completed creating triggered send");
        }

        private static void CreateTriggeredSendWithPasteHtml(string externalKey)
        {
            var triggeredEmailCreator = new TriggeredEmailCreator(GetConfig());
            triggeredEmailCreator.CreateTriggeredSendDefinitionWithPasteHtml(externalKey);
            Console.WriteLine("Completed creating triggered send");
        }

        private static void StartTriggeredSend(string externalKey)
        {
            var triggeredEmailCreator = new TriggeredEmailCreator(GetConfig());
            triggeredEmailCreator.StartTriggeredSend(externalKey);
            Console.WriteLine("Started triggered send");
        }

        private static void Send(string externalKey)
        {
            var triggeredEmail = new ExactTargetTriggeredEmail(externalKey, "sameer.mohammad@pimco.com");
            triggeredEmail.FromAddress = "Sameer.mohammad@pimco.com";
            triggeredEmail.FromName = "Master Tester";
            triggeredEmail.AddReplacementValue("Subject", "SamTest CC and BCC")
                            .AddReplacementValue("Body", "<table><tr> <th>Company</th> <th>Contact</th> <th>Country</th> </tr> <tr> <td>Test Company</td> <td>Test Contact</td> <td>Us</td> </tr> <tr> <td>Test Company 2</td> <td>Test Contact 2</td> <td>Us</td> </tr> </table>")
                            .AddReplacementValue("Head", "<style>.red{color:red}</style>");



            var emailTrigger = new EmailTrigger(GetConfig());
            emailTrigger.Trigger(triggeredEmail);
            Console.WriteLine("Triggered external key {0} to {1} successfully", triggeredEmail.ExternalKey, triggeredEmail.EmailAddress);
        }

        private static IExactTargetConfiguration GetConfig()
        {
            SimpleAES ObjAes = new SimpleAES();
            // Needs to get Loaded from Config File
            return new ExactTargetConfiguration
            {

                ApiUserName = "webtech@pimco.com",   // Generic ApiUserName
                ApiPassword = ObjAes.DecryptString("133171215054227028068033180158000111090232083231"),
                EndPoint = "https://webservice.s6.exacttarget.com/Service.asmx",//  Proper End Point Required From SMS
               ClientId = 6191809
            };
        }

        //public static void createSenderProfile()
        //{
        //    SimpleAES ObjAes = new SimpleAES();
        //    SoapClient partnerApi = new SoapClient();
        //    partnerApi.ClientCredentials.UserName.UserName = "webtech@pimco.com";
        //    partnerApi.ClientCredentials.UserName.Password = ObjAes.DecryptString("133171215054227028068033180158000111090232083231");
        //    //Instantiate SenderProfile and set general properties
        //    SenderProfile sp = new SenderProfile();
        //    sp.FromAddress = "sameer.mohammad@pimco.com";
        //    sp.FromName = "Sameer Mohammad";
        //    sp.CustomerKey = "SamSender";
        //    sp.Name = "SamSenderp";
        //    sp.Description = "Used for overriding the RMM settings";
        //    //optional - override the default RMM 
        //    sp.UseDefaultRMMRules = false;
        //    sp.UseDefaultRMMRulesSpecified = true;

        }

    }


