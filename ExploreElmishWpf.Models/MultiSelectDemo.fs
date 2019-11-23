namespace ExploreElmishWpf.Models

module MultiSelectDemo =

    open System
    open Elmish
    open Elmish.WPF

    type Line =
        {
            Id: Guid
            DisplayText: string
        }

    type Model =
        {
            Lines: Line list
        }

    type Msg =
        | Dummy

    let init () =
        {
            Lines = [
                { Id = Guid.NewGuid(); DisplayText = "Zero" }
                { Id = Guid.NewGuid(); DisplayText = "One" }
                { Id = Guid.NewGuid(); DisplayText = "Two" }
                { Id = Guid.NewGuid(); DisplayText = "Three" }
                { Id = Guid.NewGuid(); DisplayText = "Four" }
                { Id = Guid.NewGuid(); DisplayText = "Five" }
                { Id = Guid.NewGuid(); DisplayText = "Six" }
                { Id = Guid.NewGuid(); DisplayText = "Seven" }
                { Id = Guid.NewGuid(); DisplayText = "Eight" }
                { Id = Guid.NewGuid(); DisplayText = "Nine" }
            ]
        }, Cmd.none

    let update msg m =
        match msg with
        | Dummy ->
            m, Cmd.none

    let bindings () : Binding<Model, Msg> list =
        [
            "Lines" |> Binding.subModelSeq((fun m -> m.Lines), (fun line -> line), fun () ->
                [
                    "Id" |> Binding.oneWay (fun (_, line) -> line.Id)
                    "DisplayText" |> Binding.oneWay (fun (_, line) -> line.DisplayText)
                ])
        ]

    let designTimeModel =
        let model = init () |> fst
        ViewModel.designInstance model (bindings ())
