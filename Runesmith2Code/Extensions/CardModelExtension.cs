#region

using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Cards;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Field;

#endregion

namespace Runesmith2.Runesmith2Code.Extensions;

public static class CardModelExtension
{
    public class RunesmithCardModelModifier(CardModel cardModel)
    {
        private bool _justEnhanced;

        public bool JustEnhanced
        {
            get
            {
                var ret = _justEnhanced;
                _justEnhanced = false;
                return ret;
            }
            set => _justEnhanced = value;
        }

        private bool _justStasis;

        public bool JustStasis
        {
            get
            {
                var ret = _justStasis;
                _justStasis = false;
                return ret;
            }
            private set => _justStasis = value;
        }

        private int _enhanced;

        public int Enhanced
        {
            get => _enhanced;
            set
            {
                CardModel.AssertMutable();
                _enhanced = Math.Clamp(value, 0, 999999);
                if (value <= 0) return;
                JustEnhanced = true;
                EnhanceChanged?.Invoke();
            }
        }

        public CardModel CardModel { get; set; } = cardModel;

        public bool Stasis
        {
            get;
            set
            {
                CardModel.AssertMutable();
                field = value;
                if (field) JustStasis = true;
                StasisChanged?.Invoke();
            }
        }

        public RunesmithCardModelModifier Clone(CardModel cardModel)
        {
            var ret = (RunesmithCardModelModifier)MemberwiseClone();
            ret.CardModel = cardModel;
            return ret;
        }

        public void ClearFlags()
        {
            _justEnhanced = false;
            _justStasis = false;
        }

        public event Action? EnhanceChanged;
        public event Action? StasisChanged;
    }

    private static readonly HashSet<string> EnhanceableVarKeys = [BlockVar.defaultName, CalculatedBlockVar.defaultName, DamageVar.defaultName, CalculatedDamageVar.defaultName];

    extension(CardModel card)
    {
        public RunesmithCardModelModifier GetCardModelModifier()
        {
            return RunesmithField.Modifier[card]!;
        }

        public void AddEnhance(int amount, bool skipVisuals = false)
        {
            if (!card.IsMutable) return;
            var modifier = card.GetCardModelModifier();
            modifier.Enhanced += amount;
            if (skipVisuals)
                modifier.JustEnhanced = false;
        }

        public bool IsImproved()
        {
            return card.IsUpgraded || card.Enchantment != null || card.IsEnhanced() ||
                   card.IsStasis();
        }

        public bool IsEnhanced()
        {
            return card.GetCardModelModifier().Enhanced > 0;
        }

        public int GetEnhance()
        {
            return card.GetCardModelModifier().Enhanced;
        }

        public decimal GetEnhanceMultiplier()
        {
            if (card is ICardEnhanceMult cardEnhanceMult)
                return 0.5m * card.GetCardModelModifier().Enhanced * cardEnhanceMult.EnhanceMult;

            return 0.5m * card.GetCardModelModifier().Enhanced;
        }

        public void ClearEnhance()
        {
            if (!card.IsMutable) return;
            card.GetCardModelModifier().Enhanced = 0;
        }

        public void SetStasis(bool stasis)
        {
            if (!card.IsMutable) return;
            if (stasis && card is Runesmith2Card { BlockStasis: true }) return;
            card.GetCardModelModifier().Stasis = stasis;
        }

        public bool IsStasis()
        {
            return card.GetCardModelModifier().Stasis;
        }

        public bool HasPotency()
        {
            return card.DynamicVars.ContainsKey(PotencyVar.defaultName) &&
                   card.DynamicVars[PotencyVar.defaultName].BaseValue > 0;
        }

        public bool CanEnhance()
        {
            if (card.Type == CardType.Attack) return true;

            if (card.GainsBlock) return true;
            
            if (card.HasPotency())
                return true;
            
            // Probably not fool-proof but should help cover cases where Block/Damage is added as enchantment or card modifier 
            if (card.Enchantment != null && card.Enchantment.DynamicVars.Any(c => EnhanceableVarKeys.Contains(c.Key))) return true;
            return card.GetModifiers().Any(cardModifier => cardModifier.DynamicVars.Any(m => EnhanceableVarKeys.Contains(m.Key)));
        }

        public bool CanStasis()
        {
            return card.CanEnhance() && !card.IsStasis();
        }
    }
}