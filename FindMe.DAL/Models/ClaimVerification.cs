using System;
using System.Collections.Generic;

namespace FindMe.DAL.Models;

public partial class ClaimVerification
{
    public int VerificationId { get; set; }

    public int ClaimId { get; set; }

    public int OfficerId { get; set; }

    public string? ClaimMediaPath { get; set; }

    public string Action { get; set; } = null!;

    public string? ActionNote { get; set; }

    public DateTime ActionAt { get; set; }   

    public virtual Claim Claim { get; set; } = null!;

    public virtual Account Officer { get; set; } = null!;
}
