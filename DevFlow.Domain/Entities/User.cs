using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DevFlow.Domain.Enum;

namespace DevFlow.Domain.Entities
{
    public class User
    {
        
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string Email {  get; set; }
        public string PasswordHash {  get; set; }
        public DateTime CreatedAt { get; set; }
        public UserRole Role { get; set; }

    }
}
