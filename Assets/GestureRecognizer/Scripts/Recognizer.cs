using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

/// <summary>
/// Class responsible for recognize a drawing in a list of gestures patterns.
/// </summary>

public class Recognizer : MonoBehaviour {
	
	public int drawingDetail = 100;

	public List<GesturePattern> patterns;

	public RecognitionResult Recognize(List<Vector2> points){
		
		var normPoints = NormalizeDistribution (NormalizeScale(points), drawingDetail);

		var found = findPattern(normPoints);

		return found;
	}

	public struct Score{
		public float positionDistance;
		public float curvatureDistance;
		public float angleDistance;
		public float score {
			get{ 
				float posScore = Mathf.Clamp01(1f - positionDistance/30);
				float curvScore = Mathf.Clamp01 (1f - curvatureDistance / 20);
				float angleScore = Mathf.Clamp01(1f - angleDistance/50);
				var median = new List<float> (){ posScore, curvScore, angleScore }.OrderBy(e => e).ElementAt(1);
				return median;
			}
		}

		public void InitMax(){
			positionDistance = curvatureDistance = angleDistance = float.MaxValue;
		}

		public static bool operator >(Score s1, Score s2){
			return s1.score > s2.score;
		}
		public static bool operator <(Score s1, Score s2){
			return s1.score < s2.score;
		}
		public static bool operator >=(Score s1, Score s2){
			return s1.score >= s2.score;
		}
		public static bool operator <=(Score s1, Score s2){
			return s1.score <= s2.score;
		}
	}

	public struct RecognitionResult{
		public GesturePattern pattern;
		public Score score;
	}

	private string currentPatternId;
	private string debugResult = "";

	RecognitionResult findPattern(List<Vector2> normPoints){
		var bestGesture = default(GesturePattern);

		var maxScore = new Score ();
		maxScore.InitMax ();

		debugResult = "";

		for (int i = 0; i < patterns.Count; i++) {
			foreach (var inverted in new bool[]{false,true}) {
				
				currentPatternId = (inverted?"-":"") + patterns [i].id;

				var score = inverted ? 
					CalcScore (NormalizeDistribution (patterns [i].points.AsEnumerable().Reverse().ToList(), drawingDetail), normPoints)
					:
					CalcScore (NormalizeDistribution (patterns [i].points, drawingDetail), normPoints);
				
				if (score > maxScore) {
					maxScore = score;
					bestGesture = patterns [i];
				}
			}
		}

		//Debug all gestures scores
		//print (debugResult);

		return new RecognitionResult(){pattern = bestGesture, score = maxScore};
	}

	List<float> CalcAngles(List<Vector2> points){
		int step = 10;
		List<float> result = new List<float> ();

		for (int i = 0; i < points.Count; i++) {
			int i1 = Mathf.Max (i - step, 0);
			int i2 = Mathf.Min (i + step, points.Count - 1);
			var v1 = points [i1];
			var v2 = points [i2];
			var dir = v2 - v1;
			var angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
			result.Add (angle);
		}
		return result;
	}

	List<float> CalcCurvature(List<Vector2> points){
		int step = 10;
		List<float> result = new List<float> ();
		for (int i = 0; i < step; i++)
			result.Add (0);
		for (int i = step; i < points.Count-step; i++) {
			var p1 = points [i - step];
			var p2 = points [i];
			var p3 = points [i + step];
			var v1 = p2 - p1;
			var v2 = p3 - p2;
			var angle1 = Mathf.Atan2 (v1.y, v1.x) * Mathf.Rad2Deg;
			var angle2 = Mathf.Atan2 (v2.y, v2.x) * Mathf.Rad2Deg;
			var angle = Mathf.DeltaAngle (angle1, angle2);
			result.Add (angle);
		}
		for (int i = 0; i < step; i++)
			result.Add (0);
		return result;
	}

	Score CalcScore(List<Vector2> points1, List<Vector2> points2){

		float posDist = CalcPositionDistance (points1, points2);
		float curvDist = CalcCurvatureDistance (points1, points2);
		float angleDist = CalcAngleDistance (points1, points2);

		debugResult += string.Format ("{0:00.00}\t{1:00.00}\t{2:00.00}\t{3}\n", posDist, curvDist, angleDist, currentPatternId);

		return new Score (){ positionDistance = posDist, curvatureDistance = curvDist, angleDistance = angleDist };
	}


	float CalcPositionDistance(List<Vector2> points1, List<Vector2> points2){
		
		int n = points1.Count;

		float sumDistance = 0;
		for (int i = 0; i < n; i++) {
			float dif = Vector2.Distance (points1 [i], points2 [i]);
			sumDistance += dif;
		}

		return sumDistance;
	}

	float CalcCurvatureDistance(List<Vector2> points1, List<Vector2> points2){

		int n = points1.Count;

		var curv1 = CalcCurvature (points1).Select( e => Mathf.Lerp(-1f,1f,Mathf.InverseLerp(-180,180,e))).ToList() ;
		var curv2 = CalcCurvature (points2).Select( e => Mathf.Lerp(-1f,1f,Mathf.InverseLerp(-180,180,e))).ToList() ;

		float sumCurvDistance = 0;
		for (int i = 0; i < n; i++) {
			float dif = Mathf.Abs (curv1 [i] - curv2 [i]) / 2f;

			sumCurvDistance += dif;
		}

		return sumCurvDistance;
	}

	float CalcAngleDistance(List<Vector2> points1, List<Vector2> points2){

		int n = points1.Count;

		var angles1 = CalcAngles (points1);
		var angles2 = CalcAngles (points2);

		float sumAngleDistance = 0;

		for (int i = 0; i < n; i++) {
			float dif = Mathf.Abs(Mathf.DeltaAngle (angles1 [i], angles2 [i])) / 180f;
			sumAngleDistance += dif;
		}

		return sumAngleDistance;
	}

	List<Vector2> NormalizeScale(List<Vector2> points){
		float minx, miny, maxx, maxy;
		minx = maxx = points [0].x;
		miny = maxy = points [0].y;
		for (int i = 1; i < points.Count; i++) {
			var p = points [i];
			minx = Mathf.Min (minx, p.x);
			maxx = Mathf.Max (maxx, p.x);
			miny = Mathf.Min (miny, p.y);
			maxy = Mathf.Max (maxy, p.y);
		}
		Rect rect = Rect.MinMaxRect (minx, miny, maxx, maxy);
		float rectsize = Mathf.Max (rect.width, rect.height);
		rect = new Rect (rect.center - new Vector2 (rectsize / 2, rectsize / 2), new Vector2 (rectsize, rectsize));

		return points.Select (e => Rect.PointToNormalized(rect, e)).ToList ();
	}

	Vector2 FindByNormalized(List<Vector2> vs, List<float> ts, float t){
		for (int i = 0; i < ts.Count-1; i++) {
			var t1 = ts [i];
			var t2 = ts [i + 1];
			if (t1 <= t && t <= t2) {
				var v1 = vs [i];
				var v2 = vs [i + 1];
				float tt = Mathf.InverseLerp (t1, t2, t);
				return Vector2.Lerp (v1, v2, tt);
			}
		}
		return t > 0.5f ? vs [vs.Count - 1] : vs [0];
	}

	private List<Vector2> NormalizeDistribution(List<Vector2> path, int n) {

		List<float> realPos = new List<float> ();

		realPos.Add (0);
		for (int i = 1; i < path.Count; i++) {
			var v1 = path [i-1];
			var v2 = path [i];
			realPos.Add (realPos [i - 1] + Vector2.Distance (v1, v2));
		}

		float totalDist = realPos.Last ();

		var normPos = realPos.Select (e => e / totalDist).ToList ();

		var result = new List<Vector2> ();

		for (int ti = 0; ti <= n; ti++) {
			float t = (float)ti / n;
			result.Add(FindByNormalized(path, normPos, t));
		}

		return result;
	}

}
