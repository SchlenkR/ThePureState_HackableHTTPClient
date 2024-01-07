
#r "nuget: System.IdentityModel.Tokens.Jwt"

open System
open System.IdentityModel.Tokens.Jwt
open System.Security.Claims
open System.Text
open Microsoft.IdentityModel.Tokens
    
let claimUserName = "name"
let claimRights = ClaimTypes.Role

let private createSecurityKey (key: string) = 
    SymmetricSecurityKey(Encoding.ASCII.GetBytes(key))

let encode key issuer userName (rights: seq<string>) =
    let tokenDescriptor =
        let utcNow = DateTime.UtcNow
        SecurityTokenDescriptor(
            Subject = ClaimsIdentity(
                [
                    Claim(claimUserName, userName)
                    for role in rights do
                        Claim(claimRights, role)
                ]),
            Expires = utcNow.Add(TimeSpan.FromMinutes 5.0),
            Issuer = issuer,
            IssuedAt = utcNow,
            SigningCredentials = SigningCredentials(
                createSecurityKey key,
                SecurityAlgorithms.HmacSha256)
        )
    let tokenHandler = JwtSecurityTokenHandler()
    let token = tokenHandler.CreateToken(tokenDescriptor)
        
    tokenHandler.WriteToken(token)

let decode(key, issuer, tokenString: string) =
    let tokenHandler = JwtSecurityTokenHandler()
    let claimsPrincipal, secToken = 
        tokenHandler.ValidateToken(
            tokenString,
            TokenValidationParameters(
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = false,
                ValidateLifetime = false,
                IssuerSigningKey = createSecurityKey key
        )
    )
    claimsPrincipal, secToken
