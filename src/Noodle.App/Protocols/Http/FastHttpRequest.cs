using System.Text;

namespace Noodle.App.Protocols.Http;

public class FastHttpRequest
{
    public Uri Url { get; }

    public string Method { get; }

    public string UserAgent { get; set; }

    public FastHttpRequest(Uri url, string method)
    {
        Url = url;
        Method = method;
    }

    public async Task WriteAsync(Stream stream, CancellationToken cancellationToken)
    {
        var request = new StringBuilder()
            .Append($"{Method} {Url.PathAndQuery} HTTP/1.1").Append("\r\n")
            .Append($"Host: {Url.Host}").Append("\r\n")
            .Append($"User-Agent: {UserAgent}").Append("\r\n")
            //.Append("Connection: Keep-Alive").Append("\r\n")
            .Append("\r\n")
            .ToString();

        await stream.WriteAsync(Encoding.UTF8.GetBytes(request), cancellationToken);
        await stream.FlushAsync(cancellationToken);
    }
}