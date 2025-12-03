using FindMe.BLL.Interfaces;
using FindMe.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FindMe.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly IClaimService _claimService;
        private readonly IItemService _itemService;
        private readonly IService<Account> _accountService;

        public ClaimsController(IClaimService claimService, IItemService itemService, IService<Account> accountService)
        {
            _claimService = claimService;
            _itemService = itemService;
            _accountService = accountService;
        }

        // GET: Claims
        public IActionResult Index()
        {
            var claims = _claimService.GetAll();
            return View(claims);
        }

        // GET: Claims/Details/5
        public IActionResult Details(int id)
        {
            var claim = _claimService.GetById(id);
            if (claim == null)
            {
                return NotFound();
            }
            return View(claim);
        }

        // GET: Claims/Create
        public IActionResult Create()
        {
            ViewData["ItemId"] = new SelectList(_itemService.GetAll(), "ItemId", "Title");
            ViewData["ClaimerAccountId"] = new SelectList(_accountService.GetAll(), "AccountId", "FullName");
            return View();
        }

        // POST: Claims/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Claim claim, IFormFile? mediaFile)
        {
            if (ModelState.IsValid)
            {
                if (mediaFile != null)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(mediaFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await mediaFile.CopyToAsync(stream);
                    }
                    // For claim, perhaps set in verification later
                }
                claim.ClaimSubmittedAt = DateTime.Now;
                claim.ClaimStatus = ClaimStatus.Pending;
                _claimService.Add(claim);
                return RedirectToAction(nameof(Index));
            }
            ViewData["ItemId"] = new SelectList(_itemService.GetAll(), "ItemId", "Title", claim.ItemId);
            ViewData["ClaimerAccountId"] = new SelectList(_accountService.GetAll(), "AccountId", "FullName", claim.ClaimerAccountId);
            return View(claim);
        }

        // POST: Claims/Approve/5
        [HttpPost]
        public IActionResult Approve(int id)
        {
            _claimService.UpdateStatus(id, ClaimStatus.Approved, null, "Approved", null);
            var claim = _claimService.GetById(id);
            if (claim != null)
            {
                _itemService.UpdateStatus(claim.ItemId, ItemStatus.Claimed, null, "Claim approved");
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Claims/Reject/5
        [HttpPost]
        public IActionResult Reject(int id)
        {
            _claimService.UpdateStatus(id, ClaimStatus.Rejected, null, "Rejected", null);
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}