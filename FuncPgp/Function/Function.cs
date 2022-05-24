using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FuncPgp.Helper;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Text;
//using FluentValidation.Results;
using System.Text.RegularExpressions;
using System.Linq;
using System.Net.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace FuncPgp
{
    public static class Function
    {
        [FunctionName(nameof(PGPDecrypt))]
        public static async Task<IActionResult> PGPDecrypt(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
        HttpRequest req, ILogger log)
        {

            log.LogInformation($"C# HTTP trigger function {nameof(PGPDecrypt)} processed a request.");

            //string pass = req.Query["passPhrase"];
            string output = req.Query["outputPath"];
            string file = req.Query["filePath"];
            string kvsecpass = req.Query["kvsecpass"];
            string kvsecpriv = req.Query["kvsecpriv"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            file = file ?? data?.filePath;
            output = output ?? data?.outputPath;
            kvsecpass = kvsecpass ?? data?.kvsecpass;
            kvsecpriv = kvsecpriv ?? data?.kvsecpriv;

            var conn = Environment.GetEnvironmentVariable("AzureStagingStorage");

            var isSuccess = await SecurityHelper.DecryptAsyncKV(output, file, conn, kvsecpass, kvsecpriv);

            return (ActionResult)new OkObjectResult(Newtonsoft.Json.JsonConvert.SerializeObject(isSuccess));
        }

        [FunctionName(nameof(PGPEncrypt))]
        public static async Task<IActionResult> PGPEncrypt(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
        HttpRequest req, ILogger log)
        {

            log.LogInformation($"C# HTTP trigger function {nameof(PGPEncrypt)} processed a request.");

            //string pass = req.Query["passPhrase"];
            string output = req.Query["outputPath"];
            string file = req.Query["filePath"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            //pass = pass ?? data?.passPhrase;
            file = file ?? data?.filePath;
            output = output ?? data?.outputPath;

            var conn = Environment.GetEnvironmentVariable("AzureStagingStorage");

            var isSuccess = await SecurityHelper.EncryptAsync(output, file, conn);

            return (ActionResult)new OkObjectResult(Newtonsoft.Json.JsonConvert.SerializeObject(isSuccess));
        }

        [FunctionName(nameof(PGPGenKeys))]
        public static async Task<IActionResult> PGPGenKeys(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
        HttpRequest req, ILogger log)
        {

            log.LogInformation($"C# HTTP trigger function {nameof(PGPGenKeys)} processed a request.");

            //var email = Environment.GetEnvironmentVariable("EMAIL");
            //var passphrase = Environment.GetEnvironmentVariable("PASSPHRASE");

            string email = req.Query["email"];
            string pass = req.Query["passPhrase"];
            string output = req.Query["outputPath"];
            string kvsecpass = req.Query["kvsecpass"];
            string kvsecpriv = req.Query["kvsecpriv"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            email = email ?? data?.email;
            pass = pass ?? data?.passPhrase;
            output = output ?? data?.outputPath;
            kvsecpass = kvsecpass ?? data?.kvsecpass;
            kvsecpriv = kvsecpriv ?? data?.kvsecpriv;

            var conn = Environment.GetEnvironmentVariable("AzureStagingStorage");

            var isSuccess = await SecurityHelper.GenKeys(output, conn, email, pass, kvsecpass, kvsecpriv);

            return (ActionResult)new OkObjectResult(Newtonsoft.Json.JsonConvert.SerializeObject(isSuccess));
        }
     }
}