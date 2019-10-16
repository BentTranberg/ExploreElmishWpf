namespace ExploreElmishWpf.Models

module MainWindow =

    open System
    open System.Windows
    open Elmish
    open Elmish.WPF

    type Dialog =
        | Form1 of Form1.Model
        | Form2 of Form2.Model

    type Model =
        {
            Dialog: Dialog option
        }

    let init () =
        {
            Dialog = None
        }, Cmd.none

    type Msg =
        | ShowForm1
        | ShowForm2
        | Form1Msg of Form1.Msg
        | Form2Msg of Form2.Msg

    let update msg m =
        match msg with
        | ShowForm1 -> { m with Dialog = Some <| Form1 Form1.init }, Cmd.none
        | ShowForm2 -> { m with Dialog = Some <| Form2 Form2.init }, Cmd.none
        | Form1Msg Form1.Submit -> { m with Dialog = None }, Cmd.none
        | Form1Msg msg' ->
            match m.Dialog with
            | Some (Form1 m') -> { m with Dialog = Form1.update msg' m' |> Form1 |> Some }, Cmd.none
            | _ -> m, Cmd.none
        | Form2Msg Form2.Submit -> { m with Dialog = None }, Cmd.none
        | Form2Msg msg' ->
            match m.Dialog with
            | Some (Form2 m') -> { m with Dialog = Form2.update msg' m' |> Form2 |> Some }, Cmd.none
            | _ -> m, Cmd.none

    let bindings (model: Model) dispatch : Binding<Model, Msg> list =
        [
            "ShowForm1" |> Binding.cmd ShowForm1
            "ShowForm2" |> Binding.cmd ShowForm2
            "DialogVisible" |> Binding.oneWay (fun m -> m.Dialog.IsSome)
            "Form1Visible" |> Binding.oneWay
                (fun m -> match m.Dialog with Some (Form1 _) -> true | _ -> false)
            "Form2Visible" |> Binding.oneWay
                (fun m -> match m.Dialog with Some (Form2 _) -> true | _ -> false)
            "Form1" |> Binding.subModelOpt (
                (fun m -> match m.Dialog with Some (Form1 m') -> Some m' | _ -> None),
                snd,
                Form1Msg,
                Form1.bindings)
            "Form2" |> Binding.subModelOpt (
                (fun m -> match m.Dialog with Some (Form2 m') -> Some m' | _ -> None),
                snd,
                Form2Msg,
                Form2.bindings)
        ]

    let entryPoint (_: string[], mainWindow: Window) =
        Program.mkProgram init update bindings
        |> Program.runWindowWithConfig
            { ElmConfig.Default with LogTrace = true; Measure = true; MeasureLimitMs = 1 }
            mainWindow
