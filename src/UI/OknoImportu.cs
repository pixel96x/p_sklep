using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Data.Sqlite;
using Mindmagma.Curses;

namespace UI;

public sealed class OknoImportu : Okno
{
    public override void Wyswietl()
    {
        bool dzialaj = true;

        while (dzialaj)
        {
            NCurses.Clear();
            NCurses.Move(0, 0);

            if (Program.Polaczenie == null)
                throw new NullReferenceException();

            string folderDomowy = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            string sciezka = "";

            bool pytajPlikDzialaj = true;
            while (pytajPlikDzialaj)
            {
                NCurses.Clear();
                DrukujMaxText("Podaj nazwe pliku: ", Wiersz, 0, out _, out _);
                NCurses.Refresh();
                var sb = new StringBuilder();
                NCurses.GetString(sb);

                if (sb.Length < 1)
                {
                    return;
                }

                sciezka = Path.Join(folderDomowy, sb.ToString());

                if (!File.Exists(sciezka))
                {
                    NCurses.Clear();
                    Wiersz = 0;
                    DrukujMaxText("Plik o tej nazwie nie istnieje!", Wiersz, 0, out _, out _);
                    NCurses.Refresh();
                    NCurses.GetChar();
                    continue;
                }

                pytajPlikDzialaj = false;
            }

            SqliteCommand komenda;
            try
            {
                using var czytnik = new StreamReader(sciezka);
                var konfiguracjaCSV = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "," };
                using var czytnikCSV = new CsvReader(czytnik, konfiguracjaCSV);
                var lista = czytnikCSV.GetRecords<Aplikacja.ElementCSV>().ToList();

                if (lista.Capacity < 1)
                {
                    NCurses.Clear();
                    Wiersz = 0;
                    DrukujMaxText("Importowanie danych z pliku nie powiodo sie", Wiersz, 0, out _, out _);
                    NCurses.Refresh();
                    NCurses.GetChar();
                    return;
                }

                if (Program.Polaczenie == null)
                    throw new NullReferenceException();

                var sb2 = new StringBuilder();
                int i = lista.Capacity - 2;
                foreach (var element in lista)
                {
                    var str = string.Format(
                        CultureInfo.InvariantCulture, "('{0}', '{1}', {2:F}, {3:F}, {4}){5}",
                        element.Kod, element.Nazwa, element.CenaZakupu, element.CenaSprzedazy, element.Ilosc,
                        i > 1 ? "," : "");
                    sb2.Append(str);
                    --i;
                }

                komenda = Program.Polaczenie.CreateCommand();
                komenda.CommandText =
                $@"
                    BEGIN TRANSACTION;

                        DROP TABLE IF EXISTS towary;

                        CREATE TABLE towary (
                            id INTEGER PRIMARY KEY AUTOINCREMENT,
                            kod TEXT NOT NULL,
                            nazwa TEXT NOT NULL,
                            cena_zakupu MONEY,
                            cena_sprzedazy MONEY,
                            ilosc INTEGER
                        );

                        INSERT INTO towary (kod, nazwa, cena_zakupu, cena_sprzedazy, ilosc)
                        VALUES {sb2.ToString()};

                    COMMIT;
                ";
                Debug.WriteLine(komenda.CommandText);
                komenda.ExecuteNonQuery();
            }
            catch (SqliteException ex)
            {
                Debug.WriteLine($"{ex.SqliteErrorCode}: {ex.Message}");
                NCurses.Clear();
                Wiersz = 0;
                DrukujMaxText("Importowanie nie powiodlo sie", Wiersz, 0, out _, out _);
                NCurses.Refresh();
                NCurses.GetChar();
                return;
            }
            catch (Exception)
            {
                NCurses.Clear();
                Wiersz = 0;
                DrukujMaxText("Importowanie nie powiodlo sie", Wiersz, 0, out _, out _);
                NCurses.Refresh();
                NCurses.GetChar();
                return;
            }

            NCurses.Clear();
            Wiersz = 0;
            DrukujMaxText("Pomyslnie zaimportowano dane: ", Wiersz, 0, out _, out _);
            NCurses.Refresh();
            NCurses.GetChar();

            return;
        }
    }
}
