using Microsoft.AspNetCore.Mvc;
using PierresMarket.Models;
using System.Linq;

namespace PierresMarket.Controllers
{
  public class HomeController : Controller
  {
    private readonly PierresMarketContext _db;
    public HomeController(PierresMarketContext db)
    {
      _db = db;
    }
    public ActionResult Index()
    {
      ViewBag.Flavors = _db.Flavors.ToList();
      return View(_db.Treats.ToList());
    }
  }
}