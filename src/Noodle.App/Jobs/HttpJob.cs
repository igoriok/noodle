using System.Buffers;
using System.IO.Pipelines;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Noodle.App.Common;
using Noodle.App.Settings;
using Noodle.App.Stages;

namespace Noodle.App.Jobs;

public class HttpJob : PipeJob, IJob
{
    private static readonly string NewLine = "\r\n";
    private static readonly byte[] NewLineBytes = Encoding.ASCII.GetBytes(NewLine);

    private readonly HttpSettings _settings;

    public string Name => "HTTP";
    public string Description => _settings.Url.ToString();

    public IEnumerable<IStage> Pipeline
    {
        get
        {
            if (_settings.Concurrency.HasValue)
                yield return new ConcurrentStage(_settings.Concurrency.Value);

            yield return new RepeatStage();

            if (_settings.Throttle.HasValue)
                yield return new ThrottleStage(_settings.Throttle.Value);

            if (_settings.Timeout.HasValue)
                yield return new TimeoutStage(_settings.Timeout.Value);
        }
    }

    protected override string Host => _settings.Url.Host;
    protected override int Port => _settings.Url.Port;
    protected override string[] IpAddresses => _settings.IpAddresses;

    public HttpJob(HttpSettings settings)
    {
        _settings = settings;
    }

    protected override async Task<Stream> GetStreamAsync(Socket socket, CancellationToken cancellationToken)
    {
        var stream = await base.GetStreamAsync(socket, cancellationToken);

        if (string.Equals(_settings.Url.Scheme, "https", StringComparison.InvariantCultureIgnoreCase))
        {
            var ssl = new SslStream(stream, false, UserCertificateValidationCallback);

            await ssl.AuthenticateAsClientAsync(
                new SslClientAuthenticationOptions
                {
                    TargetHost = _settings.Url.Host,
                },
                cancellationToken);

            return ssl;
        }

        return stream;
    }

    protected override async Task<string> RunAsync(IDuplexPipe pipe, CancellationToken cancellationToken)
    {
        await WriteRequestAsync(pipe.Output, cancellationToken);

        var response = await ReadResponseAsync(pipe.Input, cancellationToken);

        return $"{response.StatusMessage} ({response.StatusCode})";
    }

    private async Task WriteRequestAsync(PipeWriter writer, CancellationToken cancellationToken)
    {
        var request = new StringBuilder()
            .Append($"{_settings.Method} {_settings.Url.PathAndQuery} HTTP/1.1").Append(NewLine)
            .Append($"Host: {_settings.Url.Host}").Append(NewLine)
            .Append($"User-Agent: {_settings.UserAgent}").Append(NewLine)
            // .Append("Connection: Keep-Alive").Append(NewLine)
            // .Append("Accept-Encoding: gzip, deflate").Append(NewLine)
            // .Append("Accept-Language: en-US,en;q=0.9").Append(NewLine)
            // .Append("Cache-Control: max-age=0").Append(NewLine)
            .Append(NewLine);

        await writer.WriteAsync(Encoding.ASCII.GetBytes(request.ToString()), cancellationToken);
    }

    private async Task<HttpResponse> ReadResponseAsync(PipeReader reader, CancellationToken cancellationToken)
    {
        var response = new HttpResponse();

        if (await ReadStatusAsync(response, reader, cancellationToken))
            await ReaderHeadersAsync(response, reader, cancellationToken);

        return response;
    }

    private static async Task<bool> ReadStatusAsync(HttpResponse response, PipeReader reader, CancellationToken cancellationToken)
    {
        var status = await ReadLineAsync(reader, cancellationToken);
        var parts = status?.Split(" ", 3);

        if (parts?.Length == 3 && int.TryParse(parts[1], out var code))
        {
            response.StatusCode = code;
            response.StatusMessage = parts[2];

            return true;
        }

        return false;
    }

    private static async Task<bool> ReaderHeadersAsync(HttpResponse response, PipeReader reader, CancellationToken cancellationToken)
    {
        var headers = new Dictionary<string, string>();

        while (!cancellationToken.IsCancellationRequested)
        {
            var line = await ReadLineAsync(reader, cancellationToken);

            if (string.IsNullOrEmpty(line)) break;

            var parts = line.Split(": ", 2);

            headers[parts[0]] = parts[1];
        }

        response.Headers = headers;

        return headers.Count > 0;
    }

    private static async Task<string> ReadLineAsync(PipeReader reader, CancellationToken cancellationToken)
    {
        while (true)
        {
            var result = await reader.ReadAsync(cancellationToken);

            if (TryReadLine(result.Buffer, out var position, out var line))
            {
                reader.AdvanceTo(position);

                return line;
            }

            if (result.IsCompleted)
            {
                return default;
            }
        }
    }

    private static bool TryReadLine(ReadOnlySequence<byte> buffer, out SequencePosition position, out string line)
    {
        var newLine = new ReadOnlySpan<byte>(NewLineBytes);
        var reader = new SequenceReader<byte>(buffer);

        if (reader.TryReadTo(out ReadOnlySequence<byte> subSequence, newLine))
        {
            line = Encoding.ASCII.GetString(subSequence);
            position = reader.Position;
            return true;
        }

        line = default;
        position = default;
        return false;
    }

    private static bool UserCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
    {
        return true;
    }

    private class HttpResponse
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public IReadOnlyDictionary<string, string> Headers { get; set; }
    }
}