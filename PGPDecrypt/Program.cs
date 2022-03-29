using PgpCore;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Generating public and private keys... ");

PGP pgp = new PGP();

// Generate keys
pgp.GenerateKey(@"C:\TEMP\dev_public.asc", @"C:\TEMP\dev_private.asc", "viblanca@microsoft.com", "Password12345", 3072);

Console.WriteLine("done!");

// Load keys
//FileInfo privateKey = new FileInfo(@"C:\TEMP\private.asc");

// Reference input/output files
//FileInfo inputFile = new FileInfo(@"C:\TEMP\car1-cluster1.csv.gpg");
//FileInfo decryptedFile = new FileInfo(@"C:\TEMP\decrypted.csv");

// Decrypt
//await pgp.DecryptFileAsync(inputFile, decryptedFile, privateKey, "Password12345");