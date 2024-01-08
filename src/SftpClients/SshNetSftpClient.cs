using System.Text;
using Microsoft.Extensions.Options;
using Renci.SshNet;

namespace IsolatedFunctionsConnectionClosed.SftpClients;

public class SshNetSftp(IOptions<SftpSettings> options)
{
    private readonly SftpSettings _sftpSettings = options.Value;

    /// <remarks>
    /// The SftpClient instance returned is not thread safe and must be disposed of by the caller.
    /// </remarks>
    private SftpClient GetSftpClient()
    {
        SftpClient sftpClient;
        switch (_sftpSettings.AuthMode)
        {
            case SupportedAuthMode.PrivateKey:
            {
                using var privateKeyStream = new MemoryStream(Encoding.ASCII.GetBytes(_sftpSettings.PrivateKeyRsaFormat));
                IPrivateKeySource[]? privateKey;
                if (string.IsNullOrEmpty(_sftpSettings.PrivateKeyPassPhrase))
                    privateKey = [new PrivateKeyFile(privateKeyStream)];
                else
                    privateKey = [new PrivateKeyFile(privateKeyStream, _sftpSettings.PrivateKeyPassPhrase)];

                sftpClient = new SftpClient(_sftpSettings.Host, int.Parse(_sftpSettings.Port), _sftpSettings.Username, privateKey);
                break;
            }

            case SupportedAuthMode.Password:
                sftpClient = new SftpClient(_sftpSettings.Host, int.Parse(_sftpSettings.Port), _sftpSettings.Username, _sftpSettings.Password);
                break;

            default:
                throw new InvalidOperationException("Invalid authentication mode specified, supported modes are: Password, PrivateKey");
        }

        sftpClient.Connect();
        return sftpClient;
    }

    public IEnumerable<string> ListFiles(string directoryPath)
    {
        using var sftpClient = GetSftpClient();
        
        return sftpClient.ListDirectory(directoryPath)
            .Where(sftpFile => !sftpFile.IsDirectory)
            .Select(sftpFile => sftpFile.Name);
    }
}
