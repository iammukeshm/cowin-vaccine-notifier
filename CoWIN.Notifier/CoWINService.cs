using CoWIN.Notifier.Models;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CoWIN.Notifier
{
    public static class CoWINService
    {
        public async static Task<CoWINResponse> Ping(int distrinctCode, DateTime dateTime = new DateTime())
        {
            var apiBaseUrl = $"https://cdn-api.co-vin.in/";
            var appointmentEndpoint = $"api/v2/appointment/sessions/public/calendarByDistrict?district_id={distrinctCode}&date={dateTime}";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //Added Polly as the API may be flooded with other public traffic at times.
                var response = await Policy
                           .HandleResult<HttpResponseMessage>(message => !message.IsSuccessStatusCode)
                           .WaitAndRetryAsync(new[]
                           {
                                TimeSpan.FromSeconds(1),
                                TimeSpan.FromSeconds(3),
                                TimeSpan.FromSeconds(6)
                           }, (result, timeSpan, retryCount, context) => {
                               Console.WriteLine($"Request failed with {result.Result.StatusCode}. Retry count = {retryCount}. Waiting {timeSpan} before next retry.");
                           })
                           .ExecuteAsync(() => client.GetAsync(appointmentEndpoint));
                if (response.IsSuccessStatusCode)
                {
                    var responseAsString = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonSerializer.Deserialize<CoWINResponse>(responseAsString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        ReferenceHandler = ReferenceHandler.Preserve
                    });
                    return responseObject;
                }
            }
            return null;
        }
        public async static Task<bool> Notify(string notificationMessage,string data, string apiKey)
        {
            var request = new IFTTTRequest() { value1 = notificationMessage, value2 = data };
            var apiBaseUrl = $"https://maker.ifttt.com/";
            var triggerEndpoint = $"trigger/vaccination_available/with/key/{apiKey}";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //Added Polly as the API may be flooded with other public traffic at times.
                var response = await Policy
                           .HandleResult<HttpResponseMessage>(message => !message.IsSuccessStatusCode)
                           .WaitAndRetryAsync(new[]
                           {
                                TimeSpan.FromSeconds(1),
                                TimeSpan.FromSeconds(3),
                                TimeSpan.FromSeconds(6)
                           }, (result, timeSpan, retryCount, context) => {
                               Console.WriteLine($"Request failed with {result.Result.StatusCode}. Retry count = {retryCount}. Waiting {timeSpan} before next retry.");
                           })
                           .ExecuteAsync(() => client.PostAsJsonAsync(triggerEndpoint, request));
                return response.IsSuccessStatusCode;
            }
        }
    }
}
