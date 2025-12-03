using System;
using System.Collections.Generic;

namespace FindMe.DAL.Models;

public partial class Location
{
    public int LocationId { get; set; }

    public int CampusId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public virtual Campus Campus { get; set; } = null!;

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
