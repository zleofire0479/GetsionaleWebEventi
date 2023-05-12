using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using GestionaleWebEventi.Models;

namespace GestionaleWebEventi.Filters
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class AdminFilter : Attribute, IActionFilter
    {
        public AdminFilter()
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            
            //Prima che si esegua l'autenticazione
            GestioneAutenitcazione a = new GestioneAutenitcazione(context.HttpContext.Session);
            if (a.DammiRuoloUtente() != "Admin")
            {
                context.Result = new BadRequestObjectResult("Area reserved for administrators. This incident will be reported");
                return;
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            //Dopo che si esegua l'autenticazione
        }
    }
}


