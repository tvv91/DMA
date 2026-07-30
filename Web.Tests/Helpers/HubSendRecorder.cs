namespace Web.Tests.Helpers;

internal sealed class HubSendRecorder
{
    public List<(string Method, object?[] Args)> Sends { get; } = [];

    public Task RecordSendAsync(string method, object?[] args, CancellationToken _)
    {
        Sends.Add((method, args));
        return Task.CompletedTask;
    }

    public (string Method, object?[] Args)? FindSend(string method) =>
        Sends.FirstOrDefault(s => s.Method == method);

    public IReadOnlyList<(string Method, object?[] Args)> FindSends(string method) =>
        Sends.Where(s => s.Method == method).ToList();
}
