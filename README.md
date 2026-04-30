# GhostSense – IoT XR Safety HUD

Real-time IoT-powered XR HUD for gas and environmental safety with mock mode support for hardware-free demos.
<img width="810" height="202" alt="image" src="https://github.com/user-attachments/assets/1eb8af17-df60-4cd5-9be2-3e299f4a915e" />

---

## 🚀 Overview

GhostSense is a real-time environmental safety system that transforms live IoT sensor data into intuitive visual alerts inside a heads-up display (HUD).

It detects invisible risks like gas exposure and temperature changes using sensors and visualizes them as:

* SAFE
* CAUTION
* DANGER

The system works in two modes:

* ✅ **Real hardware mode** (ESP8266 via WebSocket)
* ✅ **Mock mode** (no hardware required)

---

## 🧠 System Architecture

ESP8266 (MQ2 + DHT22)
→ WebSocket (ws://192.168.4.1:81)
→ WebSocketReceiver.cs
→ SensorManager.cs
→ HUDController.cs (UI)

---

## 📂 Scripts Overview

### 1. WebSocketReceiver.cs

Handles:

* WebSocket connection
* Receiving JSON data from ESP8266
* Mock data simulation (for testing without hardware)

Key feature:

* Uses built-in `System.Net.WebSockets` (no external packages)

---

### 2. SensorManager.cs

Central data processor:

* Stores live sensor values
* Converts gasPPM into gasLevel
* Defines safety states:

  * SAFE (0)
  * LOW (1)
  * MEDIUM (2)
  * HIGH / DANGER (3)

Important:

* Gas level is calculated inside Unity (not trusted from ESP)

---

### 3. HUDController.cs

Controls UI:

* Gas value display
* Status labels (CLEAN / LOW / MEDIUM / HIGH)
* Color-based feedback
* Central safety ring
* Blinking danger alert

---

## 🧪 MOCK MODE (FOR TEAMMATES WITHOUT HARDWARE)

This project is fully usable without sensors.

### 🔧 How to enable

1. Select the GameObject with **WebSocketReceiver**
2. In Inspector:

* Enable ✅ **Use Mock Data**
* (Optional) Enable ⚠ **Simulate Danger**

---

### 🎯 What each option does
<img width="452" height="311" alt="image" src="https://github.com/user-attachments/assets/fae7b968-4be8-40cb-bd7d-42c1a498f157" />


#### Use Mock Data = ON

* Disables WebSocket connection
* Generates fake sensor values
* Allows full demo in Unity

#### Simulate Danger = ON

Forces:

* High gas values
* High temperature
* Triggers:

  * Red UI
  * Blinking danger alert

---

## 🔌 REAL HARDWARE MODE

### Requirements

* ESP8266
* MQ-2 Gas Sensor
* DHT22 Sensor

---

### Unity Setup

1. Select **WebSocketReceiver**
2. Set:

Use Mock Data = OFF
wsUrl = ws://192.168.4.1:81

---

### Expected JSON Format

{
"rawMQ2": 320,
"gasPPM": 1560,
"gasLevel": 2,
"temp": 32.5,
"hum": 60
}

---

## 🎮 Unity Setup (Step-by-Step)

1. Open project in Unity (6000.3.10f1 LTS)

2. Scene must include:

* SensorManager (GameObject)
* WebSocketReceiver (same GameObject)
* HUD_Canvas (World Space)

3. Assign UI references in HUDController:

* Gas Text
* Gas Ring Image
* Gas Bar Image
* Temperature Text
* Humidity Text
* Status Ring
* Danger Banner

4. Press Play

---

## ⚠️ Gas Level Logic

gasPPM >= 3000 → HIGH (Danger)
gasPPM >= 1500 → MEDIUM
gasPPM >= 500 → LOW
gasPPM < 500 → SAFE

---

## 🧩 Key Features

* Real-time WebSocket data streaming
* Hardware-independent mock mode
* Clean separation of:

  * Input (WebSocketReceiver)
  * Logic (SensorManager)
  * UI (HUDController)
* Immediate visual hazard feedback

---

## 🎬 Project Vision (Hackathon Context)

This project is part of a larger XR system inspired by the **Iron Man HUD concept**, where real-world sensor data is directly visualized in the user’s field of view.

The goal is to:

* Reduce reaction time
* Improve situational awareness
* Make invisible risks visible

---

## 👤 My Contribution

**IoT & Data Pipeline**

* Designed sensor-to-Unity pipeline
* Implemented WebSocket communication
* Built mock simulation system
* Connected real-time data to XR HUD

---

## 📌 Future Scope

* BIM overlay integration
* Spatial anchoring
* AI anomaly detection
* Auto documentation system
* Multi-user XR collaboration
