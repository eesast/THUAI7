using UnityEngine;
using XCharts.Runtime;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Properties;
using Unity.VisualScripting;

public class ChartControl : SingletonMono<ChartControl>
{
    public enum ChartMode
    {
        Abridge,
        Detailed,
    };
    public ChartMode chartMode;
    public enum DataMode
    {
        Energy,
        Score,
    };
    public DataMode dataMode;
    // Button changeButton;
    TextMeshProUGUI changeText;
    ScatterChart chart;
    public int chartStep;
    RectTransform rectTransform;
    YAxis yAxis;
    public List<Tuple<int, int>> score1 = new List<Tuple<int, int>>(), energy1 = new List<Tuple<int, int>>();
    public List<Tuple<int, int>> score2 = new List<Tuple<int, int>>(), energy2 = new List<Tuple<int, int>>();
    void Start()
    {
        chartMode = ChartMode.Abridge;
        dataMode = DataMode.Energy;
        chart = GetComponent<ScatterChart>();
        yAxis = chart.EnsureChartComponent<YAxis>();
        // changeButton = transform.Find("ChangeShow").GetComponent<Button>();
        changeText = transform.Find("ChangeShow").Find("ChangeText").GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
        GetAllInfo();
    }
    void GetAllInfo()
    {
        // score
    }
    void Update()
    {
        UpdateData();
    }
    void UpdateData()
    {
        switch (chartMode)
        {
            case ChartMode.Abridge:
                switch (dataMode)
                {
                    case DataMode.Energy:
                        chart.UpdateData(0, 0, 0, 1.01);
                        chart.UpdateData(0, 0, 1, CoreParam.currentFrame.AllMessage.RedTeamEnergy);
                        chart.UpdateData(1, 0, 0, 1.01);
                        chart.UpdateData(1, 0, 1, CoreParam.currentFrame.AllMessage.BlueTeamEnergy);
                        break;
                    case DataMode.Score:
                        chart.UpdateData(0, 0, 0, 1.01);
                        chart.UpdateData(0, 0, 1, CoreParam.currentFrame.AllMessage.RedTeamScore);
                        chart.UpdateData(1, 0, 0, 1.01);
                        chart.UpdateData(1, 0, 1, CoreParam.currentFrame.AllMessage.BlueTeamScore);
                        break;
                }
                break;
            case ChartMode.Detailed:
                switch (dataMode)
                {
                    case DataMode.Energy:
                        chart.UpdateData(0, 0, 0, CoreParam.currentFrame.AllMessage.GameTime);
                        chart.UpdateData(0, 0, 1, CoreParam.currentFrame.AllMessage.RedTeamEnergy);
                        chart.UpdateData(1, 0, 0, CoreParam.currentFrame.AllMessage.GameTime);
                        chart.UpdateData(1, 0, 1, CoreParam.currentFrame.AllMessage.BlueTeamEnergy);
                        break;
                    case DataMode.Score:
                        chart.UpdateData(0, 0, 0, CoreParam.currentFrame.AllMessage.GameTime);
                        chart.UpdateData(0, 0, 1, CoreParam.currentFrame.AllMessage.RedTeamScore);
                        chart.UpdateData(1, 0, 0, CoreParam.currentFrame.AllMessage.GameTime);
                        chart.UpdateData(1, 0, 1, CoreParam.currentFrame.AllMessage.BlueTeamScore);
                        break;
                }
                break;
        }
    }
    public void ChangeDataMode()
    {
        switch (dataMode)
        {
            case DataMode.Energy:
                dataMode = DataMode.Score;
                yAxis.axisName.name = "score";
                changeText.text = "Show Energy";
                switch (chartMode)
                {
                    case ChartMode.Abridge:
                        break;
                    case ChartMode.Detailed:
                        chart.ClearData();
                        for (int i = 0; i < score1.Count; i += chartStep)
                            chart.AddData(2, score1[i].Item1, score1[i].Item2);
                        for (int i = 0; i < score2.Count; i += chartStep)
                            chart.AddData(3, score2[i].Item1, score2[i].Item2, "id");
                        chart.AddData(0, CoreParam.currentFrame.AllMessage.GameTime, CoreParam.currentFrame.AllMessage.RedTeamScore);
                        chart.AddData(1, CoreParam.currentFrame.AllMessage.GameTime, CoreParam.currentFrame.AllMessage.BlueTeamScore);
                        break;
                }
                break;
            case DataMode.Score:
                dataMode = DataMode.Energy;
                yAxis.axisName.name = "energy";
                changeText.text = "Show Score";
                switch (chartMode)
                {
                    case ChartMode.Abridge:
                        break;
                    case ChartMode.Detailed:
                        chart.ClearData();
                        for (int i = 0; i < energy1.Count; i += chartStep)
                            chart.AddData(2, energy1[i].Item1, energy1[i].Item2);
                        for (int i = 0; i < energy2.Count; i += chartStep)
                            chart.AddData(3, energy2[i].Item1, energy2[i].Item2);
                        chart.AddData(0, CoreParam.currentFrame.AllMessage.GameTime, CoreParam.currentFrame.AllMessage.RedTeamEnergy);
                        chart.AddData(1, CoreParam.currentFrame.AllMessage.GameTime, CoreParam.currentFrame.AllMessage.BlueTeamEnergy);
                        break;
                }
                break;
        }
    }
    public void ChangechartMode()
    {
        switch (chartMode)
        {
            case ChartMode.Abridge:
                chartMode = ChartMode.Detailed;
                StopAllCoroutines();
                StartCoroutine(ChangeSize(new Vector2(1000, 480)));
                chart.ClearData();
                switch (dataMode)
                {
                    case DataMode.Energy:
                        for (int i = 0; i < energy1.Count; i += chartStep)
                            chart.AddData(2, energy1[i].Item1, energy1[i].Item2);
                        for (int i = 0; i < energy2.Count; i += chartStep)
                            chart.AddData(3, energy2[i].Item1, energy2[i].Item2);
                        chart.AddData(0, CoreParam.currentFrame.AllMessage.GameTime, CoreParam.currentFrame.AllMessage.RedTeamEnergy);
                        chart.AddData(1, CoreParam.currentFrame.AllMessage.GameTime, CoreParam.currentFrame.AllMessage.BlueTeamEnergy);
                        break;
                    case DataMode.Score:
                        for (int i = 0; i < score1.Count; i += chartStep)
                            chart.AddData(2, score1[i].Item1, score1[i].Item2);
                        for (int i = 0; i < score2.Count; i += chartStep)
                            chart.AddData(3, score2[i].Item1, score2[i].Item2);
                        chart.AddData(0, CoreParam.currentFrame.AllMessage.GameTime, CoreParam.currentFrame.AllMessage.RedTeamScore);
                        chart.AddData(1, CoreParam.currentFrame.AllMessage.GameTime, CoreParam.currentFrame.AllMessage.BlueTeamScore);
                        break;
                }
                break;
            case ChartMode.Detailed:
                chartMode = ChartMode.Abridge;
                StopAllCoroutines();
                StartCoroutine(ChangeSize(new Vector2(190, 360)));
                chart.ClearData();
                switch (dataMode)
                {
                    case DataMode.Energy:
                        chart.AddData(0, CoreParam.currentFrame.AllMessage.RedTeamEnergy);
                        chart.AddData(1, CoreParam.currentFrame.AllMessage.BlueTeamEnergy);
                        break;
                    case DataMode.Score:
                        chart.AddData(0, CoreParam.currentFrame.AllMessage.RedTeamScore);
                        chart.AddData(1, CoreParam.currentFrame.AllMessage.BlueTeamScore);
                        break;
                }
                break;
        }
    }
    IEnumerator ChangeSize(Vector2 target)
    {
        Vector2 curTarget;
        float step = 0.04f;
        while (true)
        {
            curTarget = Vector2.Lerp(new Vector2(rectTransform.rect.width, rectTransform.rect.height), target, step);
            step = Mathf.Lerp(step, 0.2f, 0.05f);
            if ((curTarget - target).magnitude < 1f)
                break;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, curTarget.x);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, curTarget.y);
            yield return 0;
        }
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, target.x);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, target.y);
        yield return 0;
    }
}