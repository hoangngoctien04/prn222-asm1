using FindMe.DAL.Interfaces;
using FindMe.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindMe.DAL.Repositories
{
    public class ItemRepository : Repository<Item>, IITemRepository
    {
        public ItemRepository(FindMeDbContext context) : base(context) { }

        public IEnumerable<Item> GetFoundItems()
        {
            return _context.Items
                .Where(x => x.ReportType == ReportType.Lost)
                .ToList();
        }

        public IEnumerable<Item> GetLostItems()
        {
            return _context.Items
                .Where(x => x.ReportType == ReportType.Found)
                .ToList ();
        }
    }
}
