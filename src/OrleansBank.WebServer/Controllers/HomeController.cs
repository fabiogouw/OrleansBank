using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrleansBank.WebServer.Models;
using Orleans.Hosting;
using Orleans;
using OrleansBank.Core.Contracts;

namespace OrleansBank.WebServer.Controllers
{
    public class HomeController : Controller
    {
        private IClusterClient _host;
        public HomeController(IClusterClient host) 
        {
            _host = host;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> About()
        {
            var account = _host.GetGrain<ICheckingAccount>("0");
            ViewData["Message"] = await account.GetBalance();

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

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
    }
}
