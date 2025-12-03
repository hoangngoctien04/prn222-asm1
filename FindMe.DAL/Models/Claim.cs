using System;
using System.Collections.Generic;

namespace FindMe.DAL.Models;

public partial class Claim
{
    public int ClaimId { get; set; }

    public int ItemId { get; set; }

    public int ClaimerAccountId { get; set; }

    public string ClaimerName { get; set; } = null!;

    public string? ClaimerPhone { get; set; }

    public string? ClaimerEmail { get; set; }

    public string? ClaimReason { get; set; }

    public DateTime ClaimSubmittedAt { get; set; }

    public ClaimStatus ClaimStatus { get; set; }

    public int? AssignedOfficerId { get; set; }

    public string? DecisionNote { get; set; }

    public DateTime? DecisionAt { get; set; }

    public virtual Account? AssignedOfficer { get; set; }

    public virtual ICollection<ClaimVerification> ClaimVerifications { get; set; } = new List<ClaimVerification>();

    public virtual Account ClaimerAccount { get; set; } = null!;

    public virtual Item Item { get; set; } = null!;
}
