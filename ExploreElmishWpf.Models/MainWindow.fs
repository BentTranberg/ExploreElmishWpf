namespace ExploreElmishWpf.Models

module MainWindow =

    open System
    open System.Windows
    open Elmish
    open Elmish.WPF

    type Model =
        {
            Count: int
            StepSize: int
        }

    let initialModel =
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

    let update msg m =
        match msg with
        | Increment -> { m with Count = m.Count + m.StepSize }, Cmd.none
        | Decrement -> { m with Count = m.Count - m.StepSize }, Cmd.none
        | SetStepSize x -> { m with StepSize = x }, Cmd.none
        | Reset -> init ()

    let bindings (model: Model) dispatch : Binding<Model, Msg> list =
        [
            "CounterValue" |> Binding.oneWay (fun m -> m.Count)
            "Increment" |> Binding.cmd Increment
            "Decrement" |> Binding.cmd Decrement
            "StepSize" |> Binding.twoWay(
                (fun m -> float m.StepSize),
                int >> SetStepSize)
            "Reset" |> Binding.cmdIf (Reset, (<>) initialModel)
        ]

    let entryPoint (_: string[], mainWindow: Window) =
        Program.mkProgram init update bindings
        |> Program.runWindowWithConfig
            { ElmConfig.Default with LogTrace = true; Measure = true; MeasureLimitMs = 1 }
            mainWindow
