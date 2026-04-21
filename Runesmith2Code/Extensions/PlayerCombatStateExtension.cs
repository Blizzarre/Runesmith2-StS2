using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using Runesmith2.Runesmith2Code.Entities.Runes;
using Runesmith2.Runesmith2Code.Structs;

namespace Runesmith2.Runesmith2Code.Extensions;

public static class PlayerCombatStateExtension
{
    public class RunesmithCombatState(PlayerCombatState combatState, RuneQueue queue)
    {
        private PlayerCombatState _combatState = combatState;

        public Elements Elements
        {
            get;
            set
            {
                if (field == value) return;
                var elements = field;
                field = value;
                CombatManager.Instance.History.ElementsModified(_combatState._player.Creature.CombatState,
                    field - elements, _combatState._player);
                ElementsChanged?.Invoke(elements, field);
            }
        } = new();

        private RuneQueue _runeQueue = queue;

        public RuneQueue RuneQueue => _runeQueue;

        // todo subscribe to this
        public event Action<Elements, Elements>? ElementsChanged;

        public void GainElements(Elements amount)
        {
            if (amount.Total < 0)
            {
                throw new ArgumentException("Must not be negative", nameof(amount));
            }

            Elements = (Elements + amount).ClampZero();
        }

        public void LoseElements(Elements amount)
        {
            if (amount.Total < 0)
            {
                throw new ArgumentException("Must not be negative", nameof(amount));
            }

            Elements = (Elements - amount).ClampZero();
        }
    }
}