namespace ParserEngine
module Types = 
    open System

    [<AbstractClass>]
    type Terminal(typename: string) =
        member this.TypeName = typename
        abstract member ToString : String
        member this.ToDouble() = Double.Parse(this.ToString)
        member this.ToBool() = bool.Parse(this.ToString)

    type Boolean(value: bool) =
        inherit Terminal("Boolean")
        member this.Value = value
        override this.ToString = this.Value.ToString()

    type Number(value: double) =
        inherit Terminal("Number")
        member this.Value = value
        static member (+) (a: Number, b: Number) =
            Number(a.Value + b.Value)
        static member (-) (a: Number, b: Number) =
            Number(a.Value - b.Value)
        static member (/) (a: Number, b: Number) =
            Number(a.Value / b.Value)
        static member (*) (a: Number, b: Number) =
            Number(a.Value * b.Value)
        static member pow (a:Number, b:Number) =
            Number(a.Value ** b.Value)

        override this.ToString = this.Value.ToString()

    type Str(value: string) =
        inherit Terminal("String")
        member this.Value = value
        override this.ToString = this.Value.ToString()

    type Void() =
        inherit Terminal("Void")
        member this.Value = ""
        override this.ToString = ""