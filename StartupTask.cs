using Microsoft.Extensions.DependencyInjection;
using Pi3CameraTrigger.Model;
using Pi3CameraTrigger.Model.Camera;
using System;
using Windows.ApplicationModel.Background;
using Windows.Devices.Gpio;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace Pi3CameraTrigger
{
    public sealed class StartupTask : IBackgroundTask
    {
        private const int MOTION_SENSOR_PIN = 26;
        private const int CAMERA_FOCUS_PIN = 18;
        private const int CAMERA_SHUTTER_PIN = 23;

        private const double CAMERA_FOCUS_SECONDS = 0.5;
        private const double CAMERA_SHUTTER_SECONDS = 0.1;

        private BackgroundTaskDeferral _deferral;
        private IServiceProvider _serviceProvider;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();
            ConfigureDI();

            _serviceProvider.GetService<ICoordinator>().Start();
        }

        private void ConfigureDI()
        {
            _serviceProvider = new ServiceCollection()
                .AddSingleton<IGpioConfiguration>(s =>
                    new GpioConfiguration(MOTION_SENSOR_PIN, CAMERA_FOCUS_PIN, CAMERA_SHUTTER_PIN))
                .AddSingleton<IShutterRemoteConfiguration>(s =>
                    new ShutterRemoteConfiguration(CAMERA_FOCUS_SECONDS, CAMERA_SHUTTER_SECONDS))
                .AddSingleton<ICoordinator, Coordinator>()
                .AddSingleton<IMotionSensor, MotionSensor>()
                .AddSingleton<IShutterRemote, ShutterRemote>()
                .AddSingleton(s => GpioController.GetDefault())
                .BuildServiceProvider();
        }
    }
}
