using System;
using System.Text;
using Mindmagma.Curses;

namespace UI;

public sealed class OknoWyszukiwania : Okno
{
    public override void Wyswietl()
    {
        bool dzialaj = true;

        while (dzialaj)
        {
            NCurses.Clear();
            Wiersz = 0;
            NCurses.MoveAddString(Wiersz, 0, "Podaj fraze: ");
            NCurses.Refresh();

            var sb = new StringBuilder();
            NCurses.GetString(sb);

            if (sb.Length < 1)
            {
                dzialaj = false;
                continue;
            }

            if (Program.Polaczenie == null)
                throw new NullReferenceException();

            var komenda = Program.Polaczenie.CreateCommand();
            komenda.CommandText =
            @$"
                SELECT * FROM towary
                WHERE kod LIKE '%{sb}%'
                OR nazwa LIKE '%{sb}%';
            ";

            using (var czytnikSQL = komenda.ExecuteReader())
            {
                if (!czytnikSQL.HasRows)
                {
                    NCurses.MoveAddString(++Wiersz, 0, "Brak wynikow");
                    NCurses.Refresh();
                    NCurses.GetChar();
                    continue;
                }

                NCurses.Clear();
                Wiersz = 0;
                NCurses.Move(Wiersz, 0);

                while (czytnikSQL.Read())
                {
                    var kod = czytnikSQL.GetString(1);
                    var nazwa = czytnikSQL.GetString(2);
                    var zakup = czytnikSQL.GetDecimal(3);
                    var sprzedaz = czytnikSQL.GetDecimal(4);
                    var ilosc = czytnikSQL.GetInt32(5);

                    NCurses.MoveAddString(Wiersz++, 0, $"{kod}, {nazwa}, {zakup}, {sprzedaz}, {ilosc}");
                }

                NCurses.Refresh();
                NCurses.GetChar();
            }
        }
    }
}
