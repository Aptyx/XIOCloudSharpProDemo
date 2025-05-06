using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using MEP621_XPanel;

namespace AptyxXIOCloudSSharpDemo;

internal class SourceManager
{
    private readonly XioCloudRoomSlot _xioCloudRoom;
    private readonly ILab0 _lab0;
    internal SourceManager(XioCloudRoomSlot xioCloudRoom, ILab0 lab0)
    {
        _xioCloudRoom = xioCloudRoom;
        _lab0 = lab0;
        _lab0.btnSource1_PressEvent += (s, e) =>
        {
            if (e.SigArgs.Sig.BoolValue)
                SelectSource(XioCloudRoomSlot.eCurrentSource.AirMedia);
        };

        _lab0.btnSource2_PressEvent += (s, e) =>
        {
            if (e.SigArgs.Sig.BoolValue)
                SelectSource(XioCloudRoomSlot.eCurrentSource.Laptop);
        };

        _lab0.btnSource3_PressEvent += (s, e) =>
        {
            if (e.SigArgs.Sig.BoolValue)
                SelectSource(XioCloudRoomSlot.eCurrentSource.DigitalSignage);
        };
    }

    private void SelectSource(XioCloudRoomSlot.eCurrentSource source)
    {
        _xioCloudRoom.CurrentSourceFeedback.Value = source;
        ErrorLog.Notice($"Source changed to: {source}");
        
        _lab0.btnSource1_Selected(false);
        _lab0.btnSource2_Selected(false);
        _lab0.btnSource3_Selected(false);
        
        switch (source)
        {
            case XioCloudRoomSlot.eCurrentSource.AirMedia:
                _lab0.btnSource1_Selected(true);
                break;
            case XioCloudRoomSlot.eCurrentSource.Laptop:
                _lab0.btnSource2_Selected(true);
                break;
            case XioCloudRoomSlot.eCurrentSource.DigitalSignage:
                _lab0.btnSource3_Selected(true);
                break;
        }
    }
}