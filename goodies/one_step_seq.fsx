
// Given:
let seq1 = [   1;   2;   3 ]
let seq2 = [  10;  20;  30 ]
let seq3 = [ 100; 200; 300 ]


// Goal:
let seqRes = 
           [
              (1,10,100);
                   (2,20,200);
                        (3,30,300) 
           ]























seq {
    for x in [   1;   2;   3 ] do
    for y in [  10;  20;  30 ] do
    for z in [ 100; 200; 300 ] do
    yield x,y,z
}



























module Vide =
    open System.Collections.Generic
    
    type Vide<'v,'s> =
        's option -> ('v * 's) option

    let mkVide (v: Vide<_,_>) = v

    let bind f (m: Vide<_,_>) =
        mkVide <| fun s ->
            let ms,fs =
                match s with
                | None -> None,None
                | Some (ms,fs) -> ms,fs
            match m ms with
            | Some (mv,ms) ->
                let f = f mv
                match f fs with
                | Some (fv,fs) -> Some (fv, (Some ms, Some fs))
                | None -> None
            | None -> None
    
    let return' x =
        mkVide <| fun s -> Some (x, ())

    let ofSeq (sequence: IEnumerable<_>) =
        mkVide <| fun (s: IEnumerator<_> option) ->
            let enumerator =
                match s with
                | None ->
                    let enumerator = sequence.GetEnumerator()
                    if not (enumerator.MoveNext()) then None else Some enumerator
                | Some (enumerator) ->
                    if enumerator.MoveNext() then Some enumerator else None
            match enumerator with
            | None -> None
            | Some enumerator -> Some (enumerator.Current, enumerator)

    let counter startingValue increment =
        mkVide <| fun s ->
            let outValue =
                match s with
                | None -> startingValue
                | Some counter -> counter + increment
            Some (outValue, outValue)

    let toSeq (v: Vide<_,_>) =
        seq { 
            let mutable state = None
            let mutable run = true
            while run do
                match v state with
                | None ->
                    do run <- false
                | Some (x, state') ->
                    do state <- Some state'
                    yield x
        }

    let toList v = v |> toSeq |> Seq.toList

    type StreamBuilder() =
        member _.For(m, f) = bind f (ofSeq m)
        member _.Yield(x) = return' x
        member _.Run(v) = toSeq v

    let stream = StreamBuilder()

















Vide.stream {
    for x in [   1;   2;   3 ] do
    for y in [  10;  20;  30 ] do
    for z in [ 100; 200; 300 ] do
    yield x,y,z
}



seq {
    for x in [   1;   2;   3 ] do
    for y in [  10;  20;  30 ] do
    for z in [ 100; 200; 300 ] do
    yield x,y,z
}


