using System.Device.Gpio;

namespace Pi3CameraTrigger.Model.Gpio
{
  public interface IGpioPin
  {
    bool IsOpen { get; }
    void Open(PinMode mode);
    void Close();
    void Write(PinValue value);
    void SetMode(PinMode mode);
    PinValue Read();
    bool ModeSupported(PinMode mode);
    void RegisterCallbackForEvent(PinEventTypes eventTypes, PinChangeEventHandler handler);
  }

  public class GpioPin : IGpioPin
  {
    private readonly GpioController _gpio;

    public bool IsOpen
    {
      get
      {
        return _gpio.IsPinOpen(PinNumber);
      }
    }

    public int PinNumber { get; private set; }

    public GpioPin(GpioController gpio, int pinNumber)
    {
      _gpio = gpio;
      PinNumber = pinNumber;
    }

    public void Open(PinMode mode)
    {
      _gpio.OpenPin(PinNumber, mode);
    }

    public void Close()
    {
      _gpio.ClosePin(PinNumber);
    }

    public void Write(PinValue value)
    {
      _gpio.Write(PinNumber, value);
    }

    public void SetMode(PinMode mode)
    {
      _gpio.SetPinMode(PinNumber, mode);
    }

    public PinValue Read()
    {
      return _gpio.Read(PinNumber);
    }

    public bool ModeSupported(PinMode mode)
    {
      return _gpio.IsPinModeSupported(PinNumber, mode);
    }

    public void RegisterCallbackForEvent(PinEventTypes eventTypes, PinChangeEventHandler handler)
    {
      _gpio.RegisterCallbackForPinValueChangedEvent(PinNumber, eventTypes, handler);
    }
  }
}
