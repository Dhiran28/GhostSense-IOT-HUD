using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveformController : MonoBehaviour
{
    [Header("Line Renderer")]
    public LineRenderer lineRenderer;

    [Header("Graph Settings")]
    public int maxPoints = 50;
    public float graphWidth = 220f;
    public float graphHeight = 50f;
    public float maxGasValue = 5000f;

    [Header("Colors")]
    public Color cleanColor = new Color(0.00f, 1.00f, 0.53f);
    public Color warningColor = new Color(1.00f, 0.70f, 0.00f);
    public Color dangerColor = new Color(1.00f, 0.23f, 0.19f);

    private Queue<float> _history = new Queue<float>();
    private RectTransform _rect;

    void Start()
    {
        _rect = GetComponent<RectTransform>();
        lineRenderer = GetComponent<LineRenderer>();

        // Pre-fill with zeros
        for (int i = 0; i < maxPoints; i++)
            _history.Enqueue(0f);

        lineRenderer.positionCount = maxPoints;
        lineRenderer.useWorldSpace = false;
    }

    public void AddValue(float gasPPM)
    {
        if (_history.Count >= maxPoints)
            _history.Dequeue();
        _history.Enqueue(gasPPM);
        RedrawLine(gasPPM);
    }

    void RedrawLine(float latest)
    {
        float[] vals = new float[_history.Count];
        _history.CopyTo(vals, 0);

        lineRenderer.positionCount = vals.Length;

        float width = _rect != null ? _rect.rect.width : graphWidth;
        float height = _rect != null ? _rect.rect.height : graphHeight;

        for (int i = 0; i < vals.Length; i++)
        {
            float x = (i / (float)(vals.Length - 1))
                      * width - width / 2f;
            float y = (vals[i] / maxGasValue)
                      * height - height / 2f;
            lineRenderer.SetPosition(i, new Vector3(x, y, -1f));
        }

        // Color by level
        int level = SensorManager.Instance != null
                    ? SensorManager.Instance.gasLevel : 0;

        Color c = level == 0 ? cleanColor :
                  level == 1 ? warningColor :
                  dangerColor;

        lineRenderer.startColor = c;
        lineRenderer.endColor = c;
    }
}