namespace AuthorizationServer8.Extensions
{
    class Constants
    {
    }

    public enum FileStorageLocations : int
    {
        NotDefined = 0,
        Archive1 = 1 << 0, //1
        Archive2 = 1 << 1, //2
        Archive3 = 1 << 2, //4
        Archive4 = 1 << 3, //8
        Archive5 = 1 << 4, //16
        Archive6 = 1 << 5, //32
        WebShare = 1 << 6, //64
        AzureStorage = 1 << 7,  // 128
        AmazonStorage = 1 << 8,  // //256
        GoogleStorage = 1 << 9,  // //512
    }
}