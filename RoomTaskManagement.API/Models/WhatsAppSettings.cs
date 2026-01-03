namespace RoomTaskManagement.API.Models
{
	public class WhatsAppSettings
	{
		public string AccessToken { get; set; } = string.Empty;
		public string PhoneNumberId { get; set; } = string.Empty;
		public string ApiVersion { get; set; } = "v21.0";
		public string ApiUrl { get; set; } = "https://graph.facebook.com";
	}
}
