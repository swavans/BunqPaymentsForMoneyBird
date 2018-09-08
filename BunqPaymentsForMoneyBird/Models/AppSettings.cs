namespace BunqPaymentsForMoneyBird.Models
{
    public class AppSettings
    {
        public Bunq bunq { get; set; }
        public Paymentmail paymentMail { get; set; }
        public Invoicemail invoiceMail { get; set; }
        public Sendmail sendMail { get; set; }
    }

    public class Bunq
    {
        public string apikey { get; set; }
        public string description { get; set; }
        public string fakeKey { get; set; }
    }

    public class Paymentmail
    {
        public string mail { get; set; }
        public string password { get; set; }
        public string server { get; set; }
        public int port { get; set; }
        public bool ssl { get; set; }
    }

    public class Invoicemail
    {
        public string mail { get; set; }
        public string password { get; set; }
        public string server { get; set; }
        public int port { get; set; }
        public bool ssl { get; set; }
    }

    public class Sendmail
    {
        public string server { get; set; }
        public int port { get; set; }
    }
}