namespace Pi3CameraTrigger.Model.Gpio
{
    public interface IGpioConfiguration
    {
        GpioPin SensorPin { get; }
        GpioPin FocusPin { get; }
        GpioPin ShutterActuationPin { get; }
    }

    public sealed class GpioConfiguration : IGpioConfiguration
    {
        private const int MOTION_SENSOR_PIN = 26;
        private const int CAMERA_FOCUS_PIN = 18;
        private const int CAMERA_SHUTTER_PIN = 23;

        public GpioPin SensorPin { get; private set; }
        public GpioPin FocusPin { get; private set; }
        public GpioPin ShutterActuationPin { get; private set; }

        public GpioConfiguration(GpioPinFactory pinFactory)
        {
            SensorPin = pinFactory.GetPin(MOTION_SENSOR_PIN);
            FocusPin = pinFactory.GetPin(CAMERA_FOCUS_PIN);
            ShutterActuationPin = pinFactory.GetPin(CAMERA_SHUTTER_PIN);
        }
    }
}
