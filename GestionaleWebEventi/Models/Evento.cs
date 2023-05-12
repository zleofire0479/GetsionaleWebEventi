using System;
using System.ComponentModel.DataAnnotations;

namespace GestionaleWebEventi.Models
{
	public class Evento
	{
        public int ID { get; set; }
		public string Titolo { get; set; }
        public DateTime DataOra { get; set; }
        public string Luogo { get; set; }
        public int MaxUtenti { get; set; }
        public string PIazienda { get; set; }

        public string NomeRuoloAutorizzato { get; set; }
    }
}

