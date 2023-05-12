using System;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;
using Dapper;

namespace GestionaleWebEventi.Models
{
	public class GestioneUtenti
	{
        private string s;
        public GestioneUtenti(IConfiguration configuration)
        {
            s = configuration.GetConnectionString("DatabaseConnection");
        }

        private string ComputeSha256(string s)
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

        public Utente CercaUtente(string n, string p)
        {
            string query = @"SELECT Utenti.*, Ruoli.Nome AS NomeRuolo FROM Utenti INNER JOIN Ruoli ON Ruoli.ID = Utenti.IDruoli WHERE Email = @Email AND Password = @Password";
            using var con = new MySqlConnection(s);
            var param = new
            {
                nome = n,
                password = ComputeSha256(p)
            };
            return con.Query<Utente>(query, param).SingleOrDefault();
        }


    }
}

