// TwinModel.cs
using System;

public class TwinModel
{
    public DateTime Timestamp { get; set; }
    public double CurrentTemperature { get; set; }
    public double TargetTemperature { get; set; }
    public bool Overheated { get; set; }

    // will be extended (Pressure, Status, AlarmLevel vs)
}
