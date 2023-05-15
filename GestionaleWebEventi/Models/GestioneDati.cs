using System;
using System.Data;
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

        public Evento GetEvento(int id)
        {
            using var con = new MySqlConnection(s);
            var query = @"select Eventi.* from Eventi where ID = @id";
            var parm = new
            {
                id = id
            };
            return con.Query<Evento>(query, parm).FirstOrDefault();
        }

        public IEnumerable<Evento> ListaEventi(string piAzienda)
        {
            using var con = new MySqlConnection(s);
            var query = @"select Eventi.* from Eventi where PIazienda = @piAzienda ORDER BY Eventi.DataOra ASC";
            var parm = new
            {
                piAzienda = piAzienda
            };
            return con.Query<Evento>(query, parm);
        }

        public IEnumerable<Evento> ListaEventi(string ruolo, string piAzienda)
        {
            using var con = new MySqlConnection(s);
            var query = @"select Eventi.* from Eventi inner join Autorizzazioni on Eventi.ID = Autorizzazioni.IDevento where Autorizzazioni.IDruolo = @idRuolo ORDER BY Eventi.DataOra ASC";
            var parm = new
            {
                idRuolo = GetIdRuolo(ruolo, piAzienda)
            };
            return con.Query<Evento>(query, parm); 
        }

        public string GetRuolo(int IDruolo)
        {
            using var con = new MySqlConnection(s);
            var query = @"select Nome from Ruoli where id = @id";
            var parm = new
            {
                id = IDruolo
            };
            return con.Query<string>(query, parm).FirstOrDefault();
        }

        public IEnumerable<string> GetRuoli(string PIazienda)
        {
            using var con = new MySqlConnection(s);
            var query = @"select Nome from Ruoli where PIazienda = @PI ORDER BY Nome ASC";
            var parm = new
            {
                PI = PIazienda
            };

            return con.Query<string>(query, parm);
        }

        public int GetIdRuolo(string nomeRuolo, string piAzienda) {
            using var con = new MySqlConnection(s);
            var query = @"select ID from Ruoli where PIazienda = @PI AND Nome = @nomeRuolo";
            var parm = new
            {
                PI = piAzienda,
                nomeRuolo = nomeRuolo
            };
            return con.Query<int>(query, parm).FirstOrDefault();
        }

        public int InserisciEvento(Evento evento)
        {
            int idInserito = 0;

            using (MySqlConnection connection = new MySqlConnection(s))
            {
                connection.Open();
                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var query = @"INSERT INTO Eventi(Titolo, DataOra, Luogo, MaxUtenti, PIazienda) VALUES(@Titolo, @DataOra, @Luogo, @MaxUtenti, @PIazienda); SELECT LAST_INSERT_ID();";
                        var param = new
                        {
                            Titolo = evento.Titolo,
                            DataOra = evento.DataOra,
                            Luogo = evento.Luogo,
                            MaxUtenti = evento.MaxUtenti,
                            PIazienda = evento.PIazienda
                        };

                        idInserito = connection.ExecuteScalar<int>(query, param, transaction);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                    }
                }
            }
            return idInserito;
        }

        public bool InserisciAutorizzazioni(int idEvento, string piAzienda, IEnumerable<string> listaRuoli) {
            try {
                foreach (string ruolo in listaRuoli)
                {
                    using var con = new MySqlConnection(s);
                    var query = @"INSERT INTO Autorizzazioni(IDevento, IDruolo) VALUES(@IDevento, @IDruolo);";
                    var idRuolo = GetIdRuolo(ruolo, piAzienda);
                    if (idRuolo == null) throw new NullReferenceException();
                    var parm = new 
                    {
                        IDevento = idEvento,
                        IDruolo = idRuolo
                    };
                    con.Execute(query, parm);
                }
                return true;
            } catch (Exception ex) { return false; }
            
        }
    }
}

