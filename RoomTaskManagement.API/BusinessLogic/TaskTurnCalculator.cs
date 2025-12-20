using RoomTaskManagement.API.Models.Entities;
using RoomTaskManagement.API.Repositories.Implementations;
using RoomTaskManagement.API.Repositories.Interfaces;

namespace RoomTaskManagement.API.BusinessLogic
{
	public class TaskTurnCalculator
	{
		private readonly IUserRepository _userRepository;
		private readonly TaskAssignmentRepository _taskAssignmentRepository;

		public TaskTurnCalculator(IUserRepository userRepository, TaskAssignmentRepository taskAssignmentRepository)
		{
			_userRepository = userRepository;
			_taskAssignmentRepository = taskAssignmentRepository;
		}

		public async Task<User?> CalculateNextUserForTaskAsync(int taskId)
		{
			// Get all available users (not out of station)
			var availabelUsers = await _userRepository.GetAvailableUsersAsync();

			if (!availabelUsers.Any()) return null;

			//create a dictionary to hold the last completion time for each user
			var userLastCopmpletionTime = new Dictionary<int, DateTime?>();

			foreach (var user in availabelUsers)
			{
				var lastAssignement = await _taskAssignmentRepository.GetLastCompletedAssignmentAsync(taskId, user.Id);
				userLastCopmpletionTime[user.Id] = lastAssignement?.CompletedAt;
			}

			// Find the user with the oldest or never completed.
			var nextUser = userLastCopmpletionTime
							.OrderBy(kvp => kvp.Value ?? DateTime.MinValue)
							.First()
							.Key;
			return availabelUsers.FirstOrDefault(u => u.Id == nextUser);
		}

		public async Task<Dictionary<int, User?>> CalculateNextUsersForAllTasksAsync(IEnumerable<int> taskIds)
		{
			var result = new Dictionary<int, User?>();
			foreach (var taskId in taskIds)
			{
				result[taskId] = await CalculateNextUserForTaskAsync(taskId);
			}
			return result;
		}
	}
}