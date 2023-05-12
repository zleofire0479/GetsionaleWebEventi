using System;
using Dapper;
using MySql.Data.MySqlClient;

namespace GestionaleWebEventi.Models
{
	public class GestioneDati
	{
        private string s;
        public GestioneDati(IConfiguration configuration)
		{
            s = configuration.GetConnectionString("DatabaseConnection");
        }

        public IEnumerable<Evento> ListaEventi()
        {
            using var con = new MySqlConnection(s);
            return con.Query<Evento>("select * from Eventi"); 
        }

        public string GetRuolo(int IDruolo)
        {
            using var con = new MySqlConnection(s);
            return con.Query<string>("select Nome from Ruoli where id = " + IDruolo).FirstOrDefault();
        }

        public IEnumerable<string> GetRuoli(string PIazienda)
        {
            using var con = new MySqlConnection(s);
            var query = @"select Nome from Ruoli where PIazienda = @PI";
            var parm = new
            {
                PI = PIazienda
            };

            return con.Query<string>(query, parm);
            //return con.Query<string>("select Nome from Ruoli where PIazienda = @PIazienda");
        }

        public bool InserisciEvento(Evento evento)
        {
            using var con = new MySqlConnection(s);
            var query = @"INSERT INTO Eventi(Titolo, DataOra, Luogo, MaxUtenti, PIazienda) VALUES(@Titolo, @DataOra, @Luogo, @MaxUtenti, @PIazienda)";
            var param = new
            {
                Titolo = evento.Titolo,
                Dataora = evento.DataOra,
                Luogo = evento.Luogo,
                MaxUtenti = evento.MaxUtenti,
                PIazienda = evento.PIazienda
            };
            try
            {
                con.Execute(query, param);
                return true;
            }
            catch (Exception ex) { return false; }
        }
    }
}

