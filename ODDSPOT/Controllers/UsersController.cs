﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ODDSPOT.Controllers
{
    [Route("/")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context; // Replace YourDbContext with actual DbContext

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
        public async Task<IActionResult> UpdateFavoriteLeagues(int id, List<League> favoriteLeagues)
        {
            var user = await _context.Users.Include(u => u.Favorite_Leagues).FirstOrDefaultAsync(u => u.user_id == id);

            if (user == null)
            {
                return NotFound();
            }

            // Clear existing favorite leagues for the user
            user.Favorite_Leagues.Clear();

            // Add new favorite leagues
            user.Favorite_Leagues.AddRange(favoriteLeagues);

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

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.user_id == id);
        }


        // post league id to user
        // POST: api/Users
        [HttpPost("{id}/favoriteLeagues")]
        public async Task<IActionResult> InsertNewFavoriteLeagues(FavoriteLeague league_id)
        {
            _context.Favorite_Leagues.Add(league_id);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = league_id.user_id }, league_id);

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
    }
}
