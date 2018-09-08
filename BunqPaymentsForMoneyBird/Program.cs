using System;
using System.IO;
using Bunq.Sdk.Context;
using BunqPaymentsForMoneyBird.Models;
using Microsoft.Extensions.Configuration;

namespace BunqPaymentsForMoneyBird
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var settings = new AppSettings();

            builder.Build().Bind(settings);

            InitializeBunq(settings);

            var program = new PaymentHandler(settings);

            while (true) program.HandlePayments();
        }

        public static void InitializeBunq(AppSettings configuration)
        {
            if (File.Exists("bunq.conf"))
            {
                try
                {
                    var apiContext = ApiContext.Restore("bunq.conf");
                    BunqContext.LoadApiContext(apiContext);
                }
                catch (Exception)
                {
                    File.Delete("bunq.conf");
                    var apiContext = ApiContext.Create(ApiEnvironmentType.PRODUCTION,
                        configuration.bunq.apikey, configuration.bunq.description);
                    apiContext.Save();
                    BunqContext.LoadApiContext(apiContext);
                }
            }
            else
            {
                var apiContext = ApiContext.Create(ApiEnvironmentType.PRODUCTION,
                    configuration.bunq.apikey, configuration.bunq.description);
                apiContext.Save();
                BunqContext.LoadApiContext(apiContext);
            }
        }
    }
}