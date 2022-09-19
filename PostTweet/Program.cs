using System;
using System.Net;
using System.Security.Cryptography;
using System.IO;

namespace PostTweet
{
    class Program
    {
        static string targetURL = "https://api.twitter.com/2/tweets";
        static string consumerKey = "v2MRsZZH77VpzReb6lNlKXFrW";
        static string consumerSecret = "fTfws7Ff1uCzBotNpCgmm0SJj9w92NabiuKyFYISZiD1ztcNTH";
        static string accessToken = "1057816229774221312-U1Ao7ZdOzLGUhMdT5nZfASXjejTYmc";
        static string tokenSecure = "t3u1hcT9kSM3TP0PLmHF1CnJV4JrlifilFlbccGiacrR7";
        static string nonce;
        static string timeStamp;

        public static string GenerateHash()
        {
            string parameter_string = string.Empty;
            parameter_string += WebUtility.UrlEncode("oauth_consumer_key") + "=" + WebUtility.UrlEncode(consumerKey);
            parameter_string += "&" + WebUtility.UrlEncode("oauth_nonce") + "=" + WebUtility.UrlEncode(nonce);
            parameter_string += "&" + WebUtility.UrlEncode("oauth_signature_method") + "=" + WebUtility.UrlEncode("HMAC-SHA1");
            parameter_string += "&" + WebUtility.UrlEncode("oauth_timestamp") + "=" + WebUtility.UrlEncode(timeStamp);
            parameter_string += "&" + WebUtility.UrlEncode("oauth_token") + "=" + WebUtility.UrlEncode(accessToken);
            parameter_string += "&" + WebUtility.UrlEncode("oauth_version") + "=" + WebUtility.UrlEncode("1.0");

            string base_string = "POST&" + WebUtility.UrlEncode(targetURL);
            base_string += "&" + WebUtility.UrlEncode(parameter_string);

            string signing_key = WebUtility.UrlEncode(consumerSecret) + "&" + WebUtility.UrlEncode(tokenSecure);

            return SignData(base_string, signing_key);
        }

        static string SignData(string message, string secret)
        {
            var encoding = new System.Text.UTF8Encoding();
            var keyBytes = encoding.GetBytes(secret);
            var messageBytes = encoding.GetBytes(message);
            using (var hmashal = new HMACSHA1(keyBytes))
            {
                var hashMessage = hmashal.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashMessage);
            }
        }

        static string GenerateAuthString()
        {
            nonce = Guid.NewGuid().ToString().Replace("-", "");
            timeStamp = Convert.ToString((int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds + 5);

            string result = string.Empty;
            result += "OAuth ";
            result += "oauth_consumer_key=" + "\"" + consumerKey + "\",";
            result += "oauth_token=" + "\"" + accessToken + "\",";
            result += "oauth_signature_method=\"HMAC-SHA1\",";
            result += "oauth_timestamp=" + "\"" + timeStamp + "\",";
            result += "oauth_nonce=" + "\"" + nonce + "\",";
            result += "oauth_version=\"1.0\",";
            result += "oauth_signature=" + "\"" + WebUtility.UrlEncode(GenerateHash()) + "\"";

            return result;
        }

        static void Tweet(string context)
        {
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(targetURL);
            myReq.Headers["Authorization"] = GenerateAuthString();
            myReq.Method = "POST";
            myReq.ContentType = "application/json";
            myReq.Timeout = 3000;

            string result = "";

            using (var streamWriter = new StreamWriter(myReq.GetRequestStream()))
            {
                streamWriter.Write(context);
            }

            using (HttpWebResponse response = (HttpWebResponse)myReq.GetResponse())
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                }
            }

            Console.WriteLine(result);
        }

        static void Main(string[] args)
        {
            string content = "{\"text\":\"C#測試\"}";
            Tweet(content);

            Console.ReadKey();
        }
    }
}
