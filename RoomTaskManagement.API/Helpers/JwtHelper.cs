using Microsoft.IdentityModel.Tokens;
using RoomTaskManagement.API.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RoomTaskManagement.API.Helpers
{
	public class JwtHelper
	{
		private readonly IConfiguration _configuration;

		public JwtHelper(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public string GenerateToken(UserDto user)
		{
			ArgumentNullException.ThrowIfNull(user);

			var jwtSettings = _configuration.GetSection("JwtSettings");
			var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

			var claims = new[]
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Name, user.Username),
				new Claim(ClaimTypes.Role, user.Role),
				new Claim("FullName", user.FullName)
			};

			var token = new JwtSecurityToken(
				issuer: jwtSettings["Issuer"],
				audience: jwtSettings["Audience"],
				claims: claims,
				expires: DateTime.Now.AddHours(Convert.ToDouble(jwtSettings["ExpiryInHours"])),
				signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256)
				);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
