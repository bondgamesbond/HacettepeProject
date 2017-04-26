using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script do store "gesture".
/// </summary>

[CreateAssetMenu(menuName = "Gestures/Pattern")]
public class GesturePattern : ScriptableObject {

	public string id;
	public List<Vector2> points;

}
