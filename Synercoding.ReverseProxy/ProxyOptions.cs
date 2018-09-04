// Copyright (c) .NET Foundation and Gerard Gunnewijk. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// 
// Changes:
// - Changed namespace to seperate this project from the original https://github.com/aspnet/Proxy
// - Moved proxy options into upstreamserver and added loadbalancing server

using Microsoft.AspNetCore.Http;

namespace Synercoding.ReverseProxy
{
    public class ProxyOptions
    {
        public LoadbalancingServer[] Servers { get; set; }
    }

    public class LoadbalancingServer
    {
        /// <summary>
        /// Incoming uri host
        /// </summary>
        public string Host { get; set; }
        public int? Port { get; set; }

        public UpstreamServer[] UpstreamServers { get; set; }
    }

    public class UpstreamServer
    {
        /// <summary>
        /// Destination uri scheme
        /// </summary>
        public string Scheme { get; set; }

        /// <summary>
        /// Destination uri host
        /// </summary>
        public string Host { get; set; }

        public int? Port { get; set; }

        /// <summary>
        /// Destination uri path base to which current Path will be appended
        /// </summary>
        public PathString PathBase { get; set; }

        /// <summary>
        /// Query string parameters to append to each request
        /// </summary>
        public QueryString AppendQuery { get; set; }
    }
}
