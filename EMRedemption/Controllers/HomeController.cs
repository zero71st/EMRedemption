using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EMRedemption.Models;
using EMRedemption.Models.AppLogViewModels;
using EMRedemption.Data;

namespace EMRedemption.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult ListLogs(string keyword)
        {
            var model = new AppLogListViewModel();

            var logs = _db.AppLogs.Take(30).AsEnumerable();

            if(!string.IsNullOrEmpty(keyword))
                logs = logs.Where(l=> l.Message.Contains(keyword));

            logs = logs.ToList();

            model.AppLogs = logs.OrderByDescending(l=> l.Logged).Select(l => new AppLogViewModel
            {
                Id = l.Id,
                Level = l.Level,
                Logged = l.Logged,
                Message = l.Message,
                Callsite = l.Callsite
            }).ToList();

            model.Keyword = keyword;

            return View(model);
        }

        public IActionResult DetailLog(int id)
        {
            var model = new AppLogViewModel();

            var log = _db.AppLogs.Find(id);
            model.Logged = log.Logged;
            model.Level = log.Level;
            model.Message = log.Message;
            model.Exception = log.Exception;
            model.Callsite = log.Callsite;

            return View(model);
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
