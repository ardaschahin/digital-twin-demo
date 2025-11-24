# Edge OPC UA Digital Twin, React Dashboard

This is a small demo dashboard that visualizes live data from an OPC UA-based edge application.

The backend reads process data (e.g. boiler temperatures) from a PLC / OPC UA server and exposes a simple REST API at `/twin`.  
This React app polls that endpoint once per second and displays:

- Current temperature  
- Target temperature  
- Overheated status  
- Alarm indicator (visual)  
- Timestamp of the last update  
- A small boiler image representing the physical asset  

## Tech Stack

- **Frontend:** React, Vite, Ant Design  
- **Backend:** .NET 8 minimal API (separate project)  
- **Protocol:** OPC UA (on the backend side)  

## How to run

1. Go to the frontend folder:

   ```bash
   cd twin-dashboard
