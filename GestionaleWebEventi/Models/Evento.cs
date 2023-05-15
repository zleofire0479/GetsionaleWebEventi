using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GestionaleWebEventi.Models
{
	public class Evento
	{
        public int ID { get; set; }
		public string Titolo { get; set; }
        [ValidazioneData(ErrorMessage = "La data deve essere successiva alla data attuale.")]
        public DateTime DataOra { get; set; }
        public string Luogo { get; set; }
        public int MaxUtenti { get; set; }
        public string PIazienda { get; set; } = "";
        public IEnumerable<string> NomeRuoliAutorizzati { get; set; } = new List<string>();
    }
}

