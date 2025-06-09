namespace EFCoreWebApi.Library
{
    static public partial class Tokens
    {
        // ● Claims
        /// <summary>
        /// Returns true if a claim exists in a sequence of claims
        /// </summary>
        static public bool ContainsClaim(IEnumerable<Claim> Claims, string TokenType)
        {
            return FindClaim(Claims, TokenType) != null;
        }
        /// <summary>
        /// Finds and returns a claim, if a claim exists in a sequence of claims, else null.
        /// </summary>
        static public Claim FindClaim(IEnumerable<Claim> Claims, string TokenType)
        {
            TokenType = TokenType.ToLowerInvariant();
            return Claims.FirstOrDefault(item => item.Type.ToLowerInvariant() == TokenType);
        }

        /// <summary>
        /// Returns the value of a claim, if a claim exists in a sequence of claims, else null.
        /// </summary>
        static public string GetClaimValue(IEnumerable<Claim> Claims, string TokenType)
        {
            Claim Claim = FindClaim(Claims, TokenType);
            return Claim != null ? Claim.Value : null;
        }
        /// <summary>
        /// Returns the value of a claim, if any, or a default value.
        /// </summary>
        static public T GetClaimValue<T>(IEnumerable<Claim> Claims, string ClaimType, T DefaultValue = default(T))
        {
            Claim Claim = Claims.Where(c => c.Type == ClaimType).FirstOrDefault();
            return GetClaimValue(Claim, DefaultValue);
        }
        /// <summary>
        /// Returns the value of a claim, if not null, else returns a default value.
        /// </summary>
        static public T GetClaimValue<T>(Claim Claim, T DefaultValue = default(T))
        {
            return Claim != null ? (T)Convert.ChangeType(Claim.Value, typeof(T)) : DefaultValue;
        }
    }
}
