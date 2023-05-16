using System;
using System.ComponentModel.DataAnnotations;

namespace GestionaleWebEventi.Models
{
	public class RegistrazioneUtente
	{

        [Required(ErrorMessage = "Dato obbligatorio")]
        public string Ruolo { get; set; }

        [Required(ErrorMessage = "Dato obbligatorio")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Dato obbligatorio")]
        public string Cognome { get; set; }

        [Required(ErrorMessage = "Dato Obbligatorio")]
        public DateTime DataNascita { get; set; }

        [Required(ErrorMessage = "Dato obbligatorio")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Dato Obbligatorio")]
        [StringLength(200, MinimumLength = 6)]
        public string Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "Le password non coincidono")]
        public string ConfermaPassword { get; set; }
	}
}

