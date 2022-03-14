using System.Buffers;
using System.IO.Pipelines;
using System.Text;

namespace Noodle.App.Protocols.Http;

public class FastHttpResponse
{
    private static readonly byte[] NewLine = Encoding.ASCII.GetBytes("\r\n");

    public int StatusCode { get; private set; }

    public string StatusMessage { get; private set; }

    public IReadOnlyDictionary<string, string> Headers { get; private set; }

    public static async Task<FastHttpResponse> FromStreamAsync(PipeReader reader, CancellationToken cancellationToken)
    {
        var response = new FastHttpResponse();

        await response.ReadAsync(reader, cancellationToken);

        return response;
    }

    private async Task ReadAsync(PipeReader reader, CancellationToken cancellationToken)
    {
        if (await ReadStatusAsync(reader, cancellationToken))
            await ReaderHeadersAsync(reader, cancellationToken);
        //await ReadToEndAsync(reader, cancellationToken);

        await reader.CompleteAsync();
    }

    private async Task<bool> ReadStatusAsync(PipeReader reader, CancellationToken cancellationToken)
    {
        var status = await ReadLineAsync(reader, cancellationToken);
        var parts = status?.Split(" ", 3);

        if (parts?.Length == 3 && int.TryParse(parts[1], out var code))
        {
            StatusCode = code;
            StatusMessage = parts[2];

            return true;
        }

        return false;
    }

    private async Task<bool> ReaderHeadersAsync(PipeReader reader, CancellationToken cancellationToken)
    {
        var headers = new Dictionary<string, string>();

        while (true)
        {
            var line = await ReadLineAsync(reader, cancellationToken);

            if (string.IsNullOrEmpty(line)) break;

            var parts = line.Split(": ", 2);

            headers[parts[0]] = parts[1];
        }

        Headers = headers;

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
        var newLine = new ReadOnlySpan<byte>(NewLine);
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

    private static async Task ReadToEndAsync(PipeReader reader, CancellationToken cancellationToken)
    {
        while (true)
        {
            var result = await reader.ReadAsync(cancellationToken);

            reader.AdvanceTo(result.Buffer.End);

            if (result.IsCompleted || result.IsCanceled)
                break;
        }
    }
}