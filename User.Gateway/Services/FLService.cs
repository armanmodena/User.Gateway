using System;
using Flurl.Http;
using Flurl.Http.Configuration;
using Microsoft.Extensions.Configuration;
using User.Gateway.Services.Interfaces;

namespace User.Gateway.Services
{
    public class FLService : IFLService
    {
        public readonly IConfiguration Config;
        protected readonly IFlurlClient UrlClient;

        public FLService(
            IConfiguration config,
            IFlurlClientFactory clientFactory)
        {
            Config = config;
            UrlClient = clientFactory.Get(Config["UserService:Url"]);
        }

        public IFlurlRequest Request(string path)
        {
            return UrlClient.Request(path)
                            .WithHeader("Security-Code", "secret")
                            .WithTimeout(TimeSpan.FromMinutes(3));
        }
    }
}
