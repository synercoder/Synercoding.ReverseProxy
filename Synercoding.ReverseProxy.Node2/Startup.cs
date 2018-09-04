// Copyright (c) Gerard Gunnewijk. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Synercoding.ReverseProxy.Node2
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                var headers = context.Response.GetTypedHeaders();
                headers.ContentType = Microsoft.Net.Http.Headers.MediaTypeHeaderValue.Parse("text/html");

                await context.Response.WriteAsync("Hello World from node 2!");
            });
        }
    }
}
