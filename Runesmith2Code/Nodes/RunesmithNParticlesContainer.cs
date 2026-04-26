#region

using Godot;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;

#endregion

namespace Runesmith2.Runesmith2Code.Nodes;

[GlobalClass]
public partial class RunesmithNParticlesContainer : NParticlesContainer
{
    public override void _Ready()
    {
        base._Ready();
        if (_particles != null && _particles.Count != 0) return;
        _particles = [];
        _particles.AddRange(GetChildren().Where(n => n is GpuParticles2D).Cast<GpuParticles2D>());
    }
}