using Newtonsoft.Json;
using SI.Identity.Models;

public class IdentityClient
{
    private readonly HttpClient _httpClient;
    public IdentityClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<UserInfoDto?> GetUserInfo(string accessToken)
    {
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
        var response = await _httpClient.GetAsync("api/v1/users/current/info");
        response.EnsureSuccessStatusCode();
        var resp = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<UserInfoDto>(resp);
        return result;
    }
}