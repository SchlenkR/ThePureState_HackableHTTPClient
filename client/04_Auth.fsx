#r "nuget: FsHttp"

#load "./shared/vault.fsx"
#load "./shared/jwt.fsx"

open System
open FsHttp
open FsHttp.Operators


// --------------------


% http {
    DELETE "http://localhost:5000/cities/frankfurt"
}


// --------------------


let mkToken () =
    Jwt.encode
        Vault.localEnv.secKey
        Vault.localEnv.issuer
        "Ronald"
        [ "admin" ]


% http {
    DELETE "http://localhost:5000/cities/frankfurt"
    AuthorizationBearer (mkToken ())
}

