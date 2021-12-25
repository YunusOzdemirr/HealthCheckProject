using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace HealthCheckServerApi.HealthChecksServer
{
    public class HealthCheck : IHealthCheck
    {

        public string _host { get; set; }

        public HealthCheck(string host)
        {
            _host = host;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default(CancellationToken))
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_host);
            HttpResponseMessage response = await client.GetAsync("");
            if (response.StatusCode != HttpStatusCode.OK)
            {
              
                List<string> numbers = new List<string>();
                numbers.Add("+905554443322");
                numbers.Add("+905553334422");
                //for (int j = 0; j < numbers.Count; j++)
                //{
                //    string accountSid = Environment.GetEnvironmentVariable("sid");
                //    string authToken = Environment.GetEnvironmentVariable("oauth");
                //    TwilioClient.Init(accountSid, authToken);
                //    MessageResource.Create(
                //       body: "Eyyo your api is shit",
                //       from: new Twilio.Types.PhoneNumber("+12695253560"),
                //       to: new Twilio.Types.PhoneNumber(numbers[j]));
                //}
                context.Registration.Tags.Clear();
                context.Registration.Tags.Add("Api is have problem!!!!");
                return await Task.FromResult(new HealthCheckResult(
                          status: HealthStatus.Healthy,
                          description: "The API is fucked up"));
            }
            context.Registration.Tags.Clear();
            context.Registration.Tags.Add("Api is working.");
            return await Task.FromResult(new HealthCheckResult(
                               status: HealthStatus.Healthy,
                              description: "The API is normal"));
        }
    }
}
