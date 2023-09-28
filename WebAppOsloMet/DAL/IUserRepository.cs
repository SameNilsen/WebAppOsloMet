using WebAppOsloMet.Models;

namespace WebAppOsloMet.DAL
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAll();
        Task<User?> GetItemById(int id);
        Task Create(User user);
        Task Update(User user);
        Task<bool> Delete(int id);
    }
}
