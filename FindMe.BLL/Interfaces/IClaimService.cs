using FindMe.BLL.Interfaces;
using FindMe.DAL.Models;

namespace FindMe.BLL.Interfaces
{
    public interface IClaimService : IService<Claim>
    {
        void UpdateStatus(int claimId, ClaimStatus newStatus, int? officerId, string? action, string? mediaPath);
    }
}