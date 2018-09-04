// Copyright (c) .NET Foundation and Gerard Gunnewijk. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// 
// Changes:
// - Changed namespace to seperate this project from the original https://github.com/aspnet/Proxy
// - Added logging
// - Added support for multiple servers and upstream servers

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Synercoding.ReverseProxy
{
    public class ProxyMiddleware
    {
        private const int DefaultWebSocketBufferSize = 4096;

        private readonly RequestDelegate _next;
        private readonly ProxyOptions _options;
        private readonly ILogger _logger;

        private static readonly string[] NotForwardedWebSocketHeaders = new[] { "Connection", "Host", "Upgrade", "Sec-WebSocket-Key", "Sec-WebSocket-Version" };

        public ProxyMiddleware(RequestDelegate next, IOptions<ProxyOptions> options, ILogger<ProxyMiddleware> logger)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (options.Value.Servers.Any(s => s.Host == null))
            {
                throw new ArgumentException("Options servers must all specify hosts.", nameof(options));
            }
            if (options.Value.Servers.Any(s => !s.UpstreamServers.Any()))
            {
                throw new ArgumentException("Options server must have atleast one upstream server.", nameof(options));
            }
            if (options.Value.Servers.Any(s => s.UpstreamServers.Any(u => u.Scheme == null)))
            {
                throw new ArgumentException("Options server upstream servers must all specify schemes.", nameof(options));
            }
            if (options.Value.Servers.Any(s => s.UpstreamServers.Any(u => u.Host == null)))
            {
                throw new ArgumentException("Options server upstream servers must all specify hosts.", nameof(options));
            }
            if(logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _next = next;
            _options = options.Value;
            _logger = logger;
        }

        public Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var server = _options.Servers.FirstOrDefault(s => context.Request.Host.Host == s.Host && (s.Port.HasValue ? context.Request.Host.Port == s.Port : true));
            if (server == null)
            {
                _logger.LogDebug($"No server found with host {context.Request.Host.Host} and port {context.Request.Host.Port}");
                return _next(context);
            }

            _logger.LogDebug($"Found server: {server.Host}{(server.Port.HasValue ? (":" + server.Port) : "")} with {server.UpstreamServers.Count()} upstream servers.");

            var selectedServer = RandomGen.Next(server.UpstreamServers.Length);
            var upstreamServer = server.UpstreamServers.ElementAt(selectedServer);
            _logger.LogDebug($"Selected upstream server: {selectedServer}");

            var builder = new UriBuilder(context.Request.GetEncodedUrl());
            builder.Scheme = upstreamServer.Scheme;
            builder.Host = upstreamServer.Host;
            builder.Port = upstreamServer.Port ?? (upstreamServer.Scheme.Contains("https") ? 443 : 80);

            _logger.LogDebug($"Forwarded request to {builder.Uri.ToString()}");
            return context.ProxyRequest(builder.Uri);
        }
    }
}
