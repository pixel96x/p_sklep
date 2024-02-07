using System.Collections;
using System.Collections.Generic;

namespace Aplikacja;

public struct ElementyKoszyka
{
    public Towar Element;
    public uint Ilosc;
}

public sealed class Koszyk : IEnumerable<ElementyKoszyka>
{
    readonly List<ElementyKoszyka> _towary = [];

    public IEnumerator<ElementyKoszyka> GetEnumerator()
    {
        foreach (var towar in _towary)
        {
            yield return towar;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Dodaj(Towar towar)
    {
        if (_towary.Capacity == 0)
        {
            var element = new ElementyKoszyka() { Element = towar, Ilosc = 1};
            _towary.Add(element);
            return;
        }

        for (int i = 0; i < _towary.Capacity; ++i)
        {
            var element = _towary[i];

            if (element.Element == towar)
            {
                ++element.Ilosc;
                return;
            }
        }
    }

    public void Usun(uint IdTowaru)
    {
        if (_towary.Capacity == 0)
            return;

        for (int i = 0; i < _towary.Capacity; ++i)
        {
            var element = _towary[i];

            if (element.Element.ID == IdTowaru)
            {
                --element.Ilosc;
                if (element.Ilosc < 1)
                    _towary.Remove(element);

                return;
            }
        }
    }
}
