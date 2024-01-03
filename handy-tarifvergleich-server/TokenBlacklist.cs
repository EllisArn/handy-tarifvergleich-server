using System.ComponentModel.DataAnnotations;

namespace handy_tarifvergleich_server
{
    public class TokenBlacklist
    {
        [Key]
        public string? Token { get; set; }
        public bool IsBlacklisted { get; set; } = false;
    }
}
