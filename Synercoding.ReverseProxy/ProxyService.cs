// Copyright (c) .NET Foundation and Gerard Gunnewijk. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// 
// Changes:
// - Changed namespace to seperate this project from the original https://github.com/aspnet/Proxy
// - Added support for timeout

using Microsoft.Extensions.Options;
using System;
using System.Net.Http;

namespace Synercoding.ReverseProxy
{
    public class ProxyService
    {
        public ProxyService(IOptions<SharedProxyOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            Options = options.Value;
            Client = new HttpClient(Options.MessageHandler ?? new HttpClientHandler { AllowAutoRedirect = false, UseCookies = false });

            if (Options.RequestTimeOut.HasValue)
                Client.Timeout = Options.RequestTimeOut.Value;
        }

        public SharedProxyOptions Options { get; private set; }
        internal HttpClient Client { get; private set; }
    }
}
