using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PieChart : MonoBehaviour 
{
	public Transform graph;
	public Transform panel;
	public Transform variablesPanel;
	public Text heading;

	public float[] data;
	public string[] dataNames;
	public Color[] wedgeColors;
	public Image wedgePrefab;
	public GameObject variablePrefab;

	private void Start()
	{
		Draw("Bölümler ve Başarı Yüzdeleri", data, dataNames);
	}

	public void Draw(string title, float[] data, string[] dataNames)
	{
		/* Variables for the UI components */
		float total = 0.0f;
		float ZRotationOfWedge = 0.0f;
		float YPositionOfVariable = 322f;
		Vector3 rotation = Vector3.zero;

		heading.text = title.ToUpper();

		/* Calculate the total data value */
		for (int i = 0; i < data.Length; i++) 
		{
			total += data[i];
		}

		/* Create new wedge for the pie chart and evaluate its attributes */
		for (int i = 0; i < data.Length; i++) 
		{
			Image newWedge = (Image)Instantiate(wedgePrefab);

			newWedge.transform.SetParent(graph.transform, false);
			newWedge.color = wedgeColors[i];
			newWedge.fillAmount = data[i] / total;

			rotation = new Vector3(0.0f, 0.0f, ZRotationOfWedge);
			newWedge.transform.rotation = Quaternion.Euler(rotation);

			/* Rotation at Z axis for the next wedge */
			ZRotationOfWedge -= newWedge.fillAmount * 360.0f;

			GameObject newVariable = (GameObject)Instantiate(variablePrefab);

			newVariable.transform.SetParent(variablesPanel, false);
			newVariable.transform.position += Vector3.up * YPositionOfVariable;

			newVariable.GetComponentInChildren<Image>().color = wedgeColors[i];
			newVariable.GetComponentInChildren<Text>().text = dataNames[i] + ": " + data[i] + " (% " + (data[i] / total * 100f).ToString("#.##") + ")";

			YPositionOfVariable -= 128f;
		}
	}
}
