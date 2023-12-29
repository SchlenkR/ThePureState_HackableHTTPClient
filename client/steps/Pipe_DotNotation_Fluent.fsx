#r "nuget: FsHttp,13.3.0"

open FsHttp


// ------------------------------
// Alternatives
// ------------------------------


// Pipeline syntax
// (most idiomatic and understandable)
http {
    GET "https://localhost:5000/cities"
}
|> Request.send
|> Response.deserializeJson<list<{| name: string; twoLetterIsoCountryCode: string |}>>


// dot-notation
// (parenthesis mandatory for request definition)
(http {
        GET "https://localhost:5000/cities"
    })
    .Send()


// fluent syntax
"https://localhost:5000/cities"
    .Get()
    .Send()

