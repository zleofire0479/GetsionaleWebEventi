using System;
namespace GestionaleWebEventi.Models
{
	public class GestioneAutenitcazione
	{
        private ISession session;
        public GestioneAutenitcazione(ISession sessione)
        {
            session = sessione;
        }

        public int DammiIdUtente()
        {
            int id = 0;
            var s = session.GetInt32("idUtente");
            if (s != null)
            {
                id = (int)s;
            }
            return id;
        }

        public string DammiNomeUtente()
        {
            string nome = "Ospite";
            string s = session.GetString("nomeUtente");
            if (s != null)
            {
                nome = s;
            }
            return nome;
        }

        public string DammiCognomeUtente()
        {
            string cognome = "";
            string s = session.GetString("cognomeUtente");
            if (s != null)
            {
                cognome = s;
            }
            return cognome;
        }

        public string DammiEmailUtente()
        {
            string email = "";
            string s = session.GetString("emailUtente");
            if (s != null)
            {
                email = s;
            }
            return email;
        }

        public string DammiPIazienda()
        {
            string PI = "";
            string s = session.GetString("PIaziendaUtente");
            if (s != null)
            {
                PI = s;
            }
            return PI;
        }

        public string DammiRuoloUtente()
        {
            string ruolo = "Ospite";
            string s = session.GetString("ruoloUtente");
            if (s != null)
            {
                ruolo = s;
            }
            return ruolo;
        }

        public void ImpostaUtente(Utente u)
        {
            session.SetInt32("idUtente", u.ID);
            session.SetString("ruoloUtente", u.NomeRuolo);
            session.SetString("emailUtente", u.Email);
            session.SetString("nomeUtente", u.Nome);
            session.SetString("cognomeUtente", u.Cognome);
            session.SetString("PIaziendaUtente", u.PIazienda);
        }

        public void Esci()
        {
            session.SetInt32("idUtente", 0);
            session.SetString("ruoloUtente", "Ospite");
            session.SetString("emailUtente", "");
            session.SetString("nomeUtente", "Ospite");
            session.SetString("cognomeUtente", "");
            session.SetString("PIaziendaUtente", "");

        }
    }
}

