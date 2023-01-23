using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using SI.Identity.Models;

namespace SI.Identity.Services
{
    public interface IJwtGenerator
    {
        Task<JwtResponse> Generate(User identityUser);
    }
    public class JwtGenerator : IJwtGenerator
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<User> userManager;

        public JwtGenerator(IConfiguration configuration, UserManager<User> userManager)
        {
            this.configuration = configuration;
            this.userManager = userManager;
        }

        public async Task<JwtResponse> Generate(User identityUser)
        {
            using RSA rsa = RSA.Create();
            rsa.ImportRSAPrivateKey( // Convert the loaded key from base64 to bytes.
                source: Convert.FromBase64String(configuration["Jwt:Asymmetric:PrivateKey"]), // Use the private key to sign tokens
                bytesRead: out int _); // Discard the out variable 

            var signingCredentials = new SigningCredentials(
                key: new RsaSecurityKey(rsa),
                algorithm: SecurityAlgorithms.RsaSha256 // Important to use RSA version of the SHA algo 
            )
            {
                CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
            };

            var claims = new List<Claim>();
            foreach (var claim in await userManager.GetClaimsAsync(identityUser))
            {
                claims.Add(claim);
            }
            claims.Add(new Claim(ClaimTypes.Email, identityUser.Email));


            DateTime jwtDate = DateTime.Now;
            var securityToken =  new JwtSecurityToken(
                audience: configuration["Jwt:Audience"],
                issuer: configuration["Jwt:Issuer"],
                claims: claims,
                notBefore: jwtDate,
                expires: jwtDate.AddMinutes(60),
                signingCredentials: signingCredentials
            );

            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return new JwtResponse
            {
                Token = token,
                ExpiresAt = new DateTimeOffset(securityToken.ValidTo).ToUnixTimeSeconds()
            };
        }
     }
}
