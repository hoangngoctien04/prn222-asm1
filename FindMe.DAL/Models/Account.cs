using System;
using System.Collections.Generic;

namespace FindMe.DAL.Models;

public partial class Account
{
    public int AccountId { get; set; }

    public string Email { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? Phone { get; set; }

    public string Role { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Claim> ClaimAssignedOfficers { get; set; } = new List<Claim>();

    public virtual ICollection<Claim> ClaimClaimerAccounts { get; set; } = new List<Claim>();

    public virtual ICollection<ClaimVerification> ClaimVerifications { get; set; } = new List<ClaimVerification>();

    public virtual ICollection<ItemHistory> ItemHistories { get; set; } = new List<ItemHistory>();

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
