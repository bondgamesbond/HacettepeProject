using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Script to edit a "gesture".
/// </summary>

[CustomEditor(typeof(GesturePattern))]
public class GesturePatternEditor : Editor{

	SerializedProperty points;
	void OnEnable(){
		points = serializedObject.FindProperty ("points");
	}

	int dragIndex;

	int FindNear(SerializedProperty array, Vector2 pos){
		int nearIndex = 0;
		float minDist = Vector2.Distance(array.GetArrayElementAtIndex(0).vector2Value, pos);
		for (int i = 1; i < array.arraySize; i++) {
			var dist = Vector2.Distance(array.GetArrayElementAtIndex(i).vector2Value, pos);
			if (dist < minDist) {
				minDist = dist;
				nearIndex = i;
			}
		}
		return nearIndex;
	}

	public void RoundDrawing(){
		int n = 16;
		for (int i = 0; i < points.arraySize; i++) {
			var v = points.GetArrayElementAtIndex (i).vector2Value;
			v.x = Mathf.Round(v.x * n) / n;
			v.y = Mathf.Round (v.y * n) / n;
			points.GetArrayElementAtIndex (i).vector2Value = v;
		}
	}
	public void NegYDrawing(){
		for (int i = 0; i < points.arraySize; i++) {
			var e = points.GetArrayElementAtIndex (i);
			var v = e.vector2Value;
			v.y = -v.y;
			e.vector2Value = v;
		}
	}
	public void NormalizeDrawing(){
		Vector2 min = points.GetArrayElementAtIndex (0).vector2Value;
		Vector2 max = min;
		for (int i = 1; i < points.arraySize; i++) {
			var v = points.GetArrayElementAtIndex (i).vector2Value;
			min.x = Mathf.Min (min.x, v.x);
			min.y = Mathf.Min (min.y, v.y);
			max.x = Mathf.Max (max.x, v.x);
			max.y = Mathf.Max (max.y, v.y);
		}

		Rect rect = Rect.MinMaxRect(min.x, min.y, max.x, max.y);

		float rectsize = Mathf.Max (rect.width, rect.height);
		rect = new Rect (rect.center - new Vector2 (rectsize / 2, rectsize / 2), new Vector2 (rectsize, rectsize));

		for (int i = 0; i < points.arraySize; i++) {
			var element = points.GetArrayElementAtIndex (i);
			element.vector2Value = Rect.PointToNormalized (rect, element.vector2Value);
		}
	}

	Vector2 NegY1(Vector2 v){
		return new Vector2 (v.x, 1f - v.y);
	}


	public override void OnInspectorGUI () {
		DrawDefaultInspector ();

		this.serializedObject.Update ();

		float w = EditorGUIUtility.currentViewWidth * 0.75f;
		w = 201;

		var rect = EditorGUILayout.GetControlRect (GUILayout.Width (w), GUILayout.Height (w));
		//Debug.Log (rect);
		GUI.Box (rect, GUIContent.none);
		Handles.DrawSolidRectangleWithOutline (rect, Color.gray, Color.gray);

		Handles.color = new Color(0f, 0f, 0f, 0.2f);
		Handles.DrawLine (new Vector3(rect.xMin, rect.yMin), new Vector3(rect.xMax, rect.yMax));
		Handles.DrawLine (new Vector3(rect.xMin, rect.yMax), new Vector3(rect.xMax, rect.yMin));
		Handles.DrawLine (new Vector3(rect.xMin, rect.center.y), new Vector3(rect.xMax, rect.center.y));
		Handles.DrawLine (new Vector3(rect.center.x, rect.yMin), new Vector3(rect.center.x, rect.yMax));
		Handles.DrawLine (new Vector3(Mathf.Lerp(rect.xMin, rect.xMax, 0.25f), rect.yMin), new Vector3(Mathf.Lerp(rect.xMin, rect.xMax, 0.25f), rect.yMax));
		Handles.DrawLine (new Vector3(Mathf.Lerp(rect.xMin, rect.xMax, 0.75f), rect.yMin), new Vector3(Mathf.Lerp(rect.xMin, rect.xMax, 0.75f), rect.yMax));
		Handles.DrawLine (new Vector3(rect.xMin, Mathf.Lerp(rect.yMin, rect.yMax, 0.25f)), new Vector3(rect.xMax, Mathf.Lerp(rect.yMin, rect.yMax, 0.25f)));
		Handles.DrawLine (new Vector3(rect.xMin, Mathf.Lerp(rect.yMin, rect.yMax, 0.75f)), new Vector3(rect.xMax, Mathf.Lerp(rect.yMin, rect.yMax, 0.75f)));

		if (points.arraySize >= 2) {
			if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && rect.Contains (Event.current.mousePosition)) {
				var pos = NegY1(Rect.PointToNormalized(rect, Event.current.mousePosition));
				dragIndex = FindNear (points, pos);
				points.GetArrayElementAtIndex (dragIndex).vector2Value = pos;
			}
			if (Event.current.type == EventType.MouseDrag && Event.current.button == 1 && rect.Contains (Event.current.mousePosition)) {
				var pos = NegY1(Rect.PointToNormalized(rect, Event.current.mousePosition));
				points.GetArrayElementAtIndex (dragIndex).vector2Value = pos;
			}
			if (Event.current.type == EventType.MouseUp && Event.current.button == 1) {
				dragIndex = -1;
			}
		}

		if (Event.current.type == EventType.mouseDown && Event.current.button == 0 && rect.Contains (Event.current.mousePosition)) {
			var pos = NegY1(Rect.PointToNormalized(rect, Event.current.mousePosition));
			points.InsertArrayElementAtIndex (points.arraySize);
			points.GetArrayElementAtIndex (points.arraySize - 1).vector2Value = pos;
		}
		/*
		if (Event.current.type == EventType.MouseDrag && Event.current.button == 0 && rect.Contains (Event.current.mousePosition)) {
			var pos = Normalize (rect, Event.current.mousePosition);
			points.InsertArrayElementAtIndex (points.arraySize);
			points.GetArrayElementAtIndex (points.arraySize - 1).vector2Value = pos;
		}
		*/

		Handles.color = Color.cyan;
		for (int i = 0; i < points.arraySize-1; i++) {
			var v1 = points.GetArrayElementAtIndex (i).vector2Value;
			var v2 = points.GetArrayElementAtIndex (i+1).vector2Value;
			v1 = Rect.NormalizedToPoint (rect, NegY1(v1));
			v2 = Rect.NormalizedToPoint (rect, NegY1(v2));
			Handles.DrawLine (v1, v2);
		}

		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Clear")) {
			points.ClearArray ();
		}
		if (GUILayout.Button ("Del Last")) {
			points.DeleteArrayElementAtIndex (points.arraySize - 1);
		}
		if (GUILayout.Button ("Normalize")) {
			NormalizeDrawing();
		}
		if (GUILayout.Button ("Round")) {
			RoundDrawing ();
		}
		if (GUILayout.Button ("-y")) {
			NegYDrawing ();
		}
		GUILayout.EndHorizontal ();

		serializedObject.ApplyModifiedProperties ();
	}

}