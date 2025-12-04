using FindMe.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindMe.DAL.Interfaces
{
    public interface IITemRepository : IRepository<Item>
    {
        IEnumerable<Item> GetLostItems();
        IEnumerable<Item> GetFoundItems();
    }
}
