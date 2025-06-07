using Microsoft.AspNetCore.SignalR;
using System.Security.Principal;

namespace EFCoreWebApi.Library
{
    static public partial class Lib
    { 
        /// <summary>
        /// For debug purposes. 
        /// Reads a token by reading the <see cref="HeaderNames.Authorization"/> header 
        /// from <see cref="HttpRequest.Headers"/>, 
        /// and converting the token string to a <see cref="JwtSecurityToken"/>
        /// </summary>
        static public JwtSecurityToken ReadTokenFromRequestHeader(HttpContext HttpContext)
        {
            // here we get the token from HTTP headers
            string sToken = Lib.GetHttpHeader(HttpContext.Request, HeaderNames.Authorization);

            if (!string.IsNullOrWhiteSpace(sToken))
            {
                sToken = sToken.Replace("Bearer ", string.Empty);
                JwtSecurityToken Token = new JwtSecurityTokenHandler().ReadJwtToken(sToken);
                return Token;
            }

            return null;
        }
        static public string GetCultureCode(JwtSecurityToken Token)
        {
            return Lib.GetClaimValue(Token.Claims, JwtRegisteredClaimNames.Locale);
        }
        static public string GetApiClientId(JwtSecurityToken Token)
        {
            return Lib.GetClaimValue(Token.Claims, JwtRegisteredClaimNames.Sub);
        }

        static public ApiItemResult<TokenData> CreateAuthenticatedToken(IApiClient Client, string Culture)
        {
            JwtSettings Jwt = Lib.Settings.Jwt;
 
            // ● Claims
            /// List of registered claims from different sources
            /// https://datatracker.ietf.org/doc/html/rfc7519#section-4
            /// http://openid.net/specs/openid-connect-core-1_0.html#IDToken
            /// https://openid.net/specs/openid-connect-core-1_0.html#StandardClaims
            List<Claim> ClaimList = new List<Claim>();
            ClaimList.Add(new Claim(JwtRegisteredClaimNames.Sub, Client.Id));
            ClaimList.Add(new Claim(JwtRegisteredClaimNames.Locale, Culture));

            // JwtRegisteredClaimNames.Jti
            // sub identifies the subject(user, account, etc.)
            // jti identifies the token itself, preventing replay attacks


            // ● JWT token
            var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Jwt.EncryptionKey));
            var Credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
            var ExpirationDateTime = DateTime.UtcNow.AddMinutes(Jwt.LifeTimeMinutes);

            JwtSecurityToken JwtToken = new JwtSecurityToken(
                issuer: Jwt.Issuer,
                audience: Jwt.Audience,
                claims: ClaimList.ToArray(),
                expires: ExpirationDateTime,                                // exp claim, indicates the latest time at which the token can be used
                notBefore: DateTime.UtcNow,                                 // nbf claim, a timestamp before which the token is not valid
                signingCredentials: Credentials
            );

            // ● Token
            TokenData TokenData = new TokenData();
            TokenData.Token = new JwtSecurityTokenHandler().WriteToken(JwtToken);
            TokenData.ExpiresOn = JwtToken.ValidTo.ToString("yyyy-MM-dd HH:mm");

            // ● Response
            ApiItemResult<TokenData> Response = new();
            Response.Item = TokenData;

            return Response;
        }

        /// <summary>
        /// Generates and returns a refresh token using <see cref="RandomNumberGenerator"/>
        /// </summary>
        static public string GenerateRefreshToken()
        {
            byte[] Buffer = new byte[32];
            using (var RNG = RandomNumberGenerator.Create())
            {
                RNG.GetBytes(Buffer);
                return Convert.ToBase64String(Buffer);
            }
        }
        /// <summary>
        /// Returns the <see cref="ClaimsPrincipal"/> out of an Access Token text
        /// </summary>
        static public ClaimsPrincipal GetPrincipalFromExpiredToken(string TokenText)
        {
            JwtSettings Jwt = Lib.Settings.Jwt;
            var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Jwt.EncryptionKey));

            var ValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,               // no audience validation
                ValidateIssuer = false,                 // no issuer validation
                ValidateIssuerSigningKey = true,        // signing key validation
                IssuerSigningKey = SecurityKey,
                ValidateLifetime = false                // no expiration date validation
            };

            JwtSecurityTokenHandler TokenHandler = new();

            SecurityToken SecurityToken;
            ClaimsPrincipal Principal = TokenHandler.ValidateToken(TokenText, ValidationParameters, out SecurityToken);
            JwtSecurityToken JwtSecurityToken = SecurityToken as JwtSecurityToken;

            if (JwtSecurityToken == null || !JwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return Principal;
        }
    }
}
