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

        static public ApiItemResult<ApiToken> CreateAuthenticatedToken(IApiClient Client, string Culture)
        {
            JwtSettings Settings = Lib.Settings.Jwt;
 
            // ● Claims
            /// List of registered claims from different sources
            /// https://datatracker.ietf.org/doc/html/rfc7519#section-4
            /// http://openid.net/specs/openid-connect-core-1_0.html#IDToken
            /// https://openid.net/specs/openid-connect-core-1_0.html#StandardClaims
            List<Claim> ClaimList = new List<Claim>();
            ClaimList.Add(new Claim(JwtRegisteredClaimNames.Sub, Client.Id));
            ClaimList.Add(new Claim(JwtRegisteredClaimNames.Locale, Culture));

            // JwtRegisteredClaimNames.Jti


            // ● JWT token
            var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Settings.Secret));

            JwtSecurityToken JwtToken = new JwtSecurityToken(
                issuer: Settings.Issuer,
                audience: Settings.Audience,
                claims: ClaimList.ToArray(),
                expires: DateTime.UtcNow.AddMinutes(Settings.LifeTimeMinutes),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256)
            );

            // ● Token
            ApiToken Token = new ApiToken();
            Token.Token = new JwtSecurityTokenHandler().WriteToken(JwtToken);
            Token.ExpiresOn = JwtToken.ValidTo.ToString("yyyy-MM-dd HH:mm");

            // ● Response
            ApiItemResult<ApiToken> Response = new();
            Response.Item = Token;

            return Response;
        }
    }
}
