using System;

namespace Aplikacja;

public sealed class Towar
{
    public uint ID { get; set; }
    public string? Kod { get; set; }
    public string? Nazwa { get; set; }
    public decimal CenaZakupu { get; set; }
    public decimal CenaSprzedazy { get; set; }
    public float Marza
    {
        get { return (float) (CenaSprzedazy / CenaZakupu); }
    }
}
