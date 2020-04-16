using Stateless;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Pi3CameraTrigger.Model.Camera
{
    public interface IShutterRemote
    {
        void Focus();
        void Fire();
        void Release();
    }

    public sealed class ShutterRemote : IShutterRemote
    {
        private readonly GpioController _gpio;
        private readonly IGpioConfiguration _gpioConfig;
        private readonly IShutterRemoteConfiguration _triggerConfig;

        private readonly GpioPin _focusPin;
        private readonly GpioPin _shutterPin;

        private readonly StateMachine<ShutterRemoteState, ShutterRemoteTrigger> _remoteState;

        public ShutterRemote(GpioController gpio, IGpioConfiguration gpioConfig, IShutterRemoteConfiguration triggerConfig)
        {
            _gpio = gpio;
            _gpioConfig = gpioConfig;
            _triggerConfig = triggerConfig;

            _focusPin = gpio.OpenPin(gpioConfig.FocusPin);
            _focusPin.Write(GpioPinValue.Low);
            _focusPin.SetDriveMode(GpioPinDriveMode.Output);

            _shutterPin = gpio.OpenPin(gpioConfig.ShutterActuationPin);
            _shutterPin.Write(GpioPinValue.Low);
            _shutterPin.SetDriveMode(GpioPinDriveMode.Output);

            _remoteState = ConfigureStateMachine();
        }

        public void Focus()
        {
            _remoteState.Fire(ShutterRemoteTrigger.Focus);
        }

        public void Fire()
        {
            _remoteState.Fire(ShutterRemoteTrigger.Fire);

            // Immediately release after firing
            _remoteState.Fire(ShutterRemoteTrigger.Release);
        }

        public void Release()
        {
            _remoteState.Fire(ShutterRemoteTrigger.Release);
        }

        private StateMachine<ShutterRemoteState, ShutterRemoteTrigger> ConfigureStateMachine()
        {
            var state = new StateMachine<ShutterRemoteState, ShutterRemoteTrigger>(ShutterRemoteState.None);

            state.Configure(ShutterRemoteState.None)
                .OnEntry(() => GpioRelease())
                .Permit(ShutterRemoteTrigger.Focus, ShutterRemoteState.Focusing)
                .Ignore(ShutterRemoteTrigger.Fire);
            
            state.Configure(ShutterRemoteState.Focusing)
                .OnEntry(() => GpioFocus())
                .Permit(ShutterRemoteTrigger.Fire, ShutterRemoteState.Firing)
                .Permit(ShutterRemoteTrigger.Release, ShutterRemoteState.None);

            state.Configure(ShutterRemoteState.Firing)
                .OnEntry(() => GpioFire())
                .Permit(ShutterRemoteTrigger.Release, ShutterRemoteState.None)
                .Ignore(ShutterRemoteTrigger.Focus);

            return state;
        }

        private void GpioFocus()
        {
            Debug.WriteLine("{0:hh:mm:ss.fff} Actuating remote trigger focus", DateTime.Now);
            _focusPin.Write(GpioPinValue.High);
            Task.Delay(TimeSpan.FromSeconds(_triggerConfig.FocusDurationSeconds)).Wait();
        }

        private void GpioFire()
        {
            Debug.WriteLine("{0:hh:mm:ss.fff} Actuating remote trigger fire", DateTime.Now);
            _shutterPin.Write(GpioPinValue.High);
            Task.Delay(TimeSpan.FromSeconds(_triggerConfig.ShutterActuationSeconds)).Wait();
        }

        private void GpioRelease()
        {
            Debug.WriteLine("{0:hh:mm:ss.fff} Returning remote trigger to rest state", DateTime.Now);
            _focusPin.Write(GpioPinValue.Low);
            _shutterPin.Write(GpioPinValue.Low);
        }
    }
}
