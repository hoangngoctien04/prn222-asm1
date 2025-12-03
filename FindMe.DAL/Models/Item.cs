using System;
using System.Collections.Generic;

namespace FindMe.DAL.Models;

public partial class Item
{
    public int ItemId { get; set; }

    public int CampusId { get; set; }

    public int? LocationId { get; set; }

    public int? ReporterAccountId { get; set; }

    public ReportType ReportType { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? Identifiers { get; set; }

    public DateTime ReportedAt { get; set; }

    public ItemStatus Status { get; set; }

    public string? StorageCode { get; set; }

    public string? ImagePath { get; set; }

    public virtual Campus Campus { get; set; } = null!;

    public virtual ICollection<Claim> Claims { get; set; } = new List<Claim>();

    public virtual ICollection<ItemHistory> ItemHistories { get; set; } = new List<ItemHistory>();

    public virtual Location? Location { get; set; }

    public virtual Account? ReporterAccount { get; set; }
}
