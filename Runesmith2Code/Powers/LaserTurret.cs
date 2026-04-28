#region

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.Hooks;
using Runesmith2.Runesmith2Code.Structs;

#endregion

namespace Runesmith2.Runesmith2Code.Powers;

public class LaserTurret : Runesmith2Power, IAfterElementsGained
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public async Task AfterElementsGained(ICombatState combatState, Elements amount, Player player, CardPlay? cardPlay = null)
    {
        if (player == Owner.Player)
        {
            Flash();
            await Cmd.CustomScaledWait(0.12f, 0.24f);
            foreach (Creature hittableEnemy in CombatState.HittableEnemies)
            {
                VfxCmd.PlayOnCreatureCenter(hittableEnemy, "vfx/vfx_attack_blunt");
            }
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), CombatState.HittableEnemies, amount.Total * Amount, ValueProp.Unpowered, Owner);
        }
    }
}