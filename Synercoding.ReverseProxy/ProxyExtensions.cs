// Copyright (c) .NET Foundation and Gerard Gunnewijk. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// 
// Changes:
// - Changed namespace to seperate this project from the original https://github.com/aspnet/Proxy
// - Added try catch statement to indicate 503 and 504 responses.

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Synercoding.ReverseProxy;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Builder
{
    public static class ProxyExtensions
    {
        /// <summary>
        /// Runs proxy forwarding requests to the server specified by options.
        /// </summary>
        /// <param name="app"></param>
        public static void RunProxy(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            app.UseMiddleware<ProxyMiddleware>();
        }

        /// <summary>
        /// Runs proxy forwarding requests to the server specified by options.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="options">Proxy options</param>
        public static void RunProxy(this IApplicationBuilder app, ProxyOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            app.UseMiddleware<ProxyMiddleware>(Options.Create(options));
        }

        /// <summary>
        /// Forwards current request to the specified destination uri.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destinationUri">Destination Uri</param>
        public static async Task ProxyRequest(this HttpContext context, Uri destinationUri)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (destinationUri == null)
            {
                throw new ArgumentNullException(nameof(destinationUri));
            }

            if (context.WebSockets.IsWebSocketRequest)
            {
                await context.AcceptProxyWebSocketRequest(destinationUri.ToWebSocketScheme());
            }
            else
            {
                var proxyService = context.RequestServices.GetRequiredService<ProxyService>();

                try
                {
                    using (var requestMessage = context.CreateProxyHttpRequest(destinationUri))
                    {
                        var prepareRequestHandler = proxyService.Options.PrepareRequest;
                        if (prepareRequestHandler != null)
                        {
                            await prepareRequestHandler(context.Request, requestMessage);
                        }

                        using (var responseMessage = await context.SendProxyHttpRequest(requestMessage))
                        {
                            await context.CopyProxyHttpResponse(responseMessage);
                        }
                    }
                }
                catch (HttpRequestException ex)
                    when ((ex.InnerException as System.Net.Sockets.SocketException)?.ErrorCode == 10061) // Connection actively refused
                {
                    context.Response.Clear();
                    context.Response.StatusCode = 503;
                }
                catch (TaskCanceledException)
                {
                    context.Response.Clear();
                    context.Response.StatusCode = 504;
                }
            }
        }
    }
}
