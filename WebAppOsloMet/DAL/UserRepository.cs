using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using WebAppOsloMet.Models;

namespace WebAppOsloMet.DAL
{
    public class UserRepository : IUserRepository
    {
        private readonly PostDbContext _db;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(ILogger<UserRepository> logger, PostDbContext db)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IEnumerable<User>?> GetAll()
        {
            try
            {
                return await _db.Users.ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("[UserRepository users ToListAsync() failed when GetAll(), error " +
                    "message: {e}", e.Message);
                return null;
            }
        }

        public async Task<User?> GetItemById(int id)
        {
            try
            {
                return await _db.Users.FindAsync(id);
            }
            catch (Exception e)
            {
                _logger.LogError("[UserRepository user FindAsync(id) failed when GetItemById for " +
                    "UserID {UserID:0000}, error message: {e}", id, e.Message);
                return null;
            }
        }

        public async Task<User?> GetUserByIdentity(string id)
        {
            //_db.Users.Find(x => x.IdentityUserId == id);
            //User user = _db.Users.FirstOrDefaultAsync(x => x.IdentityUserId == id).Result;
            return await _db.Users.FirstOrDefaultAsync(x => x.IdentityUserId == id);
        }

        public async Task<bool> Create(User user)
        {
            try
            {
                _db.Users.Add(user);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("[UserRepository user creation failed for user {@user}, error " +
                    "message: {e}", user, e.Message);
                return false;
            }
        }

        public async Task<bool> Update(User user)
        {
            try
            {
                _db.Users.Update(user);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("[UserRepository user FindAsync(id) failed when updating " +
                    "the UserID {UserID:0000}, error message: {e}", user, e.Message);
                return false;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var user = await _db.Users.FindAsync(id);
                if (user == null)
                {
                    _logger.LogError("[UserRepository] user not found for the UserID {UserID:0000}", id);
                    return false;
                }
                _db.Users.Remove(user);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("[UserRepository user deletion failed for the " +
                    "UserID {UserID:0000}, error message: {e}", id, e.Message);
                return false;
            }
        }
    }
}
