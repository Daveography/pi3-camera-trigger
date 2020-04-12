using Pi3CameraTrigger.Model.Camera;

namespace Pi3CameraTrigger.Model
{
    public interface ICoordinator
    {
        void Start();
    }

    public sealed class Coordinator : ICoordinator
    {
        private readonly IMotionSensor _motionSensor;
        private readonly IShutterRemote _shutterRemote;

        public Coordinator(IMotionSensor motionSensor, IShutterRemote shutterRemote)
        {
            _motionSensor = motionSensor;
            _shutterRemote = shutterRemote;
        }

        public void Start()
        {
            _motionSensor.StartWatching(() =>
            {
                _shutterRemote.Focus();
                _shutterRemote.Fire();
            });
        }
    }
}
