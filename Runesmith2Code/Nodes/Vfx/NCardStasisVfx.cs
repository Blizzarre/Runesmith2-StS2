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

public partial class NCardStasisVfx : Control
{
    private static readonly string ScenePath = "vfx_card_stasis".ScenePath("vfx/cards");

    private NCard _cardNode = null!;
    private Node2D _particles = null!;

    private float _particlesDuration = 0.50f;

    public override void _Ready()
    {
        _particles = GetNode<Node2D>("%Particles");
    }

    public static NCardStasisVfx? Create(
        NCard cardNode
    )
    {
        if (TestMode.IsOn)
            return null;
        var child = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NCardStasisVfx>();
        child._cardNode = cardNode;
        cardNode.CardVfxContainer.AddChildSafely(child);
        return child;
    }

    private void RestartParticles()
    {
        _particles.GetChildren();
        foreach (var node in _particles.GetChildren())
            if (node is GpuParticles2D particles)
                particles.Restart();
    }

    public async Task PlayAnimation()
    {
        RestartParticles();
        RunesmithModSounds.PlayStasisSfx();
        if (!await WaitAndInterruptIfNecessary(_particlesDuration, _cardNode))
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