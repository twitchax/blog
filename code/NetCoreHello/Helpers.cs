using DocumentDb.Fluent;

namespace NetCoreHello
{
    public static class Helpers
    {
        private const string EndpointUri = "https://friendsdocdb.documents.azure.com:443/";
        private const string PrimaryKey = "42GozGHCJi4UPTLPCmXcesuwJqOiUtftNB8u55jficIRpv5IuC6KIKf3uL2hnxzCWUVvZd43LTKPnCotXGFpTw==";
        
        public static IDocumentCollection<Friend> FriendCollection => DocumentDb.Fluent.DocumentDbInstance.Connect(EndpointUri, PrimaryKey).Database("Db").Collection<Friend>("Friends");
    }

    public class Friend : HasId
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
}