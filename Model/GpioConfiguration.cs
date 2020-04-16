namespace Pi3CameraTrigger.Model
{
    public interface IGpioConfiguration
    {
        int SensorPin { get; }
        int FocusPin { get; }
        int ShutterActuationPin { get; }
    }

    public sealed class GpioConfiguration : IGpioConfiguration
    {
        public GpioConfiguration(int sensorPin, int wakeFocusPin, int shutterActuationPin)
        {
            SensorPin = sensorPin;
            FocusPin = wakeFocusPin;
            ShutterActuationPin = shutterActuationPin;
        }

        public int SensorPin { get; private set; }
        public int FocusPin { get; private set; }
        public int ShutterActuationPin { get; private set; }
    }
}
