namespace EFCoreWebApi.Controllers
{

    [Tags("Security")]
    public class AuthController : WebApiController
    {
        AuthService Service;

        // ● construction
        public AuthController(AuthService service)
        {
            Service = service;
        }

        // ● actions
        [EndpointDescription("Authenticates a client and issues an Access Token.")]
        [HttpPost("authenticate"), Produces<ItemResult<TokenResult>>, AllowAnonymous]
        public ItemResult<TokenResult> Authenticate(TokenRequest Model)
        {
            ItemResult<TokenResult> Result = new();

            if (Model == null || string.IsNullOrWhiteSpace(Model.ClientId) || string.IsNullOrWhiteSpace(Model.Secret))
            {
                Result.BadRequest("Request is not valid. ClientId and Secret required.");
                return Result;
            }

            string ClientId = Model.ClientId; 
            string Secret = Model.Secret; 
            string CultureCode = Model.Locale; 

            if (!Lib.Settings.Defaults.SupportedCultures.Contains(CultureCode))
            {
                Result.BadRequest("Requested locale not supported.");
                return Result;
            }
            else
            {
                ItemResult<IApiClient> DataResult = Service.ValidateApiClientCredentials(ClientId, Secret);
                IApiClient Client = DataResult.Item;

                if (DataResult.Succeeded && Client != null)
                {                   
                    Result = Tokens.CreateAccessToken(Client.Id, CultureCode);
                    return Result;
                }
                else
                {
                    Result.ErrorResult(StatusCodes.Status401Unauthorized, "Wrong credentials.");
                }
            }

            Result.NoDataResult();
            return Result;
        }
 
        [EndpointDescription("Issues a new Access Token, when the old one is expired, based on a passed in, and not-yet-expired, Refresh Token.")]
        [HttpPost("refresh"), Produces<ItemResult<TokenResult>>, AllowAnonymous]
        public ItemResult<TokenResult> Refresh(RefreshTokenRequest Model)
        {
            ItemResult<TokenResult> Result = new();

            if (Model == null || string.IsNullOrWhiteSpace(Model.RefreshToken) || string.IsNullOrWhiteSpace(Model.Token))
            {
                Result.BadRequest("Request is not valid. Token and RefreshToken required.");
                return Result;
            }

            // validate refresh token
            PrincipalToken PT = Tokens.GetPrincipalFromExpiredToken(Model.Token);
            ClaimsPrincipal Principal = PT.Principal;
            JwtSecurityToken JwtToken = PT.JwtToken;
 
            string Id = Tokens.GetClaimValue(JwtToken.Claims, JwtRegisteredClaimNames.Sub);  
            string RefreshTokenCacheKey = Tokens.GetRefreshTokenCachKey(Id);
            string CachedRefreshToken = Lib.Cache.Get<string>(RefreshTokenCacheKey);

            if (string.IsNullOrWhiteSpace(CachedRefreshToken) || (CachedRefreshToken != Model.RefreshToken))
            {
                Result.BadRequest("Refresh Token has expired or is invalid. Please authenticate again.");
                return Result;
            }              

            // if refresh token is ok, issue a new access and refresh token
            string CultureCode = Tokens.GetClaimValue(JwtToken.Claims, JwtRegisteredClaimNames.Locale);  // Principal.FindFirst(JwtRegisteredClaimNames.Locale)?.Value;
            if (string.IsNullOrWhiteSpace(CultureCode))
                CultureCode = Lib.Settings.Defaults.CultureCode;

            Result = Tokens.CreateAccessToken(Id, CultureCode);
            return Result;
        }
 
        [EndpointDescription("Revokes the a not-yet-expired Access Token of a client.")]
        [HttpGet("revoke"), Produces<ItemResult<TokenResult>>]
        public ApiResult Revoke()
        {
            ApiResult Result = new();

            JwtSecurityToken JwtToken = Tokens.ReadTokenFromRequestHeader(HttpContext); 

            string JtiValue = Tokens.GetClaimValue(JwtToken.Claims, JwtRegisteredClaimNames.Jti);  
            string Id = Tokens.GetClaimValue(JwtToken.Claims, JwtRegisteredClaimNames.Sub);
 
            if (string.IsNullOrWhiteSpace(JtiValue) || string.IsNullOrWhiteSpace(Id))
                Result.AddError("Client is unknown");

            // remove the Jti claim from cache
            string JtiCacheKey = Tokens.GetJtiCacheKey(JtiValue); 
            if (!Lib.Cache.ContainsKey(JtiCacheKey))
                Result.AddError("Token has already expired.");
            else
                Lib.Cache.Remove(JtiCacheKey);

            // remove the refresh token from cache
            string RefreshTokenCacheKey = Tokens.GetRefreshTokenCachKey(Id);
            Lib.Cache.Remove(RefreshTokenCacheKey);

            if (!Result.Succeeded)
                Result.BadRequest();

            return Result;
        }
    }
}
