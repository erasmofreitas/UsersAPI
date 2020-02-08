using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserAPI.Domain;
using System;
namespace UserAPI.Repository
{
    public class UserApiRepository : IUserApiRepository
    {
        private readonly UserApiContext _context;
        public UserApiRepository(UserApiContext context)
        {
            _context = context;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
        public void Add<T>(T entity) where T : class
        {

                _context.Add(entity);


        }

        public void Update<T>(T entity) where T : class
        {
            _context.Update(entity);
        }
        
        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }
        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }


        //Users
        public async Task<Users[]> GetAllUsersAsync()
        {
            IQueryable<Users> query = _context.users;

            query = query.OrderByDescending(c => c.BirthDate);
            return await query.ToArrayAsync();
        }
        public async Task<Users[]> GetAllUsersAsyncByNome(string nome)
        {
            IQueryable<Users> query = _context.users;

            query = query.OrderByDescending(c => c.BirthDate)
                            .Where(r => r.Name.ToLower().Contains(nome.ToLower()));
            return await query.ToArrayAsync();
        }

        public async Task<Users> GetAllUsersAsyncById(int UserId)
        {
            IQueryable<Users> query = _context.users;

            query = query.OrderByDescending(c => c.BirthDate)
                            .Where(c => c.Id == UserId);
                            
            return await query.FirstOrDefaultAsync();
        }
 
    }
}