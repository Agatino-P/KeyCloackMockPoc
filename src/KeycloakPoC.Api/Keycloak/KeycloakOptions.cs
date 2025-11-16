namespace KeycloakPoC.Api.Keycloak
{
    public class KeycloakOptions
    {
        public string Authority { get; set; } = string.Empty;
        public string TokenEndpoint { get; set; } = string.Empty;

        public ServiceClientOptions ServiceClient { get; set; } = new();
        public UserClientOptions UserClient { get; set; } = new();

        public class ServiceClientOptions
        {
            public string ClientId { get; set; } = string.Empty;
            public string ClientSecret { get; set; } = string.Empty;
        }

        public class UserClientOptions
        {
            public string ClientId { get; set; } = string.Empty;
        }
    }
}
