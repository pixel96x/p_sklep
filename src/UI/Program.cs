using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using CsvHelper;
using Microsoft.Data.Sqlite;
using Mindmagma.Curses;

namespace UI;

public static class Program
{
    public static bool Dzialaj { get; set; } = true;
    public static SqliteConnection? Polaczenie { get; set; }
    public static nint Ekran { get; private set; }

    public static void Main()
    {
        Inicjalizacja();

        Ekran = NCurses.InitScreen();
        NCurses.Keypad(Ekran, true);
        NCurses.Raw();
        var okno = new OknoGlowne();
        okno.Wyswietl();
        NCurses.EndWin();

        Czyszczenie();
    }

    private static void Gui()
    {
        var ekran = NCurses.InitScreen();
        NCurses.NoEcho();
        NCurses.Keypad(ekran, true);
        NCurses.Raw();
        var col = 0;
        var row = 0;

        NCurses.WindowBorder(ekran, '|', '|', '-', '-', '+', '+', '+', '+');
        NCurses.Move(++col, ++row);
        NCurses.Refresh();

        int c;
        while (true)
        {
            c = NCurses.GetChar();

            // 27 - Escape
            if (c == 27)
                break;

            NCurses.MoveWindowAddString(ekran, ++col, 1, $" Wprowadzono {(char)c} ");
            NCurses.Refresh();
        }

        NCurses.MoveWindowAddString(ekran, ++col, 1, "Wcisnij dowolny klawisz aby wyjsc...");
        NCurses.GetChar();
        NCurses.EndWin();
    }

    private static void Inicjalizacja()
    {
        CultureInfo.CurrentCulture = new CultureInfo("pl-PL");

        var sciezkaBazy = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "SklepBaza.db"
        );

        bool utworzBaze = false;
        if (!File.Exists(sciezkaBazy))
            // brak domyślnej bazy
            utworzBaze = true;

        Polaczenie = new SqliteConnection($"Data Source={sciezkaBazy}");
        Polaczenie.Open();
        if (utworzBaze)
            WstawDomyslneDaneBazy();
    }

    private static void Czyszczenie()
    {
        Polaczenie?.Close();
        Polaczenie = null;
    }

    private static void WstawDomyslneDaneBazy()
    {
        if (Polaczenie is null)
            throw new NullReferenceException();

        // towary
        var komenda = Polaczenie.CreateCommand();
        komenda.CommandText =
        @"
            CREATE TABLE towary(
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                kod TEXT NOT NULL,
                nazwa TEXT NOT NULL,
                cena_zakupu MONEY,
                cena_sprzedazy MONEY,
                ilosc INTEGER
            );
        ";
        komenda.ExecuteNonQuery();

        komenda.CommandText =
        @"
            INSERT INTO towary (kod, nazwa, cena_zakupu, cena_sprzedazy, ilosc)
            VALUES
                ('100-01-415', 'Wiertarka', 150.00, 220.00, 10),
                ('100-03-003', 'Szlifierka', 250.00, 350.00, 20),
                ('200-07-678', 'Pilarka', 890.00, 1199.00, 5),
                ('500-04-010', 'Wiertlo SDS fi 10', 55.00, 99.99, 48),
                ('500-04-018', 'Wiertlo SDS fi 18', 75.00, 124.99, 21),
                ('300-03-100', 'Kompresor 100L', 1900.00, 2499.99, 2);
        ";
        komenda.ExecuteNonQuery();
    }
}
