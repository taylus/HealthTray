using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HealthTray.Service.Test
{
    /// <summary>
    /// A test stand-in for <see cref="HttpClientHandler"/>, the default message
    /// handler which normally sends requests out to the network. <para/>
    /// 
    /// This handler maintains a queue of responses and sends them back in order.
    /// It is intended to be used in unit tests, where we don't want to hit actual
    /// services over HTTP.
    /// </summary>
    public class StubHttpClientHandler : DelegatingHandler
    {
        /// <summary>
        /// Internal queue of responses returned by this handler.
        /// </summary>
        private readonly Queue<HttpResponseMessage> responses = new Queue<HttpResponseMessage>();

        /// <summary>
        /// Queues up the given response to be returned from this handler's SendAsync.
        /// </summary>
        public void EnqueueResponse(HttpResponseMessage response)
        {
            responses.Enqueue(response);
        }

        /// <summary>
        /// Processes the given request through the message handler pipeline.
        /// </summary>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (responses.Count == 0) throw new InvalidOperationException("No responses are queued for this request.");
            return Task.FromResult(responses.Dequeue());
        }
    }
}
