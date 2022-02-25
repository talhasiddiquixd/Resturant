using Microsoft.Extensions.Configuration;
using RestaurantPOS.Helpers.RequestDTO;
using RestaurantPOS.Helpers.ResponseDTO;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Helpers
{
    public interface IJwtHelper
    {
        LoginResponseDTO GenerateJSONWebTokenForUser(TokenGenrationRequestDTO tokenGenrationRequestDTO);
    }
    public class JwtHelper: IJwtHelper
    {
        private IConfiguration _config;
        public JwtHelper(IConfiguration config)
        {
            _config = config;
        }
        public LoginResponseDTO GenerateJSONWebTokenForUser(TokenGenrationRequestDTO tokenGenrationRequestDTO)
        {
            LoginResponseDTO loginResponseDTO = new LoginResponseDTO();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim("Id", tokenGenrationRequestDTO.Id.ToString()),
                new Claim("Name", tokenGenrationRequestDTO.Name),
                new Claim("Email", tokenGenrationRequestDTO.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                //new Claim("Role", tokenGenrationRequestDTO.Role)
                //new Claim(ClaimTypes.Role, "User"),
            };
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:SessionTimeout"])),
                signingCredentials: credentials);
            var Token = new JwtSecurityTokenHandler().WriteToken(token);
            loginResponseDTO.Token = Token;
            loginResponseDTO.UserId = tokenGenrationRequestDTO.Id;
            loginResponseDTO.Email = tokenGenrationRequestDTO.Email;
            loginResponseDTO.Name = tokenGenrationRequestDTO.Name;
            loginResponseDTO.AssignedType = tokenGenrationRequestDTO.AssignedType;
            loginResponseDTO.AssignedRole = tokenGenrationRequestDTO.AssignedRole;
            loginResponseDTO.Role = tokenGenrationRequestDTO.Role;
            return loginResponseDTO;
        }
    }
}
