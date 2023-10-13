using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using WebAppOsloMet.Models;

namespace WebAppOsloMet.DAL
{
    public class PostRepository : IPostRepository
    {
        private readonly PostDbContext _db;

        public PostRepository(PostDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Post>> GetAll()
        {
            _db.Posts.Where(x => x.SubForum == "Gaming");
            return await _db.Posts.ToListAsync();
        }

        public IEnumerable<Post> GetBySubForum(string forum)
        {
            return _db.Posts.Where(x => x.SubForum == forum);
        }

        public async Task<Post?> GetItemById(int id)
        {
            return await _db.Posts.FindAsync(id);
        }

        public async Task Create(Post post)
        {
            _db.Posts.Add(post);
            await _db.SaveChangesAsync();
        }

        public async Task Update(Post post)
        {
            _db.Posts.Update(post);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> Delete(int id)
        {
            var post = await _db.Posts.FindAsync(id);
            if (post == null)
            {
                return false;
            }
            _db.Posts.Remove(post);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
