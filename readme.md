### How to Run Locally
Add the following `local.settings.json` substituting environment variables with the appropriate values:

```json
{
    "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",

        // Cron expressions for WinSCP & SSH.NET Orchestrator time trigger
        "WinScpSchedule": "0 0 0 * * *",
        "SshNetSchedule": "0 0 0 * * *",

        // Max number of iterations, connecting and listing files from the SFTP server
        "MaxIterations": 300,

        // The SFTP directory to list files from
        "SftpDirectory": "/home/sftp-lab",

        "SftpSettings:Host": "",
        "SftpSettings:Port": 22,
        "SftpSettings:Username": "",
        "SftpSettings:Password": "", // Only required if using Password authentication mode
        "SftpSettings:AuthMode": "PrivateKey", // Valid values: "PrivateKey", "Password"
        "SftpSettings:PrivateKeyPpkFormat": "", // WinSCP uses the PPK format 
        "SftpSettings:PrivateKeyRsaFormat": "", // SshNet uses the RSA Classic format, does NOT support the new Open SSH format
        "SftpSettings:PrivateKeyPassPhrase": "" // Only required if the private key is encrypted
    }
}
```

----

### Azure Deployment (if using provided ARM template)
#### Prerequisites
* Key Vault Instance
* App Insights Instance

#### Key Vault Secrets
* SftpTestPrivateKeyPpkFormat
* SftpTestPrivateKeyRsaFormat
* SftpTestPrivateKeyPassPhrase

#### ARM Template Parameters
See parameters in `./ArmTemplates/azuredeploy.parameters.json`

----

### How to Trigger the Orchestrators Manually
#### SSH.NET Orchestrator
```bash
# Azure
curl --location 'https://{INSERT FUNCTION NAME HERE}.azurewebsites.net/admin/functions/SshNetTimeTrigger' \
--header 'x-functions-key: {INSERT FUNCTION KEY HERE}' \
--header 'Content-Type: application/json' \
--data '{ "input": ""}'

# Local
curl --location 'http://localhost:7071/admin/functions/SshNetTimeTrigger' \
--header 'Content-Type: application/json' \
--data '{ "input": ""}'
```

#### WinSCP Orchestrator
```bash
# Azure
curl --location 'https://{INSERT FUNCTION NAME HERE}.azurewebsites.net/admin/functions/WinScpTimeTrigger' \
--header 'x-functions-key: {INSERT FUNCTION KEY HERE}' \
--header 'Content-Type: application/json' \
--data '{ "input": ""}'

# Local
curl --location 'http://localhost:7071/admin/functions/WinScpTimeTrigger' \
--header 'Content-Type: application/json' \
--data '{ "input": ""}'
```