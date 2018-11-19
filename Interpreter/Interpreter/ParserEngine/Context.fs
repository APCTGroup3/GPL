namespace ParserEngine
module Context =
    open System
    open Types
    open System.Collections.Generic
    open CoreParser

    type Context() =
        let mutable varStore: Collections.Generic.Dictionary<string, Terminal> = new Collections.Generic.Dictionary<string, Terminal>()

        member this.Store (id: string, var: Terminal) =
            if varStore.ContainsKey(id) then do
                varStore.Remove(id)
            varStore.Add(id, var)

        member this.Get (id: string) =
            let res = varStore.Item(id)
            res

        member this.Delete (id: string) =
            varStore.Remove(id)
