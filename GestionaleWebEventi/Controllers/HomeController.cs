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
        httpContextAccessor.HttpContext.Items["idUtente"] = gestioneAutenticazione.DammiIdUtente();
    }

    public IActionResult Index()
    {
        ViewData["nome"] = gestioneAutenticazione.DammiNomeUtente();
        ViewData["cognome"] = gestioneAutenticazione.DammiCognomeUtente();
        ViewData["ruolo"] = gestioneAutenticazione.DammiRuoloUtente();
        ViewData["NumIscrizioni"] = gestioneDati.GetNumeroIscrizioniUtente(gestioneAutenticazione.DammiIdUtente());
        ViewData["NumEventi"] = gestioneDati.GetNumEventi(gestioneAutenticazione.DammiRuoloUtente(), gestioneAutenticazione.DammiPIazienda());
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

    public IActionResult Registrazione(string id)
    {
        ViewBag.Parametro = id;
        var listaRuoli = gestioneDati.GetRuoli(id).ToList();
        listaRuoli.Remove("Admin");
        ViewBag.ListaRuoli = listaRuoli;
        return View();
    }
    //https://localhost:7126/Home/Registrazione/873286723
    [HttpPost]
    public IActionResult Registrazione(RegistrazioneUtente model, string Parametro)
    {
        ViewBag.Parametro = Parametro;
        var listaRuoli = gestioneDati.GetRuoli(Parametro).ToList();
        listaRuoli.Remove("Admin");
        ViewBag.ListaRuoli = listaRuoli;
        if (ModelState.IsValid)
        {
            Utente nuovo = new Utente()
            {
                Email = model.Email,
                Nome = model.Nome,
                Cognome = model.Cognome,
                Password = gestioneUtenti.ComputeSha256(model.Password),
                IDRuolo = gestioneDati.GetIdRuolo(model.Ruolo, Parametro),
                NomeRuolo = model.Ruolo,
                PIazienda = Parametro,
                DataNascita = model.DataNascita
            };
            if (gestioneDati.InserisciUtente(nuovo) != 0)
            {
                // Utente aggiunto con successo
                gestioneAutenticazione.ImpostaUtente(nuovo);
                return RedirectToAction("Index");
            }
            else
            {
                // Gestione dell'errore di inserimento utente
                ModelState.AddModelError("", "Errore durante l'inserimento dell'utente.");
                return View(model);
            }
        }

        // Se il modello non è valido, ritorna la view con gli errori di validazione
        return View(model);
    }

    public IActionResult Login()
    {
        return View(new Utente());
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
            }
            else
            {
                ModelState.AddModelError("", "Password errata");
                return View(id);
            }
        }
    }

    public IActionResult Exit()
    {
        gestioneAutenticazione.Esci();
        return RedirectToAction("Index");
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

    public IActionResult VisualizzaIscrizioni()
    {
        var listaIscrizioni = gestioneDati.ListaEventiSottoiscritti(gestioneAutenticazione.DammiIdUtente());
        return View(listaIscrizioni);
    }

    [HttpPost]
    public IActionResult Iscriviti(int eventoId)
    {
        if (gestioneDati.InserisciIscrizione(eventoId, gestioneAutenticazione.DammiIdUtente())) {
            gestioneUtenti.InvioMail(gestioneAutenticazione.DammiEmailUtente(), eventoId);
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

    public IActionResult AnnullaIscrizione(int id)
    {
        var idUtente = gestioneAutenticazione.DammiIdUtente();
        if (idUtente != 0) {
            var idIscrizione = gestioneDati.GetIdIscrizione(id, idUtente);
            if (gestioneDati.AnnullaIscrizione(idIscrizione))
            {
                return View(idIscrizione);
            }
            else
            {
                return View("ErroreAnnullamentoIscrizione");
            }
        } else {
            return View("Login");
        }
        
    }

    public IActionResult NumeroMassimoIscrizioniRaggiunto()
    {
        return View();
    }

}

