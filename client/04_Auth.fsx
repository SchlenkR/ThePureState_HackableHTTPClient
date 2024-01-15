#r "nuget: FsHttp"

open System
open FsHttp
open FsHttp.Operators

#load "./shared/jwt.fsx"
#load "./shared/vault.fsx"

// --------------------

let mkToken () =
    Jwt.encode
        Vault.localEnv.secKey
        Vault.localEnv.issuer
        "Ronald"
        [ "admin" ]

let httpAuth () =
    http {
        AuthorizationBearer (mkToken ())
    }

// --------------------



% httpAuth () {
    DELETE "http://localhost:5000/cities/paris"
}
