using UserAPI.Domain;
using System.Threading.Tasks;
using System;

namespace UserAPI.Repository
{
    public interface IUserApiRepository
    {
        void Add<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;

        Task<bool> SaveChangesAsync();

        //Users
        Task<Users[]> GetAllUsersAsyncByNome(string nome);
        Task<Users[]> GetAllUsersAsync();
        Task<Users> GetAllUsersAsyncById(int UserId);        
    }
}