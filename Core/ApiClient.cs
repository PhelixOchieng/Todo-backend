namespace Application.Core.ApiClient;

public sealed class ApiClient {
  private string? _basePath;
  private HttpClient _client = new();

  public ApiClient(string baseAddress, string? basePath = null) {
    _client.BaseAddress = new Uri(baseAddress);
    _basePath = basePath;
  }

	public async Task<string> GetAsync(string url) {
		string urlToUse = _BuildUrl(url);
		return await _client.GetStringAsync(urlToUse);
	}

  public async Task<T?> GetAsJsonAsync<T>(string url) {
		string urlToUse = _BuildUrl(url);
    return await _client.GetFromJsonAsync<T>(urlToUse);
  }

  private string _BuildUrl(string url) {
    if (_basePath == null)
      return url;

		return $"{_basePath}{url}";
  }
}
