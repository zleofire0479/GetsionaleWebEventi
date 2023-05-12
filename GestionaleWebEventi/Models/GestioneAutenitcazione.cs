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
            string cognome = "Ospite";
            string s = session.GetString("nomeCognome");
            if (s != null)
            {
                cognome = s;
            }
            return cognome;
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
            session.SetString("ruoloUtente", u.NomeRuolo);
            session.SetString("nomeUtente", u.Nome);
            session.SetString("cognomeUtente", u.Cognome);
            session.SetString("PIaziendaUtente", u.PIazienda);
        }

        public void Esci()
        {
            session.SetString("ruoloUtente", "Ospite");
            session.SetString("nomeUtente", "Ospite");
            session.SetString("cognomeUtente", "");
            session.SetString("PIaziendaUtente", "");

        }
    }
}

