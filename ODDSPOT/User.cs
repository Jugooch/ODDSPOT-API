using System.Collections.Generic;

namespace ODDSPOT
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
        public List<League> FavoriteLeagues { get; set; } = new List<League>();
    }
}
