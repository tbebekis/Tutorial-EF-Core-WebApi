﻿namespace EFCoreWebApi.Controllers
{

    [Tags("Security")]
    [Route("token")]
    public class AuthController : WebApiController
    {
        ApiClientService Service;

        // ● construction
        public AuthController(ApiClientService service)
        {
            Service = service;
        }

        // ● actions
        [EndpointDescription("Authenticates a client and issues an Access Token.")]
        [HttpPost("authenticate"), Produces<ItemResult<TokenResult>>, AllowAnonymous]
        public async Task<ItemResult<TokenResult>> Authenticate(TokenRequest Model)
        {
            ItemResult<TokenResult> Result = new();

            if (Model == null || string.IsNullOrWhiteSpace(Model.ClientId) || string.IsNullOrWhiteSpace(Model.Secret))
            {
                Result.ErrorResult(ApiStatusCodes.CredentialsRequired);
            }
            else
            {
                string ClientId = Model.ClientId;
                string Secret = Model.Secret;
                string CultureCode = Model.Locale;

                if (!Lib.Settings.Defaults.SupportedCultures.Contains(CultureCode))
                {
                    Result.ErrorResult(ApiStatusCodes.LocaleNotSupported);
                }
                else
                {
                    ItemResult<IApiClient> DataResult = await Service.ValidateApiClientCredentials(ClientId, Secret);
                    IApiClient Client = DataResult.Item;

                    if (DataResult.Succeeded && Client != null)
                    {
                        Result = Tokens.CreateAccessToken(Client.Id, CultureCode);
                    }
                    else
                    {
                        Result.CopyErrors(DataResult);
                    }
                }
            }

            return Result;
        }
 
        [EndpointDescription("Issues a new Access Token, when the old one is expired, based on a passed in, and not-yet-expired, Refresh Token.")]
        [HttpPost("refresh"), Produces<ItemResult<TokenResult>>, AllowAnonymous]
        public async Task<ItemResult<TokenResult>> Refresh(RefreshTokenRequest Model)
        {
            ItemResult<TokenResult> Result = new();

            if (Model == null || string.IsNullOrWhiteSpace(Model.RefreshToken) || string.IsNullOrWhiteSpace(Model.Token))
            {
                Result.ErrorResult(ApiStatusCodes.TokenAndRefreshTokenRequired);  
            }
            else
            {
                // validate refresh token
                PrincipalToken PT = Tokens.GetPrincipalFromExpiredToken(Model.Token);
                ClaimsPrincipal Principal = PT.Principal;
                JwtSecurityToken JwtToken = PT.JwtToken;

                string Id = Tokens.GetClaimValue(JwtToken.Claims, JwtRegisteredClaimNames.Sub);
                string RefreshTokenCacheKey = Tokens.GetRefreshTokenCachKey(Id);
                string CachedRefreshToken = Lib.Cache.Get<string>(RefreshTokenCacheKey);

                if (string.IsNullOrWhiteSpace(CachedRefreshToken) || (CachedRefreshToken != Model.RefreshToken))
                {
                    Result.ErrorResult(ApiStatusCodes.RefreshTokenExpired);
                }
                else
                {
                    // refresh token request is a good chance to check if the Identity is valid or not.
                    // An Admin maybe has set the Identity to blocked or something similar
                    ItemResult<ApiClient> ClientResult = await Service.GetByIdAsync(Id);
                    if (ClientResult.Item == null || ClientResult.Item.IsBlocked)
                    {
                        Result.ErrorResult(ApiStatusCodes.InvalidIdentity);
                    }
                    else
                    {
                        // get the culture
                        string CultureCode = Tokens.GetClaimValue(JwtToken.Claims, JwtRegisteredClaimNames.Locale);
                        if (string.IsNullOrWhiteSpace(CultureCode))
                            CultureCode = Lib.Settings.Defaults.CultureCode;

                        // if refresh token is ok, issue a new access and refresh token
                        Result = Tokens.CreateAccessToken(Id, CultureCode);
                    }
                }
            }


            return Result;
        }
 
        [EndpointDescription("Revokes a not-yet-expired Access Token of a client.")]
        [HttpGet("revoke"), Produces<ApiResult>]
        public ApiResult Revoke()
        {
            ApiResult Result = new();

 
            JwtSecurityToken JwtToken = Tokens.ReadTokenFromRequestHeader(HttpContext); 

            string JtiValue = Tokens.GetClaimValue(JwtToken.Claims, JwtRegisteredClaimNames.Jti);  
            string Id = Tokens.GetClaimValue(JwtToken.Claims, JwtRegisteredClaimNames.Sub);
 
            if (string.IsNullOrWhiteSpace(JtiValue) || string.IsNullOrWhiteSpace(Id))
            {
                Result.ErrorResult(ApiStatusCodes.NoIdentityInfoInToken);
            }
            else
            {
                // remove the Jti claim from cache
                string JtiCacheKey = Tokens.GetJtiCacheKey(JtiValue);
                if (!Lib.Cache.ContainsKey(JtiCacheKey))
                {
                    Result.ErrorResult(ApiStatusCodes.TokenExpired);
                }
                else
                {
                    Lib.Cache.Remove(JtiCacheKey);

                    // remove the refresh token from cache
                    string RefreshTokenCacheKey = Tokens.GetRefreshTokenCachKey(Id);
                    Lib.Cache.Remove(RefreshTokenCacheKey);
                }
            } 

            return Result;
        }
    }
}
