using System;
using System.Collections.Generic;
using System.Linq;
using Flurl.Http;
using Flurl.Http.Configuration;
using Flurl.Util;
using Microsoft.Extensions.Configuration;
using User.Gateway.Services.Interfaces;

namespace User.Gateway.Services
{
    public class FLService : IFLService
    {
        public readonly IConfiguration Config;
        protected readonly IFlurlClient UrlClient;
        protected readonly IFlurlClient IdentityClient;

        public FLService(
            IConfiguration config,
            IFlurlClientFactory clientFactory)
        {
            Config = config;
            UrlClient = clientFactory.Get(Config["UserService:Url"]);
            IdentityClient = clientFactory.Get(Config["IdentityService:Url"]);
        }

        public IFlurlRequest IdentityRequest(string path)
        {
            return IdentityClient.Request(path)
                            .AllowHttpStatus("4xx")
                            .WithHeader("Security-Code", Config["IdentityService:SecurityCode"])
                            .WithTimeout(TimeSpan.FromMinutes(3));
        }

        public IFlurlRequest Request(string path)
        {
            return UrlClient.Request(path)
                            .AllowHttpStatus("4xx")
                            .WithHeader("Security-Code", Config["UserService:SecurityCode"])
                            .WithTimeout(TimeSpan.FromMinutes(3));
        }

        public string[] FormErrors(object formErrors)
        {
            var errors = formErrors.ToKeyValuePairs().Select(kv => new { kv.Key, kv.Value });

            List<string> newErrors = new List<string>();
            foreach (var error in errors)
            {
                var newValue = error.Value.ToKeyValuePairs().Select(kv => kv.Key);
                foreach (var kv in newValue)
                {
                    newErrors.Add(kv);
                }
            }
            return newErrors.ToArray();
        }
    }
}
