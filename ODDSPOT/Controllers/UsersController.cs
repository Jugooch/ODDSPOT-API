using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ODDSPOT.Controllers
{
    [Route("/")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.user_id }, user);
        }

        // POST: api/verify
        [HttpPost("/verify")]
        public async Task<ActionResult<string>> ConfirmEmail(string email)
        {
            // Generate a 6-digit confirmation code
            var random = new Random();
            var confirmationCode = random.Next(100000, 999999).ToString();

            _context.UserConfirmations.Add(new UserConfirmations(email, confirmationCode));
            await _context.SaveChangesAsync();


            SendConfirmationEmail(email, confirmationCode);

            return confirmationCode;
        }
        private void SendConfirmationEmail(string email, string confirmationCode)
        {
            var fromAddress = new MailAddress(Environment.GetEnvironmentVariable("SMTP_EMAIL"), "OddSpot");
            var toAddress = new MailAddress(email);
            string pass = Environment.GetEnvironmentVariable("SMTP_PASSWORD"); ;
            string fromPassword = pass;
            const string subject = "Your Oddspot Confirmation Code!";
            string body = $"Your confirmation code is: <br/><h1>{confirmationCode}</h1>";

            var smtp = new SmtpClient
            {
                Host = Environment.GetEnvironmentVariable("SMTP_HOST"),
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


        [HttpGet("/verify")]
        public async Task<ActionResult<string>> VerifyEmail(string email)
        {
            var code = await _context.UserConfirmations.FirstOrDefaultAsync(e => e.email_address == email);

            if (code == null)
            {
                return NotFound("Code not found.");
            }

            return code.confirmation_code;
        }


        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User updatedUser)
        {
            if (id != updatedUser.user_id)
            {
                return BadRequest();
            }

            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            // Update user properties
            user.name = updatedUser.name;
            user.password = updatedUser.password;
            user.email_address = updatedUser.email_address;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}/favoriteLeagues")]
        public async Task<IActionResult> UpdateFavoriteLeagues(int id, List<FavoriteLeague> favoriteLeagues)
        {
            if (id != favoriteLeagues.ElementAt(0).user_id)
            {
                return BadRequest();
            }

            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            _context.Favorite_Leagues.RemoveRange(_context.Favorite_Leagues.Where(e => e.user_id == id).ToList());
            _context.Favorite_Leagues.AddRange(favoriteLeagues);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    Console.WriteLine("User found, but bad request...");
                    throw;
                }
            }

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.user_id == id);
        }


        // post league id to user
        // POST: api/Users
        [HttpPost("{id}/favoriteLeagues")]
        public async Task<IActionResult> InsertNewFavoriteLeagues(FavoriteLeague league)
        {
            _context.Favorite_Leagues.Add(league);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = league.user_id }, league);

        }

        [HttpGet("{id}/favoriteLeagues")]
        public async Task<ActionResult<IEnumerable<FavoriteLeague>>> GetFavoriteLeagues(int id)
        {
            var leagues = await _context.Favorite_Leagues.Where(e => e.user_id == id).ToListAsync();
            return leagues;
        }

        [HttpGet("{id}/favoriteLeagues/{league_id}")]
        public async Task<ActionResult<FavoriteLeague>> GetFavoriteLeague(int league_id)
        {
            var league = await _context.Favorite_Leagues.FindAsync(league_id);

            if (league == null)
            {
                return NotFound();
            }

            return league;
        }

        [HttpGet("/leagues")]
        public async Task<ActionResult<IEnumerable<League>>> GetLeagues()
        {
            var leagues = await _context.Leagues.ToListAsync();

            return leagues;
        }

        [HttpGet("/leagues/{id}")]
        public async Task<ActionResult<League>> GetLeagueById(int id)
        {
            var league = await _context.Leagues.FindAsync(id);

            return league;
        }
    }
}
