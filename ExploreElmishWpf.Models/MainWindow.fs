namespace ExploreElmishWpf.Models

module MainWindow =

    open System
    open System.Windows
    open System.Windows.Media
    open Elmish
    open Elmish.WPF

    let newGuid () = Guid.NewGuid()

    type Toolbutton =
        {
            Id: Guid
            Text: string
            IsMarkable: bool
        }

    type Tab =
        {
            Id: Guid
            Header: string
            Toolbuttons: Toolbutton list
        }

    type Msg =
        | ButtonClick of id: Guid

        | ShowForm1
        | ShowForm2
        | ShowCounter
        | ShowTabs
        | ShowMultiSelect
        | ShowHelp
        | ShowAbout

        | Form1Msg of Form1.Msg
        | Form2Msg of Form2.Msg
        | CounterPageMsg of CounterDemo.Msg
        | TabsPageMsg of TabsDemo.Msg
        | MultiSelectPageMsg of MultiSelectDemo.Msg
        | HelpContentPageMsg of HelpContent.Msg
        | AboutPageMsg of AboutBox.Msg

    type Model =
        {
            Tabs: Tab list
            MarkedButton: Guid
            Form1Page: Form1.Model option
            Form2Page: Form2.Model option
            CounterPage: CounterDemo.Model option
            TabsPage: TabsDemo.Model option
            MultiSelectPage: MultiSelectDemo.Model option
            HelpContentPage: HelpContent.Model option
            AboutPage: AboutBox.Model option
        }
        with
            member x.somePageIsVisible =
                x.Form1Page.IsSome || x.Form2Page.IsSome || x.CounterPage.IsSome
                    || x.TabsPage.IsSome || x.MultiSelectPage.IsSome
                    || x.HelpContentPage.IsSome || x.AboutPage.IsSome

    let tbNone = newGuid ()
    let tbForm1 = newGuid ()
    let tbForm2 = newGuid ()
    let tbCounter = newGuid ()
    let tbTabs = newGuid ()
    let tbMultiSelect = newGuid ()
    let tbHelpContents = newGuid ()
    let tbAbout = newGuid ()
    let tbLink = newGuid ()

    let tabs =
        let tab header toolButtons =
            { Id = newGuid (); Header = header; Toolbuttons = toolButtons }
        let toolbutton id text isMarkable =
            { Id = id; Text = text; IsMarkable = isMarkable }
        let menu = [
            toolbutton tbForm1 "Form1" true
            toolbutton tbForm2 "Form2" true
            toolbutton tbCounter "Counter" true
            toolbutton tbTabs "Tabs" true
            toolbutton tbMultiSelect "Multiselect" true
            ]
        let help = [
            toolbutton tbHelpContents "Help contents" true
            toolbutton tbAbout "About" true
            toolbutton tbLink "Web" false
            ]
        [ tab "Menu" menu; tab "Help" help ]

    let startModel =
        {
            Tabs = tabs
            MarkedButton = tbForm1
            Form1Page = None
            Form2Page = None
            CounterPage = None
            TabsPage = None
            MultiSelectPage = None
            HelpContentPage = None
            AboutPage = None
        }

    let init () = startModel, Cmd.ofMsg ShowForm1

    let findButton (id: Guid) (m: Model) =
        m.Tabs |> List.tryPick (fun tab -> tab.Toolbuttons |> List.tryFind (fun tb -> tb.Id = id))

    let update msg m =
        match msg with
        | ButtonClick id ->
            match findButton id m with
            | None -> m, Cmd.none
            | Some clickedButton ->
                let m = if clickedButton.IsMarkable then { m with MarkedButton = id } else m
                if clickedButton.Id = tbForm1 then m, Cmd.ofMsg ShowForm1
                elif clickedButton.Id = tbForm2 then m, Cmd.ofMsg ShowForm2
                elif clickedButton.Id = tbCounter then m, Cmd.ofMsg ShowCounter
                elif clickedButton.Id = tbTabs then m, Cmd.ofMsg ShowTabs
                elif clickedButton.Id = tbMultiSelect then m, Cmd.ofMsg ShowMultiSelect
                elif clickedButton.Id = tbHelpContents then m, Cmd.ofMsg ShowHelp
                elif clickedButton.Id = tbAbout then m, Cmd.ofMsg ShowAbout
                elif clickedButton.Id = tbLink then m, Cmd.none // TODO
                else m, Cmd.none

        | ShowForm1 ->
            match m.Form1Page with
            | None -> { m with Form1Page = Some Form1.init }, Cmd.none
            | Some _ -> m, Cmd.none
        | ShowForm2 ->
            match m.Form2Page with
            | None -> { m with Form2Page = Some Form2.init }, Cmd.none
            | Some _ -> m, Cmd.none
        | ShowCounter ->
            match m.CounterPage with
            | None -> CounterDemo.init () |> (fun (m', c') -> { m with CounterPage = Some m' }, Cmd.map CounterPageMsg c')
            | Some _ -> m, Cmd.none
        | ShowTabs ->
            match m.TabsPage with
            | None -> TabsDemo.init () |> (fun (m', c') -> { m with TabsPage = Some m' }, Cmd.map TabsPageMsg c')
            | Some _ -> m, Cmd.none
        | ShowMultiSelect ->
            match m.MultiSelectPage with
            | None -> MultiSelectDemo.init () |> (fun (m', c') -> { m with MultiSelectPage = Some m' }, Cmd.map MultiSelectPageMsg c')
            | Some _ -> m, Cmd.none
        | ShowHelp ->
            match m.HelpContentPage with
            | None -> { m with HelpContentPage = HelpContent.init () |> Some }, Cmd.none
            | Some _ -> m, Cmd.none
        | ShowAbout ->
            match m.AboutPage with
            | None -> { m with AboutPage = AboutBox.init |> Some }, Cmd.none
            | Some _ -> m, Cmd.none
        | Form1Msg Form1.Submit -> { m with MarkedButton = tbNone; Form1Page = None }, Cmd.none
        | Form1Msg msg' ->
            match m.Form1Page with
            | None -> m, Cmd.none
            | Some m' -> Form1.update msg' m' |> (fun m' -> { m with Form1Page = Some m' }, Cmd.none)
        | Form2Msg Form2.Submit -> { m with MarkedButton = tbNone; Form2Page = None }, Cmd.none
        | Form2Msg msg' ->
            match m.Form2Page with
            | None -> m, Cmd.none
            | Some m' -> Form2.update msg' m' |> (fun m' -> { m with Form2Page = Some m' }, Cmd.none)
        | CounterPageMsg CounterDemo.Close -> { m with MarkedButton = tbNone; CounterPage = None }, Cmd.none
        | CounterPageMsg msg' ->
            match m.CounterPage with
            | None -> m, Cmd.none
            | Some m' -> CounterDemo.update msg' m' |> (fun (m', c') -> { m with CounterPage = Some m' }, Cmd.map CounterPageMsg c')
        | TabsPageMsg TabsDemo.Close -> { m with MarkedButton = tbNone; TabsPage = None }, Cmd.none
        | TabsPageMsg msg' ->
            match m.TabsPage with
            | None -> m, Cmd.none
            | Some m' -> TabsDemo.update msg' m' |> (fun (m', c') -> { m with TabsPage = Some m' }, Cmd.map TabsPageMsg c')
        | MultiSelectPageMsg msg' ->
            match m.MultiSelectPage with
            | None -> m, Cmd.none
            | Some m' -> MultiSelectDemo.update msg' m' |> (fun (m', c') -> { m with MultiSelectPage = Some m' }, Cmd.map MultiSelectPageMsg c')
        | HelpContentPageMsg msg' ->
            match m.HelpContentPage with
            | None -> m, Cmd.none
            | Some m' -> HelpContent.update msg' m' |> (fun m' -> { m with HelpContentPage = Some m' }, Cmd.none)
        | AboutPageMsg msg' ->
            match m.AboutPage with
            | None -> m, Cmd.none
            | Some m' -> AboutBox.update msg' m' |> (fun m' -> { m with AboutPage = Some m' }, Cmd.none)

    let bindings (model: Model) dispatch : Binding<Model, Msg> list =
        [
            "Tabs" |> Binding.subModelSeq((fun m -> m.Tabs), (fun t -> t), fun () ->
                [
                    "Id" |> Binding.oneWay (fun (_, t) -> t.Id)
                    "Header" |> Binding.oneWay (fun (_, t) -> t.Header)
                    "Toolbuttons" |> Binding.subModelSeq((fun (_, t) -> t.Toolbuttons), (fun t -> t), fun () ->
                        [
                            "Id" |> Binding.oneWay (fun (_, t) -> t.Id)
                            "Text" |> Binding.oneWay (fun (_, t) -> t.Text)
                            // "ImageSource" |> Binding.oneWay (fun (_, t) -> t.ImageSource)
                            "Foreground" |> Binding.oneWay (fun (_, t) -> Brushes.Green)
                            "ButtonClick" |> Binding.cmd (fun (_, (t: Toolbutton)) -> ButtonClick t.Id)
                            "MarkerVisible" |> Binding.oneWay (fun ((m, tab), tb) -> tb.Id = m.MarkedButton && m.somePageIsVisible)
                        ])
                ])
            "Form1Page" |> Binding.subModelOpt ((fun (m: Model) -> m.Form1Page), snd, Form1Msg, Form1.bindings)
            "Form2Page" |> Binding.subModelOpt ((fun (m: Model) -> m.Form2Page), snd, Form2Msg, Form2.bindings)
            "CounterPage" |> Binding.subModelOpt ((fun (m: Model) -> m.CounterPage), snd, CounterPageMsg, CounterDemo.bindings)
            "TabsPage" |> Binding.subModelOpt ((fun (m: Model) -> m.TabsPage), snd, TabsPageMsg, TabsDemo.bindings)
            "MultiSelectPage" |> Binding.subModelOpt ((fun (m: Model) -> m.MultiSelectPage), snd, MultiSelectPageMsg, MultiSelectDemo.bindings)
            "HelpContentPage" |> Binding.subModelOpt ((fun (m: Model) -> m.HelpContentPage), snd, HelpContentPageMsg, HelpContent.bindings)
            "AboutPage" |> Binding.subModelOpt ((fun (m: Model) -> m.AboutPage), snd, AboutPageMsg, AboutBox.bindings)

            "Form1PageVisible" |> Binding.oneWay (fun m -> m.MarkedButton = tbForm1 && m.Form1Page.IsSome)
            "Form2PageVisible" |> Binding.oneWay (fun m -> m.MarkedButton = tbForm2 && m.Form2Page.IsSome)
            "CounterPageVisible" |> Binding.oneWay (fun m -> m.MarkedButton = tbCounter && m.CounterPage.IsSome)
            "TabsPageVisible" |> Binding.oneWay (fun m -> m.MarkedButton = tbTabs && m.TabsPage.IsSome)
            "MultiSelectPageVisible" |> Binding.oneWay (fun m -> m.MarkedButton = tbMultiSelect && m.MultiSelectPage.IsSome)
            "HelpContentPageVisible" |> Binding.oneWay (fun m -> m.MarkedButton = tbHelpContents && m.HelpContentPage.IsSome)
            "AboutPageVisible" |> Binding.oneWay (fun m -> m.MarkedButton = tbAbout && m.AboutPage.IsSome)
        ]

    let designTimeModel =
        let model = startModel
        let dispatch = fun _ -> ()
        ViewModel.designInstance model (bindings model dispatch)

    let entryPoint (_: string[], mainWindow: Window) =
        Program.mkProgram init update bindings
        |> Program.runWindowWithConfig
            { ElmConfig.Default with LogTrace = true; Measure = true; MeasureLimitMs = 1 }
            mainWindow
