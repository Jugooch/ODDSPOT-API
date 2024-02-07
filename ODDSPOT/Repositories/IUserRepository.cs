namespace ODDSPOT.Repositories
{
    public interface IUserRepository
    {
            Task<IEnumerable<User>> GetUsers();
            Task<User> GetUser(int userId);
            Task<User> AddUser(User user);
            Task<User> UpdateUser(User user);
            void DeleteUser(int userId);
            Task<IEnumerable<League>> GetFavoriteLeagues();
    }
}
