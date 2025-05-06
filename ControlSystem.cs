using System;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.GeneralIO;
using Crestron.SimplSharpPro.UI;
using MEP621_XPanel;

namespace AptyxXIOCloudSSharpDemo;

public class ControlSystem : CrestronControlSystem
{
    private XioCloudRoomSlot? XioCloudRoom => ControllerXioCloudRoomSlotDevice;

    private readonly Lab0 _lab0 = null!;
    private readonly Lab1 _lab1 = null!;
    private readonly Lab2 _lab2 = null!;
    private readonly XpanelForHtml5 _xpanel = null!;

    private PowerManager _powerManager = null!;
    private SourceManager _sourceManager = null!;
    private OccupancyManager _occupancyManager = null!;
    private AlertsManager _alertsManager = null!;
    private SystemManager _systemManager = null!;

    public ControlSystem() : base()
    {
        try
        {
            var componentMediator = new ComponentMediator();
            _lab0 = new Lab0(componentMediator, 1);
            _lab1 = new Lab1(componentMediator, 2);
            _lab2 = new Lab2(componentMediator, 3);
            _xpanel = new XpanelForHtml5(0x03, this);
            _lab0.AddDevice(_xpanel);
            _lab1.AddDevice(_xpanel);
            _lab2.AddDevice(_xpanel);
        }
        catch (Exception ex)
        {
            ErrorLog.Error($"ControlSystem constructor exception: {ex.Message}");
        }
    }
    
    public override void InitializeSystem()
    {
        try
        {
            if (XioCloudRoom?.Register() != eDeviceRegistrationUnRegistrationResponse.Success)
            {
                ErrorLog.Error($"XiO Cloud Room Slot registration failed: {XioCloudRoom?.RegistrationFailureReason}");
                return;
            }

            //Managers for Lab 0
            _powerManager = new PowerManager(XioCloudRoom, _lab0);
            _sourceManager = new SourceManager(XioCloudRoom, _lab0);
            
            //Managers for Lab 1
            _occupancyManager = new OccupancyManager(XioCloudRoom, _lab1);
            _alertsManager = new AlertsManager(XioCloudRoom, _lab1);
            
            //Manager for Lab 2
            _systemManager = new SystemManager(XioCloudRoom, _lab2);

            XioCloudRoom.OnlineStatusChange += OnXioCloudOnlineChange;
            XioCloudRoom.BaseEvent += OnXioCloudBaseEvent;

            _xpanel.OnlineStatusChange += OnHtml5XpanelOnlineChange;
            _xpanel.Register();
            
            ErrorLog.Notice("System initialized, XiO Cloud and HTML5 XPanel registered.");
            XioCloudRoom.AlertsFeedback.StringValue = "ALERT Program has started";
        }
        catch (Exception ex)
        {
            ErrorLog.Error($"InitializeSystem exception: {ex.Message}");
        }
    }

    private void OnXioCloudBaseEvent(GenericBase device, BaseEventArgs args)
    {
        if (XioCloudRoom == null) return;
        switch (args.EventId)
        {
            case XioCloudRoomSlot.SystemPowerOnEventId:
                _powerManager.SetSystemPower(true);
                ErrorLog.Notice("XIO Events: System Power On.");
                break;
            case XioCloudRoomSlot.SystemPowerOffEventId:
                _powerManager.SetSystemPower(false);
                ErrorLog.Notice("XIO Events: System Power Off.");
                break;
            case XioCloudRoomSlot.DisplayPowerOnEventId:
                _powerManager.SetDisplayPower(true);
                ErrorLog.Notice("XIO Events: Display Power On.");
                break;
            case XioCloudRoomSlot.DisplayPowerOffEventId:
                _powerManager.SetDisplayPower(false);
                ErrorLog.Notice("XIO Events: Display Power Off.");
                break;
            case XioCloudRoomSlot.OccupiedEventId:
                ErrorLog.Notice("XIO Events: Room Occupied.");
                break;
            case XioCloudRoomSlot.VacantEventId:
                ErrorLog.Notice("XIO Events: Room Vacant.");
                break;
            case XioCloudRoomSlot.SystemCheckEventId:
                ErrorLog.Notice("XIO Events: System Check received.");
                _systemManager.RunSystemCheck();
                break;
            case XioCloudRoomSlot.MaintenanceModeEventId:
                ErrorLog.Notice("XIO Events: Maintenance Mode received.");
                _systemManager.MaintenanceMode(XioCloudRoom.MaintenanceMode.BoolValue);
                break;
            case XioCloudRoomSlot.RoomNameEventId:
                ErrorLog.Notice("XIO Events: Room Name received.");
                _lab0.txtRoomName_Indirect(XioCloudRoom.RoomName.StringValue);
                _lab1.txtRoomName_Indirect(XioCloudRoom.RoomName.StringValue);
                _lab2.txtRoomName_Indirect(XioCloudRoom.RoomName.StringValue);
                break;
            default:
                ErrorLog.Notice($"Unhandled XiO Cloud Event ID: {args.EventId}");
                break;
        }
    }
    private void OnXioCloudOnlineChange(GenericBase device, OnlineOfflineEventArgs args)
    {
        ErrorLog.Notice($"XiO Cloud Online Status: {(args.DeviceOnLine ? "Online" : "Offline")}");
    }

    private void OnHtml5XpanelOnlineChange(GenericBase device, OnlineOfflineEventArgs args)
    {
        ErrorLog.Notice($"HTML5 XPanel Online Status: {(args.DeviceOnLine ? "Online" : "Offline")}");
    }
}
