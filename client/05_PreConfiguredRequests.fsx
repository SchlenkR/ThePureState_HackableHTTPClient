#r "nuget: FsHttp"

#load "./shared/vault.fsx"
#load "./shared/jwt.fsx"

open System
open FsHttp
open FsHttp.Operators

// --------------------

type AuthInfo = 
    {
        secKey: string
        issuer: string
    }

type EnvInfo =
    {
        authInfo: AuthInfo
        baseUrl: string
    }

module Envs =
    let local = 
        {
            baseUrl = "http://localhost:5000"
            authInfo = {
                secKey = Vault.localEnv.secKey
                issuer = Vault.localEnv.issuer 
            }
        }

    let testing = 
        {
            baseUrl = "http://localhost:6000"
            authInfo = {
                secKey = Vault.testingEnv.secKey
                issuer = Vault.testingEnv.issuer 
            }
        }

let mkToken authInfo = 
    Jwt.encode authInfo.secKey authInfo.issuer "Ronald" [ "admin" ]

let httpEnv envInfo =
    http {
        config_transformHeader (fun header ->
            let relativeUrl = header.target.address.Value
            { header with target.address = Some (envInfo.baseUrl </> relativeUrl) }
        )
        AuthorizationBearer (mkToken envInfo.authInfo)
    }

// --------------------








% httpEnv Envs.testing {
    DELETE "cities/paris"
}
