using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Mindmagma.Curses;

namespace UI;

public sealed class OknoEksportu : Okno
{
    public override void Wyswietl()
    {
        bool dzialaj = true;

        NCurses.Clear();
        Wiersz = 0;
        NCurses.Move(Wiersz, 0);

        while (dzialaj)
        {
            if (Program.Polaczenie == null)
                throw new NullReferenceException();

            string folderDomowy = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            string sciezka = "";

            bool pytajPlikDzialaj = true;
            while (pytajPlikDzialaj)
            {
                DrukujMaxText("Podaj nazwe pliku: ", Wiersz, 0, out _, out _);
                NCurses.Refresh();
                var sb = new StringBuilder();
                NCurses.GetString(sb);

                sciezka = Path.Join(folderDomowy, sb.ToString());

                if (File.Exists(sciezka))
                {
                    DrukujMaxText("Plik o takiej samej juz istnieje", Wiersz, 0, out _, out _);
                    Wiersz += 2;
                    DrukujMaxText("Usunac?", Wiersz, 0, out _, out _);
                    DrukujMaxText("T) Tak", ++Wiersz, 0, out _, out _);
                    DrukujMaxText("N) Nie", ++Wiersz, 0, out _, out _);
                    NCurses.Move(++Wiersz, 0);
                    NCurses.Refresh();

                    bool potwierdzDzialaj = true;
                    while (potwierdzDzialaj)
                    {
                        char znak = (char) NCurses.GetChar();

                        switch (znak)
                        {
                            case 't':
                            case 'T':
                                if (UsunPlik(sciezka))
                                {
                                    DrukujMaxText("Pomyslnie usunieto plik", ++Wiersz, 0, out _, out _);
                                    NCurses.Refresh();
                                    pytajPlikDzialaj = false;
                                    NCurses.GetChar();
                                }
                                else
                                {
                                    DrukujMaxText("Blad podczas usuwania pliku", ++Wiersz, 0, out _, out _);
                                    NCurses.Refresh();
                                    NCurses.GetChar();
                                }
                                break;

                            case 'n':
                            case 'N':
                                DrukujMaxText("Anulowano eksportowanie pliku", ++Wiersz, 0, out _, out _);
                                NCurses.Refresh();
                                NCurses.GetChar();
                                return;

                            default:
                                NCurses.Move(Wiersz, 0);
                                NCurses.ClearToEndOfLine();
                                NCurses.Refresh();
                                continue;
                        }

                        potwierdzDzialaj = false;
                    }
                }

                pytajPlikDzialaj = false;
            }

            try
            {
                using var strumien = File.Create(sciezka);
            }
            catch (Exception)
            {
                NCurses.Clear();
                Wiersz = 0;
                DrukujMaxText($"Nie mozna utworzyc pliku \"{folderDomowy}\"", Wiersz, 0, out _, out _);
                DrukujMaxText("Eksportowanie nie powiodlo sie", ++Wiersz, 0, out _, out _);
                NCurses.Refresh();
                NCurses.GetChar();
                return;
            }

            var komenda = Program.Polaczenie.CreateCommand();
            komenda.CommandText =
            @"
                SELECT * FROM towary;
            ";

            var lista = new List<Aplikacja.ElementCSV>();
            using var czytnikSQL = komenda.ExecuteReader();
            while (czytnikSQL.Read())
            {
                var kod = czytnikSQL.GetString(1);
                var nazwa = czytnikSQL.GetString(2);
                var zakup = czytnikSQL.GetDecimal(3);
                var sprzedaz = czytnikSQL.GetDecimal(4);
                var ilosc = czytnikSQL.GetInt32(5);

                lista.Add(new Aplikacja.ElementCSV
                    { Kod = kod, Nazwa = nazwa, CenaZakupu = zakup, CenaSprzedazy = sprzedaz, Ilosc = ilosc });
            }

            using var zapisCSV = new StreamWriter(sciezka);
            var konfiguracjaCSV = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ","};
            using (var csv = new CsvWriter(zapisCSV, konfiguracjaCSV))
            {
                csv.WriteRecords(lista);
            }

            NCurses.Clear();
            Wiersz = 0;
            NCurses.MoveAddString(Wiersz, 0, "Pomyslnie wyeksportowano baze danych");
            NCurses.Refresh();
            NCurses.GetChar();
            dzialaj = false;
        }
    }

    private bool UsunPlik(string sciezka)
    {
        if (!File.Exists(sciezka))
        {
            return false;
        }

        try
        {
            File.Delete(sciezka);
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }
}
