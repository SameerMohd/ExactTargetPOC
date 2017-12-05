namespace DataExtractor.Creation
{
    public interface ITriggeredEmailCreator
    {
        int Create(string externalKey);
        int CreateTriggeredSendDefinitionWithEmailTemplate(string externalKey, string layoutHtmlAboveBodyTag, string layoutHtmlBelowBodyTag, bool NeedCC = false, bool NeedBCC = false);
        int CreateTriggeredSendDefinitionWithPasteHtml(string externalKey);
        void StartTriggeredSend(string externalKey);
    }
}