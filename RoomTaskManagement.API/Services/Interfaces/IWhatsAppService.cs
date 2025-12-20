namespace RoomTaskManagement.API.Services.Interfaces
{
	public interface IWhatsAppService
	{
		public Task<bool> SendTaskNotificationAsync(string phoneNumber, string taskName, string userName);
	}
}
