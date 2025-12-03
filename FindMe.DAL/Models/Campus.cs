using System;
using System.Collections.Generic;

namespace FindMe.DAL.Models;

public partial class Campus
{
    public int CampusId { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public virtual ICollection<Location> Locations { get; set; } = new List<Location>();
}
