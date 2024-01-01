#r "nuget: FsHttp,13.3.0"

open System
open FsHttp

http {
    GET "https://localhost:5000/cities"
}
|> Request.send

