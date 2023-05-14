using System;
using GestionaleWebEventi.Models;
using Microsoft.AspNetCore.Mvc;
using GestionaleWebEventi.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

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
            gestioneDati = new GestioneDati(configuration);
            gestioneUtenti = new GestioneUtenti(configuration);
            gestioneAutenticazione = new GestioneAutenitcazione(httpContextAccessor.HttpContext.Session);
            httpContextAccessor.HttpContext.Items["nomeUtente"] = gestioneAutenticazione.DammiNomeUtente();
            httpContextAccessor.HttpContext.Items["cognomeUtente"] = gestioneAutenticazione.DammiCognomeUtente();
            httpContextAccessor.HttpContext.Items["emailUtente"] = gestioneAutenticazione.DammiEmailUtente();
            httpContextAccessor.HttpContext.Items["ruoloUtente"] = gestioneAutenticazione.DammiRuoloUtente();
            httpContextAccessor.HttpContext.Items["ruoloUtente"] = gestioneAutenticazione.DammiRuoloUtente();
            httpContextAccessor.HttpContext.Items["PIaziendaUtente"] = gestioneAutenticazione.DammiPIazienda();
        }

        public IActionResult ModificaEventi()
        {
            var listaEventi = gestioneDati.ListaEventi(gestioneAutenticazione.DammiPIazienda());
            return View(listaEventi);
        }

        public IActionResult AggiungiEvento()
        {
            ViewData["listaEventi"] = gestioneDati.ListaEventi(gestioneAutenticazione.DammiRuoloUtente(), gestioneAutenticazione.DammiPIazienda());
            ViewData["listaruoli"] = gestioneDati.GetRuoli(gestioneAutenticazione.DammiPIazienda());
            Evento evento = new Evento();
            return View(evento);
        }

        [HttpPost]
        public IActionResult AggiungiEvento(Evento evento)
        {
            evento.PIazienda = gestioneAutenticazione.DammiPIazienda();
            if (!ModelState.IsValid)
            {
                ViewData["listaEventi"] = gestioneDati.ListaEventi(gestioneAutenticazione.DammiRuoloUtente(), gestioneAutenticazione.DammiPIazienda());
                ViewData["listaruoli"] = gestioneDati.GetRuoli(gestioneAutenticazione.DammiPIazienda());
                return View(evento);
            }
            var id = gestioneDati.InserisciEvento(evento);
            if (id != 0)
            {
                if (gestioneDati.InserisciAutorizzazioni(id, gestioneAutenticazione.DammiPIazienda(), evento.NomeRuoliAutorizzati) ) {
                    return RedirectToAction("ElencoEventi");
                }
                ModelState.AddModelError("", "Errore creazione autorizzazioni");
                return View(evento);
            }
            ViewData["listaEventi"] = gestioneDati.ListaEventi(gestioneAutenticazione.DammiRuoloUtente(), gestioneAutenticazione.DammiPIazienda());
            ViewData["listaruoli"] = gestioneDati.GetRuoli(gestioneAutenticazione.DammiPIazienda());
            ModelState.AddModelError("", "Errore di inserimento: dati non validi o errore nel database");
            return View(evento);
        }

        
    }
}


