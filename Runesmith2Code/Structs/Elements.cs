namespace Runesmith2.Runesmith2Code.Structs;

public struct Elements(int ignis, int terra, int aqua) : IEquatable<Elements>
{
    public Elements() : this(0, 0, 0)
    {
    }
    
    public Elements(int cost) : this(cost, cost, cost)
    {
    }

    public int Ignis { get; private set; } = ignis;

    public int Terra { get; private set; } = terra;

    public int Aqua { get; private set; } = aqua;

    public void SetElements(int ignis, int terra, int aqua)
    {
        Ignis = ignis;
        Terra = terra;
        Aqua = aqua;
    }

    public int Total => Ignis + Terra + Aqua;

    public static Elements operator +(Elements a) => a;
    public static Elements operator -(Elements a) => new(-a.Ignis, -a.Terra, -a.Aqua);

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