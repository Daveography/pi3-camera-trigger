using System;

namespace Pi3CameraTrigger.Model.Camera
{
    public interface IShutterRemoteConfiguration
    {
        double FocusDurationSeconds { get; }
        double ShutterActuationSeconds { get; }
    }

    public sealed class ShutterRemoteConfiguration : IShutterRemoteConfiguration
    {
        public ShutterRemoteConfiguration(double focusDurationSeconds, double shutterActuationSeconds)
        {
            FocusDurationSeconds = focusDurationSeconds;
            ShutterActuationSeconds = shutterActuationSeconds;
        }

        public double FocusDurationSeconds { get; private set; }
        public double ShutterActuationSeconds { get; private set; }
    }
}
