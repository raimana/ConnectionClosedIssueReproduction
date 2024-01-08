using Microsoft.Extensions.Options;
using WinSCP;

namespace IsolatedFunctionsConnectionClosed.SftpClients;

public class WinScpSftp(IOptions<SftpSettings> options)
{
    private readonly SftpSettings _sftpSettings = options.Value;
    
    /// <remarks>
    /// The Session instance returned is not thread safe and must be disposed of by the caller.
    /// </remarks>
    private Session GetSftpClient()
    {
        var sessionOptions = new SessionOptions
        {
            Protocol = Protocol.Sftp,
            HostName = _sftpSettings.Host,
            UserName = _sftpSettings.Username,
            PortNumber = int.Parse(_sftpSettings.Port),

            SshHostKeyFingerprint = string.IsNullOrWhiteSpace(_sftpSettings.HostKeyFingerprint) ? null : _sftpSettings.HostKeyFingerprint,

            SshHostKeyPolicy = string.IsNullOrWhiteSpace(_sftpSettings.HostKeyFingerprint) ? SshHostKeyPolicy.GiveUpSecurityAndAcceptAny : SshHostKeyPolicy.Check
        };

        switch (_sftpSettings.AuthMode)
        {
            case SupportedAuthMode.PrivateKey:
                sessionOptions.SshPrivateKey = _sftpSettings.PrivateKeyPpkFormat;
                if (!string.IsNullOrEmpty(_sftpSettings.PrivateKeyPassPhrase)) 
                    sessionOptions.PrivateKeyPassphrase = _sftpSettings.PrivateKeyPassPhrase;
                break;

            case SupportedAuthMode.Password:
                sessionOptions.Password = _sftpSettings.Password;
                break;

            default:
                throw new InvalidOperationException($"Invalid authentication mode specified, supported modes are: {string.Join(", ", Enum.GetValues<SupportedAuthMode>())}");
        }

        var session = new Session();
        session.Open(sessionOptions);

        return session;
    }
    
    public IEnumerable<string> ListFiles(string directoryPath)
    {
        using var session = GetSftpClient();

        return session.ListDirectory(directoryPath).Files
            .Where(sftpFile => !sftpFile.IsDirectory)
            .Select(sftpFile => sftpFile.Name);
    }
}