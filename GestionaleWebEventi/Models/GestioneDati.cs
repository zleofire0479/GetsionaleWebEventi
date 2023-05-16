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

        public int GetIdIscrizione(int idEvento, int idUtente)
        {
            using var con = new MySqlConnection(s);
            var query = @"select ID from Iscrizioni where IDevento = @idevento AND IDutente = @idutente";
            var parm = new
            {
                IDevento = idEvento,
                IDutente = idUtente
            };
            return con.Query<int>(query, parm).FirstOrDefault();
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

        public int GetNumEventi(string ruolo, string piAzienda)
        {
            int count = 0;
            using (var con = new MySqlConnection(s))
            {
                var query = @"SELECT COUNT(*) FROM Eventi INNER JOIN Autorizzazioni ON Eventi.ID = Autorizzazioni.IDevento WHERE Autorizzazioni.IDruolo = @idRuolo";
                var parameters = new
                {
                    idRuolo = GetIdRuolo(ruolo, piAzienda)
                };

                count = con.ExecuteScalar<int>(query, parameters);
            }
            return count;
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

        public bool ModificaEvento(Evento evento)
        {
            using (MySqlConnection connection = new MySqlConnection(s))
            {
                connection.Open();
                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var query = @"UPDATE Eventi SET Titolo = @Titolo, DataOra = @DataOra, Luogo = @Luogo, MaxUtenti = @MaxUtenti WHERE ID = @ID";
                        var param = new
                        {
                            ID = evento.ID,
                            Titolo = evento.Titolo,
                            DataOra = evento.DataOra,
                            Luogo = evento.Luogo,
                            MaxUtenti = evento.MaxUtenti
                        };

                        int rowsAffected = connection.Execute(query, param, transaction);

                        transaction.Commit();

                        return rowsAffected > 0;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
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

        public bool InserisciIscrizione(int idEvento, int idUtente)
        {
            try
            {
                using var con = new MySqlConnection(s);
                var query = @"INSERT INTO Iscrizioni(IDevento, IDutente, Data) VALUES(@IDevento, @IDutente, @Data);";
                var parm = new
                {
                    IDevento = idEvento,
                    IDutente = idUtente,
                    Data = DateTime.Now,
                };
                con.Execute(query, parm);
                return true;
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1644) // Numero dell'errore restituito dal database nel caso in cui il trigger scatti
                {
                    return false;
                }
                else
                {
                    throw ex;
                }
            }
        }

        public bool AnnullaIscrizione(int idIscrizione)
        {
            try
            {
                using var con = new MySqlConnection(s);
                var query = @"DELETE FROM Iscrizioni WHERE ID = @IDiscrizione;";
                var parm = new
                {
                    IDiscrizione = idIscrizione
                };
                con.Execute(query, parm);
                return true;
            }
            catch (MySqlException ex)
            {
                return false;
            }
        }

        public int GetNumeroIscrizioniUtente(int idUtente)
        {
            int numeroIscrizioni = 0;
            using (var con = new MySqlConnection(s))
            {
                var query = @"SELECT COUNT(*) FROM Iscrizioni WHERE IDutente = @IDutente";
                var parameters = new { IDutente = idUtente };

                numeroIscrizioni = con.ExecuteScalar<int>(query, parameters);
            }
            return numeroIscrizioni;
        }


        public IEnumerable<Evento> ListaEventiSottoiscritti(int idutente)
        {
            using var con = new MySqlConnection(s);
            var query = @"select Eventi.* from Eventi inner join Iscrizioni on Eventi.ID = Iscrizioni.IDevento where Iscrizioni.IDutente = @idutente ORDER BY Eventi.DataOra ASC";
            var parm = new
            {
                idutente
            };
            return con.Query<Evento>(query, parm);
        }

        public int InserisciUtente(Utente utente)
        {
            try
            {
                using (var con = new MySqlConnection(s))
                {
                    con.Open();
                    using (var transaction = con.BeginTransaction())
                    {
                        var query = @"INSERT INTO Utenti(Nome, Cognome, DataNascita, IDruolo, Email, Password, PIazienda) 
                                VALUES(@Nome, @Cognome, @DataNascita, @IDruolo, @Email, @Password, @PIazienda);
                                SELECT LAST_INSERT_ID()";
                        var param = new
                        {
                            Nome = utente.Nome,
                            Cognome = utente.Cognome,
                            DataNascita = utente.DataNascita,
                            IDruolo = utente.IDRuolo,
                            Email = utente.Email,
                            Password = utente.Password,
                            PIazienda = utente.PIazienda
                        };

                        int idUtente = con.ExecuteScalar<int>(query, param, transaction);
                        transaction.Commit();

                        return idUtente;
                    }
                }
            }
            catch (Exception ex)
            {
                return 0; 
            }
        }



    }
}

