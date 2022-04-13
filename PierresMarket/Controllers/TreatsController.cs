using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PierresMarket.Models;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PierresMarket.Controllers
{
  [Authorize]
  public class TreatsController : Controller
  {
    private readonly PierresMarketContext _db;
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly UserManager<ApplicationUser> _userManager;
    public TreatsController(UserManager<ApplicationUser> userManager, PierresMarketContext db, IWebHostEnvironment hostEnvironment)
    {
      _userManager = userManager;
      _db = db;
      _hostEnvironment = hostEnvironment;
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
    public async Task<ActionResult> Create(Treat treat, int FlavorId)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      treat.User = currentUser;
      //Image info
      string wwwRootPath = _hostEnvironment.WebRootPath;
      string fileName = Path.GetFileNameWithoutExtension(treat.ImageFile.FileName);
      string extention = Path.GetExtension(treat.ImageFile.FileName);
      treat.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssffff") + extention;
      string path = Path.Combine(wwwRootPath + "/img/TreatImages/", fileName);
      using (var fileStream = new FileStream(path, FileMode.Create))
      {
        await treat.ImageFile.CopyToAsync(fileStream);
      }
      _db.Treats.Add(treat);
      _db.SaveChanges();
      if(FlavorId != 0)
      {
        _db.FlavorTreats.Add(new FlavorTreat { TreatId = treat.TreatId, FlavorId = FlavorId});
        _db.SaveChanges();
      }
      return RedirectToAction("Index");
    }

    [AllowAnonymous]
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