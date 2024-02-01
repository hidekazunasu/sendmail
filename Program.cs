using Npgsql;
using Azure.Communication.Email;
using Azure;

namespace Driver
{
    public class AzurePostgresCreate
    {
        // Obtain connection string information from the portal
        //
        private static string Host = @"mailtest001.postgres.database.azure.com";
        private static string User = @"user";
        private static string DBname = "postgres";
        private static string Password = "Password334";
        private static string Port = "5432";

        static void Main(string[] args)
        {
            // Build connection string using parameters from portal
            //
            string connString =
                String.Format(
                    "Server={0};Username={1};Database={2};Port={3};Password={4};SSLMode=Prefer",
                    Host,
                    User,
                    DBname,
                    Port,
                    Password);


            using (var conn = new NpgsqlConnection(connString))

            {
                Console.Out.WriteLine("Opening connection");
                conn.Open();
                using (var command = new NpgsqlCommand("select * from inventory", conn))
                {

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        sendmail(reader.GetString(2).ToString());
                    }
                    reader.Close();
                }

            }
            async void sendmail(string adress)
            {

                var connectionString = @"endpoint=https://cstest002.unitedstates.communication.azure.com/;accesskey=Ih7yMonp3JAijBwCeLQ3K+qVN5MoKEdkKkR3qwYXd+/zdfGcVEGfhxVbW7zz7mEIq4SpdZGi6/o9e/9JxwHYTQ==";
                var emailClient = new EmailClient(connectionString);
                // EmailContentの文字列引数はメールタイトル
                var subject = "Azure Mail Test";
                var sender = "DoNotReply@864a9094-1b48-408b-851f-9403b8eb0024.azurecomm.net";
                var htmlContent = "<html><h4>This email message is sent from Azure Communication Service Email.</h4></html>";
                var recipient = adress;
                try
                {
                    Console.WriteLine("Sending email...");
                    EmailSendOperation emailSendOperation = await emailClient.SendAsync(
                        Azure.WaitUntil.Completed,
                        sender,
                        recipient,
                        subject,
                        htmlContent);
                    EmailSendResult statusMonitor = emailSendOperation.Value;

                    Console.WriteLine($"Email Sent. Status = {emailSendOperation.Value.Status}");

                    /// Get the OperationId so that it can be used for tracking the message for troubleshooting
                    string operationId = emailSendOperation.Id;
                    Console.WriteLine($"Email operation id = {operationId}");
                }
                catch (RequestFailedException ex)
                {
                    /// OperationID is contained in the exception message and can be used for troubleshooting purposes
                    Console.WriteLine($"Email send operation failed with error code: {ex.ErrorCode}, message: {ex.Message}");
                }


            }
        }


    }
}