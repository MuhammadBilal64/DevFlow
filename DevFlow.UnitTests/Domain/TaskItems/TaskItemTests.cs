using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using DevFlow.Domain.Events;
using FluentAssertions;
using Xunit;
using TaskStatus = DevFlow.Domain.Enum.TaskStatus;

namespace DevFlow.UnitTests.Domain.TaskItems
{
    public class TaskItemTests
    {
        // ----------------------------------------------------
        // Constructor Tests
        // ----------------------------------------------------
        [Fact]
        public void Should_Create_Task_With_Valid_Data()
        {
            // Arrange
            var title = "Implement JWT";
            var description = "Add JWT Authentication";
            var projectId = 1;
            var createdBy = 2;
            var dueDate = DateTime.UtcNow.AddDays(7);
            var priority = TaskPriority.High;

            // Act
            var task = new TaskItem(
                title,
                description,
                projectId,
                createdBy,
                dueDate,
                priority);

            // Assert
            task.Title.Should().Be(title);
            task.Description.Should().Be(description);
            task.ProjectId.Should().Be(projectId);
            task.CreatedBy.Should().Be(createdBy);
            task.DueDate.Should().Be(dueDate);
            task.Priority.Should().Be(priority);
            task.Status.Should().Be(TaskStatus.Todo);
        }
        [Fact]
        public void Should_Throw_When_Title_Is_Empty()
        {
            // Arrange
            var title = "";
            var description = "Description";
            var projectId = 1;
            var createdBy = 2;
            var dueDate = DateTime.UtcNow.AddDays(7);
            var priority = TaskPriority.High;

            // Act
            Action act = () => new TaskItem(
                title,
                description,
                projectId,
                createdBy,
                dueDate,
                priority);

            // Assert
            act.Should()
                .Throw<ArgumentException>()
                .WithMessage("Task title is required.*")
                .And.ParamName.Should().Be("title");
        }

        [Fact]
        public void Should_Throw_When_ProjectId_Is_Invalid()
        {
            // Arrange
            var title = "Task";
            var description = "Description";
            var projectId = 0;
            var createdBy = 2;
            var dueDate = DateTime.UtcNow.AddDays(7);
            var priority = TaskPriority.High;

            // Act
            Action act = () => new TaskItem(
                title,
                description,
                projectId,
                createdBy,
                dueDate,
                priority);

            // Assert
            act.Should()
                .Throw<ArgumentException>()
                .WithMessage("Invalid project id.*")
                .And.ParamName.Should().Be("projectId");
        }


        [Fact]
        public void Should_Throw_When_CreatedBy_Is_Invalid()
        {
            // Arrange
            var title = "Task";
            var description = "Description";
            var projectId = 1;
            var createdBy = 0;
            var dueDate = DateTime.UtcNow.AddDays(7);
            var priority = TaskPriority.High;

            // Act
            Action act = () => new TaskItem(
                title,
                description,
                projectId,
                createdBy,
                dueDate,
                priority);

            // Assert
            act.Should()
                .Throw<ArgumentException>()
                .WithMessage("Invalid creator id.*")
                .And.ParamName.Should().Be("createdBy");
        }

        [Fact]
        public void Should_Throw_When_Creating_Task_With_Past_DueDate()
        {
            // Arrange
            var title = "Task";
            var description = "Description";
            var projectId = 1;
            var createdBy = 2;
            var dueDate = DateTime.UtcNow.AddDays(-1);
            var priority = TaskPriority.High;

            // Act
            Action act = () => new TaskItem(
                title,
                description,
                projectId,
                createdBy,
                dueDate,
                priority);

            // Assert
            act.Should()
                .Throw<ArgumentException>()
                .WithMessage("Due date cannot be in the past.*")
                .And.ParamName.Should().Be("dueDate");
        }


        // ----------------------------------------------------
        // Assign Tests
        // ----------------------------------------------------

        [Fact]
        public void Should_Assign_Task()
        {
            // Arrange
            var task = new TaskItem(
                "Implement JWT",
                "Description",
                1,
                2,
                DateTime.UtcNow.AddDays(5),
                TaskPriority.High);

            var userId = 10;

            // Act
            task.Assign(userId);

            // Assert
            task.AssignedToUserId.Should().Be(userId);
            task.AssignedAt.Should().NotBeNull();
        }

        [Fact]
        public void Should_Not_Assign_When_User_Is_Already_Assigned()
        {
            // Arrange
            var task = new TaskItem(
                "Implement JWT",
                "Description",
                1,
                2,
                DateTime.UtcNow.AddDays(5),
                TaskPriority.High);

            task.Assign(10);

            var assignedAt = task.AssignedAt;

            // Act
            task.Assign(10);

            // Assert
            task.AssignedToUserId.Should().Be(10);
            task.AssignedAt.Should().Be(assignedAt);
        }

        [Fact]
        public void Should_Throw_When_UserId_Is_Invalid()
        {
            // Arrange
            var task = new TaskItem(
                "Implement JWT",
                "Description",
                1,
                2,
                DateTime.UtcNow.AddDays(5),
                TaskPriority.High);

            // Act
            Action act = () => task.Assign(0);

            // Assert
            act.Should()
                .Throw<ArgumentException>()
                .WithMessage("Invalid User Id*")
                .And.ParamName.Should().Be("userId");
        }

        [Fact]
        public void Should_Raise_TaskAssigned_Domain_Event()
        {
            // Arrange
            var task = new TaskItem(
                "Implement JWT",
                "Description",
                1,
                2,
                DateTime.UtcNow.AddDays(5),
                TaskPriority.High);

            var userId = 10;

            // Act
            task.Assign(userId);

            // Assert
            task.DomainEvents.Should().HaveCount(1);
            var domainEvent = task.DomainEvents.First();
            domainEvent.Should().BeOfType<TaskAssignedEvent>();
            var taskAssignedEvent =
    (TaskAssignedEvent)domainEvent;
            taskAssignedEvent.UserId.Should().Be(userId);
            taskAssignedEvent.TaskId.Should().Be(task.Id);
            taskAssignedEvent.TaskTitle.Should().Be(task.Title);


        }


        // ----------------------------------------------------
        // Unassign Tests
        // ----------------------------------------------------
        [Fact]
        public void Should_Unassign_Task()
        {
            // Arrange
            var task = new TaskItem(
                "Implement JWT",
                "Description",
                1,
                2,
                DateTime.UtcNow.AddDays(5),
                TaskPriority.High);

            task.Assign(10);

            // Act
            task.Unassign();

            // Assert
            task.AssignedToUserId.Should().BeNull();
            task.AssignedAt.Should().BeNull();
        }

        [Fact]
        public void Should_Not_Unassign_When_Task_Is_Not_Assigned()
        {
            // Arrange
            var task = new TaskItem(
                "Implement JWT",
                "Description",
                1,
                2,
                DateTime.UtcNow.AddDays(5),
                TaskPriority.High);

            // Act
            task.Unassign();

            // Assert
            task.AssignedToUserId.Should().BeNull();
            task.AssignedAt.Should().BeNull();
        }


        // ----------------------------------------------------
        // UpdateStatus Tests
        // ----------------------------------------------------

        [Fact]
        public void Should_Update_Status()
        {
            // Arrange
            var task = new TaskItem(
                "Implement JWT",
                "Description",
                1,
                2,
                DateTime.UtcNow.AddDays(5),
                TaskPriority.High);

            // Act
            task.UpdateStatus(TaskStatus.InProgress);

            // Assert
            task.Status.Should().Be(TaskStatus.InProgress);
        }


        [Fact]
        public void Should_Not_Update_When_Status_Is_Same()
        {
            // Arrange
            var task = new TaskItem(
                "Implement JWT",
                "Description",
                1,
                2,
                DateTime.UtcNow.AddDays(5),
                TaskPriority.High);

            // Act
            task.UpdateStatus(TaskStatus.Todo);

            // Assert
            task.Status.Should().Be(TaskStatus.Todo);
            task.CompletedAt.Should().BeNull();
        }

        [Fact]
        public void Should_Set_CompletedAt_When_Status_Is_Completed()
        {
            // Arrange
            var task = new TaskItem(
                "Implement JWT",
                "Description",
                1,
                2,
                DateTime.UtcNow.AddDays(5),
                TaskPriority.High);

            // Act
            task.UpdateStatus(TaskStatus.Completed);

            // Assert
            task.Status.Should().Be(TaskStatus.Completed);
            task.CompletedAt.Should().NotBeNull();
        }

        [Fact]
        public void Should_Clear_CompletedAt_When_Status_Is_Changed_From_Completed()
        {
            // Arrange
            var task = new TaskItem(
                "Implement JWT",
                "Description",
                1,
                2,
                DateTime.UtcNow.AddDays(5),
                TaskPriority.High);

            task.UpdateStatus(TaskStatus.Completed);

            // Act
            task.UpdateStatus(TaskStatus.InProgress);

            // Assert
            task.Status.Should().Be(TaskStatus.InProgress);
            task.CompletedAt.Should().BeNull();
        }
        [Fact]
        public void Should_Raise_TaskCompleted_Domain_Event()
        {
            // Arrange
            var task = new TaskItem(
                "Implement JWT",
                "Description",
                1,
                2,
                DateTime.UtcNow.AddDays(5),
                TaskPriority.High);

            // Act
            task.UpdateStatus(TaskStatus.Completed);

            // Assert
            task.DomainEvents.Should().HaveCount(1);

            var domainEvent = task.DomainEvents.First();

            domainEvent.Should().BeOfType<TaskCompletedEvent>();

            var taskCompletedEvent =
                (TaskCompletedEvent)domainEvent;

            taskCompletedEvent.RecipientUserId.Should().Be(task.CreatedBy);
            taskCompletedEvent.TaskId.Should().Be(task.Id);
            taskCompletedEvent.TaskTitle.Should().Be(task.Title);
        }


        // ----------------------------------------------------
        // UpdatePriority Tests
        // ----------------------------------------------------

        [Fact]
        public void Should_Update_Priority()
        {
            // Arrange
            var task = new TaskItem(
                "Implement JWT",
                "Description",
                1,
                2,
                DateTime.UtcNow.AddDays(5),
                TaskPriority.Low);

            // Act
            task.UpdatePriority(TaskPriority.High);

            // Assert
            task.Priority.Should().Be(TaskPriority.High);
        }


        // ----------------------------------------------------
        // UpdateDueDate Tests
        // ----------------------------------------------------

        [Fact]
        public void Should_Update_DueDate()
        {
            // Arrange
            var task = new TaskItem(
                "Implement JWT",
                "Description",
                1,
                2,
                DateTime.UtcNow.AddDays(5),
                TaskPriority.High);

            var newDueDate = DateTime.UtcNow.AddDays(10);

            // Act
            task.UpdateDueDate(newDueDate);

            // Assert
            task.DueDate.Should().Be(newDueDate);
        }

        [Fact]
        public void Should_Clear_DueDate()
        {
            // Arrange
            var task = new TaskItem(
                "Implement JWT",
                "Description",
                1,
                2,
                DateTime.UtcNow.AddDays(5),
                TaskPriority.High);

            // Act
            task.UpdateDueDate(null);

            // Assert
            task.DueDate.Should().BeNull();
        }

        [Fact]
        public void Should_Throw_When_Updating_DueDate_To_Past_Date()
        {
            // Arrange
            var task = new TaskItem(
                "Implement JWT",
                "Description",
                1,
                2,
                DateTime.UtcNow.AddDays(5),
                TaskPriority.High);

            var dueDate = DateTime.UtcNow.AddDays(-1);

            // Act
            Action act = () => task.UpdateDueDate(dueDate);

            // Assert
            act.Should()
                .Throw<ArgumentException>()
                .WithMessage("Due date cannot be in the past.*")
                .And.ParamName.Should().Be("dueDate");
        }


        // ----------------------------------------------------
        // UpdateTitle Tests
        // ----------------------------------------------------

        [Fact]
        public void Should_Update_Title()
        {
            var task = new TaskItem(
                "Old Title",
                "Description",
                1,
                2,
                DateTime.UtcNow.AddDays(5),
                TaskPriority.High);

            task.UpdateTitle("New Title");

            task.Title.Should().Be("New Title");
        }
        [Fact]
        public void Should_Not_Update_When_Title_Is_Same()
        {
            var task = new TaskItem(
                "Task",
                "Description",
                1,
                2,
                DateTime.UtcNow.AddDays(5),
                TaskPriority.High);

            task.UpdateTitle("Task");

            task.Title.Should().Be("Task");
        }


        [Fact]
        public void Should_Throw_When_Updating_Title_To_Empty()
        {
            var task = new TaskItem(
                "Task",
                "Description",
                1,
                2,
                DateTime.UtcNow.AddDays(5),
                TaskPriority.High);

            Action act = () => task.UpdateTitle("");

            act.Should()
                .Throw<ArgumentException>()
                .WithMessage("Task title is required.*")
                .And.ParamName.Should().Be("title");
        }


        // ----------------------------------------------------
        // UpdateDescription Tests
        // ----------------------------------------------------

        [Fact]
        public void Should_Update_Description()
        {
            var task = new TaskItem(
                "Task",
                "Old Description",
                1,
                2,
                DateTime.UtcNow.AddDays(5),
                TaskPriority.High);

            task.UpdateDescription("New Description");

            task.Description.Should().Be("New Description");
        }
        [Fact]
        public void Should_Not_Update_When_Description_Is_Same()
        {
            var task = new TaskItem(
                "Task",
                "Description",
                1,
                2,
                DateTime.UtcNow.AddDays(5),
                TaskPriority.High);

            task.UpdateDescription("Description");

            task.Description.Should().Be("Description");
        }
    }
}
