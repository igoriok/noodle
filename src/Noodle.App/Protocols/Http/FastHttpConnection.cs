using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using Pipelines.Sockets.Unofficial;

namespace Noodle.App.Protocols.Http;

public class FastHttpConnection : IDisposable
{
    private readonly Uri _url;
    private readonly EndPoint _endpoint;

    public FastHttpConnection(Uri url)
    {
        _url = url;
        _endpoint = _url.HostNameType == UriHostNameType.Dns
            ? new DnsEndPoint(_url.DnsSafeHost, _url.Port)
            : new IPEndPoint(IPAddress.Parse(_url.Host), _url.Port);
    }

    public async Task<FastHttpResponse> SendAsync(FastHttpRequest request, CancellationToken cancellationToken)
    {
        using var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

        await socket.ConnectAsync(_endpoint, cancellationToken);

        var response = await SendCoreAsync(request, socket, cancellationToken);

        await socket.DisconnectAsync(false, cancellationToken);

        return response;
    }

    public void Dispose()
    {
    }

    private async Task<FastHttpResponse> SendCoreAsync(FastHttpRequest request, Socket socket, CancellationToken cancellationToken)
    {
        await using var stream = await GetStream(socket, cancellationToken);

        var pipe = StreamConnection.GetDuplex(stream);

        await request.WriteAsync(pipe.Output, cancellationToken);

        return await FastHttpResponse.FromStreamAsync(pipe.Input, cancellationToken);
    }

    private async Task<Stream> GetStream(Socket socket, CancellationToken cancellationToken)
    {
        var stream = new NetworkStream(socket);

        if (string.Equals(_url.Scheme, "https", StringComparison.InvariantCultureIgnoreCase))
        {
            var sslStream = new SslStream(stream, false, UserCertificateValidationCallback);

            await sslStream.AuthenticateAsClientAsync(_url.Host);

            return sslStream;
        }

        return stream;
    }

    private bool UserCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
    {
        return true;
    }
}