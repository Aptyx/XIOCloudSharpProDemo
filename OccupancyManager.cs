using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using MEP621_XPanel;

namespace AptyxXIOCloudSSharpDemo;

internal class OccupancyManager
{
    private readonly XioCloudRoomSlot _xioCloudRoom;
    private readonly ILab1 _lab1;
    private CTimer _occupancyTimer = null!;
    private bool _isLatched;

    internal OccupancyManager(XioCloudRoomSlot xioCloudRoom, ILab1 lab1)
    {
        _xioCloudRoom = xioCloudRoom;
        _lab1 = lab1;
        //---Trigger Occupancy (Timed)
        _lab1.btnOccupancy_PressEvent += (s, e) =>
        {
            if (e.SigArgs.Sig.BoolValue)
                TriggerOccupiedForDuration(10000);
        };
        
        //---Set Occupancy
        _lab1.btnOccupancyFb_PressEvent += (s, e) =>
        {
            if (e.SigArgs.Sig.BoolValue)
                SetOccupiedState(true);
        };

        _lab1.btnVacancyFb_PressEvent += (s, e) =>
        {
            if (e.SigArgs.Sig.BoolValue)
                SetOccupiedState(false);
        };

        UpdateOccupancy(false);
    }

    public void TriggerOccupiedForDuration(int durationMs)
    {
        if (_isLatched)
        {
            ErrorLog.Notice("Occupancy is latched, ignoring timed trigger.");
            return;
        }

        UpdateOccupancy(true);
        _occupancyTimer?.Stop();
        _occupancyTimer = new CTimer(_ => UpdateOccupancy(false), durationMs);
    }

    public void SetOccupiedState(bool state)
    {
        if (state)
        {
            _isLatched = true;
            _occupancyTimer?.Stop();
            UpdateOccupancy(true);
            ErrorLog.Notice("Occupancy latched ON (occupied).");
        }
        else
        {
            _isLatched = false;
            UpdateOccupancy(false);
            ErrorLog.Notice("Occupancy set to VACANT and latch released (timer allowed).");
        }
    }

    private void UpdateOccupancy(bool occupied)
    {
        _xioCloudRoom.OccupiedFeedback.BoolValue = occupied;
        _lab1.btnOccupancyFb_Selected(occupied);
        _lab1.btnVacancyFb_Selected(!occupied);
    }
}