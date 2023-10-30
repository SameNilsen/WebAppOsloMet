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

        //  Repository method for getting all posts from database.
        public async Task<IEnumerable<Post>?> GetAll()
        {         
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

        //  Method for getting all posts belonging to a subforum.
        public IEnumerable<Post>? GetBySubForum(string forum)
        {
            try
            {
                return _db.Posts.Where(x => x.SubForum == forum);  //  A query to get all posts that has the specified forum as SubForum.
            }
            catch (Exception e)
            {
                _logger.LogError("[PostRepository post Where(x => x.SubForum == forum) failed when GetBySubForum" +
                    "  error message: {e}", e.Message);
                return null;
            }
        }

        //  Method for getting a single post.
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

        //  When creating a post, this method adds it to the database.
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

        //  When updating a post, this method updates the database with the post.
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

        //  When deleting a post, this method deletes it from the database.
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
