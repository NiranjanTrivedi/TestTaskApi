using TestTaskApi.DTOs;

namespace TestTaskApi.Interface
{
    public interface IUserRepository
    {
        bool Add(User model);
        bool Update(User model);
        bool Delete(int id);
        User FindbyId(int id);
        IEnumerable<User> GetAll();

        Task<bool> AddUser(User model);
    }
}
