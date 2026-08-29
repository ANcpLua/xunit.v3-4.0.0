using Xunit.Sdk;
using Xunit.v3;

namespace xunitv3.template.Examples.Retry;

/// <summary>
/// Buffers every message of one test attempt. Disposing forwards the buffer to the real bus;
/// discarding without disposal drops the attempt (which is how a failed attempt stays invisible).
/// </summary>
public sealed class DelayedMessageBus(IMessageBus innerBus) : IMessageBus
{
    private readonly List<IMessageSinkMessage> _messages = [];

    public bool QueueMessage(IMessageSinkMessage message)
    {
        lock (_messages)
        {
            _messages.Add(message);
        }

        // The inner bus cannot be asked whether it wants to cancel without sending it the message.
        return true;
    }

    public void Dispose()
    {
        foreach (var message in _messages)
        {
            innerBus.QueueMessage(message);
        }
    }
}
