using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
namespace BlazorUI.Services
{
    public static class JwtParser
    {
        //This auxiliar class is used to parse the JWT token and extract the claims from it.
        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);

            var claims = token.Claims.ToList();
            var normalizedClaims = new List<Claim>(claims);

            foreach (var c in claims)
            {
                // Normalize role claims to "role" for easier handling in Blazor, Need ClaimTypes.Role for Identity and "identity/claims/role" for custom claims
                //Thats why I cant map all the claims to "role", because I need to keep the original claim type
                normalizedClaims.Add(c);
                if (c.Type == ClaimTypes.Role || c.Type.Contains("identity/claims/role"))
                {
                    normalizedClaims.Add(new Claim("role", c.Value));
                }
            }
            return normalizedClaims;

            //This dont work

            //return token.Claims.Select(c =>
            //c.Type.Contains("identity/claims/role")
            //? new Claim("role", c.Value)
            //: c
            //);
        }
    }
}
