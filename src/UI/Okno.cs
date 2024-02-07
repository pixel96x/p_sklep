using Mindmagma.Curses;

namespace UI;

public abstract class Okno
{
    protected int Wiersz { get; set; }
    protected int Kolumna { get; set; }
    protected int MaxWierszy { get; set; }
    protected int MaxKolumn { get; set; }

    public abstract void Wyswietl();

    public void DrukujMaxText(string tekst, int wiersz, int kolumna, out int y, out int x)
    {
        NCurses.GetMaxYX(Program.Ekran, out int maxWierszy, out int maxKolumn);
        MaxWierszy = maxWierszy;
        MaxKolumn = maxKolumn;

        int maxDlugosc = MaxKolumn - kolumna - 1;
        if (maxDlugosc < 1)
        {
            NCurses.GetYX(Program.Ekran, out y, out x);
            Wiersz = y;
            Kolumna = x;
            return;
        }

        var tekst2 = tekst.Substring(0, maxDlugosc < tekst.Length ? maxDlugosc : tekst.Length);
        NCurses.MoveAddString(wiersz, kolumna, tekst2);

        NCurses.GetYX(Program.Ekran, out y, out x);
        Wiersz = y;
        Kolumna = x;
    }
}
