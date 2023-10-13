using WebAppOsloMet.Models;

namespace WebAppOsloMet.DAL
{
    public interface IPostRepository
    {
        Task<IEnumerable<Post>> GetAll();
        IEnumerable<Post> GetBySubForum(string forum);
        Task<Post?> GetItemById(int id);
        Task Create(Post post);
        Task Update(Post post);
        Task<bool> Delete(int id);
    }
}
