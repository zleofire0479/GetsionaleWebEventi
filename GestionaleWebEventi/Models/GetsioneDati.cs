using System;
using Dapper;
using GestionaleWebEventi.Models;
using MySql.Data.MySqlClient;

namespace GestionaleWebEventi.Models
{
	public class GetsioneDati
	{
        private string s;
        public GetsioneDati(IConfiguration configuration)
		{
            s = configuration.GetConnectionString("DatabaseConnection");
        }

        public IEnumerable<Evento> ListaEventi()
        {
            using var con = new MySqlConnection(s);
            return con.Query<Evento>("select * from Eventi");
        }
    }
}

