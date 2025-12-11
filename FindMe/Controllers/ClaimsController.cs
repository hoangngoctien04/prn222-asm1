using System.IO;
using System.Threading.Tasks;
using FindMe.BLL.Interfaces;
using FindMe.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System; // nếu chưa có

namespace FindMe.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly IClaimService _claimService;
        private readonly IItemService _itemService;
        private readonly IService<Account> _accountService;
        // NEW: service cho lịch sử xác minh
        private readonly IService<ClaimVerification> _verificationService;

        public ClaimsController(
            IClaimService claimService,
            IItemService itemService,
            IService<Account> accountService,
            IService<ClaimVerification> verificationService)    // NEW
        {
            _claimService = claimService;
            _itemService = itemService;
            _accountService = accountService;
            _verificationService = verificationService;         // NEW
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

        // GET: Claims/Verify/5  (Xác minh thông tin)
        public IActionResult Verify(int id)
        {
            var claim = _claimService.GetById(id);
            if (claim == null)
            {
                return NotFound();
            }

            // ====== NẠP THÊM ITEM CHO CLAIM ======
            var item = _itemService.GetById(claim.ItemId);
            claim.Item = item;

            // ====== LỊCH SỬ XÁC MINH (ClaimVerifications) ======
            var verifications = _verificationService
                .GetAll()
                .Where(v => v.ClaimId == id)
                .OrderByDescending(v => v.ActionAt)
                .ToList();

            claim.ClaimVerifications = verifications;

            // ====== DANH SÁCH OFFICER =====
            var officers = _accountService
                .GetAll()
                .Where(a => a.Role == "SecurityOfficer" && a.IsActive)
                .ToList();

            ViewBag.Officers = new SelectList(officers, "AccountId", "FullName");

            return View(claim);
        }

        // POST: Claims/Verify/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Verify(
            int id,
            int officerId,
            string decision,          // "approve" hoặc "reject"
            string? note,
            IFormFile? mediaFile)
        {
            string? mediaPath = null;

            // Lưu file bằng chứng (nếu có)
            if (mediaFile != null && mediaFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "verification");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(mediaFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await mediaFile.CopyToAsync(stream);
                }

                mediaPath = "/verification/" + fileName;
            }

            var newStatus = decision == "approve" ? ClaimStatus.Approved : ClaimStatus.Rejected;
            var actionText = decision == "approve" ? "Approve" : "Reject";

            // Cập nhật Claim + log ClaimVerification + auto reject các claim khác nếu Approve
            _claimService.UpdateStatus(
                id,
                newStatus,
                officerId,
                actionText,
                mediaPath,
                note);

            // Nếu approve → đánh dấu Item là Claimed
            if (newStatus == ClaimStatus.Approved)
            {
                var claim = _claimService.GetById(id);
                if (claim != null)
                {
                    _itemService.UpdateStatus(
                        claim.ItemId,
                        ItemStatus.Claimed,
                        officerId,
                        "Item đã được claim sau khi xác minh");
                }
            }

            return RedirectToAction(nameof(Details), new { id });
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
                    // (Optional) sau này có thể lưu path này vào ClaimVerification UploadProof
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

        // POST: Claims/Approve/5 (phê duyệt nhanh)
        [HttpPost]
        public IActionResult Approve(int id)
        {
            // TODO: sau này lấy officerId từ user đăng nhập
            int? officerId = null; // nếu muốn log verify cho phê duyệt nhanh, set 1 id cố định

            _claimService.UpdateStatus(id, ClaimStatus.Approved, officerId, "ApproveQuick", null, "Phê duyệt nhanh");

            var claim = _claimService.GetById(id);
            if (claim != null)
            {
                _itemService.UpdateStatus(claim.ItemId, ItemStatus.Claimed, officerId, "Claim approved");
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Claims/Reject/5 (từ chối nhanh)
        [HttpPost]
        public IActionResult Reject(int id)
        {
            int? officerId = null;

            _claimService.UpdateStatus(id, ClaimStatus.Rejected, officerId, "RejectQuick", null, "Từ chối nhanh");

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
