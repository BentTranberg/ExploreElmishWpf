namespace ExploreElmishWpf.Models

module MainWindow =

    open System
    open System.Windows
    open Elmish
    open Elmish.WPF

    type Pane =
        | Form1 of Form1.Model
        | Form2 of Form2.Model
        | CounterPane of CounterPane.Model
        | TabsPane of TabsPane.Model
        | HelpPane of HelpPane.Model
        | AboutPane of AboutPane.Model

    type Model =
        {
            Pane: Pane option
        }

    let init () =
        {
            Pane = None
        }, Cmd.none

    type Msg =
        | ShowForm1
        | ShowForm2
        | ShowCounterPane
        | ShowTabsPane
        | ShowHelpPane
        | ShowAboutPane

        | Form1Msg of Form1.Msg
        | Form2Msg of Form2.Msg
        | CounterPaneMsg of CounterPane.Msg
        | TabsPaneMsg of TabsPane.Msg
        | HelpPaneMsg of HelpPane.Msg
        | AboutPaneMsg of AboutPane.Msg

    let update msg m =
        match msg with
        | ShowForm1 -> { m with Pane = Some <| Form1 Form1.init }, Cmd.none
        | ShowForm2 -> { m with Pane = Some <| Form2 Form2.init }, Cmd.none
        | ShowCounterPane ->
            let m', cmd' = CounterPane.init ()
            { m with Pane = Some <| CounterPane m' }, cmd'
        | ShowTabsPane ->
            let m', cmd' = TabsPane.init ()
            { m with Pane = Some <| TabsPane m' }, cmd'
        | ShowHelpPane -> { m with Pane = Some <| HelpPane HelpPane.init }, Cmd.none
        | ShowAboutPane -> { m with Pane = Some <| AboutPane AboutPane.init }, Cmd.none

        | Form1Msg Form1.Submit -> { m with Pane = None }, Cmd.none
        | Form1Msg msg' ->
            match m.Pane with
            | Some (Form1 m') -> { m with Pane = Form1.update msg' m' |> Form1 |> Some }, Cmd.none
            | _ -> m, Cmd.none
        | Form2Msg Form2.Submit -> { m with Pane = None }, Cmd.none
        | Form2Msg msg' ->
            match m.Pane with
            | Some (Form2 m') -> { m with Pane = Form2.update msg' m' |> Form2 |> Some }, Cmd.none
            | _ -> m, Cmd.none
        | CounterPaneMsg CounterPane.Close -> { m with Pane = None }, Cmd.none
        | CounterPaneMsg counterPaneMsg ->
            match m.Pane with
            | Some (CounterPane m') ->
                let pane, paneCmd = CounterPane.update counterPaneMsg m'
                { m with Pane = pane |> CounterPane |> Some }, paneCmd
            | _ -> m, Cmd.none
        | TabsPaneMsg TabsPane.Close -> { m with Pane = None }, Cmd.none
        | TabsPaneMsg tabsPaneMsg ->
            match m.Pane with
            | Some (TabsPane m') ->
                let pane, paneCmd = TabsPane.update tabsPaneMsg m'
                { m with Pane = pane |> TabsPane |> Some }, paneCmd
            | _ -> m, Cmd.none
        | HelpPaneMsg msg' ->
            match m.Pane with
            | Some (HelpPane m') -> { m with Pane = HelpPane.update msg' m' |> HelpPane |> Some }, Cmd.none
            | _ -> m, Cmd.none
        | AboutPaneMsg msg' ->
            match m.Pane with
            | Some (AboutPane m') -> { m with Pane = AboutPane.update msg' m' |> AboutPane |> Some }, Cmd.none
            | _ -> m, Cmd.none

    let bindings (model: Model) dispatch : Binding<Model, Msg> list =
        [
            "PaneVisible" |> Binding.oneWay (fun m -> m.Pane.IsSome)
            "NotPaneVisible" |> Binding.oneWay (fun m -> m.Pane.IsNone)

            "ShowForm1" |> Binding.cmd ShowForm1
            "ShowForm2" |> Binding.cmd ShowForm2
            "ShowCounterPane" |> Binding.cmd ShowCounterPane
            "ShowTabsPane" |> Binding.cmd ShowTabsPane
            "ShowHelpPane" |> Binding.cmd ShowHelpPane
            "ShowAboutPane" |> Binding.cmd ShowAboutPane

            "Form1Visible" |> Binding.oneWay
                (fun m -> match m.Pane with Some (Form1 _) -> true | _ -> false)
            "Form2Visible" |> Binding.oneWay
                (fun m -> match m.Pane with Some (Form2 _) -> true | _ -> false)
            "CounterPaneVisible" |> Binding.oneWay
                (fun m -> match m.Pane with Some (CounterPane _) -> true | _ -> false)
            "TabsPaneVisible" |> Binding.oneWay
                (fun m -> match m.Pane with Some (TabsPane _) -> true | _ -> false)
            "HelpPaneVisible" |> Binding.oneWay
                (fun m -> match m.Pane with Some (HelpPane _) -> true | _ -> false)
            "AboutPaneVisible" |> Binding.oneWay
                (fun m -> match m.Pane with Some (AboutPane _) -> true | _ -> false)

            "Form1" |> Binding.subModelOpt (
                (fun m -> match m.Pane with Some (Form1 m') -> Some m' | _ -> None),
                snd, Form1Msg, Form1.bindings)
            "Form2" |> Binding.subModelOpt (
                (fun m -> match m.Pane with Some (Form2 m') -> Some m' | _ -> None),
                snd, Form2Msg, Form2.bindings)
            "CounterPane" |> Binding.subModelOpt (
                (fun m -> match m.Pane with Some (CounterPane m') -> Some m' | _ -> None),
                snd, CounterPaneMsg, CounterPane.bindings)
            "TabsPane" |> Binding.subModelOpt (
                (fun m -> match m.Pane with Some (TabsPane m') -> Some m' | _ -> None),
                snd, TabsPaneMsg, TabsPane.bindings)
            "HelpPane" |> Binding.subModelOpt (
                (fun m -> match m.Pane with Some (HelpPane m') -> Some m' | _ -> None),
                snd, HelpPaneMsg, HelpPane.bindings)
            "AboutPane" |> Binding.subModelOpt (
                (fun m -> match m.Pane with Some (AboutPane m') -> Some m' | _ -> None),
                snd, AboutPaneMsg, AboutPane.bindings)
        ]

    let designTimeModel =
        let model = { Pane = None }
        let dispatch = fun _ -> ()
        ViewModel.designInstance model (bindings model dispatch)

    let entryPoint (_: string[], mainWindow: Window) =
        Program.mkProgram init update bindings
        |> Program.runWindowWithConfig
            { ElmConfig.Default with LogTrace = true; Measure = true; MeasureLimitMs = 1 }
            mainWindow
