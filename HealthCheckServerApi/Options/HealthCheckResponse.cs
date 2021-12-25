using System;
using System.Collections.Generic;

namespace HealthCheckServerApi.Options
{
    public class HealthCheckResponse
    {
        public string Status { get; set; }
        public IEnumerable<IndividualHealthCheckResponse> HealthChecks { get; set; }
        public TimeSpan HealthCheckDuration { get; set; }
    }
}
