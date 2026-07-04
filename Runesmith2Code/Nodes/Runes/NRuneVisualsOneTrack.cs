namespace Runesmith2.Runesmith2Code.Nodes.Runes;

public partial class NRuneVisualsOneTrack : NRuneVisuals
{
    private float _currTime;

    protected override Action CustomTrigger => () =>
    {
        var track = SpineAnimation.GetCurrentTrack();
        if (track?.GetAnimation().GetName() == "idle_loop")
            _currTime = track.GetTrackTime();
        SpineAnimation.SetAnimation("trigger", false);
    };

    protected override void OnTriggerCompleted()
    {
        // Restore idle animation
        var track0Data = TrackDict[0];
        SpineAnimation.SetAnimation("idle_loop");
        var track = SpineAnimation.GetCurrentTrack();
        if (track == null) return;
        track.SetTrackTime(_currTime);
        track.SetTimeScale(track0Data.TimeScale);
        track0Data.Track = track;
    }
}