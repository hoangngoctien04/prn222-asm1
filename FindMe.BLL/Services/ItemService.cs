using FindMe.BLL.Interfaces;
using FindMe.DAL.Interfaces;
using FindMe.DAL.Models;

namespace FindMe.BLL.Services
{
    public class ItemService : Service<Item>, IItemService
    {
        private readonly IRepository<ItemHistory> _historyRepository;
        private readonly IITemRepository _itemRepository;

        public ItemService(IRepository<Item> repository, IRepository<ItemHistory> historyRepository, IITemRepository itemRepository) : base(repository)
        {
            _historyRepository = historyRepository;
            _itemRepository = itemRepository;
        }

        public void UpdateStatus(int itemId, ItemStatus newStatus, int? changedById, string? note)
        {
            var item = _repository.GetById(itemId);
            if (item != null)
            {
                var oldStatus = item.Status;
                item.Status = newStatus;
                _repository.Update(item);

                // Log history
                var history = new ItemHistory
                {
                    ItemId = itemId,
                    FromStatus = oldStatus,
                    ToStatus = newStatus,
                    ChangedByAccountId = changedById,
                    Note = note,
                    ChangedAt = DateTime.Now
                };
                _historyRepository.Add(history);
            }
        }

        public IEnumerable<Item> GetFoundItems()
        {
            return _itemRepository.GetFoundItems();
        }

        public IEnumerable<Item> GetLostItems()
        {
            return _itemRepository.GetLostItems();
        }
    }
}