#r "nuget: FsHttp"

open System
open FsHttp
open FsHttp.Operators

// --------------------


% http {
    GET "http://localhost:5000/cities"
    CacheControl "no-cache"
}

