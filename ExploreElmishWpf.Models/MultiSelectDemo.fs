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
        {
            Lines = [
                { Id = Guid.NewGuid(); Marked = false; DisplayText = "Zero" }
                { Id = Guid.NewGuid(); Marked = true; DisplayText = "One" }
                { Id = Guid.NewGuid(); Marked = false; DisplayText = "Two" }
                { Id = Guid.NewGuid(); Marked = true; DisplayText = "Three" }
                { Id = Guid.NewGuid(); Marked = false; DisplayText = "Four" }
                { Id = Guid.NewGuid(); Marked = false; DisplayText = "Five" }
                { Id = Guid.NewGuid(); Marked = false; DisplayText = "Six" }
                { Id = Guid.NewGuid(); Marked = false; DisplayText = "Seven" }
                { Id = Guid.NewGuid(); Marked = false; DisplayText = "Eight" }
                { Id = Guid.NewGuid(); Marked = false; DisplayText = "Nine" }
            ]
        }, Cmd.none

    let update msg m =
        match msg with
        | SetMarked (id, marked) ->
            let lines =
                m.Lines
                |> List.map (fun line ->
                    if line.Id = id then
                        { line with Marked = marked }
                    else
                        line)
            { m with Lines = lines }, Cmd.none

    let setMarked marked (_, line) =
        let x = 0 // just to be able to set breakpoint here
        SetMarked (line.Id, marked)

    let getMarked (_, line) =
        line.Marked

    let bindings () : Binding<Model, Msg> list =
        [
            "Lines" |> Binding.subModelSeq((fun m -> m.Lines), (fun line -> line), fun () ->
                [
                    "Id" |> Binding.oneWay (fun (_, line) -> line.Id)
                    "Marked" |> Binding.twoWay (
                        getMarked,
                        setMarked
                        )
                    "DisplayText" |> Binding.oneWay (fun (_, line) -> line.DisplayText)
                ])
        ]

    let designTimeModel =
        let model = init () |> fst
        ViewModel.designInstance model (bindings ())
