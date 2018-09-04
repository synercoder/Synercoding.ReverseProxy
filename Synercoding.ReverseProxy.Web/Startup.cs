// Copyright (c) Gerard Gunnewijk. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Synercoding.ReverseProxy
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        private IConfiguration _configuration { get; }

        private IHostingEnvironment _hostingEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SharedProxyOptions>(_configuration.GetSection("Shared"));
            services.Configure<ProxyOptions>(_configuration.GetSection("LoadBalancers"));

            services.AddProxy(options =>
            {
                options.PrepareRequest = (originalRequest, message) =>
                {
                    message.Headers.Add("X-Forwarded-Host", originalRequest.Host.Host);
                    return Task.FromResult(0);
                };
            });
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.RunProxy();
        }
    }
}
