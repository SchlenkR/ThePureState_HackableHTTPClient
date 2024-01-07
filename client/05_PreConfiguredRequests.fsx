#r "nuget: FsHttp"

#load "./shared/vault.fsx"
#load "./shared/jwt.fsx"

open System
open FsHttp
open FsHttp.Operators


// --------------------


let httpAuth =
    http {
        AuthorizationBearer (
            Jwt.encode
                Vault.localEnv.secKey
                Vault.localEnv.issuer
                "Ronald"
                [ "admin" ]
        )
        CacheControl "no-cache"
    }


httpAuth {
    DELETE "http://localhost:5000/cities/frankfurt"
}

