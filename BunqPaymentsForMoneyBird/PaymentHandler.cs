using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Xml.Serialization;
using Bunq.Sdk.Model.Generated.Endpoint;
using Bunq.Sdk.Model.Generated.Object;
using BunqPaymentsForMoneyBird.Models;
using Invoice = BunqPaymentsForMoneyBird.Models.Invoice;

namespace BunqPaymentsForMoneyBird
{
    public class PaymentHandler
    {
        private readonly AppSettings _configuration;
        private readonly MailHandler _paymentLinks;
        private readonly MailHandler _invoices;

        public PaymentHandler(AppSettings configuration)
        {
            _configuration = configuration;

            _invoices = new MailHandler(_configuration.invoiceMail.server, _configuration.invoiceMail.mail,
                _configuration.invoiceMail.password, _configuration.invoiceMail.port, _configuration.invoiceMail.ssl);

            _paymentLinks = new MailHandler(_configuration.paymentMail.server, _configuration.paymentMail.mail,
                _configuration.paymentMail.password, _configuration.paymentMail.port, _configuration.paymentMail.ssl);
        }

        public void HandlePayments()
        {
            try
            {
                var message = _invoices.GetMessage().Result;
                var invoice = GetInvoice(message);
                var token =
                    GetPaymentToken(
                        invoice.LegalMonetaryTotal.PayableAmount.Value.ToString(CultureInfo.InvariantCulture),
                        invoice.ID.Value);
                SendMail(ModifyMail(message, token), invoice.AccountingCustomerParty.Party.Contact.ElectronicMail);
            }
            catch (Exception)
            {
                Console.WriteLine("Something went wrong. Plz check your configuration and start again");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        private Invoice GetInvoice(MailMessage message)
        {
            try
            {
                var xmlInvoice = message.Attachments.SingleOrDefault(x => x.Name.Contains(".xml"));
                if (xmlInvoice == null) throw new NullReferenceException();

                var serializer = new XmlSerializer(typeof(Invoice));

                var invoice = serializer.Deserialize(xmlInvoice.ContentStream) as Invoice;
                if (invoice == null) throw new NullReferenceException();

                return invoice;
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Invalid mail, no ubl");
                Console.ReadKey();
                return null;
            }
        }

        private string GetPaymentToken(string amount, string description)
        {
            RequestInquiry.Create(new Amount(amount, "EUR"),
                new Pointer("EMAIL", _configuration.paymentMail.mail), description, true);

            var message = _paymentLinks.GetMessage().Result;


            if (message.Body.Contains(amount))
            {
                var messageText = message.AlternateViews[0].ContentStream;
                var result = Encoding.UTF8.GetString((messageText as MemoryStream).ToArray());
                return result.Split("https://bunq.me/t/")[1].Split("\" target=\"")[0];
            }

            throw new NullReferenceException();
        }

        private MailMessage ModifyMail(MailMessage message, string token)
        {
            message.Body = message.Body.Replace(_configuration.bunq.fakeKey, token);
            var messageText = message.AlternateViews[0].ContentStream;
            var html = Encoding.UTF8.GetString((messageText as MemoryStream).ToArray());
            html = html.Replace(_configuration.bunq.fakeKey, token);
            message.AlternateViews[0] =
                AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html);
            return message;
        }

        private void SendMail(MailMessage mail, string targetAddress)
        {
            using (var client = new SmtpClient(_configuration.sendMail.server, _configuration.sendMail.port))
            {
                mail.To.Clear();
                mail.To.Add(targetAddress);
                client.Send(mail);
            }
        }
    }
}