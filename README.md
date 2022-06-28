# Wrapper for Unity Shapes
**[Shapes](https://assetstore.unity.com/packages/tools/particles-effects/shapes-173167) is required for this script**

This is a wrapper script for the Unity-Package [Shapes](https://acegikmo.com/shapes/)

The intention was to create an easy API to easily draw Gizmos in Scene and Editor.
Shapes on it's own is a great asset made by Freya Holmer, but setting up Shapes in immediate mode is time consuming and makes code unreadable.
Also drawing immediate Shapes in Update isn't fully supported.

### Features:
- API looks similiar to Gizmos.Draw
- Can be used in Update, LateUpdate, FixedUpdate. Although FixedUpdate may be problematic if it is running below your framerate. You can additionaly use the time parameter to display it longer.
- Display Shapes over a timespan.

### How it works:
As soon you start the application, the script will auto-generate a GameObject which holds the script and marks it as DontDestroyOnLoad. This script is required and the Shapes won't render if it isn't present.
You can disable Drawing by using the DisableDrawing Property.

Then you can call ``Draw.Line()`` for example and fill in the required parameters.

### Implemented Shapes
- Line
- Ray
- Disc
- Ring
- Text
- Cube
- Sphere
- Rectangles


### Notice
I wrote this wrapper mostly for my own, so you may suggest, ask or even extend this script on your own. Also feel free to report any Bugs or improvements.
Drawing Shapes in Editor may be unstable on certain pipelines.

I also may extend this script in the future and change some things.
