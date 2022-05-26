using HealthChecks.UI.Client;
using HealthCheckServerApi.Extensions;
using HealthCheckServerApi.HealthChecksServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using System;
using HealthCheckServerApi.Options;
using Microsoft.Extensions.Options;
using Polly.Extensions.Http;
using Polly.Timeout;
using System.Net.Http;
using Polly;

namespace HealthCheckServerApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HealthCheckServerApi", Version = "v1" });
            });

            //services
            //    .AddHealthChecksUI()
            //    .AddInMemoryStorage()
            //    .Services
            //    .AddHealthChecks()
            //    .AddCheck<HealthCheck>("random")
            //    .AddUrlGroup(new Uri("https://hsapi.infoyatirim.com/"))
            //    //.AddKubernetes(setup =>
            //    //{
            //    //    setup.WithConfiguration(k8s.KubernetesClientConfiguration.BuildConfigFromConfigFile())
            //    //        .CheckDeployment("wordpress-one-wordpress",
            //    //            d => d.Status.Replicas == 2 && d.Status.ReadyReplicas == 2)
            //    //        .CheckService("wordpress-one-wordpress", s => s.Spec.Type == "LoadBalancer")
            //    //        .CheckPod("myapp-pod", p =>  p.Metadata.Labels["app"] == "myapp" );
            //    //})
            //    .Services
            //    .AddControllers();

            //var retryPolicy = Polly.Extensions.Http.HttpPolicyExtensions
            //    .HandleTransientHttpError()
            //    .Or<Polly.Timeout.TimeoutRejectedException>()
            //    .RetryAsync(5);

            //services.AddHttpClient("uri-group") //default healthcheck registration name for uri ( you can change it on AddUrlGroup )
            //    .AddPolicyHandler(retryPolicy)
            //    .ConfigurePrimaryHttpMessageHandler(() => new System.Net.Http.HttpClientHandler()
            //    {
            //        ClientCertificateOptions = System.Net.Http.ClientCertificateOption.Manual,
            //        ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
            //        {
            //            return true;
            //        }
            //    });

            services.Configure<HealthCheckPublisherOptions>(options =>
            {
                options.Delay = TimeSpan.FromSeconds(2);
                options.Predicate = (check) => check.Tags.Contains("ready");
                options.Period = TimeSpan.FromSeconds(2);
            });

            services
                .AddHealthChecksUI(setupSettings: settings => { settings.SetEvaluationTimeInSeconds(10); })
                         .AddInMemoryStorage()
                         //.AddHealthChecksUI(setupSettings: settings =>
                         //               {
                         //                   settings
                         //                       .AddHealthCheckEndpoint("api1", "http://localhost:8001/custom/healthz")
                         //                       .AddWebhookNotification("webhook1", "http://webhook", "mypayload")
                         //                       .SetEvaluationTimeInSeconds(16);
                         //               })
                         .Services
                         .AddHealthChecks()
                         .AddUrlGroup(new Uri("https://google.com"), name: "Google")
                         .AddUrlGroup(new Uri("https://yandex.com"), name: "Yandex")
                          .AddUrlGroup(new Uri("https://ya12adsadndex.com"), name: "Yandexasdas")
                         .AddCheck("Udemy", new HealthCheck("https://udemy.com"))
                         //.AddUrlGroup(
                         //sp =>
                         //{
                         //    var remoteOptions = sp.GetRequiredService<IOptions<RemoteOptions>>().Value;
                         //    return remoteOptions.RemoteDependency;
                         //}, "Nabersin")
                         // .AddUrlGroup(new Uri("http://httpbin.org/status/200"), name: "uri-3", timeout: TimeSpan.FromSeconds(3))
                         .Services
                         .AddControllers();

            var retryPolicy = HttpPolicyExtensions
               .HandleTransientHttpError()
               .Or<TimeoutRejectedException>()
               .RetryAsync(5);

            services.AddHttpClient("uri-group") //default healthcheck registration name for uri ( you can change it on AddUrlGroup )
                .AddPolicyHandler(retryPolicy)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        return true;
                    }
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting()
               .UseEndpoints(config =>
               {
                   config.MapHealthChecks("healthz", new HealthCheckOptions
                   {
                       Predicate = _ => true,
                       ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
                   });
                   config.MapHealthChecksUI(setup =>
                   {
                       setup.UIPath = "/show-health-ui"; // this is ui path in your browser
                       setup.ApiPath = "/health-ui-api"; // the UI ( spa app )  use this path to get information from the store ( this is NOT the healthz path, is internal ui api )
                   });
                   config.MapDefaultControllerRoute();
               });

        }
    }
}
