using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using MEP621_XPanel;

namespace AptyxXIOCloudSSharpDemo;

internal class PowerManager
{
    private readonly XioCloudRoomSlot _xioCloudRoom;
    private readonly Lab0 _lab0;

    private bool SystemPowerOn { get; set; }
    private bool DisplayPowerOn { get; set; }

    internal PowerManager(XioCloudRoomSlot xioCloudRoom, Lab0 lab0)
    {
        this._xioCloudRoom = xioCloudRoom;
        this._lab0 = lab0;
        _lab0.btnSystemPowerOn_PressEvent += (s, e) =>
        {
            if (e.SigArgs.Sig.BoolValue)
                SetSystemPower(true);
        };
        
        _lab0.btnSystemPowerOff_PressEvent += (s, e) =>
        {
            if (e.SigArgs.Sig.BoolValue)
                SetSystemPower(false);
        };

        _lab0.btnDisplayPowerOn_PressEvent += (s, e) =>
        {
            if (e.SigArgs.Sig.BoolValue)
                SetDisplayPower(true);
        };

        _lab0.btnDisplayPowerOff_PressEvent += (s, e) =>
        {
            if (e.SigArgs.Sig.BoolValue)
                SetDisplayPower(false);
        };


        // Initialize all feedback states to false on load
        SystemPowerOn = false;
        DisplayPowerOn = false;
        UpdateFeedbackStates();
    }

    public void SetSystemPower(bool state)
    {
        SystemPowerOn = state;
        DisplayPowerOn = state ? true : DisplayPowerOn; // System On turns on display

        if (!state)
        {
            DisplayPowerOn = false; // System Off also turns off display
        }

        UpdateFeedbackStates();
        ErrorLog.Notice($"System Power {(SystemPowerOn ? "On" : "Off")}, Display Power {(DisplayPowerOn ? "On" : "Off")}");
    }

    public void SetDisplayPower(bool state)
    {
        DisplayPowerOn = state;
        UpdateFeedbackStates();
        ErrorLog.Notice($"Display Power {(DisplayPowerOn ? "On" : "Off")} (System remains {(SystemPowerOn ? "On" : "Off")})");
    }

    private void UpdateFeedbackStates()
    {
        _xioCloudRoom.SystemPowerFeedback.BoolValue = SystemPowerOn;
        _xioCloudRoom.DisplayPowerFeedback.BoolValue = DisplayPowerOn;

        _lab0.btnSystemPowerOn_Selected(SystemPowerOn);
        _lab0.btnSystemPowerOff_Selected(!SystemPowerOn);

        _lab0.btnDisplayPowerOn_Selected(DisplayPowerOn);
        _lab0.btnDisplayPowerOff_Selected(!DisplayPowerOn);
    }
}