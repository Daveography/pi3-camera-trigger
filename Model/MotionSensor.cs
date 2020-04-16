using System;
using System.Diagnostics;
using Windows.Devices.Gpio;

namespace Pi3CameraTrigger.Model
{
    public interface IMotionSensor
    {
        void StartWatching(TriggeredAction sensorTriggeredFunction);
    }

    public sealed class MotionSensor : IMotionSensor
    {
        private readonly GpioController _gpio;
        private readonly IGpioConfiguration _gpioConfig;

        private TriggeredAction _actionCallback;

        public MotionSensor(GpioController gpio, IGpioConfiguration gpioConfig)
        {
            _gpio = gpio;
            _gpioConfig = gpioConfig;
        }

        public void StartWatching(TriggeredAction sensorTriggeredFunction)
        {
            _actionCallback = sensorTriggeredFunction;
            var pin = _gpio.OpenPin(_gpioConfig.SensorPin);

            // Check if input pull-up resistors are supported
            pin.SetDriveMode(pin.IsDriveModeSupported(GpioPinDriveMode.InputPullUp)
                ? GpioPinDriveMode.InputPullUp
                : GpioPinDriveMode.Input);

            pin.DebounceTimeout = TimeSpan.FromMilliseconds(50);
            pin.ValueChanged += SensorTriggered_Internal;
        }

        private void SensorTriggered_Internal(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            if (e.Edge == GpioPinEdge.RisingEdge)
            {
                Debug.WriteLine("{0:hh:mm:ss.fff} Sensor triggered", DateTime.Now);

                if (_actionCallback != null)
                {
                    _actionCallback.Invoke();
                }
            }
            else
            {
                Debug.WriteLine("{0:hh:mm:ss.fff} Sensor returned to rest state", DateTime.Now);
            }
        }
    }
}
