using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Cysharp.Net.Http;
using Cysharp.Threading.Tasks;
using Data.Configs;
using JetBrains.Annotations;
using Logic.Interfaces.Sync;
using Newtonsoft.Json;
using Zenject;

namespace Logic.Sync.Utilities
{
    [UsedImplicitly]
    public class ServerAccessService : IServerAccessService, ILateDisposable
    {
        private readonly HttpClient _httpClient;

        public ServerAccessService(ServerConfig serverConfig)
        {
            YetAnotherHttpHandler httpHandler = new();

            _httpClient = new HttpClient(httpHandler, true);

#if SKILLZ_ENABLED
            if (SkillzCrossPlatform.IsMatchInProgress() &&
                SkillzCrossPlatform.GetMatchInfo().CustomServerConnectionInfo != null)
            {
                string serverIp = SkillzCrossPlatform.GetMatchInfo().CustomServerConnectionInfo.ServerIp;
                string serverPort = SkillzCrossPlatform.GetMatchInfo().CustomServerConnectionInfo.ServerPort;

                _httpClient.BaseAddress = new Uri($"{serverIp}:{serverPort}");
            }
            else
            {
                _httpClient.BaseAddress = new Uri(serverConfig.BaseServerUrl);
            }
#else
            _httpClient.BaseAddress = new Uri(serverConfig.BaseServerUrl);
#endif

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private static void EnsureSuccessStatusCode(
            HttpResponseMessage response, string exceptionExplanation = "Request failed")
        {
            if (response.IsSuccessStatusCode)
                return;

            throw new HttpRequestException(
                $"{exceptionExplanation} || status code: {response.StatusCode}");
        }

        private static T TryDeserializeResponse<T>(string response)
        {
            T result;

            try
            {
                result = JsonConvert.DeserializeObject<T>(response);
            }
            catch (JsonException je)
            {
                JsonSerializationException jse = new("Cannot deserialize json response from server", je);
                throw jse;
            }

            return result;
        }

        public async UniTask<T> Get<T>(string endPoint)
        {
            var httpResult = await _httpClient.GetStringAsync(endPoint);

            return TryDeserializeResponse<T>(httpResult);
        }

        public async UniTask<TResponse> Post<TBody, TResponse>(string endPoint, TBody payload)
        {
            using HttpContent content =
                new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            using HttpResponseMessage response = await _httpClient.PostAsync(endPoint, content);

            EnsureSuccessStatusCode(response, "Post request failed");

            return TryDeserializeResponse<TResponse>(await response.Content.ReadAsStringAsync());
        }

        public async UniTask<TResponse> Put<TBody, TResponse>(string endPoint, TBody payload)
        {
            using HttpContent content =
                new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            using HttpResponseMessage response = await _httpClient.PutAsync(endPoint, content);

            EnsureSuccessStatusCode(response, "Put request failed");

            return TryDeserializeResponse<TResponse>(await response.Content.ReadAsStringAsync());
        }

        public async UniTask Put(string endPoint)
        {
            using HttpContent content =
                new StringContent(string.Empty, Encoding.UTF8, "application/json");
            using HttpResponseMessage response = await _httpClient.PutAsync(endPoint, content);

            EnsureSuccessStatusCode(response, "Put request failed");
        }

        public async UniTask<T> Delete<T>(string endPoint)
        {
            using HttpResponseMessage response = await _httpClient.DeleteAsync(endPoint);

            EnsureSuccessStatusCode(response, "Delete request failed");

            return TryDeserializeResponse<T>(await response.Content.ReadAsStringAsync());
        }

        public async UniTask Delete(string endPoint)
        {
            using HttpResponseMessage response = await _httpClient.DeleteAsync(endPoint);

            EnsureSuccessStatusCode(response, "Delete request failed");
        }

        public void LateDispose()
        {
            _httpClient.Dispose();
        }
    }
}