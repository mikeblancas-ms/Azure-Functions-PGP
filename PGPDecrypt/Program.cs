using PgpCore;
using System;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace key_vault_console_app
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const string secretName = "mySecret";
            var keyVaultName = Environment.GetEnvironmentVariable("KEY_VAULT_NAME");
            var kvUri = $"https://{keyVaultName}.vault.azure.net";

            var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());

            Console.WriteLine("Generating public/private keypair.");

            PGP pgp = new PGP();

            Stream publicKey = new MemoryStream();
            Stream privateKey = new MemoryStream();

            var email = Environment.GetEnvironmentVariable("EMAIL");
            var passphrase = Environment.GetEnvironmentVariable("PASSPHRASE");

            pgp.GenerateKey(publicKey, privateKey, email, passphrase, 3072);
            Console.WriteLine("done!");

            //Stream pubKeySave = new FileStream(@"C:\Temp\public.asc", FileMode.Create, FileAccess.Write);
            StreamReader reader2 = new StreamReader(publicKey);
            publicKey.Position = 0;
            var publicValue = reader2.ReadToEnd();

            StreamReader reader = new StreamReader(privateKey);
            privateKey.Position = 0;
            var secretValue = reader.ReadToEnd();

            Console.WriteLine($"Creating a secret in {keyVaultName} called '{secretName}' with the value '{secretValue}' ...");
            await client.SetSecretAsync(secretName, secretValue);
            Console.WriteLine(" done.");

            Console.WriteLine("Forgetting your secret.");
            secretValue = string.Empty;
            Console.WriteLine($"Your secret is '{secretValue}'.");
            Console.WriteLine($"PublicKey\n\n{publicValue}");

            await File.WriteAllTextAsync(@"C:\Temp\public.asc", publicValue);

            //Console.WriteLine($"Retrieving your secret from {keyVaultName}.");
            //var secret = await client.GetSecretAsync(secretName);
            //Console.WriteLine($"Your secret is '{secret.Value.Value}'.");

            //Console.Write($"Deleting your secret from {keyVaultName} ...");
            //DeleteSecretOperation operation = await client.StartDeleteSecretAsync(secretName);
            // You only need to wait for completion if you want to purge or recover the secret.
            //await operation.WaitForCompletionAsync();
            //Console.WriteLine(" done.");

            //Console.Write($"Purging your secret from {keyVaultName} ...");
            //await client.PurgeDeletedSecretAsync(secretName);
            //Console.WriteLine(" done.");
        }
    }
}

// Load keys
//FileInfo privateKey = new FileInfo(@"C:\TEMP\private.asc");

// Reference input/output files
//FileInfo inputFile = new FileInfo(@"C:\TEMP\car1-cluster1.csv.gpg");
//FileInfo decryptedFile = new FileInfo(@"C:\TEMP\decrypted.csv");

// Decrypt
//await pgp.DecryptFileAsync(inputFile, decryptedFile, privateKey, "Password12345");