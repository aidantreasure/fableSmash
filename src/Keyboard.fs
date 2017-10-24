module Keyboard

open Fable.Import.Browser

module Keyboard = 

    let mutable keysPressed = Set.empty

    let update (e:KeyboardEvent, pressed) =
        let op = if pressed then Set.add else Set.remove
        keysPressed <- op e.key keysPressed
        null

    let init () =
        document.addEventListener_keydown(fun x -> update(x, true))
        document.addEventListener_keyup(fun x -> update(x, false))