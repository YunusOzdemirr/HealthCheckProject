using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace HealthCheckServerApi.HealthChecksServer
{
    public class HealthCheckWithArgs: IHealthCheck
    {
        public HealthCheckWithArgs(int i, string s)
        {
            I = i;
            S = s;
        }

        public int I { get; set; }

        public string S { get; set; }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            return CheckHealthAsync(context, cancellationToken);
        }

        private static Task WriteResponse(HttpContext context, HealthReport result)
        {
            context.Response.ContentType = "application/json; charset=utf-8";
            var options = new JsonWriterOptions
            {
                Indented = true
            };

            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, options))
                {
                    writer.WriteStartObject();
                    writer.WriteString("status", result.Status.ToString());
                    writer.WriteStartObject("results");
                    foreach (var entry in result.Entries)
                    {
                        writer.WriteStartObject(entry.Key);
                        writer.WriteString("status", entry.Value.Status.ToString());
                        writer.WriteString("description", entry.Value.Description);
                        writer.WriteStartObject("data");
                        foreach (var item in entry.Value.Data)
                        {
                            writer.WritePropertyName(item.Key);
                            JsonSerializer.Serialize(
                                writer, item.Value, item.Value?.GetType() ??
                                typeof(object));
                        }
                        writer.WriteEndObject();
                        writer.WriteEndObject();
                    }
                    writer.WriteEndObject();
                    writer.WriteEndObject();
                }

                var json = Encoding.UTF8.GetString(stream.ToArray());

                return context.Response.WriteAsync(json);
            }
        }
    }
}
