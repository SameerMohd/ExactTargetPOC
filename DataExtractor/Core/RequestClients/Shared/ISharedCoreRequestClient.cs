namespace DataExtractor.Core.RequestClients.Shared
{
    public interface ISharedCoreRequestClient
    {
        bool DoesObjectExist(string propertyName, string value, string objectType);
        string RetrieveObjectId(string propertyName, string value, string objectType);
        T RetrieveObject<T>(string propertyName, string value, string objectType);
    }
}