/*
  GhostSense ESP8266 - FINAL
  Sends sensor data as JSON over WebSocket
  WiFi AP: GhostSense_HUD / ironman1234
  WebSocket: ws://192.168.4.1:81
*/

#include <ESP8266WiFi.h>
#include <WebSocketsServer.h>
#include <DHT.h>

// ── Pins ──────────────────────────────────────────
#define MQ2_PIN   A0
#define DHT_PIN   4      // D2 = GPIO4
#define DHT_TYPE  DHT22

// ── WiFi ──────────────────────────────────────────
const char* SSID = "GhostSense_HUD";
const char* PASS = "ironman1234";

// ── Objects ───────────────────────────────────────
DHT              dht(DHT_PIN, DHT_TYPE);
WebSocketsServer ws(81);

// ── State ─────────────────────────────────────────
float   lastTemp = 25.0;
float   lastHum  = 60.0;
unsigned long lastSend = 0;

void onWsEvent(uint8_t n, WStype_t t, uint8_t* p, size_t l) {
  if      (t == WStype_CONNECTED)    Serial.println("[WS] Client connected");
  else if (t == WStype_DISCONNECTED) Serial.println("[WS] Client disconnected");
}

void setup() {
  Serial.begin(115200);
  delay(300);

  dht.begin();

  WiFi.mode(WIFI_AP);
  WiFi.softAP(SSID, PASS);
  Serial.print("[WiFi] IP: ");
  Serial.println(WiFi.softAPIP());

  ws.begin();
  ws.onEvent(onWsEvent);

  delay(2000);
  Serial.println("[SYS] Ready");
}

void loop() {
  ws.loop();

  if (millis() - lastSend < 500) return;
  lastSend = millis();

  // Read MQ2
  int   raw = analogRead(MQ2_PIN);
  int   ppm = (int)(raw * (5000.0 / 1023.0));
  int   lvl = ppm < 500 ? 0 : ppm < 1500 ? 1 : ppm < 3000 ? 2 : 3;


  // Read DHT22
  float t = dht.readTemperature();
  float h = dht.readHumidity();
  if (!isnan(t)) lastTemp = t;
  if (!isnan(h)) lastHum  = h;

  // Build JSON - simple flat format Unity can parse
  // IMPORTANT: keys must match SensorPacket field names EXACTLY
  String json = "{";
  json += "\"rawMQ2\":";  json += raw;       json += ",";
  json += "\"gasPPM\":";  json += ppm;       json += ",";
  json += "\"gasLevel\":"; json += lvl;      json += ",";
  json += "\"temp\":";    json += lastTemp;  json += ",";
  json += "\"hum\":";     json += lastHum;
  json += "}";

  ws.broadcastTXT(json);
  Serial.println(json);
}
