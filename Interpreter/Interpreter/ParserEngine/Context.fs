namespace ParserEngine
module Context =
    open System
    open System.Collections.Generic
    open EngineLibrary

    type Context() =
        let mutable varStore: Collections.Generic.Dictionary<string, Terminal> = new Collections.Generic.Dictionary<string, Terminal>()

        member this.Store (id: string, var: Terminal) =
            if varStore.ContainsKey(id) then do
                varStore.Remove(id) |> ignore
            varStore.Add(id, var)
            1

        member this.StoreArr(id:string, element:Number, var: Terminal) =
            if varStore.ContainsKey(id) then do
                let currentVar = this.Get(id)
                if not (currentVar :? Arr) then do
                    varStore.Remove(id) |> ignore
                    varStore.Add(id, new Arr()) |> ignore
                let arr = this.Get(id) :?> Arr
                arr.Add(element, var)
            else
                varStore.Add(id, new Arr()) |> ignore
                let arr = this.Get(id) :?> Arr
                arr.Add(element, var)


        member this.Get (id: string) : Terminal =
            let res = varStore.Item(id)
            res

        member this.GetArrElement(id:string, element:Number) =
            let arr = this.Get(id)
            if not (arr :? Arr) then do
                raise (Exception("Variable is not an array"))
            (arr :?> Arr).Get(element)
        

        member this.Delete (id: string) =
            varStore.Remove(id)
