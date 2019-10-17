namespace ExploreElmishWpf.Models

module TabsPane =

    open System
    open Elmish
    open Elmish.WPF

    let makeNameGenerator prefix =
        let mutable intId = 0
        fun () ->
            intId <- (+) intId 1
            prefix <| intId

    let createTabId = makeNameGenerator (sprintf "-%03i")

    type Tab =
        {
            Id: Guid
            Name: string
        }

    type Model =
        {
            Tabs: Tab list
            SelectedTab: Guid option
        }

    let init () =
        {
            Tabs = []
            SelectedTab = None
        }, Cmd.none

    type Msg =
        | AddTab
        | CloseTab of tabId: Guid
        | SelectTab of tabId: Guid option
        | Close

    let update msg m =
        match msg with
        | AddTab ->
            let newTab = { Id = Guid.NewGuid (); Name = "Tab" + createTabId () }
            { m with Tabs = m.Tabs |> List.append [newTab] }, Cmd.none
        | CloseTab tabId ->
            let newTabs = m.Tabs |> List.filter (fun t -> t.Id <> tabId)
            // Logic for selected tab: Keep existing selected if not removed. If
            // selected tab is removed, select next tab in list, or the new last tab
            // if we removed the last tab, or no tab if empty
            let newSelectedTabId =
              if m.SelectedTab <> Some tabId then m.SelectedTab
              else
                let selectedIdx = m.Tabs |> List.findIndex (fun t -> t.Id = tabId)
                newTabs
                |> List.tryItem selectedIdx
                |> Option.orElse (newTabs |> List.tryLast)
                |> Option.map (fun t -> t.Id)
            { m with Tabs = newTabs; SelectedTab = newSelectedTabId }, Cmd.none
        | SelectTab t ->
            { m with SelectedTab = t }, Cmd.none
        | Close -> m, Cmd.none  // handled by parent

    let bindings () : Binding<Model, Msg> list =
        [
            "Tabs" |> Binding.subModelSeq((fun m -> m.Tabs), (fun s -> s), fun () ->
                [
                    "Id" |> Binding.oneWay (fun (_, t) -> t.Id)
                    "Name" |> Binding.oneWay (fun (_, t) -> t.Name)
                    "Close" |> Binding.cmd (fun (_, t) -> CloseTab t.Id)
                ])
            "AddTab" |> Binding.cmd AddTab
            "SelectedTab" |> Binding.twoWayOpt((fun m -> m.SelectedTab), SelectTab)
            "Close" |> Binding.cmd Close
        ]

    let designTimeModel =
        let guidOfSelected = Guid.NewGuid ()
        let model =
            {
                Tabs =
                    [
                        { Id = Guid.NewGuid (); Name = "Tab" + createTabId () }
                        { Id = guidOfSelected; Name = "Tab" + createTabId () }
                        { Id = Guid.NewGuid (); Name = "Tab" + createTabId () }
                    ]
                SelectedTab = Some guidOfSelected
            }
        ViewModel.designInstance model (bindings ())
