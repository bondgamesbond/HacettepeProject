using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BarChart : MonoBehaviour {

    public Bar barPrefab;
    public string[] labels;
    public Color[] colors;
    public Text  xAxis;
    public Text yAxis;
    int[] valx = new int[10];
    int[] valy = new int[10];
    List<Bar> bars = new List<Bar>();
    float chartHeight;
	// Use this for initialization
	void Start () {
        for (int i = 0; i < valx.Length; i++)
        {
            valx[i] = (int)Random.Range(1,9);
        }
        for (int i = 0; i < valy.Length; i++)
        {
            valy[i] = (int)Random.Range(0, 9);
        }
        chartHeight = GetComponent<RectTransform>().sizeDelta.y;
        displayGraph("Bölümler" , valx, "Başarı Yüzdesi", valy);
	}
    void displayGraph(string name,int[] valsx, string name1, int[] valsy)
    {
        int maxValue = valsx.Max();
        xAxis.text = name;
        yAxis.text = name1;
        for (int i = 0; i < valsx.Length; i++)
        {
            Bar newBar = Instantiate(barPrefab) as Bar;
            newBar.transform.SetParent(transform);
            RectTransform rt = newBar.bar.GetComponent<RectTransform>();
            float normalizedValue = ((float)valsy[i] / (float)maxValue) * 0.95f;
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, chartHeight* normalizedValue);
            newBar.bar.color = colors[i % colors.Length];                        
            newBar.label.text = "Bölüm " + (i + 1);
            if(newBar.barValue.text != "" +0)
                newBar.barValue.text = "% " + valsy[i] + "0";
            else
                newBar.barValue.text = "";

        }
      

    }
	// Update is called once per frame
	void Update () {
	
	}
}
