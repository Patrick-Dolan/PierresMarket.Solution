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
  [Authorize]
  public class TreatsController : Controller
  {
    private readonly PierresMarketContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    public TreatsController(UserManager<ApplicationUser> userManager, PierresMarketContext db)
    {
      _userManager = userManager;
      _db = db;
    }

    [AllowAnonymous]
    public ActionResult Index()
    {
      return View(_db.Treats.ToList());
    }

    public ActionResult Create()
    {
      ViewBag.FlavorId = new SelectList(_db.Flavors, "FlavorId", "Name");
      return View();
    }

    [HttpPost]
    public ActionResult Create(Treat treat, int FlavorId)
    {
      _db.Treats.Add(treat);
      _db.SaveChanges();
      if(FlavorId != 0)
      {
        _db.FlavorTreats.Add(new FlavorTreat { TreatId = treat.TreatId, FlavorId = FlavorId});
        _db.SaveChanges();
      }
      return RedirectToAction("Index");
    }

    public ActionResult Details(int id)
    {
      Treat foundTreat = _db.Treats
        .Include(treat => treat.JoinEntities)
        .ThenInclude(join => join.JoinEntities)
        .FirstOrDefault(entry => entry.TreatId == id);
      ViewBag.FlavorId = new SelectList(_db.Flavors, "FlavorId", "Name");
      return View(foundTreat);
    }

    public ActionResult Edit(int id)
    {
      Treat foundTreat = _db.Treats.FirstOrDefault(entry => entry.TreatId == id);
      return View(foundTreat);
    }

    [HttpPost]
    public ActionResult Edit(Treat treat, int FlavorId)
    {
      if (treat.Name == null || treat.Price == 0 || treat.Description == null)
      {
        return RedirectToAction("Edit", new { id = treat.TreatId});
      }
      if (FlavorId != 0)
      {
        _db.FlavorTreats.Add(new FlavorTreat { TreatId = treat.TreatId, FlavorId = FlavorId});
      }
      _db.Entry(treat).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Details", new { id = treat.TreatId});
    }

    public ActionResult Delete(int id)
    {
      Treat foundTreat = _db.Treats
        .Include(treat => treat.JoinEntities)
        .ThenInclude(join => join.JoinEntities)
        .FirstOrDefault(entry => entry.TreatId == id);
      return View(foundTreat);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      Treat foundTreat = _db.Treats.FirstOrDefault(entry => entry.TreatId == id);
      _db.Treats.Remove(foundTreat);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult AddFlavor(Treat treat, int FlavorId)
    {
      bool isDuplicate = _db.FlavorTreats.Any(join => join.TreatId == treat.TreatId && join.FlavorId == FlavorId);
      if (FlavorId != 0 && isDuplicate == false)
      {
        _db.FlavorTreats.Add(new FlavorTreat() { TreatId = treat.TreatId, FlavorId = FlavorId });
        _db.SaveChanges();
      }
      return RedirectToAction("Details", new { id = treat.TreatId});
    }

    [HttpPost]
    public ActionResult DeleteFlavor(int joinId)
    {
      var joinEntry = _db.FlavorTreats.FirstOrDefault(entry => entry.FlavorTreatId == joinId);
      _db.FlavorTreats.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Details", new { id = joinEntry.TreatId});
    }
  }
}