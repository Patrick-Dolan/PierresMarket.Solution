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
      //Image handling
      if (treat.ImageFile != null)
      {
        string wwwRootPath = _hostEnvironment.WebRootPath;
        string fileName = Path.GetFileNameWithoutExtension(treat.ImageFile.FileName);
        string extention = Path.GetExtension(treat.ImageFile.FileName);
        treat.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssffff") + extention;
        string path = Path.Combine(wwwRootPath + "/img/TreatImages/", fileName);
        using (var fileStream = new FileStream(path, FileMode.Create))
        {
          await treat.ImageFile.CopyToAsync(fileStream);
        }
      }
      // Add treat to db
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
    public async Task<IActionResult> Edit(Treat treat, int FlavorId)
    {
      if (treat.Name == null || treat.Price == 0 || treat.Description == null)
      {
        return RedirectToAction("Edit", new { id = treat.TreatId});
      }
      if (FlavorId != 0)
      {
        _db.FlavorTreats.Add(new FlavorTreat { TreatId = treat.TreatId, FlavorId = FlavorId});
      }
      // handle image changes
      if (treat.ImageFile != null)
      {
        //Delete old image
        await DeleteImage(treat.TreatId);
        //Create new image
        string wwwRootPath = _hostEnvironment.WebRootPath;
        string fileName = Path.GetFileNameWithoutExtension(treat.ImageFile.FileName);
        string extention = Path.GetExtension(treat.ImageFile.FileName);
        treat.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssffff") + extention;
        string path = Path.Combine(wwwRootPath + "/img/TreatImages/", fileName);
        using (var fileStream = new FileStream(path, FileMode.Create))
        {
          await treat.ImageFile.CopyToAsync(fileStream);
        }
      }
      _db.Entry(treat).State = EntityState.Modified;
      if (treat.ImageFile == null)
      {
        _db.Entry(treat).Property(treat => treat.ImageName).IsModified=false;
      }
      await _db.SaveChangesAsync();
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

    public async Task<IActionResult> DeleteImage(int id)
    {
      Treat foundTreat = await _db.Treats.AsNoTracking().FirstOrDefaultAsync(treat => treat.TreatId == id);
      var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "img/TreatImages/", foundTreat.ImageName);
      if (System.IO.File.Exists(imagePath))
      {
        System.IO.File.Delete(imagePath);
      }
      return null;
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      Treat foundTreat = await _db.Treats.FindAsync(id);
      //Delete image from wwwroot/img/TreatImages/
      if (foundTreat.ImageName != null)
      {
        var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "img/TreatImages/", foundTreat.ImageName);
        if (System.IO.File.Exists(imagePath))
        {
          System.IO.File.Delete(imagePath);
        }
      }
      //Delete treat from db
      _db.Treats.Remove(foundTreat);
      await _db.SaveChangesAsync();
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