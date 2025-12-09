using FindMe.BLL.Interfaces;
using FindMe.DAL.Models;

namespace FindMe.BLL.Interfaces
{
    public interface IItemService : IService<Item>
    {
        void UpdateStatus(int itemId, ItemStatus newStatus, int? changedById, string? note);

        IEnumerable<Item> GetLostItems();
        IEnumerable<Item> GetFoundItems();
    }
}