

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace EcoChallengerTest.Utils
{
    public class NetworkClient() {
        private HttpClient _client = new HttpClient();
        private GenericFunctions gf = new GenericFunctions();

        /// <summary>
        /// Logs in with a test user and returns a valid JWT
        /// </summary>
        /// <returns>Returns the token for the given user</returns>
        public async Task<string> GetLoginToken(string email = "testuser@example.com", string password = "Password123!") {
            string url = gf.BuildUrl("/api/Login/Login");

            var userCredenciais = new LoginRequestModel{
                Email = email,
                Password = password
            };

            var res = await _client.PostAsJsonAsync(url, userCredenciais);
            if(!res.IsSuccessStatusCode)
                throw new Exception("Could not fetch user token, invalid status code received.");
            
            var json = await res.Content.ReadAsStringAsync();
            var deseriaRes = JsonSerializer.Deserialize<LoginResponseModel>(json, new JsonSerializerOptions{ PropertyNameCaseInsensitive = true});
            
            if(deseriaRes == null)
                throw new Exception("Deserialized response is null.");
            
            return deseriaRes.Token;
        }

        /// <summary>
        /// Sends a get request to a given endpoint in the application
        /// </summary>
        /// <returns>Returns the response</returns>
        public async Task<HttpResponseMessage> SendGet(string path, bool needsAuth = true) {
            string url = gf.BuildUrl(path);

            if(needsAuth) {
                string token = await GetLoginToken();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await _client.GetAsync(url);
        }

        /// <summary>
        /// Sends a get request to a given endpoint in the application
        /// </summary>
        /// <returns>Returns the response</returns>
        public async Task<HttpResponseMessage> SendPost(string path, object data, bool needsAuth = true) {
            string url = gf.BuildUrl(path);

            if(needsAuth) {
                string token = await GetLoginToken();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            
            return await _client.PostAsJsonAsync(url, data);
        }
    }
}