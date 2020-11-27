using MagazineStore.Models;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MagazineStore
{
  class MagazineStoreClient
  {
    const string BASE_URL = "http://magazinestore.azurewebsites.net";

    readonly IRestClient _client;
    readonly string _token;

    public MagazineStoreClient() {
      _client = new RestClient(BASE_URL);
      _client.UseNewtonsoftJson();

      _token = GetToken();
    }

    private IRestResponse<T> Execute<T>(RestRequest request) where T : new() {
      if (request.Resource.Contains("{token}")) {
        request.AddParameter("token", _token, ParameterType.UrlSegment); // used on every request
      }

      var response = _client.Execute<T>(request);     
      
      if (!response.IsSuccessful) {
        throw new Exception(response.ErrorMessage, response.ErrorException);
      }

      return response;
    }

    private Task<IRestResponse<T>> ExecuteAsync<T>(RestRequest request) where T : new() {
      if (request.Resource.Contains("{token}")) {
        request.AddParameter("token", _token, ParameterType.UrlSegment); // used on every request
      }

      return _client.ExecuteAsync<T>(request);
    }

    private string GetToken() {
      var response = Execute<ApiResponse>(new RestRequest("/api/token"));
      var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(response.Content);
      return apiResponse.Token;
    }

    public async Task<List<string>> GetCategoriesAsync() {
      var response = await ExecuteAsync<ApiResponse>(new RestRequest("/api/categories/{token}"));
      var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(response.Content);
      return apiResponse.Data.ToObject<List<string>>();
    }

    public async Task<List<Magazine>> GetMagazinesAsync(string category) {
      var request = new RestRequest("/api/magazines/{token}/{category}");
      request.AddParameter("category", category, ParameterType.UrlSegment);
      
      var response = await ExecuteAsync<ApiResponse>(request);
      var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(response.Content);
      return apiResponse.Data.ToObject<List<Magazine>>();
    }

    public async Task<List<Subscriber>> GetSubscribersAsync() {
      var response = await ExecuteAsync<ApiResponse>(new RestRequest("/api/subscribers/{token}"));
      var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(response.Content);
      return apiResponse.Data.ToObject<List<Subscriber>>();
    }

    public AnswerResponse PostAnswer(Answer answer) {
      var request = new RestRequest("/api/answer/{token}", Method.POST);
      request.AddJsonBody(answer);

      var response = Execute<ApiResponse>(request);
      var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(response.Content);
      return apiResponse.Data.ToObject<AnswerResponse>();
    }
  }
}
