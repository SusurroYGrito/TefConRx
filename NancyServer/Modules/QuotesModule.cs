using System;
using System.Threading;
using Nancy;

namespace NancyServer
{
    public class QuotesModule : NancyModule
    {
        public QuotesModule():base("quotes")
        {
            Get["/{symbol}"] = parameters =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(5));
                return Response.AsText("105");
            };
        }
    }
}