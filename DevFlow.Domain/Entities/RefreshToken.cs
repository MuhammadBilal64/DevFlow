using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevFlow.Domain.Entities
{
    public class RefreshToken { 
    
        public int Id {  get; set; }
        public String Token { get; set; } = null!;
        public int UserId {  get; set; }
        public User User { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime ? RevokedAt { get; set; }
        public string? ReplacedByToken { get; set; } 

    }
}
