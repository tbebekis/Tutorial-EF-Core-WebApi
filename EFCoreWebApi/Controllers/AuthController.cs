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
        [Produces<ApiItemResult<TokenResult>>]        
        [HttpPost("authenticate"), AllowAnonymous]
        public ApiItemResult<TokenResult> Authenticate(TokenRequest Model)
        {
            ApiItemResult<TokenResult> Result = new();

            if (Model == null)
            {
                Result.BadRequest("Request is not valid.");
                return Result;
            }

            string ClientId = Model.ClientId; 
            string Secret = Model.Secret; 
            string CultureCode = Model.Locale;

            if (string.IsNullOrWhiteSpace(CultureCode))
                CultureCode = Lib.Settings.Defaults.CultureCode;

            if (!Lib.Settings.Defaults.SupportedCultures.Contains(CultureCode))
            {
                Result.BadRequest("Requested locale not supported.");
            }
            else
            {
                ApiItemResult<IApiClient> DataResult = Service.ValidateApiClientCredentials(ClientId, Secret);
                IApiClient Client = DataResult.Item;

                if (DataResult.Succeeded && Client != null)
                {                   
                    Result = Tokens.CreateAuthenticatedToken(Client.Id, CultureCode);
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

        [EndpointDescription("Revokes the Access Token of a client.")]
        [Produces<ApiItemResult<TokenResult>>]
        [HttpPost("revoke")]
        public ApiResult Revoke()
        {
            ApiResult Result = new();
 
            ClaimsPrincipal Principal = HttpContext.User;
            
            string JtiValue = Principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            string Id = Principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrWhiteSpace(JtiValue) || string.IsNullOrWhiteSpace(Id))
                Result.AddError("Client is unknown");

            // remove the Jti claim from cache
            string JtiCacheKey = Tokens.GetJtiCacheKey(JtiValue);
            object Value;
            if (!Lib.Cache.TryGetValue(JtiCacheKey, out Value))
                Result.AddError("Token has already expired.");
            else
                Lib.Cache.Remove(JtiCacheKey);

            // remove the refresh token from cache
            string ClientIdCacheKey = Tokens.GetClientIdCachKey(Id);
            if (Lib.Cache.TryGetValue(ClientIdCacheKey, out Value))
                Lib.Cache.Remove(ClientIdCacheKey);

            if (!Result.Succeeded)
                Result.BadRequest();

            return Result;
        }

        [EndpointDescription("Issues a new Access Token based on a passed in Refresh Token.")]
        [Produces<ApiItemResult<TokenResult>>]
        [HttpPost("refresh"), AllowAnonymous]
        public ApiItemResult<TokenResult> Refresh(RefreshTokenRequest Model)
        {
            ApiItemResult<TokenResult> Result = new();

            if (Model == null || string.IsNullOrWhiteSpace(Model.RefreshToken) || string.IsNullOrWhiteSpace(Model.Token))
            {
                Result.BadRequest("Request is not valid.");
                return Result;
            }

            // validate refresh token
            ClaimsPrincipal Principal = Tokens.GetPrincipalFromExpiredToken(Model.Token);
            string Id = Principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            string ClientIdCacheKey = Tokens.GetClientIdCachKey(Id);
            object Value;
            if (!Lib.Cache.TryGetValue(ClientIdCacheKey, out Value) 
                || Value is not string 
                || Value.ToString() != Model.RefreshToken)
            {
                Result.BadRequest("Refresh Token has expired or is invalid. Please authenticate again.");
                return Result;
            }

            // if refresh token is ok, issue a new access and refresh token
            string CultureCode = Principal.FindFirst(JwtRegisteredClaimNames.Locale)?.Value;


            Result = Tokens.CreateAuthenticatedToken(Id, CultureCode);
            return Result;

        }
    }
}
