using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WechatSDKCore.WechatPay.V3
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Threading;
    using System.Threading.Tasks;

    namespace HttpHandlerDemo
    {
        // 使用方法
        // HttpClient client = new HttpClient(new HttpHandler("{商户号}", "{商户证书序列号}"));
        // ...
        // var response = client.GetAsync("https://api.mch.weixin.qq.com/v3/certificates");
        public class HttpHandler : DelegatingHandler
        {
            private readonly string merchantId;
            private readonly string serialNo;

            public HttpHandler(string merchantId, string merchantSerialNo)
            {
                InnerHandler = new HttpClientHandler();

                this.merchantId = merchantId;
                this.serialNo = merchantSerialNo;
            }

            protected async override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                var auth = await BuildAuthAsync(request);
                string value = $"WECHATPAY2-SHA256-RSA2048 {auth}";
                request.Headers.Add("Authorization", value);
                request.Headers.Add("Accept", "application/json");//如果缺少这句代码就会导致下单接口请求失败，报400错误（Bad Request）
                request.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");//如果缺少这句代码就会导致下单接口请求失败，报400错误（Bad Request）

                return await base.SendAsync(request, cancellationToken);
            }

            protected async Task<string> BuildAuthAsync(HttpRequestMessage request)
            {
                string method = request.Method.ToString();
                string body = "";
                if (method == "POST" || method == "PUT" || method == "PATCH")
                {
                    var content = request.Content;
                    body = await content.ReadAsStringAsync();//debug的时候在这里打个断点，看看body的值是多少，如果跟你传入的参数不一致，说明是有问题的，一定参考我的方法
                }

                string uri = request.RequestUri.PathAndQuery;
                var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
                string nonce = Path.GetRandomFileName();

                string message = $"{method}{uri}{timestamp}{nonce}{body}";
                string signature = Sign(message);
                return $"mchid={merchantId},nonce_str={nonce},timestamp={timestamp},serial_no={serialNo},signature={signature}";
            }

            protected string Sign(string message)
            {
                // NOTE： 私钥不包括私钥文件起始的-----BEGIN PRIVATE KEY-----
                //        亦不包括结尾的-----END PRIVATE KEY-----
                string privateKey = "MIIEFDCCAvygAwIBAgIUT68k1I+yA2NelWLmZQM800VM97cwDQYJKoZIhvcNAQELBQAwXjELMAkGA1UEBhMCQ04xEzARBgNVBAoTClRlbnBheS5jb20xHTAbBgNVBAsTFFRlbnBheS5jb20gQ0EgQ2VudGVyMRswGQYDVQQDExJUZW5wYXkuY29tIFJvb3QgQ0EwHhcNMjMwMzMwMDU0ODI1WhcNMjgwMzI4MDU0ODI1WjBuMRgwFgYDVQQDDA9UZW5wYXkuY29tIHNpZ24xEzARBgNVBAoMClRlbnBheS5jb20xHTAbBgNVBAsMFFRlbnBheS5jb20gQ0EgQ2VudGVyMQswCQYDVQQGDAJDTjERMA8GA1UEBwwIU2hlblpoZW4wggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQC5Iv2TpW+jzcchA/DlNJ8xt1Kb6wo63qUuGVIOLlN7r5up+PV2dB5p2XG6bch4NERSLwI0iQI82oqcvIDjMfGzoujLoq1FJMSkV95z2cgTv5I8E+BxY6wukKkjbyfUa5CCAbYptVlxNBZfXy6VNOEk/KFFCuq5WOgbSX8OYhqgGDUzMU4R9332lx8Vet1Dvjel/WOoAO6McyKiFPJJt6BosJ0q+gCwcseS/bh0DoBvrT+wm0Nbunjh0QJw3zdaxJ8j/yLkBVrtE+rQfyvGrE5wtL0ITSgvIFy3fawHkKDYpspp8JEb8ZwQp1pds7HsbQ1DOIp32aBh1yGd+ozYxjYlAgMBAAGjgbkwgbYwCQYDVR0TBAIwADALBgNVHQ8EBAMCA/gwgZsGA1UdHwSBkzCBkDCBjaCBiqCBh4aBhGh0dHA6Ly9ldmNhLml0cnVzLmNvbS5jbi9wdWJsaWMvaXRydXNjcmw/Q0E9MUJENDIyMEU1MERCQzA0QjA2QUQzOTc1NDk4NDZDMDFDM0U4RUJEMiZzZz1IQUNDNDcxQjY1NDIyRTEyQjI3QTlEMzNBODdBRDFDREY1OTI2RTE0MDM3MTANBgkqhkiG9w0BAQsFAAOCAQEAlhvrh/bjfsueiAuEt+ggCHU6LBXN4WwMkHPGmFZdvzL+eUAXv8YVBOh/1vcnYkEBR8FZSBbTODEAqCQLn+M1y4Dr86oa4/unII1Xuv+BCPIVDUuYxVN3L/5eV6LxkXPHK8uK1ye+IUTqLk7qF1upOaVHs7QcuG0q4gN2vilWvHtwX3DNthSkxB3/0muQPJuzasjWjj4lhU7XYWwmxB/Qtc2sf2r4Z9CeKJIfVSZZR44DgDohwimED/Rf2thPhLXHCd7T3nZu/I/bHIZasfGdXJb9BmgRg3NWx4hmJVg+Y3z/xooDyd9MVeCZEF70byn8hLIC32uTK+ZKzGrnqy2Jug==";
                byte[] keyData = Convert.FromBase64String(privateKey);
                using (CngKey cngKey = CngKey.Import(keyData, CngKeyBlobFormat.Pkcs8PrivateBlob))
                using (RSACng rsa = new RSACng(cngKey))
                {
                    byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
                    return Convert.ToBase64String(rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));
                }
            }
        }
    }
}
