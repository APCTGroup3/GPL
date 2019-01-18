namespace ParserEngine
module Context =
    open System
    open System.Collections.Generic
    open EngineLibrary
    (* Defines a program context in which local variables are stored *)
    type Context() =
        let mutable varStore: Collections.Generic.Dictionary<string, Terminal> = new Collections.Generic.Dictionary<string, Terminal>()

        //Saves a value to the variable store
        member this.Store (id: string, var: Terminal) =
            if varStore.ContainsKey(id) then do
                varStore.Remove(id) |> ignore
            varStore.Add(id, var)
            1


        //Saves a value to an element in a given array
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

        //Retrieves and returns the value with the given variable name
        member this.Get (id: string) : Terminal =
            let res = varStore.Item(id)
            res

        //Returns the specified element of a given array
        member this.GetArrElement(id:string, element:Number) =
            let arr = this.Get(id)
            if not (arr :? Arr) then do
                raise (Exception("Variable is not an array"))
            (arr :?> Arr).Get(element)

        //Deletes an element from the variable store
        member this.Delete (id: string) =
            varStore.Remove(id)
