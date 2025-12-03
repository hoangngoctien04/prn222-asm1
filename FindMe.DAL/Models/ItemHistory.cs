using System;
using System.Collections.Generic;

namespace FindMe.DAL.Models;

public partial class ItemHistory
{
    public int HistoryId { get; set; }

    public int ItemId { get; set; }

    public int? ChangedByAccountId { get; set; }

    public ItemStatus? FromStatus { get; set; }

    public ItemStatus ToStatus { get; set; }

    public string? Note { get; set; }

    public DateTime ChangedAt { get; set; }

    public virtual Account? ChangedByAccount { get; set; }

    public virtual Item Item { get; set; } = null!;
}
