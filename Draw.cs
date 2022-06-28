using Shapes;
using System.Collections.Generic;
using UnityEngine;
// Tip for running Shapes in HDRP:
// Enable Custom-Pass in HDRP-Asset

namespace Sperlich.Debug.Draw {
	[ExecuteAlways]
	public class Draw : ImmediateModeShapeDrawer {

		static Draw _instance;
		public static Draw Instance {
			get {
				if (_instance == null) {
					_instance = FindObjectOfType<Draw>();
					if (_instance == null) {
						_instance = new GameObject("Drawer").AddComponent<Draw>();
					}
				}
				return _instance;
			}
		}
		static IList<DrawLine> Lines { get; set; } = new List<DrawLine>();
		static IList<DrawDisc> Discs { get; set; } = new List<DrawDisc>();
		static IList<DrawRing> Rings { get; set; } = new List<DrawRing>();
		static IList<DrawText> Texts { get; set; } = new List<DrawText>();
		static IList<DrawSphere> Spheres { get; set; } = new List<DrawSphere>();
		static IList<DrawCube> Cubes { get; set; } = new List<DrawCube>();
		static IList<DrawRectangle> Rectangles { get; set; } = new List<DrawRectangle>();
		public static ThicknessSpace LineThicknessSpace = ThicknessSpace.Pixels;
		/// <summary>
		/// Disables the ability to Draw. Queued Shapes will still be drawn after reenabling. To clear them call KillAll().
		/// </summary>
		public static bool DisableDrawing { get; set; }

		[RuntimeInitializeOnLoadMethod]
		public static void Initialize() {
			DontDestroyOnLoad(Instance);
		}

		public override void DrawShapes(Camera cam) {
			if (DisableDrawing) return;
			using (Shapes.Draw.Command(cam)) {
				Shapes.Draw.ResetAllDrawStates();
				for (int i = 0; i < Lines.Count; i++) {
					var l = Lines[i];
					Shapes.Draw.LineGeometry = l.geometry;
					Shapes.Draw.Thickness = l.thickness;
					Shapes.Draw.ThicknessSpace = LineThicknessSpace;

					if (IsTransparent(l.startColor) || IsTransparent(l.endColor)) {
						Shapes.Draw.BlendMode = ShapesBlendMode.Transparent;
					} else {
						Shapes.Draw.BlendMode = ShapesBlendMode.Opaque;
					}

					SetZTest(l.zTest);
					Shapes.Draw.Line(l.start, l.end, l.thickness, l.endCaps, l.startColor, l.endColor);
					if (l.time <= 0) {
						Lines.RemoveAt(i);
						i--;
					} else {
						Lines.Add(new DrawLine() {
							start = l.start,
							end = l.end,
							thickness = l.thickness,
							endCaps = l.endCaps,
							startColor = l.startColor,
							endColor = l.endColor,
							geometry = l.geometry,
							time = l.time - Time.deltaTime,
							zTest = l.zTest,
						});
					}
				}
			}
			using (Shapes.Draw.Command(cam)) {
				for (int i = 0; i < Discs.Count; i++) {
					var d = Discs[i];
					Shapes.Draw.DiscGeometry = d.geometry;
					if (d.softFill) {
						d.startColor = new Color(d.startColor.r, d.startColor.g, d.startColor.b, 0);
					}

					if (IsTransparent(d.startColor) || IsTransparent(d.endColor)) {
						Shapes.Draw.BlendMode = ShapesBlendMode.Transparent;
					} else {
						Shapes.Draw.BlendMode = ShapesBlendMode.Opaque;
					}

					DiscColors colors = CreateDiscColor(d.startColor, d.endColor, true);
					SetZTest(d.zTest);

					Shapes.Draw.Disc(d.pos, d.normal, d.radius, colors);
					if (d.time <= 0) {
						Discs.RemoveAt(i);
						i--;
					} else {
						Discs.Add(new DrawDisc() {
							pos = d.pos,
							normal = d.normal,
							radius = d.radius,
							time = d.time - Time.deltaTime,
							endColor = d.endColor,
							geometry = d.geometry,
							softFill = d.softFill,
							startColor = d.startColor,
							zTest = d.zTest
						});
					}
				}
			}
			using (Shapes.Draw.Command(cam)) {
				for (int i = 0; i < Rings.Count; i++) {
					var r = Rings[i];
					Shapes.Draw.DiscGeometry = r.geometry;
					Shapes.Draw.Thickness = r.thickness;
					if (r.softFill) {
						r.startColor = new Color(r.startColor.r, r.startColor.g, r.startColor.b, 0);
					}

					if (IsTransparent(r.startColor) || IsTransparent(r.endColor)) {
						Shapes.Draw.BlendMode = ShapesBlendMode.Transparent;
					} else {
						Shapes.Draw.BlendMode = ShapesBlendMode.Opaque;
					}

					DiscColors colors = CreateDiscColor(r.startColor, r.endColor, true);
					SetZTest(r.zTest);

					Shapes.Draw.Ring(r.pos, r.normal, r.radius, colors);
					if (r.time <= 0) {
						Rings.RemoveAt(i);
						i--;
					} else {
						Rings.Add(new DrawRing() {
							pos = r.pos,
							normal = r.normal,
							radius = r.radius,
							startColor = r.startColor,
							endColor = r.endColor,
							geometry = r.geometry,
							thickness = r.thickness,
							softFill = r.softFill,
							time = r.time - Time.deltaTime,
							zTest = r.zTest
						});
					}
				}
			}
			using (Shapes.Draw.Command(cam)) {
				for (int i = 0; i < Texts.Count; i++) {
					var t = Texts[i];
					Shapes.Draw.ThicknessSpace = ThicknessSpace.Pixels;
					SetZTest(t.zTest);

					Shapes.Draw.Text(t.pos, t.normal, t.text, TextAlign.Center, t.fontSize, t.color);
					if (t.time <= 0) {
						Texts.RemoveAt(i);
						i--;
					} else {
						Texts[i] = new DrawText() {
							pos = t.pos,
							color = t.color,
							fontSize = t.fontSize,
							normal = t.normal,
							text = t.text,
							time = t.time - Time.deltaTime,
							zTest = t.zTest
						};
					}
				}
			}
			using (Shapes.Draw.Command(cam)) {
				for (int i = 0; i < Spheres.Count; i++) {
					var s = Spheres[i];
					SetZTest(s.zTest);

					Shapes.Draw.Sphere(s.pos, s.radius, s.color);
					if (s.time <= 0) {
						Spheres.RemoveAt(i);
						i--;
					} else {
						Spheres[i] = new DrawSphere() {
							pos = s.pos,
							radius = s.radius,
							color = s.color,
							time = s.time - Time.deltaTime,
							zTest = s.zTest,
						};
					}
				}
			}
			using (Shapes.Draw.Command(cam)) {
				for (int i = 0; i < Cubes.Count; i++) {
					var c = Cubes[i];
					SetZTest(c.zTest);

					if (IsTransparent(c.color)) {
						Shapes.Draw.BlendMode = ShapesBlendMode.Transparent;
					} else {
						Shapes.Draw.BlendMode = ShapesBlendMode.Opaque;
					}

					Shapes.Draw.Cuboid(c.pos, c.normal, c.size, c.color);
					if (c.time <= 0) {
						Cubes.RemoveAt(i);
						i--;
					} else {
						Cubes.Add(new DrawCube() {
							pos = c.pos,
							size = c.size,
							color = c.color,
							normal = c.normal,
							time = c.time - Time.deltaTime,
							zTest = c.zTest
						});
					}
				}
			}
			using (Shapes.Draw.Command(cam)) {
				for (int i = 0; i < Rectangles.Count; i++) {
					var r = Rectangles[i];
					SetZTest(r.zTest);

					if (IsTransparent(r.color)) {
						Shapes.Draw.BlendMode = ShapesBlendMode.Transparent;
					} else {
						Shapes.Draw.BlendMode = ShapesBlendMode.Opaque;
					}

					Shapes.Draw.Rectangle(r.pos, r.normal, new Rect(0, 0, r.size.x, r.size.y), r.radius, r.color);
					if (r.time <= 0) {
						Rectangles.RemoveAt(i);
						i--;
					} else {
						Rectangles.Add(new DrawRectangle() {
							pos = r.pos,
							color = r.color,
							size = r.size,
							normal = r.normal,
							zTest = r.zTest,
							time = r.time - Time.deltaTime,
						});
					}
				}
			}
		}
		/// <summary>
		/// Clears all currently queued Draw-Calls. Also cancels all Shapes with a duration.
		/// </summary>
		public void KillAll() {
			Lines = new List<DrawLine>();
			Discs = new List<DrawDisc>();
			Rings = new List<DrawRing>();
			Texts = new List<DrawText>();
			Spheres = new List<DrawSphere>();
			Cubes = new List<DrawCube>();
			Rectangles = new List<DrawRectangle>();
		}

		public static void Ray(Vector3 origin, Vector3 direction, float time, bool zTest = false) => Line(origin, origin + direction * 2, Color.white, time, zTest);
		public static void Ray(Vector3 origin, Vector3 direction, Color color, float time = 0, bool zTest = false) => Line(origin, origin + direction * 2, 2f, color, time, zTest);
		public static void Ray(Vector3 origin, Vector3 direction, float length, Color color, float time = 0, bool zTest = false) => Line(origin, origin + direction * length, color, time, zTest);
		public static void Line(Vector3 start, Vector3 end, Color color, float time = 0, bool zTest = true) => Line(start, end, 2f, color, time, zTest);
		public static void Line(Vector3 start, Vector3 end, float thickness, float time = 0, bool zTest = true) => Line(start, end, thickness, Color.white, time, zTest);
		public static void Line(Vector3 start, Vector3 end, float thickness, Color color, float time, bool zTest = true) => Line(start, end, thickness, color, color, time, zTest);
		public static void Line(Vector3 start, Vector3 end, Color color, bool zTest = true) => Line(start, end, 1f, color, 0f, zTest);
		public static void Line(Vector3 start, Vector3 end, float thickness, Color color, bool zTest = true) => Line(start, end, thickness, color, 0f, zTest);
		public static void Line(Vector3 start, Vector3 end, float thickness, Color color, LineGeometry geometry, float time = 0, bool zTest = false) => Line(start, end, thickness, color, color, LineEndCap.Round, geometry, time, zTest);
		public static void Line(Vector3 start, Vector3 end, float thickness, Color startColor, Color endColor, float time = 0, bool zTest = true) => Line(start, end, thickness, startColor, endColor, LineEndCap.Round, time, zTest);
		public static void Line(Vector3 start, Vector3 end, float thickness, Color startColor, Color endColor, LineEndCap endCaps, float time = 0, bool zTest = true) => Line(start, end, thickness, startColor, endColor, endCaps, LineGeometry.Billboard, time, zTest);
		public static void Line(Vector3 start, Vector3 end, float thickness, Color startColor, Color endColor, LineEndCap endCaps, LineGeometry geometry, float time = 0, bool zTest = false) {
			Lines.Add(new DrawLine() {
				start = start,
				end = end,
				startColor = startColor,
				endColor = endColor,
				thickness = thickness,
				endCaps = endCaps,
				geometry = geometry,
				zTest = zTest,
				time = time
			});
		}

		public static void Disc(Vector3 pos, Vector3 normal, float radius, Color startcolor, Color endColor, bool zTest = true) => Disc(pos, normal, radius, startcolor, endColor, false, 0, zTest);
		public static void Disc(Vector3 pos, Vector3 normal, float radius, Color color, bool zTest) => Disc(pos, normal, radius, color, false, 0, zTest);
		public static void Disc(Vector3 pos, Vector3 normal, float radius, Color color, float time = 0, bool zTest = true) => Disc(pos, normal, radius, color, false, time, zTest);
		public static void Disc(Vector3 pos, Vector3 normal, bool softFill = false, float time = 0, bool zTest = true) => Disc(pos, normal, 1f, softFill, time, zTest);
		public static void Disc(Vector3 pos, Vector3 normal, float radius, bool softFill = false, bool zTest = true) => Disc(pos, normal, radius, Color.white, softFill, 0, zTest);
		public static void Disc(Vector3 pos, Vector3 normal, float radius, bool softFill = false, float time = 0, bool zTest = true) => Disc(pos, normal, radius, Color.white, softFill, time, zTest);
		public static void Disc(Vector3 pos, Vector3 normal, float radius, Color color, bool softFill = false, bool zTest = true) => Disc(pos, normal, radius, color, color, softFill, 0, zTest);
		public static void Disc(Vector3 pos, Vector3 normal, float radius, Color color, bool softFill = false, float time = 0, bool zTest = true) => Disc(pos, normal, radius, color, color, softFill, time, zTest);
		public static void Disc(Vector3 pos, Vector3 normal, float radius, Color startColor, Color endColor, bool softFill = false, bool zTest = true) => Disc(pos, normal, radius, startColor, endColor, DiscGeometry.Flat2D, softFill, 0, zTest);
		public static void Disc(Vector3 pos, Vector3 normal, float radius, Color startColor, Color endColor, bool softFill = false, float time = 0, bool zTest = true) => Disc(pos, normal, radius, startColor, endColor, DiscGeometry.Flat2D, softFill, time, zTest);
		public static void Disc(Vector3 pos, Vector3 normal, float radius, Color startColor, Color endColor, DiscGeometry geometry, bool softFill = false, float time = 0, bool zTest = true) {
			Discs.Add(new DrawDisc() {
				pos = pos,
				normal = normal,
				startColor = startColor,
				endColor = endColor,
				geometry = geometry,
				radius = radius,
				softFill = softFill,
				zTest = zTest,
				time = time
			});
		}

		public static void Ring(Vector3 pos, Vector3 normal, float thickness = 1f, bool softFill = false, float time = 0, bool zTest = true) => Ring(pos, normal, 1f, thickness, softFill, time, zTest);
		public static void Ring(Vector3 pos, Vector3 normal, float radius, float thickness, bool softFill = false, float time = 0, bool zTest = true) => Ring(pos, normal, radius, thickness, Color.white, Color.white, softFill, time, zTest);
		public static void Ring(Vector3 pos, Vector3 normal, float radius, float thickness, Color color, bool softFill = false, bool zTest = true) => Ring(pos, normal, radius, thickness, color, color, softFill, 0, zTest);
		public static void Ring(Vector3 pos, Vector3 normal, float radius, float thickness, Color color, bool softFill = false, float time = 0, bool zTest = true) => Ring(pos, normal, radius, thickness, color, color, softFill, time, zTest);
		public static void Ring(Vector3 pos, Vector3 normal, float radius, float thickness, Color startColor, Color endColor, bool softFill = false, bool zTest = true) => Ring(pos, normal, radius, thickness, startColor, endColor, DiscGeometry.Flat2D, softFill, 0, zTest);
		public static void Ring(Vector3 pos, Vector3 normal, float radius, float thickness, Color startColor, Color endColor, bool softFill = false, float time = 0, bool zTest = true) => Ring(pos, normal, radius, thickness, startColor, endColor, DiscGeometry.Flat2D, softFill, time, zTest);
		public static void Ring(Vector3 pos, Vector3 normal, float radius, float thickness, Color startColor, Color endColor, DiscGeometry geometry, bool softFill = false, float time = 0, bool zTest = true) {
			Rings.Add(new DrawRing() {
				pos = pos,
				normal = normal,
				startColor = startColor,
				endColor = endColor,
				geometry = geometry,
				zTest = zTest,
				radius = radius,
				thickness = thickness,
				softFill = softFill,
				time = time,
			});
		}

		public static void Text(Vector3 pos, string text, float fontSize, Color color, bool zTest = false) => Text(pos, text, fontSize, color, 0, zTest);
		public static void Text(Vector3 pos, string text, float time = 0, bool zTest = false) => Text(pos, text, 14, time, zTest);
		public static void Text(Vector3 pos, string text, float fontSize, float time = 0, bool zTest = false) => Text(pos, text, fontSize, Color.white, time, zTest);
		public static void Text(Vector3 pos, string text, float fontSize, Color color, float time = 0, bool zTest = false) => Text(pos, (pos - Camera.main.transform.position).normalized, text, fontSize, color, time, zTest);
		public static void Text(Vector3 pos, Vector3 normal, string text, float time = 0, bool zTest = false) => Text(pos, normal, text, 14, time, zTest);
		public static void Text(Vector3 pos, Vector3 normal, string text, float fontSize, float time = 0, bool zTest = false) => Text(pos, normal, text, fontSize, Color.white, time, zTest);
		public static void Text(Vector3 pos, Vector3 normal, string text, float fontSize, Color color, float time = 0, bool zTest = false) {
			Texts.Add(new DrawText() {
				pos = pos,
				normal = normal,
				color = color,
				fontSize = fontSize,
				text = text,
				zTest = zTest,
				time = time
			});
		}

		public static void Sphere(Vector3 pos, float radius, Color color, bool zTest) => Sphere(pos, radius, color, 0, zTest);
		public static void Sphere(Vector3 pos, float time = 0, bool zTest = false) => Sphere(pos, 1f, time, zTest);
		public static void Sphere(Vector3 pos, float radius, float time = 0, bool zTest = false) => Sphere(pos, radius, Color.white, time, zTest);
		public static void Sphere(Vector3 pos, float radius, Color color, float time = 0, bool zTest = false) {
			Spheres.Add(new DrawSphere() {
				pos = pos,
				radius = radius,
				color = color,
				zTest = zTest,
				time = time
			});
		}

		public static void Cube(Vector3 pos, Vector3 size, Color color, bool zTest) => Cube(pos, size, color, 0, zTest);
		public static void Cube(Vector3 pos, Vector3 normal, Vector3 size, Color color, bool zTest) => Cube(pos, normal, size, color, 0, zTest);
		public static void Cube(Vector3 pos, float time = 0, bool zTest = false) => Cube(pos, Vector3.one, time, zTest);
		public static void Cube(Vector3 pos, Vector3 size, float time = 0, bool zTest = false) => Cube(pos, size, Color.white, time, zTest);
		public static void Cube(Vector3 pos, Color color, float time = 0, bool zTest = false) => Cube(pos, Vector3.one, color, time, zTest);
		public static void Cube(Vector3 pos, Vector3 size, Color color, float time = 0, bool zTest = false) => Cube(pos, Vector3.right, size, color, time, zTest);
		public static void Cube(Vector3 pos, Vector3 normal, Vector3 size, Color color, float time = 0, bool zTest = false) {
			Cubes.Add(new DrawCube() {
				pos = pos,
				normal = normal,
				size = size,
				color = color,
				zTest = zTest,
				time = time
			});
		}

		public static void Rectangle(Vector3 pos, Vector2 size, Color color, bool zTest) => Rectangle(pos, size, color, 0, 0, zTest);
		public static void Rectangle(Vector3 pos, Vector2 size, Color color, float cornerRadius, bool zTest) => Rectangle(pos, size, color, cornerRadius, 0, zTest);
		public static void Rectangle(Vector3 pos, float time = 0, bool zTest = false) => Rectangle(pos, Color.white, time, zTest);
		public static void Rectangle(Vector3 pos, Color color, float time = 0, bool zTest = false) => Rectangle(pos, Vector3.up, Vector2.one, color, time, zTest);
		public static void Rectangle(Vector3 pos, Vector2 size, Color color, float cornerRadius, float time = 0, bool zTest = false) => Rectangle(pos, Vector3.up, size, color, cornerRadius, time, zTest);
		public static void Rectangle(Vector3 pos, Vector3 normal, Vector2 size, float time = 0, bool zTest = false) => Rectangle(pos, normal, size, Color.white, time, zTest);
		public static void Rectangle(Vector3 pos, Vector3 normal, Vector2 size, Color color, float time = 0, bool zTest = false) => Rectangle(pos, normal, size, color, 0, time, zTest);
		public static void Rectangle(Vector3 pos, Vector3 normal, Vector2 size, Color color, float cornerRadius, float time = 0, bool zTest = false) {
			Rectangles.Add(new DrawRectangle() {
				pos = pos,
				normal = normal,
				color = color,
				radius = cornerRadius,
				size = size,
				zTest = zTest,
				time = time
			});
		}

		public static void SetZTest(bool state) {
			if (state) {
				Shapes.Draw.ZTest = UnityEngine.Rendering.CompareFunction.LessEqual;
			} else {
				Shapes.Draw.ZTest = UnityEngine.Rendering.CompareFunction.Always;
			}
		}
		public static bool IsTransparent(Color col2) => col2.a < 1f;
		public static DiscColors CreateDiscColor(Color mainColor, Color secondaryColor) => CreateDiscColor(mainColor, secondaryColor, false, true);
		public static DiscColors CreateDiscColor(Color mainColor, Color secondaryColor, bool isRadial) => CreateDiscColor(mainColor, secondaryColor, isRadial, false);
		static DiscColors CreateDiscColor(Color mainColor, Color secondaryColor, bool IsRadial, bool IsAngular) {
			DiscColors colors;
			if (!IsRadial && !IsAngular) {
				colors = DiscColors.Flat(mainColor);
			} else if (IsRadial) {
				colors = DiscColors.Radial(mainColor, secondaryColor);
			} else if (IsAngular) {
				colors = DiscColors.Angular(mainColor, secondaryColor);
			} else {
				colors = DiscColors.Flat(mainColor);
			}
			return colors;
		}

		struct DrawLine {
			public Vector3 start;
			public Vector3 end;
			public Color startColor;
			public Color endColor;
			public LineEndCap endCaps;
			public LineGeometry geometry;
			public float thickness;
			public bool zTest;
			public float time;
		}
		struct DrawDisc {
			public Vector3 pos;
			public Vector3 normal;
			public Color startColor;
			public Color endColor;
			public DiscGeometry geometry;
			public bool zTest;
			public bool softFill;
			public float radius;
			public float time;
		}
		struct DrawRing {
			public Vector3 pos;
			public Vector3 normal;
			public Color startColor;
			public Color endColor;
			public DiscGeometry geometry;
			public float radius;
			public float thickness;
			public bool zTest;
			public bool softFill;
			public float time;
		}
		struct DrawText {
			public Vector3 pos;
			public Vector3 normal;
			public Color color;
			public float fontSize;
			public string text;
			public bool zTest;
			public float time;
		}
		struct DrawSphere {
			public Vector3 pos;
			public Color color;
			public float radius;
			public bool zTest;
			public float time;
		}
		struct DrawCube {
			public Vector3 pos;
			public Vector3 normal;
			public Color color;
			public Vector3 size;
			public bool zTest;
			public float time;
		}
		struct DrawRectangle {
			public Vector3 pos;
			public Vector2 size;
			public Vector3 normal;
			public Color color;
			public float radius;
			public bool zTest;
			public float time;
		}
	}
}