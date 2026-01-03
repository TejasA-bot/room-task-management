using Microsoft.Extensions.Options;
using RoomTaskManagement.API.Models;
using RoomTaskManagement.API.Services.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace RoomTaskManagement.API.Services.Implementations
{
	public class WhatsAppService : IWhatsAppService
	{
		private readonly WhatsAppSettings _settings;
		private readonly ILogger<WhatsAppService> _logger;
		private readonly HttpClient _httpClient;

		public WhatsAppService(
			IOptions<WhatsAppSettings> settings,
			ILogger<WhatsAppService> logger,
			IHttpClientFactory httpClientFactory)
		{
			_settings = settings.Value;
			_logger = logger;
			_httpClient = httpClientFactory.CreateClient();
		}

		public async Task<bool> SendTaskNotificationAsync(string phoneNumber, string taskName, string userName)
		{
			try
			{
				// Validate settings
				if (string.IsNullOrEmpty(_settings.AccessToken) ||
					string.IsNullOrEmpty(_settings.PhoneNumberId))
				{
					_logger.LogWarning("WhatsApp settings not configured properly");
					return false;
				}

				// Clean phone number - remove any non-digit characters
				phoneNumber = new string(phoneNumber.Where(char.IsDigit).ToArray());

				// Add country code if not present (assuming India +91)
				if (!phoneNumber.StartsWith("91") && phoneNumber.Length == 10)
				{
					phoneNumber = "91" + phoneNumber;
				}

				_logger.LogInformation($"Sending WhatsApp to: {phoneNumber}");

				var url = $"{_settings.ApiUrl}/{_settings.ApiVersion}/{_settings.PhoneNumberId}/messages";

				var messagePayload = new
				{
					messaging_product = "whatsapp",
					recipient_type = "individual",
					to = phoneNumber,
					type = "text",
					text = new
					{
						preview_url = false,
						body = $" Room Task Alert\n\nHello {userName},\n\n{taskName} needs to be done.\n\nIt's your turn!\n\n- Room Task Management"
					}
				};

				var jsonContent = JsonSerializer.Serialize(messagePayload, new JsonSerializerOptions
				{
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase
				});

				_logger.LogInformation($"Request URL: {url}");
				_logger.LogInformation($"Request Payload: {jsonContent}");

				var request = new HttpRequestMessage(HttpMethod.Post, url);
				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.AccessToken);
				request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

				var response = await _httpClient.SendAsync(request);
				var responseContent = await response.Content.ReadAsStringAsync();

				_logger.LogInformation($"WhatsApp API Response Status: {response.StatusCode}");
				_logger.LogInformation($"WhatsApp API Response: {responseContent}");

				if (response.IsSuccessStatusCode)
				{
					_logger.LogInformation($" WhatsApp message sent successfully to {phoneNumber}");
					return true;
				}
				else
				{
					_logger.LogError($" WhatsApp API error: Status={response.StatusCode}, Response={responseContent}");
					return false;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError($" Exception sending WhatsApp notification: {ex.Message}");
				_logger.LogError($"Stack Trace: {ex.StackTrace}");
				return false;
			}
		}
	}
}
