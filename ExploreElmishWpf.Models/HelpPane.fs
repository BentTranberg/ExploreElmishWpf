﻿namespace ExploreElmishWpf.Models

module HelpPane =

    open System
    open Elmish
    open Elmish.WPF

    type Model = unit

    type Msg =
        | Dummy

    let init =
        ()

    let update msg m =
        m

    let bindings () : Binding<Model, Msg> list =
        [
        ]
