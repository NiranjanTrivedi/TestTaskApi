using TestTaskApi.Database;
using TestTaskApi.DTOs;
using TestTaskApi.Interface;

namespace TestTaskApi.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseContext context;
        public UserRepository(DatabaseContext context)
        {
            this.context=context;
        }
        public bool Add(User model)
        {
            try
            {
                context.Users.Add(model);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> AddUser(User model)
        {
            context.Users.Add(model);
            await context.SaveChangesAsync();
            return true;
        }

        public bool Delete(int id)
        {
            var data = this.FindbyId(id);
            if (data == null)
                return false;
            context.Users.Remove(data);
            context.SaveChanges();
            return true;
        }

        public User FindbyId(int id)
        {
            return context.Users.Find(id);
        }

        public IEnumerable<User> GetAll()
        {
            return context.Users.ToList();
        }

        public bool Update(User model)
        {
            try
            {
                context.Users.Update(model);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
