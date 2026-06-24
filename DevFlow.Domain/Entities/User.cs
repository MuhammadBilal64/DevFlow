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

        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public UserRole Role { get; set; }
        public ICollection<RefreshToken> RefreshTokens {  get; set; }=new List<RefreshToken>();
        public ICollection<Workspace> CreatedWorkspaces { get; set; } = new List<Workspace>();
        public ICollection<WorkspaceMember> WorkspaceMemberships {  get; set; }=new List<WorkspaceMember>();
        public ICollection<Project> CreatedProjects { get; set; } = new List<Project>();

    }
}
