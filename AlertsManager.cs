using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using MEP621_XPanel;

namespace AptyxXIOCloudSSharpDemo;

internal class AlertsManager
{
    private readonly XioCloudRoomSlot _xioCloudRoom;
    private readonly ILab1 _lab1;
    private string _pendingAlertMessage = string.Empty;

    internal AlertsManager(XioCloudRoomSlot xioCloudRoom, ILab1 lab1)
    {
        _xioCloudRoom = xioCloudRoom;
        _lab1 = lab1;
        _lab1.txtInputField_OutputTextEvent += (s, e) => ReceiveAlertInput(e.SigArgs);
        _lab1.btnAlert_PressEvent += (s, e) =>
        {
            if (e.SigArgs.Sig.BoolValue)
                SendAlert();
        };
        
        _lab1.txtAlert_Indirect("Please send us a message!");
    }

    public void ReceiveAlertInput(SigEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Sig.StringValue))
        {
            ErrorLog.Warn("Received empty or null SigEventArgs for alert input.");
            return;
        }

        _pendingAlertMessage = e.Sig.StringValue;
        _lab1.txtInputField_IndirectText(_pendingAlertMessage);
        ErrorLog.Notice($"Stored alert input from SigEventArgs: {_pendingAlertMessage}");
    }

    public void SendAlert()
    {
        if (_pendingAlertMessage.Length == 0)
        {
            ErrorLog.Notice("SendAlert called but no pending message is stored.");
            return;
        }

        _xioCloudRoom.AlertsFeedback.StringValue = $"ALERT {_pendingAlertMessage}";
        ErrorLog.Notice($"Alert sent to XiO Cloud: {_pendingAlertMessage}");

        // Clear after sending
        _pendingAlertMessage = "";

        // Clear the input field on the UI
        _lab1.txtInputField_IndirectText(_pendingAlertMessage);
    }
}