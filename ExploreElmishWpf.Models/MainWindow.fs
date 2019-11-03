namespace ExploreElmishWpf.Models

module MainWindow =

    open System
    open System.Windows
    open Elmish
    open Elmish.WPF

    type WorkPane =
        | Form1Page of Form1.Model
        | Form2Page of Form2.Model
        | CounterPage of CounterDemo.Model
        | TabsPage of TabsDemo.Model

    type HelpPane =
        | HelpContentPage of HelpContent.Model
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
        | ShowCounterPage
        | ShowTabsPage
        | ShowHelpContentPage
        | ShowAboutPage

        | Form1Msg of Form1.Msg
        | Form2Msg of Form2.Msg
        | CounterPageMsg of CounterDemo.Msg
        | TabsPageMsg of TabsDemo.Msg
        | HelpContentPageMsg of HelpContent.Msg
        | AboutPageMsg of AboutBox.Msg

    let update msg m =
        match msg with
        | SetTabIndex tabIndex -> { m with TabIndex = tabIndex }, Cmd.none

        | ShowForm1 -> { m with WorkPane = Form1.init |> Form1Page |> Some }, Cmd.none
        | ShowForm2 -> { m with WorkPane = Form2.init |> Form2Page |> Some }, Cmd.none
        | ShowCounterPage ->
            let m', cmd' = CounterDemo.init ()
            { m with WorkPane = CounterPage m' |> Some }, cmd'
        | ShowTabsPage ->
            let m', cmd' = TabsDemo.init ()
            { m with WorkPane = TabsPage m' |> Some }, cmd'
        | ShowHelpContentPage -> { m with HelpPane = HelpContent.init () |> HelpContentPage |> Some }, Cmd.none
        | ShowAboutPage -> { m with HelpPane = AboutBox.init |> AboutPage |> Some }, Cmd.none

        | Form1Msg Form1.Submit -> { m with WorkPane = None }, Cmd.none
        | Form1Msg msg' ->
            match m.WorkPane with
            | Some (Form1Page m') -> { m with WorkPane = Form1.update msg' m' |> Form1Page |> Some }, Cmd.none
            | _ -> m, Cmd.none
        | Form2Msg Form2.Submit -> { m with WorkPane = None }, Cmd.none
        | Form2Msg msg' ->
            match m.WorkPane with
            | Some (Form2Page m') -> { m with WorkPane = Form2.update msg' m' |> Form2Page |> Some }, Cmd.none
            | _ -> m, Cmd.none
        | CounterPageMsg CounterDemo.Close -> { m with WorkPane = None }, Cmd.none
        | CounterPageMsg counterPageMsg ->
            match m.WorkPane with
            | Some (CounterPage m') ->
                let pane, paneCmd = CounterDemo.update counterPageMsg m'
                { m with WorkPane = pane |> CounterPage |> Some }, paneCmd
            | _ -> m, Cmd.none
        | TabsPageMsg TabsDemo.Close -> { m with WorkPane = None }, Cmd.none
        | TabsPageMsg tabsPageMsg ->
            match m.WorkPane with
            | Some (TabsPage m') ->
                let pane, paneCmd = TabsDemo.update tabsPageMsg m'
                { m with WorkPane = pane |> TabsPage |> Some }, paneCmd
            | _ -> m, Cmd.none
        | HelpContentPageMsg msg' ->
            match m.HelpPane with
            | Some (HelpContentPage m') -> { m with HelpPane = HelpContent.update msg' m' |> HelpContentPage |> Some }, Cmd.none
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

            "ShowForm1Page" |> Binding.cmd ShowForm1
            "ShowForm2Page" |> Binding.cmd ShowForm2
            "ShowCounterPage" |> Binding.cmd ShowCounterPage
            "ShowTabsPage" |> Binding.cmd ShowTabsPage
            "ShowHelpContentPage" |> Binding.cmd ShowHelpContentPage
            "ShowAboutPage" |> Binding.cmd ShowAboutPage

            "Form1PageVisible" |> Binding.oneWay
                (fun m -> match m.WorkPane with Some (Form1Page _) -> true | _ -> false)
            "Form2PageVisible" |> Binding.oneWay
                (fun m -> match m.WorkPane with Some (Form2Page _) -> true | _ -> false)
            "CounterPageVisible" |> Binding.oneWay
                (fun m -> match m.WorkPane with Some (CounterPage _) -> true | _ -> false)
            "TabsPageVisible" |> Binding.oneWay
                (fun m -> match m.WorkPane with Some (TabsPage _) -> true | _ -> false)
            "HelpContentPageVisible" |> Binding.oneWay
                (fun m -> match m.HelpPane with Some (HelpContentPage _) -> true | _ -> false)
            "AboutPageVisible" |> Binding.oneWay
                (fun m -> match m.HelpPane with Some (AboutPage _) -> true | _ -> false)

            "Form1Page" |> Binding.subModelOpt (
                (fun m -> match m.WorkPane with Some (Form1Page m') -> Some m' | _ -> None),
                snd, Form1Msg, Form1.bindings)
            "Form2Page" |> Binding.subModelOpt (
                (fun m -> match m.WorkPane with Some (Form2Page m') -> Some m' | _ -> None),
                snd, Form2Msg, Form2.bindings)
            "CounterPage" |> Binding.subModelOpt (
                (fun m -> match m.WorkPane with Some (CounterPage m') -> Some m' | _ -> None),
                snd, CounterPageMsg, CounterDemo.bindings)
            "TabsPage" |> Binding.subModelOpt (
                (fun m -> match m.WorkPane with Some (TabsPage m') -> Some m' | _ -> None),
                snd, TabsPageMsg, TabsDemo.bindings)
            "HelpContentPage" |> Binding.subModelOpt (
                (fun m -> match m.HelpPane with Some (HelpContentPage m') -> Some m' | _ -> None),
                snd, HelpContentPageMsg, HelpContent.bindings)
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
