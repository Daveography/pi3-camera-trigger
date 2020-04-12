using Stateless;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Device.Gpio;
using Pi3CameraTrigger.Model.Gpio;

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
        private readonly IGpioConfiguration _gpioConfig;
        private readonly IShutterRemoteConfiguration _triggerConfig;
        private readonly StateMachine<ShutterRemoteState, ShutterRemoteTrigger> _remoteState;

        public ShutterRemote(IGpioConfiguration gpioConfig, IShutterRemoteConfiguration triggerConfig)
        {
            _gpioConfig = gpioConfig;
            _triggerConfig = triggerConfig;

            _gpioConfig.FocusPin.Open(PinMode.Output);
            _gpioConfig.FocusPin.Write(PinValue.Low);

            _gpioConfig.ShutterActuationPin.Open(PinMode.Output);
            _gpioConfig.ShutterActuationPin.Write(PinValue.Low);

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
            _gpioConfig.FocusPin.Write(PinValue.High);
            Task.Delay(TimeSpan.FromSeconds(_triggerConfig.FocusDurationSeconds)).Wait();
        }

        private void GpioFire()
        {
            Debug.WriteLine("{0:hh:mm:ss.fff} Actuating remote trigger fire", DateTime.Now);
            _gpioConfig.ShutterActuationPin.Write(PinValue.High);
            Task.Delay(TimeSpan.FromSeconds(_triggerConfig.ShutterActuationSeconds)).Wait();
        }

        private void GpioRelease()
        {
            Debug.WriteLine("{0:hh:mm:ss.fff} Returning remote trigger to rest state", DateTime.Now);
            _gpioConfig.FocusPin.Write(PinValue.Low);
            _gpioConfig.ShutterActuationPin.Write(PinValue.Low);
        }
    }
}
