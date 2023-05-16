using System;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using System.Net.Mail;
using Microsoft.VisualBasic;
using MySqlX.XDevAPI.Relational;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Runtime.ConstrainedExecution;
using System.Xml;

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


        public void InvioMail(string Mail, Evento evento, string nome)
        {
            using (MailMessage mail = new MailMessage("cdl.3ventorganaizer@gmail.com", Mail))
            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                mail.Subject = "Conferma Iscrizione";
                string body = @"<table style=""width: 100%; max-width: 600px; margin: 0 auto; font-family: Arial, sans-serif; background-color: rgb(73, 102, 206); padding: 20px;"">
                                    <tr>
                                        <td style=""text-align: center;"">
                                            <h2 style=""color: #333;"">Iscrizione avvenuta con successo!</h2>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style=""background-color: #f9f9f9; padding: 20px;"">
                                            <p>Ciao [Nome Utente],</p>
                                            <p>Grazie per esserti iscritto al nostro servizio. Siamo felici di averti a bordo!</p>
                                            <p>Di seguito troverai i dettagli della tua iscrizione:</p>
                                            <ul>
                                                <li><strong>Nome:</strong> [Nome Utente]</li>
                                                <li><strong>Email:</strong> [Indirizzo Email]</li>
                                                <li><strong>Titolo evento:</strong> [Titolo]</li>
                                                <li><strong>Data evento:</strong> [Data di iscrizione]</li>
                                            </ul>
                                            <p>Se hai domande o necessiti di ulteriori informazioni, non esitare a contattarci.</p>
                                            <p>Benvenuto nella nostra comunità!</p>
                                            <p>Cordiali saluti,</p>
                                            <p>Il team di CDL Event Organizer</p>
                                            <br>
                                            <p style=""text-align: center;"">
                                                <a href=""[URL Annullamento Iscrizione]"" style=""display: inline-block; padding: 10px 20px; background-color: #ff0000; color: #fff; text-decoration: none;"">Annulla Iscrizione</a>
                                            </p>
                                        </td>
                                    </tr>
                                </table>";
                body = body.Replace("[Nome Utente]", nome)
                   .Replace("[Titolo]", evento.Titolo)
                   .Replace("[Indirizzo Email]", Mail)
                   .Replace("[Data di iscrizione]", evento.DataOra.ToString())
                   .Replace("[URL Annullamento Iscrizione]", "https://localhost:7126/Home/AnnullaIscrizione/" + evento.ID);
                mail.Body = body;
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

