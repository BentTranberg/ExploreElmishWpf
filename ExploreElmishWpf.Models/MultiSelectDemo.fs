namespace ExploreElmishWpf.Models

module MultiSelectDemo =

    open System
    open Elmish
    open Elmish.WPF

    type Line =
        {
            Id: Guid
            Marked: bool
            DisplayText: string
        }

    type Model =
        {
            Lines: Line list
        }

    type Msg =
        | SetMarked of Guid * bool

    let init () =
        let line marked text = { Id = Guid.NewGuid(); Marked = marked; DisplayText = text }
        {
            Lines = [
                line false "Zero"
                line true "One"
                line true "Two"
                line false "Three"
                line true "Four"
                line true "Five"
                line false "Six"
            ]
        }, Cmd.none

    let update msg m =
        match msg with
        | SetMarked (id, marked) ->
            let lines = m.Lines |> List.map (fun line ->
                if line.Id = id then { line with Marked = marked } else line)
            { m with Lines = lines }, Cmd.none

    let bindings () : Binding<Model, Msg> list =
        [
            "Lines" |> Binding.subModelSeq((fun m -> m.Lines), (fun line -> line.Id), fun () ->
                [
                    "Id" |> Binding.oneWay (fun (_, line) -> line.Id)
                    "Marked" |> Binding.twoWay ((fun (m, line) -> line.Marked),
                        (fun marked (m, line) -> SetMarked (line.Id, marked)))
                    "DisplayText" |> Binding.oneWay (fun (_, line) ->
                        line.DisplayText + (if line.Marked then " is checked" else " is not checked"))
                ])
        ]

    let designTimeModel =
        let model = init () |> fst
        ViewModel.designInstance model (bindings ())
