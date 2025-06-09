namespace EFCoreWebApi.Library
{
    static public partial class Tokens
    {
        public const string CachePrefix_Jti = "Jti";
        public const string CachePrefix_ClientId = "Client.Id";

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
            return Tokens.GetClaimValue(Token.Claims, JwtRegisteredClaimNames.Locale);
        }
        static public string GetApiClientId(JwtSecurityToken Token)
        {
            return Tokens.GetClaimValue(Token.Claims, JwtRegisteredClaimNames.Sub);
        }

        static public string GetJtiCacheKey(string JtiValue)
        {
            string Result = $"{CachePrefix_Jti}+{JtiValue}";
            return Result;
        }
        static public string GetClientIdCachKey(string Id)
        {
            string Result = $"{CachePrefix_ClientId}+{Id}";
            return Result;
        }
        /// <summary>
        /// Generates an Access Token based on a database table Id which identifies the client application and a requested locale.
        /// </summary>
        static public ApiItemResult<TokenResult> CreateAuthenticatedToken(string Id, string CultureCode)
        {
            JwtSettings Jwt = Lib.Settings.Jwt;

            if (string.IsNullOrWhiteSpace(CultureCode))
                CultureCode = Lib.Settings.Defaults.CultureCode;

            // ● handle Jti claim
            string JtiValue = Lib.GenId();
            string JtiCacheKey = GetJtiCacheKey(JtiValue);
            Lib.Cache.Set(JtiCacheKey, JtiValue, Jwt.TokenLifeTimeMinutes);

            // ● handle refresh token
            string RefreshToken = GenerateRefreshToken();
            string ClientIdCacheKey = GetClientIdCachKey(Id);
            Lib.Cache.Set(JtiCacheKey, RefreshToken, Jwt.RefreshTokenLifeTimeMinutes);

            // ● Claims
            /// List of registered claims from different sources
            /// https://datatracker.ietf.org/doc/html/rfc7519#section-4
            /// http://openid.net/specs/openid-connect-core-1_0.html#IDToken
            /// https://openid.net/specs/openid-connect-core-1_0.html#StandardClaims
            List<Claim> ClaimList = new List<Claim>();
            ClaimList.Add(new Claim(JwtRegisteredClaimNames.Sub, Id));          // sub identifies the subject(user, account, etc.)
            ClaimList.Add(new Claim(JwtRegisteredClaimNames.Jti, JtiValue));    // jti identifies the token itself, preventing replay attacks
            ClaimList.Add(new Claim(JwtRegisteredClaimNames.Locale, CultureCode));
 
            // ● JWT token
            var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Jwt.EncryptionKey));
            var Credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
            var ExpirationDateTime = DateTime.UtcNow.AddMinutes(Jwt.TokenLifeTimeMinutes);

            JwtSecurityToken JwtToken = new JwtSecurityToken(
                issuer: Jwt.Issuer,
                audience: Jwt.Audience,
                claims: ClaimList.ToArray(),
                expires: ExpirationDateTime,                                // exp claim, indicates the latest time at which the token can be used
                notBefore: DateTime.UtcNow,                                 // nbf claim, a timestamp before which the token is not valid
                signingCredentials: Credentials
            );

            // ● Token
            TokenResult TokenResult = new TokenResult();
            TokenResult.Token = new JwtSecurityTokenHandler().WriteToken(JwtToken);
            TokenResult.ExpiresOn = JwtToken.ValidTo; //JwtToken.ValidTo.ToString("yyyy-MM-dd HH:mm");
            TokenResult.RefreshToken = RefreshToken;

            // ● Response
            ApiItemResult<TokenResult> Result = new();
            Result.Item = TokenResult;

            return Result;
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
