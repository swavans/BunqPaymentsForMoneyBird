using Bunq.Sdk.Context;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using BunqPaymentsForMoneyBird.Models;

namespace BunqPaymentsForMoneyBird
{
    public class Program
    {
        private readonly AppSettings _configuration;
        private readonly MailHandler _invoices;
        private readonly MailHandler _paymentLinks;

        public static void Main(string[] args)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            AppSettings settings = new AppSettings();

            builder.Build().Bind(settings);

            InitializeBunq(settings);

            PaymentHandler program = new PaymentHandler(settings);

            while (true)
            {
                program.HandlePayments();
            }
        }

        public static void InitializeBunq(AppSettings configuration)
        {
            if (File.Exists("bunq.conf"))
            {
                try
                {
                    ApiContext apiContext = ApiContext.Restore("bunq.conf");
                    BunqContext.LoadApiContext(apiContext);
                }
                catch (Exception)
                {
                    File.Delete("bunq.conf");
                    ApiContext apiContext = ApiContext.Create(ApiEnvironmentType.PRODUCTION,
                        configuration.bunq.apikey, configuration.bunq.description);
                    apiContext.Save();
                    BunqContext.LoadApiContext(apiContext);
                }
            }
            else
            {
                ApiContext apiContext = ApiContext.Create(ApiEnvironmentType.PRODUCTION,
                    configuration.bunq.apikey, configuration.bunq.description);
                apiContext.Save();
                BunqContext.LoadApiContext(apiContext);
            }
        }

    }
}
