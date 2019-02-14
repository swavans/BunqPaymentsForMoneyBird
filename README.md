# MoneyBirdBunqLink
This tool can be used to intercept mails from moneybird, call the bunq api and retrieve a payment url and then send the mail to the original target.

# Requirements
 - 2 Mail accounts dedicated to this. YOU CAN RECEIVE NO OTHER MAIL HERE AND MAIL RECEIVED WILL BE DELETED(and preferably your own mail server)
 - UBL Bills enabled for all Accounts
 - The following line of text in your moneybird mail template "https://bunq.me/t/11111111-2222-3333-4444-555555555555" 
 - A bunq api key (read more here https://www.bunq.com/developer)
 - .net core 2.1 runtime installed
 
# Setup
 - Open "appsettings.json" in notepad++ or any other text editor and change the configuration to your liking. If you change the FAKE API key then please also change your moneybird template.
 - The paymentmail adress is the adress that bunq will send the url's too
 - The invoicemail is the adress you will provide in moneybird when you send the invoice
 
 # Usage
 - Start the program by opening a commandline interface and executing the following command "dotnet BunqPaymentsForMoneybird.dll"
 - Send your invoices normally with one exception instead of when you send them sending them to your contacts email, send them to the invoice email adress you have set up. DO NOT CHANGE YOUR CUSTOMERS CONTACT INFO but only the send to email after you pressed the send invoice button the first time since the mail adress specified here will be used by the program to send the final invoice

