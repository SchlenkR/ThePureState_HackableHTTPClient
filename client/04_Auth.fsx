#r "nuget: FsHttp"

open System
open FsHttp
open FsHttp.Operators


// --------------------

#load "./shared/jwt.fsx"
#load "./shared/vault.fsx"

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

% http {
    GET "http://localhost:5000/cities"
}


let httpAuth () =
    http {
        AuthorizationBearer (mkToken ())
    }

% httpAuth () {
    DELETE "http://localhost:5000/cities/paris"
}
