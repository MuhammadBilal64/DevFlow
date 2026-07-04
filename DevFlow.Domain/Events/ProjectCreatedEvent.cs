using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DevFlow.Domain.Events
{
    public sealed class ProjectCreatedEvent:IDomainEvent
    {
       
        public string ProjectName { get; } = null!;
        public int WorkspaceId {  get; }
        public int CreatedBy {  get; }
        public ProjectCreatedEvent(string projectName,int workspaceId,int createdBy)
        {
          
            ProjectName = projectName;
            WorkspaceId = workspaceId;
            CreatedBy   = createdBy;
        }

    }
}
