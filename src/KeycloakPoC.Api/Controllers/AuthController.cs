using System.Text.Json;
using KeycloakPoC.Api.Keycloak;
using KeycloakPoC.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace KeycloakPoC.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly KeycloakOptions _keycloakOptions;

        public AuthController(
            IHttpClientFactory httpClientFactory,
            IOptions<KeycloakOptions> keycloakOptions)
        {
            _httpClientFactory = httpClientFactory;
            _keycloakOptions = keycloakOptions.Value;
        }

        [HttpPost("service-token")]
        public async Task<IActionResult> GetServiceToken([FromBody] ServiceTokenRequest _)
        {
            var client = _httpClientFactory.CreateClient("keycloak");
            var form = new Dictionary<string, string>
            {
                {"grant_type", "client_credentials"},
                {"client_id", _keycloakOptions.ServiceClient.ClientId},
                {"client_secret", _keycloakOptions.ServiceClient.ClientSecret}
            };

            var response = await client.PostAsync(_keycloakOptions.TokenEndpoint, new FormUrlEncodedContent(form));
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, content);

            var token = JsonSerializer.Deserialize<TokenResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return Ok(token);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            var client = _httpClientFactory.CreateClient("keycloak");
            var form = new Dictionary<string, string>
            {
                {"grant_type", "password"},
                {"client_id", _keycloakOptions.UserClient.ClientId},
                {"username", request.Username},
                {"password", request.Password}
            };

            var response = await client.PostAsync(_keycloakOptions.TokenEndpoint, new FormUrlEncodedContent(form));
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, content);

            var token = JsonSerializer.Deserialize<TokenResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return Ok(token);
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            return Ok(claims);
        }
    }
}
