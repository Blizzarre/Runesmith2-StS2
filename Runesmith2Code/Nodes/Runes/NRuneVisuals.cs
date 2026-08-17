#region

using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Random;
using Runesmith2.Runesmith2Code.Extensions;

#endregion

namespace Runesmith2.Runesmith2Code.Nodes.Runes;

[GlobalClass]
public partial class NRuneVisuals : Node2D
{
    private bool _isTriggering;

    private NRuneParticlesContainer? _backParticles;

    private NRuneParticlesContainer? _frontParticles;

    private CpuParticles2D? _breakParticles;

    private bool _hasTrigger;

    private Color _targetModulate = Colors.White;

    private float _targetTimeScale = 1f;

    protected readonly Dictionary<int, TrackData> TrackDict = [];

    private int _triggerTrack = 1;

    private Tween? _timeScaleTween;
    private Tween? _depletedTween;
    private Node2D? _visual;
    private MegaSprite? _spineVisuals;

    protected virtual Action? CustomTrigger => null;

    protected virtual MixBlend TriggerMixBlend => MixBlend.MixBlend_Add;

    private bool IsSpineNode
    {
        get
        {
            if (IsInstanceValid(_visual)) return _visual.GetClass() == "SpineSprite";
            return false;
        }
    }

    protected SpineAnimationAccess SpineAnimation => new(_spineVisuals);

    private float CurrTimeScale
    {
        get;
        set
        {
            field = value;
            SetAllTimeScale(field);
        }
    }

    public override void _Ready()
    {
        _visual = GetNode<Node2D>("%Visuals");
        _backParticles = GetNode<NRuneParticlesContainer>("%BackParticles");
        _frontParticles = GetNode<NRuneParticlesContainer>("%FrontParticles");
        _breakParticles = GetNode<CpuParticles2D>("%BreakParticles");

        if (IsSpineNode)
        {
            _spineVisuals = new MegaSprite(_visual);
            if (_spineVisuals.GetSkeleton()?.GetData() == null)
            {
                GD.PushWarning($"Spine skeleton data failed to load for {Name}, disabling spine animation.");
                _spineVisuals = null;
            }

            _spineVisuals?.ConnectAnimationEvent(
                Callable.From<GodotObject, GodotObject, GodotObject, GodotObject>(OnAnimationEvent));
            _spineVisuals?.ConnectAnimationCompleted(
                Callable.From<GodotObject, GodotObject, GodotObject>(OnAnimationCompleted));
        }

        OnStart();
    }

    private void OnAnimationCompleted(GodotObject _, GodotObject __, GodotObject trackEntry)
    {
        var track = new MegaTrackEntry(trackEntry);
        if (track.GetAnimation().GetName() != "trigger") return;
        OnTriggerCompleted();
        TriggerFinished();
    }

    protected virtual void OnTriggerCompleted()
    {
    }

    private void OnAnimationEvent(GodotObject _, GodotObject __, GodotObject ___, GodotObject spineEvent)
    {
        if (new MegaEvent(spineEvent).GetData().GetEventName() == "trigger_end") OnTriggerEnd();
    }

    private void OnStart()
    {
        var skelData = _spineVisuals?.GetSkeleton()?.GetData();
        if (skelData == null) return;

        var animNames = skelData.GetAnimationNames().ToHashSet();

        _hasTrigger = animNames.Contains("trigger");

        // default option, don't call spine functions too much
        if (animNames.Contains("idle_loop"))
        {
            SpineAnimation.SetAnimation("idle_loop", true);
            var track = SpineAnimation.GetCurrentTrack(0);
            var timeScale = Rng.Chaotic.NextFloat(0.9f, 1.1f);
            if (track == null) return;
            track.SetTimeScale(timeScale);
            track.SetTrackTime(Rng.Chaotic.NextFloat() * track.GetAnimationEnd());
            TrackDict.Add(0, new TrackData(track, timeScale));
            return;
        }

        // Use layered animation if there's idle/loop_N
        var index = 0;
        while (true)
        {
            var name = $"idle/loop_{index + 1}";
            if (animNames.Contains(name))
            {
                SpineAnimation.SetAnimation(name, true, index);
                var track = SpineAnimation.GetCurrentTrack(index);
                var timeScale = Rng.Chaotic.NextFloat(0.9f, 1.1f);
                if (track != null)
                {
                    track.SetTimeScale(timeScale);
                    track.SetTrackTime(Rng.Chaotic.NextFloat() * track.GetAnimationEnd());
                    TrackDict.Add(index, new TrackData(track, timeScale));
                }
            }
            else
            {
                _triggerTrack = index + 1;
                return;
            }

            index++;
        }
    }

    // Called when the Rune is triggered from any source
    public void OnTrigger()
    {
        _backParticles?.Restart();
        if (!_hasTrigger && CustomTrigger == null) return;

        _isTriggering = true;
        if (CustomTrigger == null)
        {
            SpineAnimation.SetAnimation("trigger", false, _triggerTrack);
            var track = SpineAnimation.GetCurrentTrack(_triggerTrack);
            track?.SetMixBlend(TriggerMixBlend);
            SpineAnimation.GetAnimationState()?.AddEmptyAnimation(_triggerTrack);
        }
        else
        {
            CustomTrigger();
        }
    }

    // Called when trigger_end event has been emitted from animation. Used for triggering particles.
    // Can also be called manually if no event was included
    protected void OnTriggerEnd()
    {
        _frontParticles?.PlayOneShot();
    }

    public void OnBreak()
    {
        _breakParticles?.Restart();
        _frontParticles?.PlayOneShot();
        _timeScaleTween?.Kill();
        _depletedTween?.Kill();
        _visual?.Visible = false;
    }

    protected void SetAnimTimeScaleTween(Tween tween)
    {
        if (_timeScaleTween != null && _timeScaleTween.IsRunning()) _timeScaleTween.Kill();
        _timeScaleTween = tween;
    }

    private void SetAllTimeScale(float scale)
    {
        foreach (var data in TrackDict.Values) data.Track.SetTimeScale(scale * data.TimeScale);
    }

    protected void TriggerFinished()
    {
        _isTriggering = false;

        UpdateChargeVisualAfterAnimationFinished();
    }

    // Change visual based on charge status. If trigger anim and/or anim scale tween is playing, waits until all are finished playing before darkening.
    private void UpdateChargeVisualAfterAnimationFinished()
    {
        if (_isTriggering) return;

        if (_depletedTween != null && _depletedTween.IsRunning()) _depletedTween.Kill();
        _depletedTween = CreateTween();
        _depletedTween.SetParallel();
        _depletedTween.TweenProperty(_visual, "modulate", _targetModulate, 0.1);
        _depletedTween.TweenProperty(this, "CurrTimeScale", _targetTimeScale, 0.1);
    }

    private bool _currChargeDepleted;


    public void SetChargeStatus(bool isDepleted, Color darkenedColor)
    {
        if (isDepleted == _currChargeDepleted) return;

        _targetModulate = isDepleted ? darkenedColor : Colors.White;
        _targetTimeScale = isDepleted ? 0.35f : 1;
        _currChargeDepleted = isDepleted;

        UpdateChargeVisualAfterAnimationFinished();
    }

    protected class TrackData(MegaTrackEntry track, float timeScale)
    {
        internal MegaTrackEntry Track { get; set; } = track;
        internal float TimeScale => timeScale;
    }
}