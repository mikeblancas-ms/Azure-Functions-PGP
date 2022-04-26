# Azure Functions PgpCore
Azure Functions C# sample for PGP encrypt and decrypt.
This is based on code from [ikaur3009](https://github.com/ikaur3009) and [PgpCore](https://github.com/mattosaurus/PgpCore) library.
## Azure Key Vault
Added Azure Key Vault integration 2022-04-26
## Usage
### Keypair issues
*BouncyCastle unknown packet type encountered: 20* when decrypting using exported private key from GnuPG/Gpg4win. Generate Keypair using this library, import public key to GPG/Gpg4win keyring of source server, and configure private key in Function app.
### Environment Variables
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobs":"DefaultEndpointsProtocol=https;AccountName=myAccountName;AccountKey=myAccountKey",
    "KEY_VAULT_NAME":"kvURI",
    "KVSecretName": "secretName",
    "PGP_PublicKey":"container/folder/publickey.asc"
  }
}
```
### Invoke
GenKeys
```
POST http://localhost:7071/api/PGPGenKeys?
{
  "outputPath":"container/folder/publickey.asc",
  "email":"email@address.com",
  "passPhrase":"secretpassphrase"
}
```
Encrypt
```
POST http://localhost:7071/api/PGPEncrypt?
{
  "filePath":"container/folder/inputfilename",
  "outputPath":"container/folder/outputfilename"
}
```
Decrypt
```
POST http://localhost:7071/api/PGPDecrypt?
{
  "filePath":"container/folder/inputfilename",
  "outputPath":"container/folder/outputfilename",
  "passPhrase":"secretpassphrase"
}  
```
### Publish
[Publish to Azure](https://docs.microsoft.com/en-us/azure/azure-functions/functions-develop-vs?tabs=in-process#publish-to-azure)
