using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PierresMarket.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PierresMarket.Controllers
{
  public class FlavorsController : Controller
  {
    private readonly PierresMarketContext _db;
    public FlavorsController(PierresMarketContext db)
    {
      _db = db;
    }

    public ActionResult Index()
    {
      return View(_db.Flavors.ToList());
    }
  }
}