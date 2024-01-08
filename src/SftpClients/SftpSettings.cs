namespace IsolatedFunctionsConnectionClosed.SftpClients;

public record SftpSettings
{
    public const string Section = nameof(SftpSettings);
    public string Host { get; init; }
    public string Port { get; init; }
    public SupportedAuthMode AuthMode { get; init; }
    public string Username { get; init; }
    public string Password { get; init; }
    public string PrivateKeyPpkFormat { get; init; }
    public string PrivateKeyRsaFormat { get; init; }
    public string PrivateKeyPassPhrase { get; init; }
    public string HostKeyFingerprint { get; init; }
}

public enum SupportedAuthMode { Password, PrivateKey }
