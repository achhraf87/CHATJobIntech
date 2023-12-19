using JobIntechCHAT.API.Interfaces;
using JobIntechCHAT.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JobIntechCHAT.API.Services
{
    public class TokenServices : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        public TokenServices(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
            string tokenKey = config["TokenKey"];
            if (string.IsNullOrWhiteSpace(tokenKey) || tokenKey.Length < 64)
            {
                _key = GenerateNewTokenKey();
                config["TokenKey"] = Convert.ToBase64String(_key.Key);
            }
            else
            {
                _key = new SymmetricSecurityKey(Convert.FromBase64String(tokenKey));
            }
        }

        private SymmetricSecurityKey GenerateNewTokenKey()
        {
            byte[] keyBytes = new byte[64]; // 64 bytes = 512 bits
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(keyBytes);
            }

            return new SymmetricSecurityKey(keyBytes);
        }


        public string CreateToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
            };
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
