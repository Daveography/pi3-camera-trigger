using System.Device.Gpio;

namespace Pi3CameraTrigger.Model.Gpio
{
  public interface IGpioPinFactory
  {
    GpioPin GetPin(int pinNumber);
  }

  public class GpioPinFactory : IGpioPinFactory
  {
    private readonly GpioController _gpio;

    public GpioPinFactory(GpioController gpio)
    {
      _gpio = gpio;
    }

    public GpioPin GetPin(int pinNumber)
    {
      return new GpioPin(_gpio, pinNumber);
    }
  }
}