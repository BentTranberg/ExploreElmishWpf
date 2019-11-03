namespace ExploreElmishWpf.Models

module CounterDemo =

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
            "Count" |> Binding.oneWay (fun m -> m.Count)
            "Increment" |> Binding.cmd Increment
            "Decrement" |> Binding.cmd Decrement
            "StepSize" |> Binding.twoWay ((fun m -> float m.StepSize), int >> SetStepSize)
            "Reset" |> Binding.cmdIf (Reset, (<>) initialModel)
            "Close" |> Binding.cmd Close
        ]

    // This is the design time model actually used for CounterPane, and it's the only place in the solution where
    // this way of doing it is demonstrated.
    // Unfortunately it does not work reliably in .NET Core 3.0 at this time. You will frequently not see the
    // design time data.
    // If you change the framework target to one of .NET Framework, then it will work reliably.
    // If you type a letter in front of the "{" after "DataContext", and then remove it, refresh is triggered.
    // However, the data won't display properly for long, so you have to repeat doing this "manual" refresh.
    // The corresponding XAML :
    // d:DataContext="{x:Static vm:CounterPane.designTimeModel}"
    let dtModel = { Count = 3; StepSize = 7 }
    let dtBindings = bindings ()
    let designTimeModel = ViewModel.designInstance dtModel dtBindings

// An alternative design time model that does not have the problem.
// This way of doing it works reliably also in .NET Core 3.0, and is the choice elsewhere in this solution until
// the problem is fixed. The drawback is that the runtime model type can't be used - but that turns out not to
// be that much of a drawback. It's usually simpler and quicker, and the fact that the two models might get out
// of sync turns out not to be a big deal. And although the runtime model can't be used directly, often the
// surrounding functionality is quite helpful.
// The reason for this choice is that I will be developing for .NET Core 3+ in the future, and not .NET Framework,
// so I want to go with the solution that works reliably for .NET Core now, to avoid frustrations while designing.
// The corresponding XAML :
// d:DataContext="{d:DesignInstance vm:CounterPaneDesignTimeModel, IsDesignTimeCreatable=True}"
type CounterPaneDesignTimeModel() =
    member val Count = 4 with get, set
    member val StepSize = 8 with get, set
