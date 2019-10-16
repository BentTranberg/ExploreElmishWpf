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

    let init =
        {
            Count = 0
            StepSize = 1
        }

    type Msg =
        | Increment
        | Decrement
        | SetStepSize of int
        | Reset

    let update msg m =
        match msg with
        | Increment -> { m with Count = m.Count + m.StepSize }
        | Decrement -> { m with Count = m.Count - m.StepSize }
        | SetStepSize x -> { m with StepSize = x }
        | Reset -> init

    let bindings () : Binding<Model, Msg> list =
        [
            "CounterValue" |> Binding.oneWay (fun m -> m.Count)
            "Increment" |> Binding.cmd Increment
            "Decrement" |> Binding.cmd Decrement
            "StepSize" |> Binding.twoWay(
                (fun m -> float m.StepSize),
                int >> SetStepSize)
            "Reset" |> Binding.cmdIf (Reset, (<>) init)
        ]

    let entryPoint (_: string[], mainWindow: Window) =
        Program.mkSimpleWpf (fun () -> init) update bindings
        |> Program.withConsoleTrace
        |> Program.runWindowWithConfig
            { ElmConfig.Default with LogConsole = true; Measure = true }
            mainWindow
