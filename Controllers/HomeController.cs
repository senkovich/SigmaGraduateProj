using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SigmaGraduateProj.Models;

namespace SigmaGraduateProj.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly TransactionsDBContext dBContext;

        public HomeController(ILogger<HomeController> logger, TransactionsDBContext transactionsDBContext)
        {
            _logger = logger;
            dBContext = transactionsDBContext;
        }        

        public IActionResult Index()
        {
            return Redirect("~/swagger/index.html");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
