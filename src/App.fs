// #r "../../node_modules/fabel-core/Fable.Core.dll"
// #load "../../node_modules/fable-import-three/Fable.Import.Three.fs"


module EthanThree


open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open System.Diagnostics.CodeAnalysis

// [<System.Diagnostics.CodeAnalysis.SuppressMessage("NameConventions", "*")>]
// type IOrbitControls =
//     abstract target: Three.Vector3 with get,     set
//     abstract minPolarAngle: float with get, set
//     abstract maxPolarAngle: float with get, set

// let OrbitControls: JsConstructor<Three.Camera, Browser.HTMLElement, IOrbitControls> =
//    importDefault "./lib/OrbitControls.js"


// [<System.Diagnostics.CodeAnalysis.SuppressMessage("NameConventions", "*")>]
// type IStats =
//     abstract setMode: int -> unit
//     abstract dom: Browser.HTMLElement with get
//     abstract ``begin``: unit -> unit
//     abstract ``end``: unit -> unit

// let Stats: JsConstructor<IStats> =
//    importDefault "../node_modules/stats.js/build/stats.min.js"


// /// Represents the API exposed by FirstPersonControls script
// [<System.Diagnostics.CodeAnalysis.SuppressMessage("NameConventions", "*")>]
// type IFirstPersonControls =
//     abstract movementSpeed: float with get, set
//     abstract lookSpeed: float with get, set
//     abstract handleResize: unit -> unit
//     abstract update: float -> unit

// let FirstPersonControls: JsConstructor<Three.Camera, Browser.HTMLElement, IFirstPersonControls> =
//     importDefault "./lib/FirstPersonControls.js"

let init() =

//    #if TUTORIAL
//    let getWidth() = 800.
//    let getHeight() = 450.
//    #else
    let getWidth() = Browser.window.innerWidth
    let getHeight() = Browser.window.innerHeight
//    #endif

    let container = Browser.document.getElementById("container")
    let camera = Three.PerspectiveCamera(
                    60.0, getWidth() / getHeight(), 50.0, 20000.0)
    camera.position.z <- 2000.
    camera.position.y <- 50.

    let scene = Three.Scene()

    let renderer = Three.WebGLRenderer()
    renderer.setClearColor("#bfd1e5")
    (renderer :> Three.Renderer).setSize(getWidth(), getHeight())
    let domElement = (renderer :> Three.Renderer).domElement
    container.innerHTML <- ""
    container.appendChild(domElement) |> ignore

    
    let ambientLight = Three.AmbientLight(U2.Case2 "white", 0.4)
    scene.add ambientLight


    let onWindowResize(e:Browser.UIEvent):obj =
        camera.aspect <- getWidth() / getHeight()
        camera.updateProjectionMatrix()
        (renderer :> Three.Renderer).setSize(getWidth(), getHeight())
        null
    Browser.window.addEventListener_resize(
         Func<_,_> onWindowResize, false)


    let onMouseMove(e:Browser.UIEvent):obj =
        null
    Browser.window.addEventListener_mousemove(
        Func<_,_> onMouseMove, false)


    renderer, scene, camera

let renderer,scene,camera = init()


let render() =

    renderer.render(scene, camera)

let rec animate (dt:float) =
    Browser.window.requestAnimationFrame(Func<_,_> animate)
    |> ignore
    render()

// kick it off
animate(0.0)
