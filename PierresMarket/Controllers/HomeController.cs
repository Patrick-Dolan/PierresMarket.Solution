using Microsoft.AspNetCore.Mvc;

namespace PierresMarket.Controllers
{
  public class HomeController : Controller
  {
    public ActionResult Index()
    {
      return View();
    }
  }
}