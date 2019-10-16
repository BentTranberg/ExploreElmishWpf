namespace ExploreElmishWpf.Models

module CounterPane =

    open System
    open System.Windows
    open Elmish
    open Elmish.WPF

    type Model =
        {
            Count: int
            StepSize: int
        }

    let private initialModel =
        {
            Count = 0
            StepSize = 1
        }

    let init () =
        initialModel, Cmd.none

    type Msg =
        | Increment
        | Decrement
        | SetStepSize of int
        | Reset
        | Close

    let update msg m =
        match msg with
        | Increment -> { m with Count = m.Count + m.StepSize }, Cmd.none
        | Decrement -> { m with Count = m.Count - m.StepSize }, Cmd.none
        | SetStepSize x -> { m with StepSize = x }, Cmd.none
        | Reset -> init ()
        | Close -> m, Cmd.none  // handled by parent

    let bindings () : Binding<Model, Msg> list =
        [
            "CounterValue" |> Binding.oneWay (fun m -> m.Count)
            "Increment" |> Binding.cmd Increment
            "Decrement" |> Binding.cmd Decrement
            "StepSize" |> Binding.twoWay(
                (fun m -> float m.StepSize),
                int >> SetStepSize)
            "Reset" |> Binding.cmdIf (Reset, (<>) initialModel)
            "Close" |> Binding.cmd Close
        ]
