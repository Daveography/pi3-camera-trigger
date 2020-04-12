using System;
using System.Diagnostics;
using Pi3CameraTrigger.Model.Gpio;
using System.Device.Gpio;

namespace Pi3CameraTrigger.Model
{
    public interface IMotionSensor
    {
        void StartWatching(TriggeredAction sensorTriggeredFunction);
    }

    public sealed class MotionSensor : IMotionSensor
    {
        private readonly IGpioConfiguration _gpioConfig;

        private TriggeredAction _actionCallback;

        public MotionSensor(IGpioConfiguration gpioConfig)
        {
            _gpioConfig = gpioConfig;
        }

        public void StartWatching(TriggeredAction sensorTriggeredFunction)
        {
            _actionCallback = sensorTriggeredFunction;
            var sensorPin = _gpioConfig.SensorPin;

            // Check if input pull-up resistors are supported
            sensorPin.Open(sensorPin.ModeSupported(PinMode.InputPullUp)
                ? PinMode.InputPullUp
                : PinMode.Input);

            // pin.DebounceTimeout = TimeSpan.FromMilliseconds(50);
            sensorPin.RegisterCallbackForEvent(PinEventTypes.Rising, (sender, args) => {
                Debug.WriteLine("{0:hh:mm:ss.fff} Sensor triggered", DateTime.Now);

                if (_actionCallback != null)
                {
                    _actionCallback.Invoke();
                }
            });

            sensorPin.RegisterCallbackForEvent(PinEventTypes.Falling, (sender, args) =>
              Debug.WriteLine("{0:hh:mm:ss.fff} Sensor returned to rest state", DateTime.Now)
            );
        }
    }
}
