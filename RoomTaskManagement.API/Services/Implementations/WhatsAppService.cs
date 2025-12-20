using RoomTaskManagement.API.Services.Interfaces;

namespace RoomTaskManagement.API.Services.Implementations
{
	public class WhatsAppService : IWhatsAppService
	{
		private readonly IConfiguration _configuration;
		private readonly ILogger<WhatsAppService> _logger;

		public WhatsAppService(IConfiguration configuration, ILogger<WhatsAppService> logger)
		{
			_configuration = configuration;
			_logger = logger;
		}

		public async Task<bool> SendTaskNotificationAsync(string phoneNumber, string taskName, string userName)
		{
			try
			{
				// For now, we'll just log the notification
				// We'll implement actual WhatsApp API integration on Day 3
				var message = $"Hello {userName}, {taskName} needs to be done. It's your turn!";

				_logger.LogInformation($"WhatsApp notification to {phoneNumber}: {message}");

				// TODO: Implement WhatsApp Cloud API call here on Day 3
				// This is a placeholder that returns true
				await Task.CompletedTask;

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error sending WhatsApp notification: {ex.Message}");
				return false;
			}
		}
	}
}
