using System;
using GestionaleWebEventi.Models;
using Microsoft.AspNetCore.Mvc;
using GestionaleWebEventi.Filters;

namespace GestionaleWebEventi.Controllers
{
    [AdminFilter]
    public class AdminController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private GestioneDati gestioneDati;
        private GestioneUtenti gestioneUtenti;
        private GestioneAutenitcazione gestioneAutenticazione;
        public AdminController(ILogger<HomeController> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _configuration = configuration;
            //string idsessione = httpContextAccessor.HttpContext.Session.Id;
            //Console.WriteLine("ID è " + idsessione);
            gestioneDati = new GestioneDati(configuration);
            gestioneUtenti = new GestioneUtenti(configuration);
            gestioneAutenticazione = new GestioneAutenitcazione(httpContextAccessor.HttpContext.Session);
            httpContextAccessor.HttpContext.Items["nomeUtente"] = gestioneAutenticazione.DammiNomeUtente();
            httpContextAccessor.HttpContext.Items["ruoloUtente"] = gestioneAutenticazione.DammiRuoloUtente();
        }

        public IActionResult AggiungiEvento()
        {
            ViewData["listaEventi"] = gestioneDati.ListaEventi();
            ViewData["listaruoli"] = gestioneDati.GetRuoli(gestioneAutenticazione.DammiPIazienda());
            return View(new Evento());
        }

        [HttpPost]
        public IActionResult AggiungiEvento(Evento evento)
        {
            if (!ModelState.IsValid)
            {
                ViewData["listaEventi"] = gestioneDati.ListaEventi();
                return View(evento);
            }
            if (gestioneDati.InserisciEvento(evento) == true)
            {

                return RedirectToAction("ElencoEventi");
            }
            else
            {
                ModelState.AddModelError("", "Errore di inserimento: dati non validi");
                return View(evento);
            }
        }

        
    }
}

