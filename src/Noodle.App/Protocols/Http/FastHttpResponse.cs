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

    public static async Task<FastHttpResponse> FromStreamAsync(Stream stream, CancellationToken cancellationToken)
    {
        var response = new FastHttpResponse();

        await response.ReadAsync(stream, cancellationToken);

        return response;
    }

    private async Task ReadAsync(Stream stream, CancellationToken cancellationToken)
    {
        var reader = PipeReader.Create(stream, new StreamPipeReaderOptions());

        await ReadStatusAsync(reader, cancellationToken);
        await ReaderHeadersAsync(reader, cancellationToken);
        //await ReadToEndAsync(reader, cancellationToken);

        await reader.CompleteAsync();
    }

    private async Task ReadStatusAsync(PipeReader reader, CancellationToken cancellationToken)
    {
        var status = await ReadLineAsync(reader, cancellationToken);

        if (!string.IsNullOrEmpty(status))
        {
            var parts = status.Split(" ", 3);

            if (parts.Length == 3)
            {
                StatusCode = int.Parse(parts[1]);
                StatusMessage = parts[2];
            }
        }
    }

    private async Task ReaderHeadersAsync(PipeReader reader, CancellationToken cancellationToken)
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
    }

    private static async Task<string> ReadLineAsync(PipeReader reader, CancellationToken cancellationToken)
    {
        while (true)
        {
            var result = await reader.ReadAsync(cancellationToken);

            if (TryReadLine(result.Buffer, out var consumed, out var line))
            {
                reader.AdvanceTo(result.Buffer.GetPosition(consumed));

                return line;
            }

            if (result.IsCompleted)
            {
                return default;
            }
        }
    }

    private static bool TryReadLine(ReadOnlySequence<byte> buffer, out long consumed, out string line)
    {
        var newLine = new ReadOnlySpan<byte>(NewLine);
        var reader = new SequenceReader<byte>(buffer);

        if (reader.TryReadTo(out ReadOnlySequence<byte> subSequence, newLine))
        {
            line = Encoding.ASCII.GetString(subSequence);
            consumed = reader.Consumed;
            return true;
        }

        line = default;
        consumed = 0;
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