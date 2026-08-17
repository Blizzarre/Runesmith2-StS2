using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.TestSupport;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Utils;

namespace Runesmith2.Runesmith2Code.Nodes.Vfx;

public partial class NCardEnhanceVfx : Control
{
    private static readonly string ScenePath = "vfx_card_enhance".ScenePath("vfx/cards");

    private NCard _cardNode = null!;

    private Node2D _shine = null!;
    private GpuParticles2D _starsParticles = null!;

    private Tween? _tween;


    public override void _Ready()
    {
        _starsParticles = GetNode<GpuParticles2D>("%StarsParticles");
        _shine = GetNode<Node2D>("%Shine");
    }

    public static NCardEnhanceVfx? Create(
        NCard cardNode
    )
    {
        if (TestMode.IsOn)
            return null;
        var child = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NCardEnhanceVfx>();
        child._cardNode = cardNode;
        cardNode.CardVfxContainer.AddChildSafely(child);
        return child;
    }

    private float _particlesDuration = 0.9f;

    private float _coverShortDuration = 0.25f;
    private float _coverDuration = 0.35f;

    public async Task PlayAnimation(bool shortVersion = false)
    {
        RunesmithModSounds.PlayEnhanceSfx();

        const float starsEmittingTime = 0.5f;

        var coverDuration = shortVersion ? _coverShortDuration : _coverDuration;
        _tween = CreateTween().SetParallel();
        _tween.TweenProperty(_shine, "position:y", -155f, coverDuration).From(580f)
            .SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Quad);
        _tween.TweenCallback(Callable.From(() => _starsParticles.Restart()))
            .SetDelay(coverDuration * starsEmittingTime);

        var maxDur = Math.Max(coverDuration * starsEmittingTime + _particlesDuration, coverDuration);

        if (!await WaitAndInterruptIfNecessary(maxDur, _cardNode))
        {
            this.QueueFreeSafely();
            return;
        }

        this.QueueFreeSafely();
    }

    private async Task<bool> WaitAndInterruptIfNecessary(float seconds, NCard cardNode)
    {
        var currTime = 0f;
        while (currTime <= seconds)
        {
            if (!cardNode.IsInsideTree()) return false;
            currTime += await this.AwaitProcessFrame();
        }

        return true;
    }
}