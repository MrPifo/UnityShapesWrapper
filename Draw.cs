using Shapes;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Callbacks;
#endif

namespace Sperlich.Debug.Draw {
	[ExecuteAlways]
	public class Draw : ImmediateModeShapeDrawer {

		static Draw instance;
		public static Draw Instance => instance;
		static Queue<DrawLine> Lines { get; set; } = new Queue<DrawLine>();
		static Queue<DrawDisc> Discs { get; set; } = new Queue<DrawDisc>();
		static Queue<DrawRing> Rings { get; set; } = new Queue<DrawRing>();
		static Queue<DrawText> Texts { get; set; } = new Queue<DrawText>();

#if UNITY_EDITOR
		[DidReloadScripts]
		public static void ScriptReload() {
			if(Instance == null && !FindObjectOfType<Draw>()) {
				instance = new GameObject("ShapesDrawer").AddComponent<Draw>();
			} else {
				instance = FindObjectOfType<Draw>();
			}
		}
#endif

		public override void DrawShapes(Camera cam) {
			using(Shapes.Draw.Command(cam)) {
				while(Lines.Count > 0) {
					var l = Lines.Dequeue();
					Shapes.Draw.ResetAllDrawStates();
					Shapes.Draw.LineGeometry = l.geometry;
					Shapes.Draw.Thickness = l.thickness;
					Shapes.Draw.ThicknessSpace = ThicknessSpace.Pixels;

					if(IsTransparent(l.startColor) || IsTransparent(l.endColor)) {
						Shapes.Draw.BlendMode = ShapesBlendMode.Transparent;
					} else {
						Shapes.Draw.BlendMode = ShapesBlendMode.Opaque;
					}

					SetZTest(l.zTest);
					Shapes.Draw.Line(l.start, l.end, l.thickness, l.endCaps, l.startColor, l.endColor);
				}
				while(Discs.Count > 0) {
					var d = Discs.Dequeue();
					Shapes.Draw.ResetAllDrawStates();
					Shapes.Draw.DiscGeometry = d.geometry;
					if(d.softFill) {
						d.startColor = new Color32(d.startColor.r, d.startColor.g, d.startColor.b, 0);
					}

					if(IsTransparent(d.startColor) || IsTransparent(d.endColor)) {
						Shapes.Draw.BlendMode = ShapesBlendMode.Transparent;
					} else {
						Shapes.Draw.BlendMode = ShapesBlendMode.Opaque;
					}

					DiscColors colors = CreateDiscColor(d.startColor, d.endColor, true);
					SetZTest(d.zTest);

					Shapes.Draw.Disc(d.pos, d.normal, d.radius, colors);
				}
				while(Rings.Count > 0) {
					var r = Rings.Dequeue();
					Shapes.Draw.ResetAllDrawStates();
					Shapes.Draw.DiscGeometry = r.geometry;
					Shapes.Draw.Thickness = r.thickness;
					if(r.softFill) {
						r.startColor = new Color32(r.startColor.r, r.startColor.g, r.startColor.b, 0);
					}

					if(IsTransparent(r.startColor) || IsTransparent(r.endColor)) {
						Shapes.Draw.BlendMode = ShapesBlendMode.Transparent;
					} else {
						Shapes.Draw.BlendMode = ShapesBlendMode.Opaque;
					}

					DiscColors colors = CreateDiscColor(r.startColor, r.endColor, true);
					SetZTest(r.zTest);

					Shapes.Draw.Ring(r.pos, r.normal, r.radius, colors);
				}
				while(Texts.Count > 0) {
					var t = Texts.Dequeue();
					Shapes.Draw.ResetAllDrawStates();
					Shapes.Draw.ThicknessSpace = ThicknessSpace.Pixels;
					SetZTest(t.zTest);

					Shapes.Draw.Text(t.pos, t.normal, t.text, TextAlign.Center, t.fontSize, t.color);
				}
			}
		}

		public static void Ray(Vector3 origin, Vector3 direction, Color32 color, bool zTest = false) => Line(origin, origin + direction * 2, 2f, color, zTest);
		public static void Line(Vector3 start, Vector3 end, Color32 color, bool zTest = false) => Line(start, end, 2f, color, zTest);
		public static void Line(Vector3 start, Vector3 end, float thickness, bool zTest = true) => Line(start, end, thickness, Color.white, zTest);
		public static void Line(Vector3 start, Vector3 end, float thickness, Color32 color, bool zTest = true) => Line(start, end, thickness, color, color, zTest);
		public static void Line(Vector3 start, Vector3 end, float thickness, Color32 startColor, Color32 endColor, bool zTest = true) => Line(start, end, thickness, startColor, endColor, LineEndCap.Round, zTest);
		public static void Line(Vector3 start, Vector3 end, float thickness, Color32 startColor, Color32 endColor, LineEndCap endCaps, bool zTest = true) => Line(start, end, thickness, startColor, endColor, endCaps, LineGeometry.Billboard, zTest);
		public static void Line(Vector3 start, Vector3 end, float thickness, Color32 startColor, Color32 endColor, LineEndCap endCaps, LineGeometry geometry, bool zTest) {
			Lines.Enqueue(new DrawLine() {
				start = start,
				end = end,
				startColor = startColor,
				endColor = endColor,
				thickness = thickness,
				endCaps = endCaps,
				geometry = geometry,
				zTest = zTest
			});
		}

		public static void Disc(Vector3 pos, Vector3 normal, bool softFill = false, bool zTest = true) => Disc(pos, normal, 1f, softFill, zTest);
		public static void Disc(Vector3 pos, Vector3 normal, float radius, bool softFill = false, bool zTest = true) => Disc(pos, normal, radius, Color.white, softFill, zTest);
		public static void Disc(Vector3 pos, Vector3 normal, float radius, Color32 color,  bool softFill = false, bool zTest = true) => Disc(pos, normal, radius, color, color, softFill, zTest);
		public static void Disc(Vector3 pos, Vector3 normal, float radius, Color32 startColor, Color32 endColor, bool softFill = false, bool zTest = true) => Disc(pos, normal, radius, startColor, endColor, DiscGeometry.Flat2D, softFill, zTest);
		public static void Disc(Vector3 pos, Vector3 normal, float radius, Color32 startColor, Color32 endColor, DiscGeometry geometry, bool softFill = false, bool zTest = true) {
			Discs.Enqueue(new DrawDisc() {
				pos = pos,
				normal = normal,
				startColor = startColor,
				endColor = endColor,
				geometry = geometry,
				radius = radius,
				softFill = softFill,
				zTest = zTest
			});
		}

		public static void Ring(Vector3 pos, Vector3 normal, float thickness = 1f, bool softFill = false, bool zTest = true) => Ring(pos, normal, 1f, thickness, softFill, zTest);
		public static void Ring(Vector3 pos, Vector3 normal, float radius, float thickness, bool softFill = false, bool zTest = true) => Ring(pos, normal, radius, thickness, Color.black, Color.black, softFill, zTest);
		public static void Ring(Vector3 pos, Vector3 normal, float radius, float thickness, Color32 color, bool softFill = false, bool zTest = true) => Ring(pos, normal, radius, thickness, color, color, softFill, zTest);
		public static void Ring(Vector3 pos, Vector3 normal, float radius, float thickness, Color32 startColor, Color32 endColor, bool softFill = false, bool zTest = true) => Ring(pos, normal, radius, thickness, startColor, endColor, DiscGeometry.Flat2D, softFill, zTest);
		public static void Ring(Vector3 pos, Vector3 normal, float radius, float thickness, Color32 startColor, Color32 endColor, DiscGeometry geometry, bool softFill = false, bool zTest = true) {
			Rings.Enqueue(new DrawRing() {
				pos = pos,
				normal = normal,
				startColor = startColor,
				endColor = endColor,
				geometry = geometry,
				zTest = zTest,
				radius = radius,
				thickness = thickness,
				softFill = softFill
			});
		}

		public static void Text(Vector3 pos, string text, bool zTest = false) => Text(pos, text, 14, zTest);
		public static void Text(Vector3 pos, string text, float fontSize, bool zTest = false) => Text(pos, text, fontSize, Color.white, zTest);
		public static void Text(Vector3 pos, string text, float fontSize, Color color, bool zTest = false) => Text(pos, (pos - Camera.main.transform.position).normalized, text, fontSize, color, zTest);
		public static void Text(Vector3 pos, Vector3 normal, string text, bool zTest = false) => Text(pos, normal, text, 14, zTest);
		public static void Text(Vector3 pos, Vector3 normal, string text, float fontSize, bool zTest = false) => Text(pos, normal, text, fontSize, Color.white, zTest);
		public static void Text(Vector3 pos, Vector3 normal, string text, float fontSize, Color color, bool zTest = false) {
			Texts.Enqueue(new DrawText() {
				pos = pos,
				normal = normal,
				color = color,
				fontSize = fontSize,
				text = text,
				zTest = zTest
			});
		}

		public static void SetZTest(bool state) {
			if(state) {
				Shapes.Draw.ZTest = UnityEngine.Rendering.CompareFunction.LessEqual;
			} else {
				Shapes.Draw.ZTest = UnityEngine.Rendering.CompareFunction.Always;
			}
		}
		public static bool IsTransparent(Color32 col2) => col2.a < 255;
		public static DiscColors CreateDiscColor(Color32 mainColor, Color32 secondaryColor) => CreateDiscColor(mainColor, secondaryColor, false, true);
		public static DiscColors CreateDiscColor(Color32 mainColor, Color32 secondaryColor, bool isRadial) => CreateDiscColor(mainColor, secondaryColor, isRadial, false);
		static DiscColors CreateDiscColor(Color32 mainColor, Color32 secondaryColor, bool IsRadial, bool IsAngular) {
			DiscColors colors;
			if(!IsRadial && !IsAngular) {
				colors = DiscColors.Flat(mainColor);
			} else if(IsRadial) {
				colors = DiscColors.Radial(mainColor, secondaryColor);
			} else if(IsAngular) {
				colors = DiscColors.Angular(mainColor, secondaryColor);
			} else {
				colors = DiscColors.Flat(mainColor);
			}
			return colors;
		}

		struct DrawLine {
			public Vector3 start;
			public Vector3 end;
			public Color32 startColor;
			public Color32 endColor;
			public LineEndCap endCaps;
			public LineGeometry geometry;
			public float thickness;
			public bool zTest;
		}
		struct DrawDisc {
			public Vector3 pos;
			public Vector3 normal;
			public Color32 startColor;
			public Color32 endColor;
			public DiscGeometry geometry;
			public bool zTest;
			public bool softFill;
			public float radius;
		}
		struct DrawRing {
			public Vector3 pos;
			public Vector3 normal;
			public Color32 startColor;
			public Color32 endColor;
			public DiscGeometry geometry;
			public float radius;
			public float thickness;
			public bool zTest;
			public bool softFill;
		}
		struct DrawText {
			public Vector3 pos;
			public Vector3 normal;
			public Color32 color;
			public float fontSize;
			public string text;
			public bool zTest;
		}
	}
}
