# Wrapper for Unity Shapes
**[Shapes](https://assetstore.unity.com/packages/tools/particles-effects/shapes-173167) is required for this script**

This is a wrapper script for the Unity-Package [Shapes](https://acegikmo.com/shapes/)

The intention was to create a simple to use API to easily draw Gizmos in Scene and Editor.
Shapes on it's is a great asset made by Freya Holmer, but setting up Shapes in immediate mode is a bit time consuming and makes code unreadibly very quick.
Also drawing in immediate mode isn't fully supported.

### Features:
- API looks almost like Gizmos.Draw
- Can be used in Update, LateUpdate, FixedUpdate. To make sure Shapes are visible, they should at minimum run in frame update rate.
- No instantiating of Shapes

### How it works:
As soon as you import this script, every scene you open or start will generate a GameObject which holds the script automaticially.
If you don't want to behave it like this delete the ``ScriptReload`` method.

Then you can call ``Draw.Line()`` for example and fill in the required parameters.

### Implemented Shapes
- Line
- Ray
- Disc
- Ring
- Text



### Notice
I wrote this wrapper mostly for my own, so you may suggest, ask or even extend this script on your own. Also feel free to report Bugs or whatever else.

I also may extend this script in the future and change some things.
