namespace ExploreElmishWpf.Models

module Form2 =

    open System
    open Elmish
    open Elmish.WPF

    type Model =
        {
            Input1: string
            Input2: string
        }

    type Msg =
        | Text1Input of string
        | Text2Input of string
        | Submit

    let init =
        {
            Input1 = ""
            Input2 = ""
        }

    let update msg m =
        match msg with
        | Text1Input s -> { m with Input1 = s }
        | Text2Input s -> { m with Input2 = s }
        | Submit -> m  // handled by parent

    let bindings () : Binding<Model, Msg> list =
        [
            "Input1" |> Binding.twoWay ((fun m -> m.Input1), Text1Input)
            "Input2" |> Binding.twoWay ((fun m -> m.Input2), Text2Input)
            "Submit" |> Binding.cmd Submit
        ]
