using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plumsail.DataSource
{
    class DebugRequestHandler : DelegatingHandler
    {
        public DebugRequestHandler(HttpMessageHandler? innerHandler = null)
        {
            InnerHandler = innerHandler ?? new HttpClientHandler();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Console.WriteLine("");
            Console.WriteLine(string.Format("Request: {0} {1}", request.Method, request.RequestUri));
            Console.WriteLine("Request headers:");
            foreach (var header in request.Headers)
            {
                Console.WriteLine(string.Format("{0}: {1}", header.Key, string.Join(',', header.Value)));
            }
            if (request.Content is not null)
            {
                Console.WriteLine("");
                Console.WriteLine("Request body:");
                var body = await request.Content.ReadAsStringAsync();
                Console.WriteLine(body);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
