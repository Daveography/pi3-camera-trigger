using Microsoft.Extensions.DependencyInjection;
using Pi3CameraTrigger.Model;
using Pi3CameraTrigger.Model.Camera;
using Pi3CameraTrigger.Model.Gpio;
using System;
using Windows.ApplicationModel.Background;
using System.Device.Gpio;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace Pi3CameraTrigger
{
    public sealed class StartupTask : IBackgroundTask
    {

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
                .AddSingleton(s => new GpioController())
                .AddSingleton<IGpioPinFactory, GpioPinFactory>()
                .AddSingleton<IGpioConfiguration, GpioConfiguration>()
                .AddSingleton<IShutterRemoteConfiguration>(s =>
                    new ShutterRemoteConfiguration(CAMERA_FOCUS_SECONDS, CAMERA_SHUTTER_SECONDS))
                .AddSingleton<ICoordinator, Coordinator>()
                .AddSingleton<IMotionSensor, MotionSensor>()
                .AddSingleton<IShutterRemote, ShutterRemote>()
                .BuildServiceProvider();
        }
    }
}
