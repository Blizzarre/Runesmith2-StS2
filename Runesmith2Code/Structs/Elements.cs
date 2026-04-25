using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.DynamicVars;

namespace Runesmith2.Runesmith2Code.Structs;

public struct Elements(int ignis, int terra, int aqua) : IEquatable<Elements>
{
    public Elements() : this(0, 0, 0)
    {
    }

    public Elements(int cost) : this(cost, cost, cost)
    {
    }

    /// <summary>
    /// Construct Elements struct with Elements values in the DynamicVars of the provided CardModel
    /// </summary>
    /// <param name="cardModel"></param>
    public Elements(CardModel cardModel) : this()
    {
        var elements = cardModel.DynamicVars.TryGetValue(ElementsVar.defaultName, out var var) ? var.IntValue : 0;
        if (elements > 0)
        {
            Ignis = Terra = Aqua = elements;
            return;
        }

        Ignis = cardModel.DynamicVars.TryGetValue(IgnisVar.defaultName, out var var1) ? var1.IntValue : 0;
        Terra = cardModel.DynamicVars.TryGetValue(TerraVar.defaultName, out var var2) ? var2.IntValue : 0;
        Aqua = cardModel.DynamicVars.TryGetValue(AquaVar.defaultName, out var var3) ? var3.IntValue : 0;
    }

    public static Elements WithIgnis(int cost)
    {
        return new Elements(cost, 0, 0);
    }

    public static Elements WithTerra(int cost)
    {
        return new Elements(0, cost, 0);
    }

    public static Elements WithAqua(int cost)
    {
        return new Elements(0, 0, cost);
    }

    public int ByIndex(int index)
    {
        return index switch
        {
            0 => Ignis,
            1 => Terra,
            2 => Aqua,
            _ => 0
        };
    }

    public int Ignis { get; set; } = ignis;

    public int Terra { get; set; } = terra;

    public int Aqua { get; set; } = aqua;

    public void SetElements(int ignis, int terra, int aqua)
    {
        Ignis = ignis;
        Terra = terra;
        Aqua = aqua;
    }

    public int Total => Ignis + Terra + Aqua;

    public static Elements operator +(Elements a)
    {
        return a;
    }

    public static Elements operator -(Elements a)
    {
        return new Elements(-a.Ignis, -a.Terra, -a.Aqua);
    }

    public static Elements operator +(Elements a, Elements b)
    {
        return new Elements(a.Ignis + b.Ignis, a.Terra + b.Terra, a.Aqua + b.Aqua);
    }

    public static Elements operator -(Elements a, Elements b)
    {
        return new Elements(a.Ignis - b.Ignis, a.Terra - b.Terra, a.Aqua - b.Aqua);
    }

    public static bool operator ==(Elements a, Elements b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(Elements a, Elements b)
    {
        return !(a == b);
    }

    public bool Equals(Elements other)
    {
        return Ignis == other.Ignis && Terra == other.Terra && Aqua == other.Aqua;
    }

    public override bool Equals(object? obj)
    {
        return obj is Elements other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Ignis, Terra, Aqua);
    }

    public override string ToString()
    {
        return $"ignis {Ignis}-terra {Terra}-aqua {Aqua}";
    }

    public Elements ClampZero()
    {
        Ignis = Math.Max(Ignis, 0);
        Terra = Math.Max(Terra, 0);
        Aqua = Math.Max(Aqua, 0);

        return this;
    }

    public void Max(Elements other)
    {
        Ignis = Math.Max(Ignis, other.Ignis);
        Terra = Math.Max(Terra, other.Terra);
        Aqua = Math.Max(Aqua, other.Aqua);
    }

    public bool CanSpend(Elements other)
    {
        return Ignis >= other.Ignis && Terra >= other.Terra && Aqua >= other.Aqua;
    }
}