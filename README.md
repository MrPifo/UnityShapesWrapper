# **Wrapper for Unity Shapes**  
**[Shapes](https://assetstore.unity.com/packages/tools/particles-effects/shapes-173167) is required for this script**  

This is a lightweight wrapper for the Unity package [Shapes](https://acegikmo.com/shapes/), designed to simplify and streamline drawing operations in both the **Scene** and **Editor**.  

**Shapes**, created by Freya Holmer, is an incredible asset, but using it in **immediate mode** can be time-consuming and clutter your code. Additionally, rendering immediate-mode Shapes in `Update()` isn’t fully supported. This wrapper provides an easy-to-use API to make drawing Gizmos effortless.  

## ✨ **Features**  
- **Gizmos-Like API** – Designed to feel familiar, similar to `Gizmos.Draw`.  
- **Frame-Independent Rendering** – Works in `Update()`, `LateUpdate()`, and `FixedUpdate()`.  
- **Time-Based Display** – Render Shapes for a specified duration.  
- **Works in Both Runtime and Editor** – Easily draw debug visuals in any mode.  

## 🔍 **How It Works**  
Simply call `Draw.Line()`, `Draw.Sphere()`, or any other provided method and pass the required parameters.  

## **Implemented Shapes**  
- **Line**  
- **Ray**  
- **Disc**  
- **Ring**  
- **Cone**  
- **Text**  
- **Cube**  
- **Sphere**  
- **Square**  
- **Box**  
- **Rectangle**  
- **Triangle**  
- **FlatTriangle**  

## ⚠ **Important Notes**  
- This wrapper was originally created for personal use, but feel free to **suggest features, report bugs, or modify it as you like**.  
- **Editor mode rendering may be unstable in certain rendering pipelines.**  
- Future updates may introduce improvements and changes.  

---

This keeps everything clean, structured, and professional while still maintaining an approachable tone. Let me know if you need further refinements! 🚀  
