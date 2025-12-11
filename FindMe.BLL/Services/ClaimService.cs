using System;
using System.Linq;                     
using FindMe.BLL.Interfaces;
using FindMe.DAL.Interfaces;
using FindMe.DAL.Models;

namespace FindMe.BLL.Services
{
    public class ClaimService : Service<Claim>, IClaimService
    {
        private readonly IRepository<ClaimVerification> _verificationRepository;

        public ClaimService(
            IRepository<Claim> repository,
            IRepository<ClaimVerification> verificationRepository)
            : base(repository)
        {
            _verificationRepository = verificationRepository;
        }

        public void UpdateStatus(
            int claimId,
            ClaimStatus newStatus,
            int? officerId,
            string action,
            string? mediaPath,
            string? actionNote = null)
        {
            var claim = _repository.GetById(claimId);
            if (claim == null)
            {
                throw new ArgumentException($"Claim {claimId} không tồn tại.", nameof(claimId));
            }

            // ======================
            // 1. Cập nhật claim hiện tại
            // ======================
            claim.ClaimStatus = newStatus;

            if (officerId.HasValue)
            {
                claim.AssignedOfficerId = officerId.Value;
            }

            claim.DecisionAt = DateTime.Now;
            claim.DecisionNote = !string.IsNullOrWhiteSpace(actionNote)
                ? actionNote
                : action;

            _repository.Update(claim);

            // Log XÁC MINH THÔNG TIN cho claim hiện tại
            if (officerId.HasValue)
            {
                var verification = new ClaimVerification
                {
                    ClaimId = claimId,
                    OfficerId = officerId.Value,
                    ClaimMediaPath = mediaPath,
                    Action = action,
                    ActionNote = actionNote,
                    ActionAt = DateTime.Now
                };

                _verificationRepository.Add(verification);
            }

            // ==========================================================
            // 2. Processing 2: NẾU APPROVE → auto reject các claim khác
            //    cùng Item đang Pending (nhiều người claim 1 món đồ)
            // ==========================================================
            if (newStatus == ClaimStatus.Approved)
            {
                var otherClaims = _repository
                    .GetAll()
                    .Where(c => c.ItemId == claim.ItemId      // cùng Item
                                && c.ClaimId != claimId        // không phải claim hiện tại
                                && c.ClaimStatus == ClaimStatus.Pending)
                    .ToList();

                foreach (var other in otherClaims)
                {
                    other.ClaimStatus = ClaimStatus.Rejected;
                    other.DecisionAt = DateTime.Now;
                    other.DecisionNote = "Tự động từ chối: item đã được duyệt cho người khác";
                    _repository.Update(other);

                    if (officerId.HasValue)
                    {
                        _verificationRepository.Add(new ClaimVerification
                        {
                            ClaimId = other.ClaimId,
                            OfficerId = officerId.Value,
                            ClaimMediaPath = null,
                            Action = "AutoRejectConflict",
                            ActionNote = "Có claim khác đã được approve cho item này",
                            ActionAt = DateTime.Now
                        });
                    }
                }
            }
        }
    }
}
