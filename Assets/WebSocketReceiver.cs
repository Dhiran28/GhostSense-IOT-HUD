// WebSocketReceiver.cs
// Attach to the SAME GameObject as SensorManager
// Uses System.Net.WebSockets - NO external packages needed

using UnityEngine;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class WebSocketReceiver : MonoBehaviour
{
    [Header("Connection")]
    public string wsUrl = "ws://192.168.4.1:81";
    public bool useMockData = true;

    [Header("Mock (only when useMockData = true)")]
    public bool simulateDanger = false;

    private ClientWebSocket _ws;
    private CancellationTokenSource _cts;
    private string _pending = null;
    private float _t = 0f;

    async void Start()
    {
        if (useMockData)
        {
            Debug.Log("[GS] MOCK MODE active");
            return;
        }
        await Connect();
    }

    async Task Connect()
    {
        try
        {
            _cts = new CancellationTokenSource();
            _ws = new ClientWebSocket();
            Debug.Log("[WS] Connecting to: " + wsUrl);
            await _ws.ConnectAsync(new Uri(wsUrl), _cts.Token);
            Debug.Log("[WS] Connected!");
            _ = ReceiveLoop();
        }
        catch (Exception e)
        {
            Debug.LogError("[WS] Connect FAILED: " + e.Message);
        }
    }

    async Task ReceiveLoop()
    {
        var buf = new byte[1024];
        Debug.Log("[WS] Receive loop started");
        while (_ws != null && _ws.State == WebSocketState.Open)
        {
            try
            {
                var seg = new ArraySegment<byte>(buf);
                var result = await _ws.ReceiveAsync(seg, _cts.Token);
                if (result.MessageType == WebSocketMessageType.Close) break;
                int count = result.Count;
                string raw = Encoding.UTF8.GetString(buf, 0, count);
                Debug.Log("[WS] RECEIVED: " + raw);
                _pending = raw;
            }
            catch (Exception e)
            {
                Debug.LogWarning("[WS] Receive error: " + e.Message);
                break;
            }
        }
        Debug.Log("[WS] Receive loop ended. State: " + _ws?.State);
    }

    void Update()
    {
        if (!useMockData)
        {
            if (_pending != null)
            {
                ParseAndApply(_pending);
                _pending = null;
            }
            return;
        }

        // Mock mode
        _t += Time.deltaTime;
        var p = new SensorPacket();
        if (simulateDanger)
        {
            p.rawMQ2 = 750;
            p.gasPPM = 3670;
            p.gasLevel = 3;
            p.temp = 47f;
            p.hum = 55f;
        }
        else
        {
            p.rawMQ2 = (int)(80f + Mathf.Sin(_t * 0.35f) * 35f);
            p.gasPPM = (int)(p.rawMQ2 * (5000f / 1023f));
            p.gasLevel = p.rawMQ2 < 100 ? 0 : p.rawMQ2 < 300 ? 1 : p.rawMQ2 < 600 ? 2 : 3;
            p.temp = 25f + Mathf.Sin(_t * 0.1f) * 2f;
            p.hum = 60f + Mathf.Sin(_t * 0.08f) * 7f;
        }
        SensorManager.Instance?.Apply(p);
    }

    void ParseAndApply(string json)
    {
        Debug.Log("[WS] Parsing: " + json);
        try
        {
            SensorPacket p = JsonUtility.FromJson<SensorPacket>(json);
            if (p == null)
            {
                Debug.LogError("[WS] FromJson returned null - check JSON format");
                return;
            }
            SensorManager.Instance?.Apply(p);
        }
        catch (Exception e)
        {
            Debug.LogError("[WS] Parse FAILED: " + e.Message + " | JSON was: " + json);
        }
    }

    async void OnApplicationQuit()
    {
        _cts?.Cancel();
        if (_ws != null && _ws.State == WebSocketState.Open)
            await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure,
                                 "quit", CancellationToken.None);
    }
}