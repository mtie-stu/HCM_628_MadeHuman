namespace MadeHuman_Server.Model
{
    public class OAuthDataItem
    {
        // Khóa lưu theo format: "{typeof(T).FullName}-{userKey}"
        public string Key { get; set; } = default!;
        public string Value { get; set; } = default!;    // JSON
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    }
}
