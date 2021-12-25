using System;

namespace HealthCheckServerApi.Options
{
    public class RemoteOptions
    {
        public Uri RemoteDependency { get; set; } = new Uri("https://google.com");
    }
}
