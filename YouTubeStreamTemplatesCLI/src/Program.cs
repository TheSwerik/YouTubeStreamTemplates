using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;

namespace YouTubeStreamTemplatesCLI
{
    internal static class Program
    {
        private const string allowedChars = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789.-~";

        internal static async Task Main(string[] args)
        {
            await using var stream = new FileStream(@"D:\Projects\YouTubeStreamTemplates\client_id.json", FileMode.Open,
                                                    FileAccess.Read);
            var secrets = GoogleClientSecrets.Load(stream).Secrets;

            //Step 1:
            Console.WriteLine("Generating Code Verifier...");
            var codeVerifier = GenerateRandomString(new Random().Next(43, 129));
            var asciiBytes = Encoding.ASCII.GetBytes(codeVerifier);
            Console.WriteLine("Generated Code Verifier:\t\t" + Encoding.ASCII.GetString(asciiBytes));
            var hashedCode = SHA256.HashData(asciiBytes);
            var hashedString = hashedCode.Aggregate(string.Empty, (current, x) => current + $"{x:x2}");
            Console.WriteLine("Hashed Code Verifier:\t\t\t" + hashedString);
            var base64Code = Convert.ToBase64String(Encoding.UTF8.GetBytes(hashedString));
            Console.WriteLine("Base64 encoded hashed Code Verifier:\t" + base64Code);

            //Step 2-4:
            const string? url = "http://127.0.0.1:9004/";
            Console.WriteLine($"\nStart Listening on {url}...");
            var listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();

            var requestUrl = @"https://accounts.google.com/o/oauth2/v2/auth?" +
                             @"scope=https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fyoutube.readonly&" +
                             @"response_type=code&" +
                             @"redirect_uri=http%3A//127.0.0.1%3A9004&" +
                             // @$"code_challenge={base64Code.Replace("==", "")}&" +
                             // @"code_challenge_method=S256&" +
                             @$"client_id={secrets.ClientId}";
            Console.WriteLine("Request Url:");
            Console.WriteLine(requestUrl);
            var url2 = requestUrl.Replace("&", "^&");
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url2}") {CreateNoWindow = true});


            var code = "";
            while (listener.IsListening)
            {
                Console.WriteLine($"Listening on {url}...");
                var context = await listener.GetContextAsync();
                if (context.Request.Url == null) continue;

                Console.WriteLine("Got Request:");
                Console.WriteLine(context.Request.Url.Query);
                var query = context.Request.Url.Query.Replace("?", "");
                var index = query.IndexOf("&", StringComparison.Ordinal);
                if (index >= 0) query = query.Remove(index);
                code = query.Split("=")[1];
                Console.WriteLine("Code:");
                Console.WriteLine(code);
                break;
            }

            //Step 5:
            var client = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
                                                    {
                                                        new KeyValuePair<string?, string?>(
                                                            "client_id", secrets.ClientId),
                                                        new KeyValuePair<string?, string?>(
                                                            "client_secret", secrets.ClientSecret),
                                                        // "test123"),
                                                        new KeyValuePair<string?, string?>("code", code),
                                                        // new KeyValuePair<string?, string?>("code_verifier", codeVerifier),
                                                        new KeyValuePair<string?, string?>(
                                                            "grant_type", "authorization_code"),
                                                        new KeyValuePair<string?, string?>("redirect_uri", url)
                                                    });
            Console.WriteLine("\nExchange authorization code for refresh and access tokens...");
            Console.WriteLine("Request Params:");
            Console.WriteLine(await content.ReadAsStringAsync());
            var response = await client.PostAsync(@"https://oauth2.googleapis.com/token", content);
            Console.WriteLine("Response:");
            Console.WriteLine(await response.Content.ReadAsStringAsync());

            Console.WriteLine($"Stop listening on {url}.");
            listener.Stop();
        }

        private static string GenerateRandomString(int length)
        {
            var rnd = new byte[length];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(rnd);
            }

            var chars = new char[length];
            for (var i = 0; i < length; i++) chars[i] = allowedChars[rnd[i] % allowedChars.Length];
            return new string(chars);
        }

        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain,
                                                     SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}