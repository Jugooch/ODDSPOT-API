using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static ODDSPOT.Repositories.UserRepository;

namespace ODDSPOT.Repositories
{
    public class UserRepository : IUserRepository
    {
            private readonly AppDbContext dbContext;

            public UserRepository(AppDbContext dbContext)
            {
                this.dbContext = dbContext;
            }

            public async Task<IEnumerable<User>> GetUsers()
            {
                return await dbContext.Users.ToListAsync();
            }

            public async Task<User> GetUser(int userId)
            {
                return await dbContext.Users
                    .FirstOrDefaultAsync(e => e.user_id == userId);
            }

            public async Task<User> AddUser(User user)
            {
                var result = await dbContext.Users.AddAsync(user);
                await dbContext.SaveChangesAsync();
                return result.Entity;
            }

            public async Task<User> UpdateUser(User user)
            {
                var result = await dbContext.Users
                        .FirstOrDefaultAsync(e => e.user_id == user.user_id);

                if (result != null)
                {
                    result.name = user.name;
                    result.password = user.password;
                    result.email_address = user.email_address;
                    result.FavoriteLeagues = user.FavoriteLeagues;

                    await dbContext.SaveChangesAsync();

                    return result;
                }

                return null;
            }

            public async void DeleteUser(int userId)
            {
                throw new NotImplementedException();
            }

            public async Task<IEnumerable<League>> GetFavoriteLeagues()
            {
                throw new NotImplementedException();
            }

            public async Task<IEnumerable<League>> UpdateFavoriteLeagues()
            {
                throw new NotImplementedException();
            }
    }
}
