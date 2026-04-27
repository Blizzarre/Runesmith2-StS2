#region

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Cards;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Field;

#endregion

namespace Runesmith2.Runesmith2Code.Extensions;

public static class CardModelExtension
{
    public static bool CanEnhance(this CardModel cardModel)
    {
        if (cardModel.Type == CardType.Attack) return true;

        if (cardModel.GainsBlock) return true;

        if (cardModel.HasPotency())
            return true;

        return false;
    }

    public class RunesmithCardModelModifier
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
            private set => _justEnhanced = value;
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
                _enhanced = Math.Clamp(value, 0, 999);
                if (_enhanced > 0) JustEnhanced = true;
                EnhanceChanged?.Invoke();
            }
        }

        private bool _isStasis;
        public CardModel CardModel { get; set; }

        public RunesmithCardModelModifier(CardModel cardModel)
        {
            CardModel = cardModel;
        }

        public bool Stasis
        {
            get => _isStasis;
            set
            {
                CardModel.AssertMutable();
                _isStasis = value;
                if (_isStasis) JustStasis = true;
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

    public static RunesmithCardModelModifier GetCardModelModifier(this CardModel cardModel)
    {
        if (RunesmithField.Modifier[cardModel] == null)
            return RunesmithField.Modifier[cardModel] = new RunesmithCardModelModifier(cardModel);
        return RunesmithField.Modifier[cardModel]!;
    }

    public static void AddEnhance(this CardModel cardModel, int amount)
    {
        if (!cardModel.IsMutable) return;
        cardModel.GetCardModelModifier().Enhanced += amount;
    }

    public static bool IsEnhanced(this CardModel cardModel)
    {
        return cardModel.GetCardModelModifier().Enhanced > 0;
    }

    public static int GetEnhance(this CardModel cardModel)
    {
        return cardModel.GetCardModelModifier().Enhanced;
    }

    public static decimal GetEnhanceMultiplier(this CardModel cardModel)
    {
        if (cardModel is ICardEnhanceMult cardEnhanceMult)
            return 0.5m * cardModel.GetCardModelModifier().Enhanced * cardEnhanceMult.EnhanceMult;

        return 0.5m * cardModel.GetCardModelModifier().Enhanced;
    }

    public static void ClearEnhance(this CardModel cardModel)
    {
        if (!cardModel.IsMutable) return;
        cardModel.GetCardModelModifier().Enhanced = 0;
    }

    public static void SetStasis(this CardModel cardModel, bool stasis)
    {
        if (!cardModel.IsMutable) return;
        cardModel.GetCardModelModifier().Stasis = stasis;
    }

    public static bool IsStasis(this CardModel cardModel)
    {
        return cardModel.GetCardModelModifier().Stasis;
    }

    public static bool HasPotency(this CardModel cardModel)
    {
        return cardModel.DynamicVars.ContainsKey(PotencyVar.defaultName) &&
               cardModel.DynamicVars[PotencyVar.defaultName].BaseValue > 0;
    }
}