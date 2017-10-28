// #r "../../node_modules/fabel-core/Fable.Core.dll"
// #load "../../node_modules/fable-import-three/Fable.Import.Three.fs"


module EthanThree


open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Keyboard


type Sparkle = {
    obj : Three.Object3D;

    ttl : float;
    direction : Three.Vector3;
    startPoint : Three.Vector3;
}

type Letter = {
    obj : Three.Object3D;
    text : string;
    direction : Three.Vector3;
    age : float;
}

let getWidth() = Browser.window.innerWidth
let getHeight() = Browser.window.innerHeight

let initCamera () =
    let camera = Three.PerspectiveCamera(45.0, getWidth() / getHeight(), 0.01, 2000.0)
    camera.matrixAutoUpdate <- true
    camera.rotationAutoUpdate <- true
    camera.position.z <- 2.0
    camera.position.set(0.,0.,400.) |> ignore
    camera

let initOrthoCamera () =
    let cameraOrtho = Three.OrthographicCamera( - getWidth() / 2., getWidth() / 2., getHeight() / 2., - getHeight() / 2., 1., 100. )
    cameraOrtho.position.z <- 10.
    cameraOrtho

let initLights (scene:Three.Scene) =

    let ambientLight = Three.AmbientLight(U2.Case2 "#3C3C3C", 0.7)
    scene.add(ambientLight)

    let spotLight = Three.SpotLight(U2.Case2 "grey", 1.)
    spotLight.position.set(0., 0., 200.) |> ignore
    spotLight.lookAt (Three.Vector3 (0., 0., 0.))
    scene.add(spotLight)

    // // White directional light 
    // let directionalLight = Three.DirectionalLight(U2.Case2 "#FFFFFF", 1. );
    // scene.add( directionalLight );

let initGeometry (scene:Three.Scene) x y =
    let cube = Three.BoxBufferGeometry(20.1, 20.1, 20.1)
    let matProps = createEmpty<Three.MeshLambertMaterialParameters>
    // let matProps = createEmpty<Three.MeshBasicMaterialParameters>
    matProps.color <- Some (U2.Case2 "#9430B3")
    
    let mesh = Three.Mesh(cube, Three.MeshLambertMaterial(matProps))
    mesh.position.set(x,y,0.) |> ignore
    scene.add(mesh)
    
    mesh

let initFloor (scene:Three.Scene) =

    let matProps = createEmpty<Three.MeshBasicMaterialParameters>
    // matProps.map <- Some(floorTexture)
    matProps.side <- Some(Three.DoubleSide)
    matProps.color <- Some(U2.Case2 "#FFFFFF")
    let floorMaterial = Three.MeshBasicMaterial( matProps )
    let floorGeometry = Three.PlaneBufferGeometry(1000., 1000., 10., 10.);
    let floor = new Three.Mesh(floorGeometry :> Three.BufferGeometry, floorMaterial);
    floor.position.y <- -0.5
    floor.rotation.x <- Math.PI / 2.

    scene.add(floor);

let initRenderer () =

    let renderer = Three.WebGLRenderer()
    renderer.setClearColor("#0A1D2D")
    renderer.autoClear <- false
    (renderer :> Three.Renderer).setSize(getWidth(), getHeight())

    let container = if Browser.document.getElementById("graphicsContainer") <> null
                    then Browser.document.getElementById("graphicsContainer")
                    else Browser.document.body

    container.innerHTML <- ""
    container.appendChild((renderer :> Three.Renderer).domElement) |> ignore

    renderer

let initOrtho () =
    let scene = Three.Scene()
    let cameraOrtho = initOrthoCamera()

    let ambientLight = Three.AmbientLight(U2.Case2 "#3C3C3C", 1.0)
    scene.add(ambientLight)

    let spotLight = Three.SpotLight(U2.Case2 "#FFFFFF")
    spotLight.position.set(0., 0., 60.) |> ignore
    // spotLight.angle <- Math.PI / 10.
    scene.add(spotLight)

    scene, cameraOrtho, spotLight

let mutable font : obj Option = None

let init() =

    Keyboard.init ()

    let scene = Three.Scene()
    let camera = initCamera() 
    
    initLights scene
    // initFloor scene
    // let cube = initGeometry scene 50. 50. 

    let fontLoader = Three.FontLoader ()
    fontLoader.load("Montserrat_Bold.json", Func<string,_> (fun x ->  
                font <- Some (x :> obj)
                ) )

    scene, camera

let scene,camera = init()
let sceneOrtho, cameraOrtho, spotLightOrtho = initOrtho()
let renderer = initRenderer()

let onWindowResize(_:Browser.UIEvent):obj =
    camera.aspect <- getWidth() / getHeight()
    camera.updateProjectionMatrix()

    cameraOrtho.left <- - getWidth() / 2.
    cameraOrtho.right <- getWidth() / 2.
    cameraOrtho.top <- getHeight() / 2.
    cameraOrtho.bottom <- - getHeight() / 2.
    cameraOrtho.updateProjectionMatrix();

    (renderer :> Three.Renderer).setSize(getWidth(), getHeight())
    
    null

Browser.window.addEventListener_resize(
     Func<_,_> onWindowResize, false)

let contextMenu(e:Browser.PointerEvent):obj =
    e.preventDefault ()
    null
Browser.window.addEventListener_contextmenu(Func<_,_> contextMenu, true)
let mutable mouseX = 0.
let mutable mouseY = 0.

let onMouseMove(e:Browser.MouseEvent):obj =
    mouseX <- e.clientX
    mouseY <- e.clientY
    null

Browser.document.addEventListener_mousemove(
    Func<_,_> onMouseMove, false)

let splitN n list =
    let rec inner xs = function
        | (0, ys) | (_, ([] as ys)) -> (List.rev xs), ys
        | (n, x::ys) -> inner (x::xs) (n-1, ys)
    inner [] (n, list)

// let splitP (predicate:'a -> bool) list =
//     let rec inner xs = function
//         | (false, ys) | (_, ([] as ys)) -> (List.rev xs), ys
//         | (true, x::ys) -> inner (x::xs) ((predicate x), ys)
//     inner [] (true, list)


let addSparkle x y (scene:Three.Scene) =
    let sphere = Three.BoxBufferGeometry(10.0, 10.0, 10.0)
    let matProps = createEmpty<Three.MeshLambertMaterialParameters>
    matProps.color <- Some (U2.Case2 "red")
    
    let mesh = Three.Mesh(sphere, Three.MeshLambertMaterial(matProps))
    mesh.position.set(x,y,-40.) |> ignore

    scene.add(mesh)
    mesh

let updateSparkle (sparkle:Sparkle) duration delta =

    sparkle.obj.position.addScaledVector (sparkle.direction, delta / 10.) |> ignore
    sparkle.obj.rotateX(delta / 10.) |> ignore
    sparkle.obj.rotateY(delta / 5.) |> ignore
    ()


let rnd = Random();

let updateSparkles (sparkles:Sparkle list) duration delta =
    
    let x = mouseX - (getWidth() /2.)
    let y = mouseY - (getHeight() /2.)

    let newList = 
        if sparkles.IsEmpty || sparkles.Head.startPoint.x <> x && sparkles.Head.startPoint.y <> y then

            // Console.WriteLine(sprintf "%f %f - %f %f " x y (getWidth()) (getHeight()))
            let mesh = addSparkle  x -y sceneOrtho

            let newSpark = 
                    {
                        obj = mesh
                        ttl = 10.
                        direction = Three.Vector3(rnd.NextDouble() - 0.5, rnd.NextDouble() - 0.5)
                        startPoint = Three.Vector3(x, -y)
                    }
            newSpark :: sparkles 

        else
            sparkles


    // divide the list into new and old
    let s = splitN 50 newList

    // remove old ones from the scene
    List.iter (fun (x:Sparkle) -> sceneOrtho.remove x.obj) (snd s)

    // move existing entries
    List.iter (fun x -> updateSparkle x duration delta) (fst s)
    fst s

let itemNotInList pred list =
    Option.isNone (List.tryFind pred list)

let keyboardDebounce list duration text  = 
    let res = itemNotInList (fun y -> text = y.text && y.age + 300. > duration ) list
    res

let letterStartingPosition () =
    let x = (rnd.NextDouble() - 0.5) * 400.
    let y = (rnd.NextDouble() - 0.5) * 200.

    Three.Vector3(x,y,0.)

let escapeDirection () =
    let theta = rnd.NextDouble () * (Math.PI * 2.)
    let z = - abs ((rnd.NextDouble() - 0.5) * 2.)
    let x = ((sqrt (1. - (z*z))) * (cos theta))
    let y = ((sqrt (1. - (z*z))) * (sin theta))
    Three.Vector3(x,y,z)

let createNewLetter (scene:Three.Scene) duration x = 
        let text = x
        let props = createEmpty<Three.TextGeometryParameters>
        props.font <- font.Value :?> Three.Font
        
        let textGeom = Three.TextGeometry(text, props)
        
        let matProps = createEmpty<Three.MeshLambertMaterialParameters>
        matProps.color <- Some(U2.Case2("red"))
        let material = Three.MeshLambertMaterial( matProps )
        let geom = Three.BufferGeometry().fromGeometry(textGeom)
        let mesh = Three.Mesh(geom , material )
        //mesh.position.set(-20.,-20., 10.) |> ignore
        mesh.position.add(letterStartingPosition ()) |> ignore
        scene.add( mesh );

        { 
            obj = mesh 
            text = text
            direction = (escapeDirection ())
            age = duration
        }
let removeLetter (scene:Three.Scene) (letter:Letter) =
    scene.remove (letter.obj)

let updateLetter delta (letter:Letter) =
    letter.obj.position.addScaledVector (letter.direction, delta / 10.) |> ignore

let updateText texts duration delta =

    let texts = 
        if Keyboard.keysPressed.Count > 0 then
            let nt =
                Set.toSeq Keyboard.keysPressed
                // have we made one recently?
                |> Seq.filter (keyboardDebounce texts duration)
                // make the new ones
                |> Seq.map (fun x -> createNewLetter scene duration x)                    
                |> Seq.toList 
            List.append nt texts            
        else texts

    let agePred x = (x.age + 10000.) > duration 
    let inversePred (pred:'a->bool) (x:'a) = not (pred (x))
    
    let keep = List.filter agePred texts
    let remove = List.filter (inversePred agePred)  texts

    List.iter (removeLetter scene) remove
    List.iter (updateLetter delta) keep

    keep

let update (state:Sparkle list * Letter list) duration delta =
    let sparkles = updateSparkles (fst state) duration delta
    let text = updateText (snd state) duration delta 
    sparkles,text

let render() =

    renderer.clear()
    renderer.render(scene, camera)
    renderer.clearDepth()
    renderer.render(sceneOrtho, cameraOrtho)

let rec animate state (prevDt:float) (dt:float) =

    let state = update state dt (dt-prevDt)
    render() 
    Browser.window.requestAnimationFrame(Browser.FrameRequestCallback (animate state dt)) |> ignore


// initial state
let state = ([], [])
// kick it off
animate state 0. 0.