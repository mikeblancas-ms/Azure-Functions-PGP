# Azure Functions PgpCore
Azure Functions C# sample for PGP encrypt and decrypt.
This is based on code from [ikaur3009](https://github.com/ikaur3009) and [PgPCore](https://github.com/mattosaurus/PgpCore) library.
## Usage
### Keypair
Use DSA + ElGamal keypair
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
  passPhrase=secretpassphrase
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
