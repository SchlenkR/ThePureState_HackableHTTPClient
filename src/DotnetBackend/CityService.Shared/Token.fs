module CityService.Shared.Token

open System
open System.IdentityModel.Tokens.Jwt
open System.Security.Claims
open System.Text
open Microsoft.IdentityModel.Tokens
open Microsoft.AspNetCore.Http
open System.Runtime.CompilerServices
    
let claimUserName = ClaimTypes.Name
let claimRights = ClaimTypes.Role

let createSecurityKey (key: string) = 
    SymmetricSecurityKey(Encoding.ASCII.GetBytes(key))

let encode(key, issuer, userName, rights: seq<string>) =
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
                SecurityAlgorithms.HmacSha256Signature)
        )
    let tokenHandler = JwtSecurityTokenHandler()
    let token = tokenHandler.CreateToken(tokenDescriptor)
        
    tokenHandler.WriteToken(token)

let getTokenValidationParams key issuer =
    TokenValidationParameters(
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = false,
        ValidateLifetime = false,
        IssuerSigningKey = createSecurityKey key
    )

let decode(key, issuer, tokenString: string) =
    let tokenHandler = JwtSecurityTokenHandler()
    let claimsPrincipal, secToken = 
        tokenHandler.ValidateToken(
            tokenString,
            getTokenValidationParams key issuer)
    claimsPrincipal, secToken

let getClaimValues claimsType (claims: Claim seq) =
    claims
    |> Seq.filter (fun x -> x.Type = claimsType)
    |> Seq.map (fun claim -> claim.Value)
    |> Seq.toList

type AuthExtensions =

    [<Extension>]
    static member GetUserName(claims: Claim seq) =
        claims
        |> getClaimValues claimUserName
        |> Seq.exactlyOne

    [<Extension>]
    static member GetRoleNames(claims: Claim seq) =
        claims 
        |> getClaimValues claimRights

    // shortcuts for Request
    
    [<Extension>]
    static member GetUserName(request: HttpRequest) =
        request.HttpContext.User.Claims |> AuthExtensions.GetUserName

    [<Extension>]
    static member GetRoleNames(request: HttpRequest) =
        request.HttpContext.User.Claims |> AuthExtensions.GetRoleNames
