using FindMe.BLL.Interfaces;
using FindMe.BLL.Services;
using FindMe.DAL.Interfaces;
using FindMe.DAL.Models;

namespace FindMe.BLL.Services
{
    public class ClaimService : Service<Claim>, IClaimService
    {
        private readonly IRepository<ClaimVerification> _verificationRepository;

        public ClaimService(IRepository<Claim> repository, IRepository<ClaimVerification> verificationRepository) : base(repository)
        {
            _verificationRepository = verificationRepository;
        }

        public void UpdateStatus(int claimId, ClaimStatus newStatus, int? officerId, string? action, string? mediaPath)
        {
            var claim = _repository.GetById(claimId);
            if (claim != null)
            {
                claim.ClaimStatus = newStatus;
                claim.AssignedOfficerId = officerId;
                _repository.Update(claim);

                // Log verification
                var verification = new ClaimVerification
                {
                    ClaimId = claimId,
                    OfficerId = officerId ?? 0, // Assuming officerId is required
                    ClaimMediaPath = mediaPath,
                    Note = action,
                    ActionAt = DateTime.Now
                };
                _verificationRepository.Add(verification);
            }
        }
    }
}