using System;
using Mindmagma.Curses;

namespace UI;

public sealed class OknoBazy : Okno
{
    public override void Wyswietl()
    {
        bool dzialaj = true;

        NCurses.Clear();
        NCurses.Move(0, 0);

        while (dzialaj)
        {
            if (Program.Polaczenie == null)
                throw new NullReferenceException();

            var komenda = Program.Polaczenie.CreateCommand();
            komenda.CommandText =
            @"
                SELECT * FROM towary;
            ";

            using (var czytnikSQL = komenda.ExecuteReader())
            {
                while (czytnikSQL.Read())
                {
                    var kod = czytnikSQL.GetString(1);
                    var nazwa = czytnikSQL.GetString(2);
                    var zakup = czytnikSQL.GetDecimal(3);
                    var sprzedaz = czytnikSQL.GetDecimal(4);
                    var ilosc = czytnikSQL.GetInt32(5);

                    NCurses.MoveAddString(Wiersz++, 0, $"{kod}, {nazwa}, {zakup}, {sprzedaz}, {ilosc}");
                }
            }

            NCurses.GetChar();
            dzialaj = false;
        }
    }
}
