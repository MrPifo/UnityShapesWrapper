using Shapes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sperlich.Drawing {
	public static class Draw {

		public static Color DefaultWhite { get; set; } = Color.white * 2.5f;

		private static readonly LinkedList<DrawLine> lines = new LinkedList<DrawLine>();
		private static readonly LinkedList<DrawSphere> spheres = new LinkedList<DrawSphere>();
		private static readonly LinkedList<DrawRectangle> rectangles = new LinkedList<DrawRectangle>();
		private static readonly LinkedList<DrawCube> cubes = new LinkedList<DrawCube>();
		private static readonly LinkedList<DrawCone> cones = new LinkedList<DrawCone>();
		private static readonly LinkedList<DrawDisc> discs = new LinkedList<DrawDisc>();
		private static readonly LinkedList<DrawRing> rings = new LinkedList<DrawRing>();
		private static readonly LinkedList<DrawTriangle> triangles = new LinkedList<DrawTriangle>();

#if UNITY_EDITOR
		[InitializeOnLoadMethod()]
		static void EditorInit() {
			if (Application.isPlaying == false) {
				EditorApplication.playModeStateChanged -= PlayModeChanged;
				EditorApplication.playModeStateChanged += PlayModeChanged;
				RenderPipelineManager.endCameraRendering -= Render;
				RenderPipelineManager.endCameraRendering += Render;
				KillAll();
			}
		}
		static void PlayModeChanged(PlayModeStateChange mode) {
			if (mode == PlayModeStateChange.EnteredEditMode) {
				RenderPipelineManager.endCameraRendering += Render;
			}
			if (mode == PlayModeStateChange.ExitingPlayMode) {
				RenderPipelineManager.endCameraRendering -= Render;
			}
			KillAll();
		}
#endif
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		static void Initialize() {
			RenderPipelineManager.endCameraRendering += Render;
			KillAll();
		}
		public static void Render(ScriptableRenderContext context, Camera cam) {
			using (Shapes.Draw.Command(cam, RenderPassEvent.BeforeRenderingPostProcessing)) {
				LinkedListNode<DrawLine> line = lines.First;
				LinkedListNode<DrawRectangle> rectangle = rectangles.First;
				LinkedListNode<DrawDisc> disc = discs.First;
				LinkedListNode<DrawSphere> sphere = spheres.First;
				LinkedListNode<DrawCube> cube = cubes.First;
				LinkedListNode<DrawCone> cone = cones.First;
				LinkedListNode<DrawRing> ring = rings.First;
				LinkedListNode<DrawTriangle> triangle = triangles.First;

				Shapes.Draw.ThicknessSpace = ThicknessSpace.Pixels;
				while (line != null) {
					LinkedListNode<DrawLine> next = line.Next;
					line.Value.Draw();
					if (line.Value.IsAlive() == false) {
						lines.Remove(line);
					}
					line = next;
				}
				while (rectangle != null) {
					LinkedListNode<DrawRectangle> next = rectangle.Next;
					rectangle.Value.Draw();
					if (rectangle.Value.IsAlive() == false) {
						rectangles.Remove(rectangle);
					}
					rectangle = next;
				}
				while (disc != null) {
					LinkedListNode<DrawDisc> next = disc.Next;
					disc.Value.Draw();
					if (disc.Value.IsAlive() == false) {
						discs.Remove(disc);
					}
					disc = next;
				}
				while (sphere != null) {
					LinkedListNode<DrawSphere> next = sphere.Next;
					sphere.Value.Draw();
					if (sphere.Value.IsAlive() == false) {
						spheres.Remove(sphere);
					}
					sphere = next;
				}
				while (cube != null) {
					LinkedListNode<DrawCube> next = cube.Next;
					cube.Value.Draw();
					if (cube.Value.IsAlive() == false) {
						cubes.Remove(cube);
					}
					cube = next;
				}
				while (cone != null) {
					LinkedListNode<DrawCone> next = cone.Next;
					cone.Value.Draw();
					if (cone.Value.IsAlive() == false) {
						cones.Remove(cone);
					}
					cone = next;
				}
				Shapes.Draw.ThicknessSpace = ThicknessSpace.Meters;
				while (ring != null) {
					LinkedListNode<DrawRing> next = ring.Next;
					ring.Value.Draw();
					if (ring.Value.IsAlive() == false) {
						rings.Remove(ring);
					}
					ring = next;
				}
				while (triangle != null) {
					LinkedListNode<DrawTriangle> next = triangle.Next;
					triangle.Value.Draw();
					if (triangle.Value.IsAlive() == false) {
						triangles.Remove(triangle);
					}
					triangle = next;
				}
			}
		}

		#region Helpers
		public static float Remap(float source, float sourceFrom, float sourceTo, float targetFrom, float targetTo) {
			return targetFrom + (source - sourceFrom) * (targetTo - targetFrom) / (sourceTo - sourceFrom);
		}
		public static void KillAll() {
			lines.Clear();
		}
		public static void SetZTest(bool useDepth) {
			if (useDepth) {
				Shapes.Draw.ZTest = UnityEngine.Rendering.CompareFunction.LessEqual;
			} else {
				Shapes.Draw.ZTest = UnityEngine.Rendering.CompareFunction.Always;
			}
		}
		public static void SetTransparent(Color col) {
			if (col.a < 1f) {
				Shapes.Draw.BlendMode = ShapesBlendMode.Transparent;
			} else {
				Shapes.Draw.BlendMode = ShapesBlendMode.Opaque;
			}
		}
		public static void SetTransparent(Color col, Color col2) {
			if (col.a < 1f || col2.a < 1f) {
				Shapes.Draw.BlendMode = ShapesBlendMode.Transparent;
			} else {
				Shapes.Draw.BlendMode = ShapesBlendMode.Opaque;
			}
		}
		#endregion

		#region Line
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Line(Vector3 from, Vector3 to, Color color, bool useDepth = true, float time = 0) => Line(from, to, 1f, color, color, LineEndCap.None, LineGeometry.Billboard, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Line(Vector3 from, Vector3 to, float thickness, Color color, bool useDepth = true, float time = 0) => Line(from, to, thickness, color, color, LineEndCap.None, LineGeometry.Billboard, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Line(Vector3 from, Vector3 to, float thickness, Color fromColor, Color toColor, bool useDepth = true, float time = 0) => Line(from, to, thickness, fromColor, toColor, LineEndCap.None, LineGeometry.Billboard, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Line(Vector3 from, Vector3 to, float thickness, Color fromColor, Color toColor, LineEndCap lineEndCaps, LineGeometry geometry, bool useDepth, float time) {
			lines.AddLast(new DrawLine(from, to, thickness, fromColor, toColor, lineEndCaps, geometry, useDepth, time));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void Line(DrawLine line) {
			lines.AddLast(line);
		}
		#endregion
		#region Ray
		/*[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Ray(Vector3 from, Vector3 to, Color arrowColor, bool useDepth = true, float time = 0) => Ray(from, to, Color.white * 2, arrowColor, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Ray(Vector3 from, Vector3 to, Color lineColor, Color arrowColor, bool useDepth = true, float time = 0) {
			lines.AddLast(new DrawLine(from, to, 1f, lineColor, lineColor, LineEndCap.None, LineGeometry.Billboard, useDepth, time));
			cones.AddLast(new DrawCone(to, (to - from), arrowColor, 0.07f, 0.175f, useDepth, time));
		}*/
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Ray(Vector3 origin, Vector3 direction, float length, bool useDepth = true, float time = 0) => Ray(origin, direction, length, 2f, DefaultWhite, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Ray(Vector3 origin, Vector3 direction, float length, Color arrowColor, bool useDepth = true, float time = 0) => Ray(origin, direction, length, 2f, arrowColor, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Ray(Vector3 origin, Vector3 direction, float length, Color lineColor, Color arrowColor, bool useDepth = true, float time = 0) => Ray(origin, direction, length, 2f, lineColor, arrowColor, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Ray(Vector3 origin, Vector3 direction, float length, float thickness, Color arrowColor, bool useDepth = true, float time = 0) => Ray(origin, direction, length, thickness, DefaultWhite, arrowColor, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Ray(Vector3 origin, Vector3 direction, float length, float thickness, Color lineColor, Color arrowColor, bool useDepth = true, float time = 0) {
			lines.AddLast(new DrawLine(origin, origin + direction * length, thickness, lineColor, lineColor, LineEndCap.None, LineGeometry.Billboard, useDepth, time));
			cones.AddLast(new DrawCone(origin + direction * length, direction, arrowColor, 0.14f, 0.3f, useDepth, time));
		}
		#endregion
		#region Cone
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Cone(Vector3 position, Vector3 pointDir, float size, bool useDepth = true, float time = 0) => Cone(position, pointDir, size, DefaultWhite, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Cone(Vector3 position, Vector3 pointDir, float size, Color color, bool useDepth = true, float time = 0) => Cone(position, pointDir, size / 2f, size, color, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Cone(Vector3 position, Vector3 pointDir, float radius, float length, Color color, bool useDepth = true, float time = 0) {
			cones.AddLast(new DrawCone(position, pointDir, color, radius, length, useDepth, time));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void Cone(DrawCone cone) {
			cones.AddLast(cone);
		}
		#endregion
		#region Sphere
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Sphere(Vector3 position, bool useDepth = true, float time = 0) => Sphere(position, 1f, Color.white, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Sphere(Vector3 position, Color color, bool useDepth = true, float time = 0) => Sphere(position, 1f, color, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Sphere(Vector3 position, float radius, bool useDepth = true, float time = 0) => Sphere(position, radius, Color.white, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Sphere(Vector3 position, float radius, Color color, bool useDepth = true, float time = 0) {
			spheres.AddLast(new DrawSphere(position, color, radius, useDepth, time));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void Sphere(DrawSphere sphere) {
			spheres.AddLast(sphere);
		}
		#endregion
		#region Box
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Box(Vector3 position, Vector3 size, Color color, bool useDepth = true, float time = 0) => Box(position, Vector3.up, size, color, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Box(Vector3 position, Color color, bool useDepth = true, float time = 0) => Box(position, 1f, color, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Box(Vector3 position, float size, Color color, bool useDepth = true, float time = 0) => Box(position, Vector3.up, Vector3.one * size, color, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Box(Vector3 position, Vector3 orientation, float size, Color color, bool useDepth = true, float time = 0) => Box(position, orientation, Vector3.one * size, color, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Box(Vector3 position, Vector3 orientation, Vector3 size, Color color, bool useDepth = true, float time = 0) {
			cubes.AddLast(new DrawCube(position, orientation, size, color, useDepth, time));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void Box(DrawCube cube) {
			cubes.AddLast(cube);
		}
		#endregion
		#region Rectangle
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Square(Vector3 position, Color color, bool useDepth = true, float time = 0) => Square(position, Vector3.up, 1f, color, 0f, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Square(Vector3 position, float size, Color color, bool useDepth = true, float time = 0) => Square(position, Vector3.up, size, color, 0f, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Square(Vector3 position, float size, Color color, float radius, bool useDepth = true, float time = 0) => Square(position, Vector3.up, size, color, radius, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Square(Vector3 position, Vector3 normal, float size, Color color, bool useDepth = true, float time = 0) => Square(position, normal, size, color, 0f, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Square(Vector3 position, Vector3 normal, float size, Color color, float radius, bool useDepth = true, float time = 0) {
			rectangles.AddLast(new DrawRectangle(position, normal, size, size, radius, color, useDepth, time));
		}
		#endregion
		#region Rectangle
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Rectangle(Vector3 position, Vector2 size, Color color, bool useDepth = true, float time = 0) => Rectangle(position, Vector3.up, size, color, 0, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Rectangle(Vector3 position, float width, float height, Color color, bool useDepth = true, float time = 0) => Rectangle(position, Vector3.up, width, height, color, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Rectangle(Vector3 position, Vector3 normal, float width, float height, Color color, bool useDepth = true, float time = 0) => Rectangle(position, normal, width, height, color, 0f, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Rectangle(Vector3 position, Vector3 normal, Vector2 size, Color color, float radius, bool useDepth = true, float time = 0) => Rectangle(position, normal, size.x, size.y, color, radius, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Rectangle(Vector3 position, Vector3 normal, float width, float height, Color color, float radius, bool useDepth = true, float time = 0) {
			rectangles.AddLast(new DrawRectangle(position, normal, width, height, radius, color, useDepth, time));
		}
		#endregion
		#region Cube
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Cube(Vector3 position, Color color, bool useDepth = true, float time = 0) => Cube(position, Vector3.up, 1f, color, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Cube(Vector3 position, Vector3 normal, Color color, bool useDepth = true, float time = 0) => Cube(position, normal, 1f, color, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Cube(Vector3 position, float size, Color color, bool useDepth = true, float time = 0) => Cube(position, Vector3.up, size, color, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Cube(Vector3 position, Vector3 normal, float size, Color color, bool useDepth = true, float time = 0) => Cube(position, normal, size, color, 0f, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Cube(Vector3 position, Vector3 normal, float size, Color color, float radius, bool useDepth = true, float time = 0) {
			rectangles.AddLast(new DrawRectangle(position, normal, size, size, radius, color, useDepth, time));
		}
		#endregion
		#region Disc
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Disc(Vector3 position, float radius, Color color, bool useDepth = true, float time = 0) => Disc(position, Vector3.up, radius, color, color, false, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Disc(Vector3 position, float radius, Color innerColor, Color outerColor, bool softFill, bool useDepth = true, float time = 0) => Disc(position, Vector3.up, radius, innerColor, outerColor, softFill, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Disc(Vector3 position, Vector3 normal, float radius, Color color, bool useDepth = true, float time = 0) => Disc(position, normal, radius, color, color, false, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Disc(Vector3 position, Vector3 normal, float radius, Color color, bool softFill, bool useDepth = true, float time = 0) => Disc(position, normal, radius, Color.clear, color, softFill, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Disc(Vector3 position, Vector3 normal, float radius, Color innerColor, Color outerColor, bool useDepth = true, float time = 0) => Disc(position, normal, radius, innerColor, outerColor, false, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Disc(Vector3 position, Vector3 normal, float radius, Color innerColor, Color outerColor, bool softFill, bool useDepth, float time = 0) {
			DiscColors colors;
			if (softFill) {
				innerColor = new Color(innerColor.r, innerColor.g, innerColor.b, 0);
			}
			if (innerColor != outerColor || softFill) {
				colors = DiscColors.Radial(innerColor, outerColor);
			} else {
				colors = innerColor;
			}

			discs.AddLast(new DrawDisc(position, normal, colors, radius, useDepth, time));
		}
		#endregion
		#region Ring
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Ring(Vector3 position, float radius, float thickness, Color color, bool useDepth = true, float time = 0) => Ring(position, Vector3.up, radius, thickness, color, color, false, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Ring(Vector3 position, float radius, float thickness, Color innerColor, Color outerColor, bool softFill, bool useDepth = true, float time = 0) => Ring(position, Vector3.up, radius, thickness, innerColor, outerColor, softFill, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Ring(Vector3 position, Vector3 normal, float radius, float thickness, Color color, bool useDepth = true, float time = 0) => Ring(position, normal, radius, thickness, color, color, false, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Ring(Vector3 position, Vector3 normal, float radius, float thickness, Color color, bool softFill, bool useDepth = true, float time = 0) => Ring(position, normal, radius, thickness, Color.clear, color, softFill, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Ring(Vector3 position, Vector3 normal, float radius, float thickness, Color innerColor, Color outerColor, bool useDepth = true, float time = 0) => Ring(position, normal, radius, thickness, innerColor, outerColor, false, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Ring(Vector3 position, Vector3 normal, float radius, float thickness, Color innerColor, Color outerColor, bool softFill, bool useDepth = true, float time = 0) {
			DiscColors colors;
			if (softFill) {
				innerColor = new Color(innerColor.r, innerColor.g, innerColor.b, 0);
			}
			if (innerColor != outerColor || softFill) {
				colors = DiscColors.Radial(innerColor, outerColor);
			} else {
				colors = innerColor;
			}

			rings.AddLast(new DrawRing(position, normal, colors, radius, thickness, useDepth, time));
		}
		#endregion
		#region Triangle
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void FlatTriangle(Vector3 position, float angle, float size, Color color, bool useDepth = true, float time = 0) => FlatTriangle(position, angle, size, color, 1f, 0f, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void FlatTriangle(Vector3 position, float angle, float size, Color color, float lengthPercent, bool useDepth = true, float time = 0) => FlatTriangle(position, angle, size, color, lengthPercent, 0f, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void FlatTriangle(Vector3 position, float angle, float size, Color color, float lengthPercent, float borderRadius, bool useDepth = true, float time = 0) {
			angle %= 360;
			Vector2 dir = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad)).normalized;

			Triangle(position, new Vector3(dir.x, 0, dir.y), size, color, borderRadius, lengthPercent, useDepth, time);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Triangle(Vector3 position, Vector3 normal, float size, Color color, bool useDepth = true, float time = 0) => Triangle(position, normal, size, color, 0, 1f, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Triangle(Vector3 position, Vector3 normal, float size, Color color, float borderRadius, bool useDepth = true, float time = 0) => Triangle(position, normal, size, color, borderRadius, 1f, useDepth, time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Triangle(Vector3 position, Vector3 normal, float size, Color color, float borderRadius, float tipPercentage, bool useDepth = true, float time = 0) {
			triangles.AddLast(new DrawTriangle(position, normal, size, color, borderRadius, tipPercentage, useDepth, time));
		}
		#endregion

		public readonly struct DrawLine : IDrawBase {

			readonly Vector3 start;
			readonly Vector3 end;
			readonly Color colorA;
			readonly Color colorB;
			readonly LineEndCap lineEndCaps;
			readonly LineGeometry geometry;
			readonly float thickness;
			readonly bool useDepth;
			readonly DateTime endTime;

			public DrawLine(Vector3 start, Vector3 end, float thickness, Color colorA, Color colorB, LineEndCap lineEndCaps, LineGeometry geometry, bool useDepth, float time) {
				this.start = start;
				this.end = end;
				this.colorA = colorA;
				this.colorB = colorB;
				this.lineEndCaps = lineEndCaps;
				this.geometry = geometry;
				this.thickness = thickness;
				this.useDepth = useDepth;
				this.endTime = DateTime.UtcNow.AddSeconds(time);
			}

			public readonly void Draw() {
				SetZTest(useDepth);
				SetTransparent(colorA, colorB);

				Shapes.Draw.LineGeometry = geometry;
				Shapes.Draw.Line(start, end, thickness, lineEndCaps, colorA, colorB);
			}
			public readonly bool IsAlive() {
				return DateTime.UtcNow <= endTime;
			}
		}
		public readonly struct DrawCone : IDrawBase {

			readonly Vector3 pos;
			readonly Vector3 pointDir;
			readonly Color color;
			readonly float radius;
			readonly float length;
			readonly bool useDepth;
			readonly DateTime endTime;

			public DrawCone(Vector3 pos, Vector3 pointDir, Color color, float radius, float length, bool useDepth, float time) {
				this.pos = pos;
				this.pointDir = pointDir;
				this.color = color;
				this.radius = radius;
				this.length = length;
				this.useDepth = useDepth;
				this.endTime = DateTime.UtcNow.AddSeconds(time);
			}

			public readonly void Draw() {
				SetZTest(useDepth);
				SetTransparent(color);

				Shapes.Draw.Cone(pos, pointDir, radius, length, true, color);
			}
			public readonly bool IsAlive() {
				return DateTime.UtcNow <= endTime;
			}
		}
		public readonly struct DrawCube : IDrawBase {

			readonly Vector3 position;
			readonly Vector3 orientation;
			readonly Quaternion rotation;
			readonly Vector3 size;
			readonly Color color;
			readonly bool useDepth;
			readonly DateTime endTime;

			public DrawCube(Vector3 position, Vector3 orientation, Vector3 size, Color color, bool useDepth, float time) : this() {
				this.position = position;
				this.orientation = orientation;
				this.rotation = Quaternion.LookRotation(orientation.normalized, Vector3.up) * Quaternion.Euler(90, 0, 0);
				this.size = size;
				this.color = color;
				this.useDepth = useDepth;
				this.endTime = DateTime.UtcNow.AddSeconds(time);
			}

			public readonly void Draw() {
				SetZTest(useDepth);
				SetTransparent(color);

				Shapes.Draw.Cuboid(position, rotation, size, color);
			}
			public readonly bool IsAlive() {
				return DateTime.UtcNow <= endTime;
			}

		}
		public readonly struct DrawSphere : IDrawBase {

			readonly Vector3 origin;
			readonly Color color;
			readonly float radius;
			readonly bool useDepth;
			readonly DateTime endTime;

			public DrawSphere(Vector3 origin, Color color, float radius, bool useDepth, float time) {
				this.origin = origin;
				this.color = color;
				this.radius = radius / 2f;
				this.useDepth = useDepth;
				this.endTime = DateTime.UtcNow.AddSeconds(time);
			}

			public readonly void Draw() {
				SetZTest(useDepth);
				SetTransparent(color);

				Shapes.Draw.Sphere(origin, radius, color);
			}
			public readonly bool IsAlive() {
				return DateTime.UtcNow <= endTime;
			}
		}
		public readonly struct DrawRectangle : IDrawBase {

			readonly Vector3 position;
			readonly Vector3 normal;
			readonly float width;
			readonly float height;
			readonly float cornerRadius;
			readonly Color color;
			readonly bool useDepth;
			readonly DateTime endTime;

			public DrawRectangle(Vector3 position, Vector3 normal, float width, float height, float cornerRadius, Color color, bool useDepth, float time) : this() {
				this.position = position;
				this.normal = normal;
				this.width = width;
				this.height = height;
				this.cornerRadius = Remap(Mathf.Clamp01(cornerRadius), 0f, 1f, 0f, 0.5f);
				this.color = color;
				this.useDepth = useDepth;
				this.endTime = DateTime.UtcNow.AddSeconds(time);
			}

			public readonly void Draw() {
				SetZTest(useDepth);
				SetTransparent(color);

				Shapes.Draw.Rectangle(position, normal, width, height, RectPivot.Center, cornerRadius, color);
			}
			public readonly bool IsAlive() {
				return DateTime.UtcNow <= endTime;
			}
		}
		public readonly struct DrawDisc : IDrawBase {

			readonly Vector3 position;
			readonly Vector3 normal;
			readonly DiscColors color;
			readonly float radius;
			readonly bool useDepth;
			readonly DateTime endTime;

			public DrawDisc(Vector3 position, Vector3 normal, DiscColors color, float radius, bool useDepth, float time) : this() {
				this.position = position;
				this.normal = normal;
				this.color = color;
				this.radius = radius / 2f;
				this.useDepth = useDepth;
				this.endTime = DateTime.UtcNow.AddSeconds(time);
			}

			public readonly void Draw() {
				SetZTest(useDepth);
				SetTransparent(color.innerStart, color.outerStart);

				Shapes.Draw.Disc(position, normal, radius, color);
			}
			public readonly bool IsAlive() {
				return DateTime.UtcNow <= endTime;
			}

		}
		public readonly struct DrawRing {

			readonly Vector3 position;
			readonly Vector3 normal;
			readonly DiscColors color;
			readonly float radius;
			readonly float thickness;
			readonly bool useDepth;
			readonly DateTime endTime;

			public DrawRing(Vector3 position, Vector3 normal, DiscColors color, float radius, float thickness, bool useDepth, float time) : this() {
				this.position = position;
				this.normal = normal;
				this.color = color;
				this.thickness = Mathf.Clamp(thickness, 0, radius / 2f);
				this.radius = radius / 2f - this.thickness / 2;
				this.useDepth = useDepth;
				this.endTime = DateTime.UtcNow.AddSeconds(time);
			}

			public readonly void Draw() {
				SetZTest(useDepth);
				SetTransparent(color.innerStart, color.outerStart);

				Shapes.Draw.Ring(position, normal, radius, thickness, color);
			}
			public readonly bool IsAlive() {
				return DateTime.UtcNow <= endTime;
			}
		}
		public readonly struct DrawTriangle : IDrawBase {

			readonly Vector3 corner1;
			readonly Vector3 corner2;
			readonly Vector3 corner3;
			readonly Color color;
			readonly float tipPercentage;
			readonly float borderRadius;
			readonly bool useDepth;
			readonly DateTime endTime;

			public DrawTriangle(Vector3 position, Vector3 normal, float size, Color color, float borderRadius, float tipPercentage, bool useDepth, float time) {
				this.tipPercentage = Mathf.Clamp(tipPercentage, 0.01f, float.MaxValue);
				size /= 2;
				normal.Normalize();

				// Calculate the three corners of the equilateral triangle
				corner1 = position + (size * 1.82f * tipPercentage) * normal;
				corner2 = position + size * Vector3.Cross(normal, Vector3.up);
				corner3 = position - size * Vector3.Cross(normal, Vector3.up);

				this.color = color;
				this.borderRadius = borderRadius;
				this.useDepth = useDepth;
				this.endTime = DateTime.UtcNow.AddSeconds(time);
			}

			public readonly void Draw() {
				SetZTest(useDepth);
				SetTransparent(color);

				Shapes.Draw.Triangle(corner1, corner2, corner3, borderRadius, color, color, color);
			}
			public readonly bool IsAlive() {
				return DateTime.UtcNow <= endTime;
			}

		}
		public interface IDrawBase {

			bool IsAlive();
			void Draw();

		}
	}
}