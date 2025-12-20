using Microsoft.AspNetCore.SignalR;

namespace RoomTaskManagement.API.Hubs
{
	public class TaskHub : Hub
	{
		public async Task TaskUpdated(string message)
		{
			ArgumentException.ThrowIfNullOrEmpty(message);

			await Clients.All.SendAsync("ReceiveTaskUpdate", message);
		}

		public async Task TaskTriggered(int taskId, string taskName, string assignedTo)
		{
			if (string.IsNullOrEmpty(taskName))
			{
				throw new ArgumentException("Task name cannot be null or empty", nameof(taskName));
			}

			if (string.IsNullOrEmpty(assignedTo))
			{
				throw new ArgumentException("AssignedTo cannot be null or empty", nameof(assignedTo));
			}

			if (taskId < 1)
			{
				throw new ArgumentException("Invalid task Id", nameof(taskId));
			}

			await Clients.All.SendAsync("ReceiveTaskTriggered", new
			{
				taskId,
				taskName,
				assignedTo,
				timestamp = DateTime.UtcNow
			});
		}

		public async Task TaskCompleted(int taskId, string taskName, string completedBy)
		{
			if (string.IsNullOrEmpty(taskName))
			{
				throw new ArgumentException("Task name cannot be null or empty", nameof(taskName));
			}
			if (string.IsNullOrEmpty(completedBy))
			{
				throw new ArgumentException("CompletedBy cannot be null or empty", nameof(completedBy));
			}
			if (taskId < 1)
			{
				throw new ArgumentException("Invalid task Id", nameof(taskId));
			}
			await Clients.All.SendAsync("ReceiveTaskCompleted", new
			{
				taskId,
				taskName,
				completedBy,
				timestamp = DateTime.UtcNow
			});
		}

		public async Task UserStatusChanged(int userId, string userName, bool isOutOfStation)
		{
			if (string.IsNullOrEmpty(userName))
			{
				throw new ArgumentException("User name cannot be null or empty", nameof(userName));
			}
			if (userId < 1)
			{
				throw new ArgumentException("Invalid user Id", nameof(userId));
			}
			await Clients.All.SendAsync("ReceiveUserStatusChanged", new
			{
				userId,
				userName,
				isOutOfStation,
				timestamp = DateTime.UtcNow
			});
		}
	}
}
