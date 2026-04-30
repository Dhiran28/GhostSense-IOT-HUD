// HUDController.cs
// Attach to HUD_Canvas
// Drag UI objects into Inspector fields

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    [Header("Gas Panel")]
    public TMP_Text gasValueText;
    public TMP_Text gasStatusText;
    public Image gasRingImage;
    public Image gasBarImage;
    public GameObject dangerBanner;

    [Header("Environment Panel")]
    public TMP_Text tempText;
    public TMP_Text humText;

    [Header("Centre Status Ring")]
    public Image statusRingImage;
    public TMP_Text statusLabelText;

    [Header("Colours")]
    public Color colSafe = new Color(0.24f, 1.00f, 0.63f);
    public Color colMedium = new Color(1.00f, 0.75f, 0.25f);
    public Color colHigh = new Color(1.00f, 0.30f, 0.43f);

    static readonly string[] LABELS =
        { "CLEAN AIR", "LOW GAS", "MEDIUM GAS", "HIGH GAS  ⚠" };

    private SensorManager _sm;
    private float _blinkT;

    void Start() => _sm = SensorManager.Instance;

    void Update()
    {
        if (_sm == null) return;
        DoGas();
        DoEnv();
        DoRing();
        DoBlink();
    }

    void DoGas()
    {
        Color c = GetColor(_sm.gasLevel);
        if (gasValueText)
        {
            gasValueText.text = _sm.gasPPM > 999
                ? (_sm.gasPPM / 1000f).ToString("F1") + " k ppm"
                : _sm.gasPPM + " ppm";
            gasValueText.color = c;
        }
        if (gasStatusText) { gasStatusText.text = LABELS[_sm.gasLevel]; gasStatusText.color = c; }
        if (gasRingImage) gasRingImage.color = c;
        if (gasBarImage)
        {
            gasBarImage.fillAmount = Mathf.Clamp01(_sm.rawMQ2 / 1023f);
            gasBarImage.color = c;
        }
    }

    void DoEnv()
    {
        if (tempText) { tempText.text = _sm.temp.ToString("F1") + " C"; tempText.color = _sm.isTempAlert ? colMedium : colSafe; }
        if (humText) humText.text = _sm.humidity.ToString("F0") + " %";
    }

    void DoRing()
    {
        Color c = GetColor(_sm.gasLevel);
        string lbl = _sm.isDanger ? "DANGER" : _sm.isMedium ? "CAUTION" : "SAFE";
        if (statusRingImage) statusRingImage.color = c;
        if (statusLabelText) { statusLabelText.text = lbl; statusLabelText.color = c; }
    }

    void DoBlink()
    {
        if (_sm.isDanger)
        {
            _blinkT += Time.deltaTime * 5f;
            dangerBanner?.SetActive(Mathf.Sin(_blinkT) > 0);
        }
        else { dangerBanner?.SetActive(false); _blinkT = 0f; }
    }

    Color GetColor(int l) => l == 3 ? colHigh : l == 2 ? colMedium : colSafe;
}