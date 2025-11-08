using WeatherApp.Domain.Enums;

namespace WeatherApp.Domain.Entities;

public class Alert
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public AlertType Type { get; private set; }
    public string Title { get; private set; }
    public string Message { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ExpiresAt { get; private set; }

    public Alert(AlertType type, string title, string message, DateTimeOffset? expiresAt)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message is required.", nameof(message));

        Type = type;
        Title = title.Trim();
        Message = message.Trim();
        ExpiresAt = expiresAt;
    }

    private Alert()
    {
        Title = string.Empty;
        Message = string.Empty;
    }

    public void Update(AlertType type, string title, string message, DateTimeOffset? expiresAt)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message is required.", nameof(message));

        Type = type;
        Title = title.Trim();
        Message = message.Trim();
        ExpiresAt = expiresAt;
    }
}
