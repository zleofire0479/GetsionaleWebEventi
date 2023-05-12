using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GestionaleWebEventi.Models;

namespace GestionaleWebEventi.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private GestioneDati gestioneDati;

    public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
    {
        _logger = logger;
        gestioneDati = new Models.GestioneDati(configuration);
    }

    public IActionResult Index()
    {
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
        var listaEventi = gestioneDati.ListaEventi();
        return View(listaEventi);
    }
}

