namespace testauth.Models
{
    public class RefreshToken
    {

        public required string token { get; set; }

        public DateTime TokenCreate { get; set; } = DateTime.Now;

        public DateTime TokenExpires { get; set; }
    
    }
}
