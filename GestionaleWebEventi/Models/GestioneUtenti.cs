using System;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using System.Net.Mail;

namespace GestionaleWebEventi.Models
{
	public class GestioneUtenti
	{
        private string s;
        public GestioneUtenti(IConfiguration configuration)
        {
            s = configuration.GetConnectionString("DatabaseConnection");
        }

        public string ComputeSha256(string s)
        {
            using SHA256 sha = SHA256.Create();
            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(s));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }

        public Utente CercaUtente(string email)
        {
            string query = @"SELECT Utenti.*, Ruoli.Nome AS NomeRuolo FROM Utenti INNER JOIN Ruoli ON Ruoli.ID = Utenti.IDruolo WHERE Email = @email";
            using var con = new MySqlConnection(s);
            var param = new
            {
                email = email,
            };
            return con.Query<Utente>(query, param).SingleOrDefault();
        }


        public void InvioMail(string Mail, int idEvento)
        {
            using (MailMessage mail = new MailMessage("cdl.3ventorganaizer@gmail.com", Mail))
            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                mail.Subject = "Conferma Iscrizione";
                mail.Body =
                    "<div class=\"form - group mb - 3\">" +
                    "<h1><strong>Grazie per esserti iscritto a questo evento!</strong></h1>" +
                    "<p><h3>Questa Email è una mail di conferma!</h3>" +
                    "</div>"
                    ;
                mail.IsBodyHtml = true;

                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("cdl.3ventorganaizer@gmail.com", "xqbizazaowqpyggw\n");
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
        }

        public bool VerificaPassword(string email, string password)
        {
            password = ComputeSha256(password);
            string query = @"SELECT Utenti.* FROM Utenti WHERE Email = @email AND Password = @password";
            using var con = new MySqlConnection(s);
            var param = new
            {
                email = email,
                password = password
            };
            return con.Query<Utente>(query, param).Count() == 1;
        }


    }
}

