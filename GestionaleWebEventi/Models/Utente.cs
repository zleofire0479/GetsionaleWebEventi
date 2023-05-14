using System;
namespace GestionaleWebEventi.Models
{
    public class Utente
    {
        public string CF { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int IDRuolo { get; set; }
        public string NomeRuolo { get; set; }
        public string PIazienda { get; set; }
        public DateTime DataNascita { get; set; }
    }
}

