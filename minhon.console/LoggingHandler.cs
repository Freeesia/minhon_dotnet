internal class LoggingHandler(HttpMessageHandler innerHandler) : DelegatingHandler(innerHandler)
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Request: {request.Method} {request.RequestUri}");
        if (request.Content != null)
        {
            Console.WriteLine(await request.Content.ReadAsStringAsync(cancellationToken));
        }

        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        Console.WriteLine($"Response: {response.StatusCode} {response.ReasonPhrase}");
        if (response.Content != null)
        {
            Console.WriteLine(await response.Content.ReadAsStringAsync(cancellationToken));
        }

        return response;
    }
}
