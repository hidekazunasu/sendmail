using Npgsql;
using Azure.Communication.Email;
using Azure;

namespace Driver
{
    public class AzurePostgresCreate
    {
        // Obtain connection string information from the portal
        //
        private static string Host = @"Dbのホスト";
        private static string User = @"userName";
        private static string DBname = "DBname";
        private static string Password = "Password";
        private static string Port = "5432"; #デフォルトでは5432

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
            void sendmail(string adress)
            {

                var connectionString = @"接続文字列"; # 通信サービスから取得した接続文字列
                // EmailContentの文字列引数はメールタイトル
                var subject = "Azure Mail Test";
                var sender = "送信元メールアドレス";
                var htmlContent = "<html><h4>This email message is sent from Azure Communication Service Email.</h4></html>";
                var recipient = adress;
                try
                {
                    EmailSendOperation emailSendOperation = emailClient.Send(
                        Azure.WaitUntil.Completed,
                        sender,
                        recipient,
                        subject,
                        htmlContent);

                    Console.WriteLine($"Email Sent. Status = {emailSendOperation.Value.Status}");

                }
                catch (RequestFailedException ex)
                {
                    Console.WriteLine($"Email send operation failed with error code: {ex.ErrorCode}, message: {ex.Message}");
                    Console.WriteLine($"Exception details: {ex.ToString()}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected exception: {ex.GetType().FullName}, message: {ex.Message}");
                    Console.WriteLine($"Exception details: {ex.ToString()}");
                }


            }
        }


    }
}
