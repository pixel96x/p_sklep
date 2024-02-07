using System;
using Mindmagma.Curses;
using SQLitePCL;

namespace UI;

public sealed class OknoGlowne : Okno
{
    public override void Wyswietl()
    {
        bool dzialaj = true;


        while (dzialaj)
        {
            NCurses.Clear();
            Wiersz = 0;
            DrukujMaxText("1) Wyswietl stan magazynu", Wiersz, 0, out _, out _);
            DrukujMaxText("2) Eksportuj stan magazynu", ++Wiersz, 0, out _, out _);
            DrukujMaxText("3) Importuj stan magazynu", ++Wiersz, 0, out _, out _);
            DrukujMaxText("4) Wyszukiwanie", ++Wiersz, 0, out _, out _);
            DrukujMaxText("0) Wyjdz", ++Wiersz, 0, out _, out _);

            char znak = (char) NCurses.GetChar();

            if (!int.TryParse(znak.ToString(), out int opcja))
                continue;

            switch (opcja)
            {
                case 0:
                    dzialaj = false;
                    break;

                case 1:
                    new OknoBazy().Wyswietl();
                    break;

                case 2:
                    new OknoEksportu().Wyswietl();
                    break;

                case 3:
                    new OknoImportu().Wyswietl();
                    break;

                case 4:
                    new OknoWyszukiwania().Wyswietl();
                    break;

                default:
                    continue;
            }
        }

        NCurses.Clear();
        NCurses.Move(0, 0);
    }
}
