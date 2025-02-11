using System;
using System.Diagnostics;
using UnityEngine;

namespace Sperlich.Drawing {
	[ExecuteAlways]
	public class DrawOptimizer : MonoBehaviour {

		public int drawLines;
		public int drawSpheres;
		public int drawCubes;
		public int drawCones;
		public int drawRects;
		public int drawDiscs;
		public int drawRings;
		public int drawTriangles;

		[Header("Benchmark")]
		public float frameTime;
		public float oldInit;
		public float newInit;

		Stopwatch watch = new Stopwatch();
		DateTime before;

		void OnDrawGizmos() {
			frameTime = (float)(DateTime.Now - before).TotalMilliseconds;
			before = DateTime.Now;
			BenchmarkNew();
		}

		void BenchmarkNew() {
			watch.Restart();
			UnityEngine.Random.InitState(0);

			for (int i = 0; i < drawCubes; i++) {
				Draw.Box(new Vector3(i, 0, 3), new Vector3(0f, 1f, UnityEngine.Random.Range(-1f, 1f)).normalized, new Vector3(0.5f, 1f, 0.5f), GetRainbowColorFromIndex(i));
			}
			for (int i = 0; i < drawCones; i++) {
				Draw.Cone(new Vector3(i, 0, 5), new Vector3(0f, 1f, UnityEngine.Random.Range(-1f, 1f)).normalized, 1f, GetRainbowColorFromIndex(i));
			}
			for (int i = 0; i < drawSpheres; i++) {
				Draw.Sphere(new Vector3(i, 0, 7), 1f, GetRainbowColorFromIndex(i));
			}
			for (int i = 0; i < drawRects; i++) {
				Draw.Rectangle(new Vector3(i, 0, 9), new Vector3(0f, 1f, UnityEngine.Random.Range(-1f, 1f)).normalized, 0.7f, 1f, GetRainbowColorFromIndex(i), Draw.Remap(i, 0, drawRects, 0f, 1f));
			}
			for (int i = 0; i < drawDiscs; i++) {
				Draw.Disc(new Vector3(i, 0, 11), new Vector3(0f, 1f, UnityEngine.Random.Range(-1f, 1f)).normalized, 1f, Color.clear, GetRainbowColorFromIndex(i), true);
			}
			for (int i = 0; i < drawRings; i++) {
				Draw.Ring(new Vector3(i, 0, 13), new Vector3(0f, 1f, UnityEngine.Random.Range(-1f, 1f)).normalized, 1f, 0.25f, GetRainbowColorFromIndex(i), i % 2 == 0 ? GetRainbowColorFromIndex(i) : Color.clear);
			}
			for (int i = 0; i < drawTriangles; i++) {
				Draw.FlatTriangle(new Vector3(i, 0, 15), (i % 16) * 22.5f, 1f, GetRainbowColorFromIndex(i), 2, 0.2f);
			}
			for (int i = 0; i < drawTriangles; i++) {
				Draw.Triangle(new Vector3(i, 0, 17), new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized, 1f, GetRainbowColorFromIndex(i), 0f);
			}
			watch.Stop();
			newInit = (float)watch.Elapsed.TotalMilliseconds;
		}
		Color GetRainbowColorFromIndex(int i) => Color.HSVToRGB(Draw.Remap(Mathf.Sin(i / 25f), -1f, 1f, 0f, 1f), 1, 1);

	}
}