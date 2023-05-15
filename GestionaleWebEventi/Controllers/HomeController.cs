using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GestionaleWebEventi.Models;
using Microsoft.AspNetCore.Http;

namespace GestionaleWebEventi.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration;
    private GestioneDati gestioneDati;
    private GestioneUtenti gestioneUtenti;
    private GestioneAutenitcazione gestioneAutenticazione;

    public HomeController(ILogger<HomeController> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _configuration = configuration;
        gestioneDati = new GestioneDati(configuration);
        gestioneUtenti = new GestioneUtenti(configuration);
        gestioneAutenticazione = new GestioneAutenitcazione(httpContextAccessor.HttpContext.Session);
        gestioneDati = new Models.GestioneDati(configuration);
        httpContextAccessor.HttpContext.Items["nomeUtente"] = gestioneAutenticazione.DammiNomeUtente();
        httpContextAccessor.HttpContext.Items["cognomeUtente"] = gestioneAutenticazione.DammiCognomeUtente();
        httpContextAccessor.HttpContext.Items["emailUtente"] = gestioneAutenticazione.DammiEmailUtente();
        httpContextAccessor.HttpContext.Items["ruoloUtente"] = gestioneAutenticazione.DammiRuoloUtente();
        httpContextAccessor.HttpContext.Items["PIaziendaUtente"] = gestioneAutenticazione.DammiPIazienda();

    }

    public IActionResult Index()
    {
        ViewData["nome"] = gestioneAutenticazione.DammiNomeUtente();
        ViewData["cognome"] = gestioneAutenticazione.DammiCognomeUtente();
        ViewData["ruolo"] = gestioneAutenticazione.DammiRuoloUtente();
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult ElencoEventi()
    {
        var listaEventi = gestioneDati.ListaEventi(gestioneAutenticazione.DammiRuoloUtente(), gestioneAutenticazione.DammiPIazienda());
        return View(listaEventi);
    }

    public IActionResult DettaglioEvento(int id)
    {
        Evento evento = gestioneDati.GetEvento(id);
        if (evento == null)
        {
            return NotFound();      //TODO
        }

        return View(evento);
    }

    [HttpPost]
    public IActionResult Iscriviti(int eventoId)
    {
        if (gestioneDati.InserisciIscrizione(eventoId, gestioneAutenticazione.DammiIdUtente())) {
            return RedirectToAction("ConfermaIscrizione");
        }
        else {
            return RedirectToAction("NumeroMassimoIscrizioniRaggiunto");
        }
    }

    public IActionResult ConfermaIscrizione()
    {
        return View();
    }

    public IActionResult NumeroMassimoIscrizioniRaggiunto()
    {
        return View();
    }

    public IActionResult Login()
    {
        return View(new Utente());
    }

    public IActionResult Exit()
    {
        gestioneAutenticazione.Esci();
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Login(Utente id)
    {
        Utente ut = gestioneUtenti.CercaUtente(id.Email);
        if (ut == null)
        {
            ModelState.AddModelError("", "Email non registrata");
            return View(id);
        }
        else
        {
            if (gestioneUtenti.VerificaPassword(id.Email, id.Password))
            {
                gestioneAutenticazione.ImpostaUtente(ut);
                return RedirectToAction("Index");
            } else {
                ModelState.AddModelError("", "Password errata");
                return View(id);
            }
        }
    }
}

