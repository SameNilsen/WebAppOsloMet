using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using WebAppOsloMet.Models;

namespace WebAppOsloMet.DAL
{
    public class PostRepository : IPostRepository
    {
        private readonly PostDbContext _db;
        private readonly ILogger<PostRepository> _logger;

        public PostRepository(ILogger<PostRepository> logger, PostDbContext db)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IEnumerable<Post>?> GetAll()
        {
            //_db.Posts.Where(x => x.SubForum == "Gaming");
            try
            {
                return await _db.Posts.ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("[PostRepository posts ToListAsync() failed when GetAll(), error " +
                    "message: {e}", e.Message);
                return null;
            }
        }

        public IEnumerable<Post> GetBySubForum(string forum)
        {
            return _db.Posts.Where(x => x.SubForum == forum);
        }

        public async Task<Post?> GetItemById(int id)
        {
            try
            {
                return await _db.Posts.FindAsync(id);
            }
            catch (Exception e)
            {
                _logger.LogError("[PostRepository post FindAsync(id) failed when GetItemById for" +
                    " PostID {PostID:0000}, error message: {e}", id, e.Message);
                return null;
            }
        }

        public async Task<bool> Create(Post post)
        {
            try
            {
                _db.Posts.Add(post);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("[PostRepository post creation failed for post {@post}, error" +
                    "message: {e}", post, e.Message);
                return false;
            }
        }

        public async Task<bool> Update(Post post)
        {
            try
            {
                _db.Posts.Update(post);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("[PostRepository post FindAsync(id) failed when updating the" +
                    " PostID {PostID:0000}, error message: {e}", post, e.Message);
                return false;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var post = await _db.Posts.FindAsync(id);
                if (post == null)
                {
                    _logger.LogError("[PostRepository post not found for the PostID {PostID:0000}", id);
                    return false;
                }
                _db.Posts.Remove(post);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("[PostRepository post deletion failed for " +
                    "the PostID {PostID:0000}, error message: {e}", id, e.Message);
                return false;
            }
        }
    }
}
