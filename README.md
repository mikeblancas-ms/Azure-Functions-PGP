# Azure Functions PgpCore
Azure Functions C# sample for PGP encrypt and decrypt.
This is based on code from [ikaur3009](https://github.com/ikaur3009) and [PgPCore](https://github.com/mattosaurus/PgpCore) library.
## Usage
### Keypair issues
*BouncyCastle unknown packet type encountered: 20* when decrypting using exported private key from GnuPG/Gpg4win. Generate Keypair using this library, import public key to GPG/Gpg4win keyring of source server, and configure private key in Function app. Here is [sample](PGPKeyGen) C# console app for generating keypair.
### Environment Variables
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobs":"DefaultEndpointsProtocol=https;AccountName=myAccountName;AccountKey=myAccountKey",
    "PGP_PrivateKey":"container/folder/secret.asc",
    "PGP_PublicKey":"container/folder/public.asc"
  }
}
```
### Invoke
Encrypt
```
POST http://localhost:7071/api/PGPEncrypt?
  filePath=container/folder/filename
  outputPath=container/folder/filename
```
Decrypt
```
POST http://localhost:7071/api/PGPDecrypt?
  filePath=container/folder/filename
  outputPath=container/folder/filename
  passPhrase=secretpassphrase
```
### Publish
[Publish to Azure](https://docs.microsoft.com/en-us/azure/azure-functions/functions-develop-vs?tabs=in-process#publish-to-azure)
