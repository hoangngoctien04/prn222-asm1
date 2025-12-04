using FindMe.BLL.Interfaces;
using FindMe.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FindMe.Controllers
{
    public class ItemsController : Controller
    {
        private readonly IItemService _itemService;
        private readonly IService<Campus> _campusService;
        private readonly IService<Location> _locationService;
        private readonly IService<Account> _accountService;

        public ItemsController(IItemService itemService, IService<Campus> campusService, IService<Location> locationService, IService<Account> accountService)
        {
            _itemService = itemService;
            _campusService = campusService;
            _locationService = locationService;
            _accountService = accountService;
        }

        // GET: Items
        public IActionResult Index()
        {
            var items = _itemService.GetAll();
            return View(items);
        }

        // GET: Items/Details/5
        public IActionResult Details(int id)
        {
            var item = _itemService.GetById(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // GET: Items/Create
        public IActionResult Create()
        {
            ViewData["CampusId"] = new SelectList(_campusService.GetAll(), "CampusId", "Name");
            ViewData["LocationId"] = new SelectList(_locationService.GetAll(), "LocationId", "Name");
            ViewData["ReporterAccountId"] = new SelectList(_accountService.GetAll(), "AccountId", "FullName");
            return View();
        }

        // POST: Items/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Item item, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    item.ImagePath = "/images/" + fileName;
                }
                item.ReportedAt = DateTime.Now;
                item.Status = ItemStatus.Pending;
                _itemService.Add(item);
                return RedirectToAction(nameof(Index));
            }
            ViewData["CampusId"] = new SelectList(_campusService.GetAll(), "CampusId", "Name", item.CampusId);
            ViewData["LocationId"] = new SelectList(_locationService.GetAll(), "LocationId", "Name", item.LocationId);
            ViewData["ReporterAccountId"] = new SelectList(_accountService.GetAll(), "AccountId", "FullName", item.ReporterAccountId);
            return View(item);
        }

        // GET: Items/Edit/5
        public IActionResult Edit(int id)
        {
            var item = _itemService.GetById(id);
            if (item == null)
            {
                return NotFound();
            }
            ViewData["CampusId"] = new SelectList(_campusService.GetAll(), "CampusId", "Name", item.CampusId);
            ViewData["LocationId"] = new SelectList(_locationService.GetAll(), "LocationId", "Name", item.LocationId);
            ViewData["ReporterAccountId"] = new SelectList(_accountService.GetAll(), "AccountId", "FullName", item.ReporterAccountId);
            return View(item);
        }

        // GET: Item/Report/LostItem
        [HttpGet]
        public IActionResult RepostLost()
        {
            return View(new Item
            {
                ReportType = ReportType.Lost,
                ReportedAt = DateTime.Now
            });
        }

        // POST: Item/Report/LostItem/List
        [HttpPost]
        public IActionResult RepostLost(Item item)
        {
            if (!ModelState.IsValid) return View(item);
            
            item.ReportType = ReportType.Lost;
            item.ReportedAt = DateTime.Now;
            item.Status = ItemStatus.Pending;

            _itemService.Add(item);

            return RedirectToAction("LostList");
        }

        public IActionResult LostList()
        {
            var list = _itemService.GetLostItems();
            return View(list);
        }

        // GET: Item/Report/FoundItem
        [HttpGet]
        public IActionResult RepostFound()
        {
            return View(new Item
            {
                ReportType = ReportType.Found,
                ReportedAt = DateTime.Now
            });
        }

        // POST: Item/Report/LostItem/List
        [HttpPost]
        public IActionResult RepostFound(Item item)
        {
            if (!ModelState.IsValid) return View(item);

            item.ReportType = ReportType.Found;
            item.ReportedAt = DateTime.Now;
            item.Status = ItemStatus.Pending;

            _itemService.Add(item);

            return RedirectToAction("FoundList");
        }

        public IActionResult FoundList()
        {
            var list = _itemService.GetFoundItems();
            return View(list);
        }

        // POST: Items/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Item item, IFormFile? imageFile)
        {
            if (id != item.ItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    item.ImagePath = "/images/" + fileName;
                }
                _itemService.Update(item);
                return RedirectToAction(nameof(Index));
            }
            ViewData["CampusId"] = new SelectList(_campusService.GetAll(), "CampusId", "Name", item.CampusId);
            ViewData["LocationId"] = new SelectList(_locationService.GetAll(), "LocationId", "Name", item.LocationId);
            ViewData["ReporterAccountId"] = new SelectList(_accountService.GetAll(), "AccountId", "FullName", item.ReporterAccountId);
            return View(item);
        }

        // GET: Items/Delete/5
        public IActionResult Delete(int id)
        {
            var item = _itemService.GetById(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // POST: Items/Verify/5
        [HttpPost]
        public IActionResult Verify(int id)
        {
            _itemService.UpdateStatus(id, ItemStatus.Verified, null, "Verified by officer");
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Items/Claim/5
        [HttpPost]
        public IActionResult Claim(int id)
        {
            _itemService.UpdateStatus(id, ItemStatus.Claimed, null, "Marked as claimed");
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Items/Return/5
        [HttpPost]
        public IActionResult Return(int id)
        {
            _itemService.UpdateStatus(id, ItemStatus.Returned, null, "Marked as returned");
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}