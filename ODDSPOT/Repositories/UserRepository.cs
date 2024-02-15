using System.Net.Mail;
using Microsoft.EntityFrameworkCore;

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
        public async Task<UserConfirmations> ConfirmEmail(string email)
        {
            // Generate a 6-digit confirmation code
            var random = new Random();
            var confirmationCode = random.Next(100000, 999999).ToString();

            SendConfirmationEmail(email, confirmationCode);
            return await dbContext.UserConfirmations.FirstOrDefaultAsync(e => e.email_address == email);
        }

        private void SendConfirmationEmail(string email, string confirmationCode)
        {
            var fromAddress = new MailAddress("oddspotsportshub@gmail.com", "OddSpot");
            var toAddress = new MailAddress(email);
            const string fromPassword = "Oddspot1234!";
            const string subject = "Your Oddspot Confirmation Code!";
            string body = $"Your confirmation code is: <br/><h1>{confirmationCode}</h1>";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
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

                await dbContext.SaveChangesAsync();

                return result;
            }

            return null;
        }

        public async void DeleteUser(int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<FavoriteLeague>> GetFavoriteLeagues()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<FavoriteLeague>> UpdateFavoriteLeagues(int userId, IEnumerable<FavoriteLeague> favoriteLeagues)
        {
            // Retrieve the user and their current favorite leagues to update
            var user = await dbContext.Users
                                      .FirstOrDefaultAsync(u => u.user_id == userId);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            dbContext.Favorite_Leagues.RemoveRange(dbContext.Favorite_Leagues.Where(e => e.user_id == userId).ToList());
            dbContext.Favorite_Leagues.AddRange(favoriteLeagues);

            // Save changes to the database
            await dbContext.SaveChangesAsync();

            return favoriteLeagues;
        }


        public async Task<FavoriteLeague> AddFavoriteLeague(FavoriteLeague favoriteLeague)
        {
            var result = await dbContext.Favorite_Leagues.AddAsync(favoriteLeague);
            await dbContext.SaveChangesAsync();
            return result.Entity;
        }
    }
}
