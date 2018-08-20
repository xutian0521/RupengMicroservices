using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using System;
using System.Collections.Generic;

namespace JWTTest
{
    class Program
    {
        static void Main(string[] args)
        {


            Encrypt();
            Decrypt();
            Console.ReadKey();
        }

        static void Encrypt()
        {
            #region 1) 加密

            double exp = (DateTime.UtcNow.AddSeconds(10) - new DateTime(1970, 1, 1)).TotalSeconds;
            var payload = new Dictionary<string, object>
            {
                { "UserId", 123 },
                { "UserName", "admin" },
                { "exp", exp +100 }
            };
            var secret = "GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";//不要泄露
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            var token = encoder.Encode(payload, secret);
            Console.WriteLine(token);
            #endregion

        }
        static void Decrypt()
        {
            #region 2) 解密


            var token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJVc2VySWQiOjEyMywiVXNlck5hbWUiOiJhZG1pbiIsImV4cCI6MTUzNDc1MzExMS45MDIyfQ.ZOxY4S4bMQhd1augq1K8mj8YeJGsdIiGs1fKd0Mx1ts";
            var secret = "GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";
            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);
                var json = decoder.Decode(token, secret, verify: true);
                Console.WriteLine(json);
            }
            catch (FormatException)
            {
                Console.WriteLine("Token format invalid");
            }
            catch (TokenExpiredException)
            {
                Console.WriteLine("Token has expired");
            }
            catch (SignatureVerificationException)
            {
                Console.WriteLine("Token has invalid signature");
            }
            #endregion
        }
    }
}
