using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Field;

namespace Runesmith2.Runesmith2Code.Extensions;

public static class CardModelExtension
{
    public static bool IsEnhanceable(this CardModel cardModel)
    {
        // TODO weird checks. Need better way to check for damage and block values
        if (cardModel.Type == CardType.Attack)
        {
            return true;
        }

        if (cardModel.GainsBlock)
        {
            return true;
        }

        // TODO: Rename vars for better compatibility (RunesmithPotencyVar)
        if (cardModel.DynamicVars.ContainsKey(PotencyVar.defaultName) && cardModel.DynamicVars[nameof(PotencyVar)].BaseValue > 0)
        {
            return true;
        }
        
        return false;
    }
    
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
                cardModel.AssertMutable();
                _enhanced = Math.Clamp(value, 0, 999);
                if (_enhanced > 0) JustEnhanced = true;
                EnhanceChanged?.Invoke();
            }
        }
        
        private bool _isStasis;
        public bool Stasis
        {
            get => _isStasis;
            set
            {
                cardModel.AssertMutable();
                _isStasis = value;
                if (_isStasis) JustStasis = true;
                StasisChanged?.Invoke();
            }
        }
        
        public event Action? EnhanceChanged;
        public event Action? StasisChanged;
    }

    public static RunesmithCardModelModifier GetCardModelModifier(this CardModel cardModel)
    {
        if (RunesmithField.Modifier[cardModel] == null) return RunesmithField.Modifier[cardModel] = new RunesmithCardModelModifier(cardModel);
        return RunesmithField.Modifier[cardModel]!;
    }

    public static void AddEnhance(this CardModel cardModel, int amount)
    {
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
    
    public static void ClearEnhance(this CardModel cardModel)
    {
        cardModel.GetCardModelModifier().Enhanced = 0;
    }

    public static void SetStasis(this CardModel cardModel, bool stasis)
    {
        cardModel.GetCardModelModifier().Stasis = stasis;
    }
    
    public static bool IsStasis(this CardModel cardModel)
    {
        return cardModel.GetCardModelModifier().Stasis;
    }
}