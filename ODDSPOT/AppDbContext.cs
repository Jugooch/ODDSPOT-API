using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ODDSPOT
{
    public class AppDbContext : DbContext
    {

        static readonly string connectionString = "server=ik1eybdutgxsm0lo.cbetxkdyhwsb.us-east-1.rds.amazonaws.com;database=tzzks6xi5k79glvf; user=y2h37gkgayqc5jl0; password=yvuzb7to8r2324aj";
        public DbSet<User> Users { get; set; }
        public DbSet<League> Leagues { get; set; }
        public DbSet<FavoriteLeague> Favorite_Leagues { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }
    }

    public class User
    {
        [Key]
        public int user_id { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public string email_address { get; set; }
        public List<League> Favorite_Leagues { get; set; } = new List<League>();
    }

    public class League
    {
        [Key]
        public int league_id { get; set; }
        public string league_key { get; set; }
        public string title { get; set; }
        public string description { get; set; }
    }


    public class FavoriteLeague
    {
        public FavoriteLeague(int league_id, int user_id)
        {
            league_id = league_id;
            user_id = user_id;
        }
        [Key]
        public int id { get; set; }
        public int league_id { get; set; }
        public int user_id { get; set; }
    }

}
