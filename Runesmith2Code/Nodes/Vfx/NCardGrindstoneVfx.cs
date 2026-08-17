using BaseLib.Audio;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.TestSupport;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Utils;

namespace Runesmith2.Runesmith2Code.Nodes.Vfx;

public partial class NCardGrindstoneVfx : Control
{
    private static readonly string ScenePath = "vfx_card_grindstone".ScenePath("vfx/cards");

    private TextureRect _dissolve = null!;

    private TextureRect _cardGlowContainer = null!;

    private Node2D _scrollingParticles = null!;

    private Node2D _particles = null!;

    private Sprite2D _lineGlow = null!;

    private ShaderMaterial _dissolveMaterial = null!;

    private Tween? _tween;

    private NCard _cardNode = null!;

    private CardModel _endCard = null!;

    private static Vector2 _originalCardScale = new(1f, 1f);

    public override void _Ready()
    {
        _dissolve = GetNode<TextureRect>("%Dissolve");
        _cardGlowContainer = GetNode<TextureRect>("%CardGlowContainer");
        _scrollingParticles = GetNode<Node2D>("%ScrollingParticles");
        _particles = GetNode<Node2D>("%Particles");
        _lineGlow = GetNode<Sprite2D>("%LineGlow");

        _dissolve.Visible = false;
        _cardGlowContainer.Visible = false;
        _scrollingParticles.Visible = false;
        SetParticlesEmitting(false);

        _dissolveMaterial = (ShaderMaterial)_dissolve.Material;
        _dissolveMaterial.SetShaderParameter("erosion_tex_offset",
            new Vector2(Rng.Chaotic.NextFloat(), Rng.Chaotic.NextFloat()));
    }

    private void SetParticlesEmitting(bool emitting)
    {
        _particles.GetChildren();
        foreach (var node in _particles.GetChildren())
            if (node is GpuParticles2D particles)
                particles.Emitting = emitting;
    }

    public static NCardGrindstoneVfx? Create(
        NCard cardNode,
        CardModel endCard
    )
    {
        if (TestMode.IsOn)
            return null;
        var child = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NCardGrindstoneVfx>();
        child._cardNode = cardNode;
        child._endCard = endCard;
        cardNode.CardVfxContainer.AddChildSafely(child);
        return child;
    }

    // public override void _ExitTree() => _tween?.Kill();

    private async Task<bool> WaitAndInterruptIfNecessary(float seconds, NCard cardNode)
    {
        var currTime = 0f;
        while (currTime <= seconds)
        {
            if (!cardNode.IsInsideTree() || _endCard.Pile == null) return false;
            currTime += await this.AwaitProcessFrame();
        }

        return true;
    }

    private static void UpdateCard(NCard cardNode, CardModel endCard)
    {
        if (endCard.Pile == null) return;
        if (cardNode.Model != null) NPlayerHand.Instance?.TryCancelCardPlay(cardNode.Model);
        cardNode.Model = endCard;
        cardNode.UpdateVisuals(endCard.Pile.Type, CardPreviewMode.Normal);
        if (NCombatRoom.Instance?.Ui.Hand.GetCardHolder(endCard) is NHandCardHolder nHandCardHolder)
            nHandCardHolder.UpdateCard();
    }

    private float _coverShortDuration = 0.20f;
    private float _coverDuration = 0.30f;

    private float _idleShortDuration = 0.08f;
    private float _idleDuration = 0.15f;

    private float _revealShortDuration = 0.20f;
    private float _revealDuration = 0.30f;

    public async Task PlayAnimation(bool shortVersion = false)
    {
        RunesmithModSounds.PlayGrindStoneSfx();

        _dissolve.Visible = true;
        _cardGlowContainer.Visible = true;
        _scrollingParticles.Visible = true;
        SetParticlesEmitting(true);

        var coverDuration = shortVersion ? _coverShortDuration : _coverDuration;
        var idleDuration = shortVersion ? _idleShortDuration : _idleDuration;
        var revealDuration = shortVersion ? _revealShortDuration : _revealDuration;

        _tween = CreateTween().SetParallel();
        _tween.TweenProperty(_scrollingParticles, "position:y", -211f, coverDuration).From(211f)
            .SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Quad);
        _tween.TweenProperty(_dissolveMaterial, "shader_parameter/v_mask", 1f, coverDuration).From(0f)
            .SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Quad);
        _tween.TweenProperty(_cardGlowContainer, "modulate", Colors.White, coverDuration).From(Colors.Transparent);
        _tween.TweenProperty(_lineGlow, "self_modulate", new Color(_lineGlow.Modulate, 0), coverDuration * 0.1)
            .FromCurrent().SetDelay(coverDuration * 0.9)
            .SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Quad);
        _tween.SetParallel(false);
        _tween.TweenCallback(Callable.From(() => SetParticlesEmitting(false)));

        if (!await WaitAndInterruptIfNecessary(coverDuration + idleDuration, _cardNode))
        {
            _cardNode.Scale = _originalCardScale;
            this.QueueFreeSafely();
            return;
        }

        UpdateCard(_cardNode, _endCard);

        _tween = CreateTween().SetParallel();
        _tween.TweenProperty(_dissolveMaterial, "shader_parameter/erosion_time", 1f, revealDuration).From(0f);
        _tween.TweenProperty(_cardGlowContainer, "modulate", Colors.Transparent, revealDuration).From(Colors.White);

        if (!await WaitAndInterruptIfNecessary(revealDuration, _cardNode))
        {
            _cardNode.Scale = _originalCardScale;
            this.QueueFreeSafely();
            return;
        }

        _ = TaskHelper.RunSafely(DelayedFree());
    }

    private async Task DelayedFree()
    {
        await Cmd.Wait(2f);
        this.QueueFreeSafely();
    }
}