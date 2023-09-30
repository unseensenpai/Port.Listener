using Serilog;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace PortListener.Extensions
{
    public class MailSender
    {
        private const string FROM = "**";

        private const string HOST = "**";

        private const int PORT = 1234;

        private const string PASSWD = "**";

        public static void SendPingHTMLMail(Dictionary<string, int> keyValuePairs)
        {
            try
            {
                using MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress("**.com.tr", "Hatalı Ip Portları Hakkında", Encoding.UTF8),                    
                    Subject = "Hatalı Ip Portları Hakkında"                   
                };
                mailMessage.Body = GetPingHTMLTemplate(keyValuePairs);
                mailMessage.IsBodyHtml = true;
                mailMessage.To.Add("**.com.tr");
                mailMessage.To.Add("**.com.tr");
                mailMessage.To.Add("**.com.tr");
                mailMessage.HeadersEncoding = Encoding.UTF8;

                using SmtpClient smtpClient = new(HOST, PORT)
                {
                    Credentials = new NetworkCredential(FROM, PASSWD)
                };
                smtpClient.EnableSsl = true;
                smtpClient.Send(mailMessage);
            }
            catch (Exception exception)
            {
                Log.Error("Hata raporu gönderilirken bir hata meydana geldi!", exception);
            }
        }
        private static string GetPingHTMLTemplate(Dictionary<string, int> ipErrorPairs)
        {
            StringBuilder htmlTableDetailBuilder = new();
            foreach (var ip in ipErrorPairs.Keys)
            {
                var itemStringTable = $@"<tr>
                <td>{ip}</td>
                <td>AREA NAME</td>
                <td>{ipErrorPairs[ip]}</td>
                </tr>";

                htmlTableDetailBuilder.Append(itemStringTable);
            }

            var htmlTemplate = @$" <!DOCTYPE html>
<html lang=""tr"">
<head>
  <meta charset=""UTF-8"">
  <title>Unseen Ping</title>
  <style>
    body {{
      font-family: sans-serif;
    }}

    header {{
      background-color: #000;
      color: #fff;
      padding: 1rem 2rem;
    }}

    h1 {{
      font-size: 2rem;
    }}

    .mini-header {{
      font-size: 1rem;
      margin-bottom: 1rem;
    }}

    table {{
      width: 100%;
      border-collapse: collapse;
      border: 1px solid #ccc;
    }}

    th, td {{
      text-align: left;
      padding: 0.5rem;
    }}

    .açıklama {{
      margin-bottom: 1rem;
    }}

    footer {{
      background-color: #ccc;
      color: #000;
      padding: 1rem 2rem;
    }}
  </style>
</head>
<body>
  <header>
    <h1>Unseen Ping Bildirim Maili</h1>
  </header>

  <main>
    <h2 class=""mini-header"">Başarısız sonuçlanan IP'ler ve başarısız işlem sayıları</h2>

    <table>
      <thead>
        <tr>
          <th>IP Adresi</th>
          <th>Bölge</th>
          <th>Hata Sayısı</th>
        </tr>
      </thead>
      <tbody>
        {htmlTableDetailBuilder}
      </tbody>
    </table>
    </br>
    <div class=""açıklama"">
      Bu tablo, IP adresleri, işyerleri ve hata sayılarını göstermektedir.
    </div>
  </main>

  <footer>
  <div class=""copyright"">
    <p> <img src=""***.logo.png"" alt=""Marka logosu""> </p>
    <p>Copyright &copy; 2023. Tüm hakları saklıdır.</p>
  </div>
</footer>
</body>
</html>";

            return htmlTemplate;
        }
    }
}
