namespace ExploreElmishWpf.Models

module HelpContent =

    open System
    open Elmish
    open Elmish.WPF

    type Model =
        {
            ProofId: Guid
        }

    type Msg =
        | Dummy

    let init () =
        {
            ProofId = Guid.NewGuid()
        }

    let update msg m =
        m

    let bindings () : Binding<Model, Msg> list =
        [
            "ProofId" |> Binding.oneWay (fun m -> string m.ProofId)
        ]
