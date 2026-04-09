namespace BlazorUI.Services
{
    // This class defines two typed HTTP clients, PublicApiClient and ProtectedApiClient, which are used to interact with public and protected APIs respectively.
    public class Typed_clients
    {
        public class PublicApiClient
        {
            public HttpClient Http { get; }

            public PublicApiClient(HttpClient http)
            {
                Http = http;
            }
        }

        public class ProtectedApiClient
        {
            public HttpClient Http { get; }

            public ProtectedApiClient(HttpClient http)
            {
                Http = http;
            }
        }

    }
}
