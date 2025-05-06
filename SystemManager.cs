using Crestron.SimplSharpPro;
using MEP621_XPanel;
using System.Collections.Generic;

namespace AptyxXIOCloudSSharpDemo;

internal class SystemManager
{
    private readonly XioCloudRoomSlot _xioCloudRoom;
    private readonly Lab2 _lab2;

    private readonly Dictionary<int, bool> _checkStates = new Dictionary<int, bool>
    {
        {1, false},
        {2, false},
        {3, false},
        {4, false}
    };

    internal SystemManager(XioCloudRoomSlot xioCloudRoom, Lab2 lab2)
    {
        _xioCloudRoom = xioCloudRoom;
        _lab2 = lab2;

        _lab2.btnStartCheck_PressEvent += (s, e) =>
        {
            if (e.SigArgs.Sig.BoolValue)
                StartManualCheck();
        };

        _lab2.btnCheck1_PressEvent += (s, e) =>
        {
            if (e.SigArgs.Sig.BoolValue)
                ToggleCheck(1);
        };

        _lab2.btnCheck2_PressEvent += (s, e) =>
        {
            if (e.SigArgs.Sig.BoolValue)
                ToggleCheck(2);
        };

        _lab2.btnCheck3_PressEvent += (s, e) =>
        {
            if (e.SigArgs.Sig.BoolValue)
                ToggleCheck(3);
        };

        _lab2.btnCheck4_PressEvent += (s, e) =>
        {
            if (e.SigArgs.Sig.BoolValue)
                ToggleCheck(4);
        };

        _lab2.btnCheckComplete_PressEvent += (s, e) =>
        {
            if (e.SigArgs.Sig.BoolValue)
                EndManualCheck();
        };
    }

    private void ToggleCheck(int checkNumber)
    {
        _checkStates[checkNumber] = !_checkStates[checkNumber];
        UpdateUiCheckbox(checkNumber, _checkStates[checkNumber]);
    }

    private void UpdateUiCheckbox(int checkNumber, bool state)
    {
        ushort value = state ? (ushort)1 : (ushort)0;

        switch (checkNumber)
        {
            case 1:
                _lab2.btnCheck1_Mode(value);
                break;
            case 2:
                _lab2.btnCheck2_Mode(value);
                break;
            case 3:
                _lab2.btnCheck3_Mode(value);
                break;
            case 4:
                _lab2.btnCheck4_Mode(value);
                break;
        }
    }

    private void StartManualCheck()
    {
        _xioCloudRoom.SystemCheckStateFeedback.Value = XioCloudRoomSlot.eSystemCheckState.Running;
    }

    private void EndManualCheck()
    {
        var messages = new[]
        {
            _checkStates[1] ? "Lights are functioning" : "Lights are NOT functioning",
            _checkStates[2] ? "Desk is tidy" : "Desk is NOT tidy",
            _checkStates[3] ? "Garbage can is empty" : "Garbage can is NOT empty",
            _checkStates[4] ? "Touch screen is clean" : "Touch screen is NOT clean"
        };

        foreach (var message in messages)
        {
            _xioCloudRoom.SystemCheckMessageFeedback.StringValue = message;
        }

        _xioCloudRoom.SystemCheckStateFeedback.Value = XioCloudRoomSlot.eSystemCheckState.Success;

        // Reset check states and UI after completing
        for (int i = 1; i <= 4; i++)
        {
            _checkStates[i] = false;
            UpdateUiCheckbox(i, false);
        }
    }

    public void RunSystemCheck()
    {
        _lab2.btnSystemCheck_Selected(true);
        _xioCloudRoom.SystemCheckStateFeedback.Value = XioCloudRoomSlot.eSystemCheckState.Running;
        _xioCloudRoom.SystemCheckMessageFeedback.StringValue = "System Check started";
        _xioCloudRoom.SystemCheckMessageFeedback.StringValue = "Microphone 1 is working!";
        _xioCloudRoom.SystemCheckMessageFeedback.StringValue = "Microphone 2 is working!";
        _xioCloudRoom.SystemCheckMessageFeedback.StringValue = "Microphone 3 not working!";
        _xioCloudRoom.SystemCheckMessageFeedback.StringValue = "More Cowbell!";
        _xioCloudRoom.SystemCheckStateFeedback.Value = XioCloudRoomSlot.eSystemCheckState.Success;

        Task.Delay(5000).Wait();
        _lab2.btnSystemCheck_Selected(false);
    }

    public void MaintenanceMode(bool state)
    {
        _lab2.btnMaintenanceOn_Selected(state);
        _lab2.btnMaintenanceOff_Selected(!state);
    }
}