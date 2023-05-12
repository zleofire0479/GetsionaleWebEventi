using System;
namespace GestionaleWebEventi.Models
{
	public class Iscrizione
	{
        public int ID { get; set; }
        public DateTime Data { get; set; }
        public int IDevento { get; set; }
        public string CFutente { get; set; }
    }
}

