using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using System.Linq;

/// <summary>
/// Renderer to automatic draw a gesture on Canvas
/// </summary>

public class GesturePatternDraw : UILineRenderer {

	public GesturePattern pattern;

	protected override void OnPopulateMesh (UnityEngine.UI.VertexHelper vh) {
		DrawPattern ();
		base.OnPopulateMesh (vh);
	}

	public void DrawPattern(){
		
		var size = this.rectTransform.sizeDelta;

		Rect rect = this.rectTransform.rect;
		rect.center += rect.size / 2;

		var patternPoints = pattern.points
			.Select (e => Rect.NormalizedToPoint (rect, e))
			.ToArray ();

		this.Points = patternPoints;

	}
}

