# Synercoding.ReverseProxy

This project is based upon https://github.com/aspnet/Proxy.

With Microsoft.AspNetCore.Proxy you could proxy a request to one single server.

This project is a small rewrite of Microsoft.AspNetCore.Proxy to enable multiple servers and multiple upstream servers.
The sample project has two Hello World nodes.

You can configure the loadbalancer using the `appsettings.json`:
<pre><code>{
  "Shared": {
    "RequestTimeOut": "00:00:30.000"
  },
  "LoadBalancers": {
    "Servers": [
      {
        "Host": "localhost",
        "Port": 11000,
        "UpstreamServers": [
          {
            "Host": "localhost",
            "Port": 11001,
            "Scheme": "http"
          },
          {
            "Host": "localhost",
            "Port": 11002,
            "Scheme": "http"
          }
        ]
      }
    ]
  }</code></pre>
  
  ## Example usage
  I found it difficult to work with Azure Functions on a local staging server with domains and certificates. Thus I used this project in IIS to setup a proxy to the Azure Functions using the following config (and yes I know this could have been done with the original Microsoft.AspNetCore.Proxy as well):
  <pre><code>
  "LoadBalancers": {
    "Servers": [
      {
        "Host": "functions-staging.example.com",
        "UpstreamServers": [
          {
            "Host": "127.0.0.1",
            "Port": 7071,
            "Scheme": "http"
          }
        ]
      }
    ]
  }</code></pre>
  
  
  ## Licensing
  I hope I did the licensing correctly. Original code was found on https://github.com/aspnet/Proxy under Apache License, Version 2.0
  I left the license remark on all the original files I used, added myself in the copyright notice and I stated my changes to those files. If I did anything in error regarding the license (or any other part), please let me know.
