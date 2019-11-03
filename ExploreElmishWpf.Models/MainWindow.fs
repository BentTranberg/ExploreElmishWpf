namespace ExploreElmishWpf.Models

module MainWindow =

    open System
    open System.Windows
    open Elmish
    open Elmish.WPF

    type WorkPane =
        | Form1 of Form1.Model
        | Form2 of Form2.Model
        | CounterPane of CounterPane.Model
        | TabsPane of TabsPane.Model

    type HelpPane =
        | HelpContentPane of HelpContentPane.Model
        | AboutPage of AboutBox.Model

    type Model =
        {
            TabIndex: int
            WorkPane: WorkPane option
            HelpPane: HelpPane option
        }

    let init () =
        {
            TabIndex = 0
            WorkPane = None
            HelpPane = None
        }, Cmd.none

    type Msg =
        | SetTabIndex of tabIndex: int

        | ShowForm1
        | ShowForm2
        | ShowCounterPane
        | ShowTabsPane
        | ShowHelpContentPane
        | ShowAboutPage

        | Form1Msg of Form1.Msg
        | Form2Msg of Form2.Msg
        | CounterPaneMsg of CounterPane.Msg
        | TabsPaneMsg of TabsPane.Msg
        | HelpContentPaneMsg of HelpContentPane.Msg
        | AboutPageMsg of AboutBox.Msg

    let update msg m =
        match msg with
        | SetTabIndex tabIndex -> { m with TabIndex = tabIndex }, Cmd.none

        | ShowForm1 -> { m with WorkPane = Form1.init |> Form1 |> Some }, Cmd.none
        | ShowForm2 -> { m with WorkPane = Form2.init |> Form2 |> Some }, Cmd.none
        | ShowCounterPane ->
            let m', cmd' = CounterPane.init ()
            { m with WorkPane = CounterPane m' |> Some }, cmd'
        | ShowTabsPane ->
            let m', cmd' = TabsPane.init ()
            { m with WorkPane = TabsPane m' |> Some }, cmd'
        | ShowHelpContentPane -> { m with HelpPane = HelpContentPane.init () |> HelpContentPane |> Some }, Cmd.none
        | ShowAboutPage -> { m with HelpPane = AboutBox.init |> AboutPage |> Some }, Cmd.none

        | Form1Msg Form1.Submit -> { m with WorkPane = None }, Cmd.none
        | Form1Msg msg' ->
            match m.WorkPane with
            | Some (Form1 m') -> { m with WorkPane = Form1.update msg' m' |> Form1 |> Some }, Cmd.none
            | _ -> m, Cmd.none
        | Form2Msg Form2.Submit -> { m with WorkPane = None }, Cmd.none
        | Form2Msg msg' ->
            match m.WorkPane with
            | Some (Form2 m') -> { m with WorkPane = Form2.update msg' m' |> Form2 |> Some }, Cmd.none
            | _ -> m, Cmd.none
        | CounterPaneMsg CounterPane.Close -> { m with WorkPane = None }, Cmd.none
        | CounterPaneMsg counterPaneMsg ->
            match m.WorkPane with
            | Some (CounterPane m') ->
                let pane, paneCmd = CounterPane.update counterPaneMsg m'
                { m with WorkPane = pane |> CounterPane |> Some }, paneCmd
            | _ -> m, Cmd.none
        | TabsPaneMsg TabsPane.Close -> { m with WorkPane = None }, Cmd.none
        | TabsPaneMsg tabsPaneMsg ->
            match m.WorkPane with
            | Some (TabsPane m') ->
                let pane, paneCmd = TabsPane.update tabsPaneMsg m'
                { m with WorkPane = pane |> TabsPane |> Some }, paneCmd
            | _ -> m, Cmd.none
        | HelpContentPaneMsg msg' ->
            match m.HelpPane with
            | Some (HelpContentPane m') -> { m with HelpPane = HelpContentPane.update msg' m' |> HelpContentPane |> Some }, Cmd.none
            | _ -> m, Cmd.none
        | AboutPageMsg msg' ->
            match m.HelpPane with
            | Some (AboutPage m') -> { m with HelpPane = AboutBox.update msg' m' |> AboutPage |> Some }, Cmd.none
            | _ -> m, Cmd.none

    let bindings (model: Model) dispatch : Binding<Model, Msg> list =
        [
            "TabIndex" |> Binding.twoWay ((fun m -> m.TabIndex), (fun tabIndex -> SetTabIndex tabIndex))
            "NoPanesVisible" |> Binding.oneWay (fun m -> m.WorkPane.IsNone && m.HelpPane.IsNone)
            "WorkPaneVisible" |> Binding.oneWay (fun m -> m.WorkPane.IsSome && m.TabIndex = 0)
            "HelpPaneVisible" |> Binding.oneWay (fun m -> m.HelpPane.IsSome && m.TabIndex = 1)

            "ShowForm1" |> Binding.cmd ShowForm1
            "ShowForm2" |> Binding.cmd ShowForm2
            "ShowCounterPane" |> Binding.cmd ShowCounterPane
            "ShowTabsPane" |> Binding.cmd ShowTabsPane
            "ShowHelpContentPane" |> Binding.cmd ShowHelpContentPane
            "ShowAboutPage" |> Binding.cmd ShowAboutPage

            "Form1Visible" |> Binding.oneWay
                (fun m -> match m.WorkPane with Some (Form1 _) -> true | _ -> false)
            "Form2Visible" |> Binding.oneWay
                (fun m -> match m.WorkPane with Some (Form2 _) -> true | _ -> false)
            "CounterPaneVisible" |> Binding.oneWay
                (fun m -> match m.WorkPane with Some (CounterPane _) -> true | _ -> false)
            "TabsPaneVisible" |> Binding.oneWay
                (fun m -> match m.WorkPane with Some (TabsPane _) -> true | _ -> false)
            "HelpContentPaneVisible" |> Binding.oneWay
                (fun m -> match m.HelpPane with Some (HelpContentPane _) -> true | _ -> false)
            "AboutPaneVisible" |> Binding.oneWay
                (fun m -> match m.HelpPane with Some (AboutPage _) -> true | _ -> false)

            "Form1" |> Binding.subModelOpt (
                (fun m -> match m.WorkPane with Some (Form1 m') -> Some m' | _ -> None),
                snd, Form1Msg, Form1.bindings)
            "Form2" |> Binding.subModelOpt (
                (fun m -> match m.WorkPane with Some (Form2 m') -> Some m' | _ -> None),
                snd, Form2Msg, Form2.bindings)
            "CounterPane" |> Binding.subModelOpt (
                (fun m -> match m.WorkPane with Some (CounterPane m') -> Some m' | _ -> None),
                snd, CounterPaneMsg, CounterPane.bindings)
            "TabsPane" |> Binding.subModelOpt (
                (fun m -> match m.WorkPane with Some (TabsPane m') -> Some m' | _ -> None),
                snd, TabsPaneMsg, TabsPane.bindings)
            "HelpContentPane" |> Binding.subModelOpt (
                (fun m -> match m.HelpPane with Some (HelpContentPane m') -> Some m' | _ -> None),
                snd, HelpContentPaneMsg, HelpContentPane.bindings)
            "AboutPage" |> Binding.subModelOpt (
                (fun m -> match m.HelpPane with Some (AboutPage m') -> Some m' | _ -> None),
                snd, AboutPageMsg, AboutBox.bindings)
        ]

    let designTimeModel =
        let model = { TabIndex = 0; WorkPane = None; HelpPane = None }
        let dispatch = fun _ -> ()
        ViewModel.designInstance model (bindings model dispatch)

    let entryPoint (_: string[], mainWindow: Window) =
        Program.mkProgram init update bindings
        |> Program.runWindowWithConfig
            { ElmConfig.Default with LogTrace = true; Measure = true; MeasureLimitMs = 1 }
            mainWindow
