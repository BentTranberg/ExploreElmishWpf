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
        with
            member x.form1Page = match x.WorkPane with Some (Form1Page page) -> Some page | _ -> None
            member x.form2Page = match x.WorkPane with Some (Form2Page page) -> Some page | _ -> None
            member x.counterPage = match x.WorkPane with Some (CounterPage page) -> Some page | _ -> None
            member x.tabsPage = match x.WorkPane with Some (TabsPage page) -> Some page | _ -> None
            member x.helpContentPage = match x.HelpPane with Some (HelpContentPage page) -> Some page | _ -> None
            member x.aboutPage = match x.HelpPane with Some (AboutPage page) -> Some page | _ -> None

    let showTab0 m = { m with TabIndex = 0 }
    let showTab1 m = { m with TabIndex = 1 }
    let clearWorkPane m = { m with WorkPane = None }
    let clearWorkPaneCmd m = { m with WorkPane = None }, Cmd.none
    let setWorkPaneIf cond f m = if cond then { m with WorkPane = Some (f ()) } else m
    let setHelpPaneIf cond f m = if cond then { m with HelpPane = Some (f ()) } else m
    let setWorkPaneCmdIf cond f m =
        if cond then
            let m', c' = f ()
            { m with WorkPane = Some m' }, c'
        else
            m, Cmd.none

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

        | ShowForm1 -> m |> showTab0 |> setWorkPaneIf m.form1Page.IsNone (fun () -> Form1.init |> Form1Page), Cmd.none
        | ShowForm2 -> m |> showTab0 |> setWorkPaneIf m.form2Page.IsNone (fun () -> Form2.init |> Form2Page), Cmd.none
        | ShowCounterPage -> m |> showTab0 |> setWorkPaneCmdIf m.counterPage.IsNone (fun () ->
                let m', c' = CounterDemo.init ()
                CounterPage m', c')
        | ShowTabsPage -> m |> showTab0 |> setWorkPaneCmdIf m.tabsPage.IsNone (fun () ->
                let m', c' = TabsDemo.init ()
                TabsPage m', c')
        | ShowHelpContentPage -> m |> showTab1 |> setHelpPaneIf m.helpContentPage.IsNone (HelpContent.init >> HelpContentPage), Cmd.none
        | ShowAboutPage -> m |> showTab1 |> setHelpPaneIf m.aboutPage.IsNone (fun () -> AboutBox.init |> AboutPage), Cmd.none

        | Form1Msg Form1.Submit -> clearWorkPaneCmd m
        | Form1Msg msg' ->
            match m.form1Page with
            | Some m' -> { m with WorkPane = Form1.update msg' m' |> Form1Page |> Some }, Cmd.none
            | _ -> m, Cmd.none
        | Form2Msg Form2.Submit -> clearWorkPaneCmd m
        | Form2Msg msg' ->
            match m.form2Page with
            | Some m' -> { m with WorkPane = Form2.update msg' m' |> Form2Page |> Some }, Cmd.none
            | _ -> m, Cmd.none
        | CounterPageMsg CounterDemo.Close -> clearWorkPaneCmd m
        | CounterPageMsg counterPageMsg ->
            match m.counterPage with
            | Some m' ->
                let pane, paneCmd = CounterDemo.update counterPageMsg m'
                { m with WorkPane = pane |> CounterPage |> Some }, paneCmd
            | _ -> m, Cmd.none
        | TabsPageMsg TabsDemo.Close -> clearWorkPaneCmd m
        | TabsPageMsg tabsPageMsg ->
            match m.tabsPage with
            | Some m' ->
                let pane, paneCmd = TabsDemo.update tabsPageMsg m'
                { m with WorkPane = pane |> TabsPage |> Some }, paneCmd
            | _ -> m, Cmd.none
        | HelpContentPageMsg msg' ->
            match m.helpContentPage with
            | Some m' -> { m with HelpPane = HelpContent.update msg' m' |> HelpContentPage |> Some }, Cmd.none
            | _ -> m, Cmd.none
        | AboutPageMsg msg' ->
            match m.aboutPage with
            | Some m' -> { m with HelpPane = AboutBox.update msg' m' |> AboutPage |> Some }, Cmd.none
            | _ -> m, Cmd.none

    let bindings (model: Model) dispatch : Binding<Model, Msg> list =
        [
            "TabIndex" |> Binding.twoWay ((fun m -> m.TabIndex), (fun tabIndex -> SetTabIndex tabIndex))
            "NoPanesVisible" |> Binding.oneWay (fun m -> m.WorkPane.IsNone && m.TabIndex = 0 || m.HelpPane.IsNone && m.TabIndex = 1)
            "WorkPaneVisible" |> Binding.oneWay (fun m -> m.WorkPane.IsSome && m.TabIndex = 0)
            "HelpPaneVisible" |> Binding.oneWay (fun m -> m.HelpPane.IsSome && m.TabIndex = 1)

            "ShowForm1Page" |> Binding.cmd ShowForm1
            "ShowForm2Page" |> Binding.cmd ShowForm2
            "ShowCounterPage" |> Binding.cmd ShowCounterPage
            "ShowTabsPage" |> Binding.cmd ShowTabsPage
            "ShowHelpContentPage" |> Binding.cmd ShowHelpContentPage
            "ShowAboutPage" |> Binding.cmd ShowAboutPage

            "Form1PageVisible" |> Binding.oneWay (fun m -> m.form1Page.IsSome)
            "Form2PageVisible" |> Binding.oneWay (fun m -> m.form2Page.IsSome)
            "CounterPageVisible" |> Binding.oneWay (fun m -> m.counterPage.IsSome)
            "TabsPageVisible" |> Binding.oneWay (fun m -> m.tabsPage.IsSome)
            "HelpContentPageVisible" |> Binding.oneWay (fun m -> m.helpContentPage.IsSome)
            "AboutPageVisible" |> Binding.oneWay (fun m -> m.aboutPage.IsSome)

            "Form1Page" |> Binding.subModelOpt ((fun (m: Model) -> m.form1Page), snd, Form1Msg, Form1.bindings)
            "Form2Page" |> Binding.subModelOpt ((fun (m: Model) -> m.form2Page), snd, Form2Msg, Form2.bindings)
            "CounterPage" |> Binding.subModelOpt ((fun (m: Model) -> m.counterPage), snd, CounterPageMsg, CounterDemo.bindings)
            "TabsPage" |> Binding.subModelOpt ((fun (m: Model) -> m.tabsPage), snd, TabsPageMsg, TabsDemo.bindings)
            "HelpContentPage" |> Binding.subModelOpt ((fun (m: Model) -> m.helpContentPage), snd, HelpContentPageMsg, HelpContent.bindings)
            "AboutPage" |> Binding.subModelOpt ((fun (m: Model) -> m.aboutPage), snd, AboutPageMsg, AboutBox.bindings)
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
