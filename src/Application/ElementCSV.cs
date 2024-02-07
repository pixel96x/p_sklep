using System;
using System.Diagnostics;

namespace Aplikacja;

public sealed class ElementCSV
{
    public string? Kod { get; set; }
    public string? Nazwa { get; set; }
    public decimal CenaZakupu { get; set; }
    public decimal CenaSprzedazy { get; set; }
    public int Ilosc { get; set; }
}
