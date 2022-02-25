using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantPOS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .ConfigureWebHostDefaults(webBuilder =>
        //        {
        //            webBuilder.UseStartup<Startup>();
        //        });


        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.Configure<KestrelServerOptions>(context.Configuration.GetSection("Kestrel"));
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureKestrel(serverOptions =>
                {
                    serverOptions.AddServerHeader = true;
                    //The maximum number of concurrent open TCP connections can be set for the entire app with the following code
                    serverOptions.Limits.MaxConcurrentConnections = 100;
                    //There's a separate limit for connections that have been upgraded from HTTP or HTTPS to another protocol (for example, on a WebSockets request). After a connection is upgraded, it isn't counted against the MaxConcurrentConnections limit.
                    serverOptions.Limits.MaxConcurrentUpgradedConnections = 100;
                    //The default maximum request body size is 30,000,000 bytes, which is approximately 28.6 MB.
                    serverOptions.Limits.MaxRequestBodySize = 10 * 1024;
                    //Kestrel checks every second if data is arriving at the specified rate in bytes/second. If the rate drops below the minimum, the connection is timed out. The grace period is the amount of time that Kestrel gives the client to increase its send rate up to the minimum; the rate isn't checked during that time. The grace period helps avoid dropping connections that are initially sending data at a slow rate due to TCP slow-start.
                    serverOptions.Limits.MinRequestBodyDataRate = new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                    serverOptions.Limits.MinResponseDataRate = new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                    //In case of IP whitelisting
                    //serverOptions.Listen(IPAddress.Loopback, 5000);
                    //serverOptions.Listen(IPAddress.Loopback, 5001, listenOptions =>
                    //{
                    //    listenOptions.UseHttps("testCert.pfx",
                    //        "testPassword");
                    //});
                    //Gets or sets the keep-alive timeout. Defaults to 2 minutes.
                    serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(1);
                    serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(1);
                    //Http2.MaxStreamsPerConnection limits the number of concurrent request streams per HTTP/2 connection. Excess streams are refused. The default value is 100.
                    serverOptions.Limits.Http2.MaxStreamsPerConnection = 100;
                    //The HPACK decoder decompresses HTTP headers for HTTP/2 connections. Http2.HeaderTableSize limits the size of the header compression table that the HPACK decoder uses. The value is provided in octets and must be greater than zero (0).
                    serverOptions.Limits.Http2.HeaderTableSize = 4096;
                    //Http2.MaxFrameSize indicates the maximum allowed size of an HTTP/2 connection frame payload received or sent by the server. The value is provided in octets and must be between 2^14 (16,384) and 2^24-1 (16,777,215). The default value is 2^14 (16,384).
                    serverOptions.Limits.Http2.MaxFrameSize = 16384;
                    //Http2.MaxRequestHeaderFieldSize indicates the maximum allowed size in octets of request header values. This limit applies to both name and value in their compressed and uncompressed representations. The value must be greater than zero (0). The default value is 8,192.
                    serverOptions.Limits.Http2.MaxRequestHeaderFieldSize = 8192;
                    //Http2.InitialConnectionWindowSize indicates the maximum request body data in bytes the server buffers at one time aggregated across all requests (streams) per connection. Requests are also limited by Http2.InitialStreamWindowSize. The value must be greater than or equal to 65,535 and less than 2^31 (2,147,483,648). The default value is 128 KB (131,072).
                    serverOptions.Limits.Http2.InitialConnectionWindowSize = 131072;
                    //Http2.InitialStreamWindowSize indicates the maximum request body data in bytes the server buffers at one time per request (stream). Requests are also limited by Http2.InitialConnectionWindowSize. The value must be greater than or equal to 65,535 and less than 2^31 (2,147,483,648). The default value is 96 KB (98,304).
                    serverOptions.Limits.Http2.InitialStreamWindowSize = 98304;
                    //AllowSynchronousIO controls whether synchronous IO is allowed for the request and response. The default value is false.
                    serverOptions.AllowSynchronousIO = false;
                });
                webBuilder.UseStartup<Startup>();
            });
    }
}