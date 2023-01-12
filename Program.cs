
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZadanieProgramistyczne
{
    class Program
    {
        static void Main(string[] args)
        {
            // inicjalizacja potrzebnych obiektów oraz zmiennych
            List<DzienPracy> DniPracyRcp1 = new List<DzienPracy>();
            List<DzienPracy> DniPracyRcp2 = new List<DzienPracy>();
            List<Pracownik> Pracownicy = new List<Pracownik>();

            TimeSpan ts = TimeSpan.Parse(String.Format("{0:00:00}", "13:43"));

            //Pobieranie danych z Excela
            DataTable rcp1 = KonwertujCsvDoDataTable("../../RcpFiles/rcp1.csv"); //cieżki do plików 
            DataTable rcp2 = KonwertujCsvDoDataTable("../../RcpFiles/rcp2.CSV");

            int licznikWierszyRcp1 = 0; //zmienna pomocnicza do wypisywania logów błędów
            //Mapowanie danych z plików Excel
            MapowanieDanychRcp1(DniPracyRcp1, rcp1, licznikWierszyRcp1);
            MapowanieDanychRcp2(DniPracyRcp2, rcp2);

            //Wczytywanie pracowników dla rcp1
            Pracownik pracownik = new Pracownik();
            string kodPoprzedniegoPracownikaRcp1 = DniPracyRcp1.First().KodPracownika; //zmienna pomocnicza do porównywania kodu ostanio przeglądanego pracownika
            DodajPracownikow(DniPracyRcp1, Pracownicy, pracownik, kodPoprzedniegoPracownikaRcp1);
            //Wczytywanie pracowników dla rcp2
            string kodPoprzedniegoPracownikaRcp2 = DniPracyRcp2.First().KodPracownika; //zmienna pomocnicza do porównywania kodu ostanio przeglądanego pracownika
            DodajPracownikow(DniPracyRcp2, Pracownicy, pracownik, kodPoprzedniegoPracownikaRcp2);

            //Wypisanie pracowników w konsoli
            WypiszPracownikow(Pracownicy);
           
        }

        private static void WypiszPracownikow(List<Pracownik> Pracownicy)
        {
            foreach (var p in Pracownicy)
            {
                Console.WriteLine(p.KodPracownika + ": " + Environment.NewLine);
                foreach (var d in p.DniPracy)
                {
                    Console.WriteLine("data: " + String.Format("{0:MM/dd/yyyy}", d.Data) + " " + d.GodzinaWejscia + " - " + d.GodzinaWyjscia + "\n");
                }
            }
            Console.ReadLine();
            Console.ReadKey();
        }

        private static void MapowanieDanychRcp2(List<DzienPracy> DniPracyRcp2, DataTable rcp2)
        {
            for (int i = 0; i < rcp2.Rows.Count - 1; i++)
            {
                try
                {
                    DniPracyRcp2.Add(new DzienPracy
                    {
                        KodPracownika = (string)rcp2.Rows[i].ItemArray[0],
                        Data = Convert.ToDateTime(rcp2.Rows[i].ItemArray[1]),
                        GodzinaWejscia = TimeSpan.Parse(String.Format("{0:00:00}", rcp2.Rows[i].ItemArray[2])),
                        GodzinaWyjscia = TimeSpan.Parse(String.Format("{0:00:00}", rcp2.Rows[i + 1].ItemArray[2].ToString()))
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine("błąd w lini: " + i.ToString() + " " + ex.Message);
                }

            }
        }

        private static void MapowanieDanychRcp1(List<DzienPracy> DniPracyRcp1, DataTable rcp1, int licznikWierszyRcp1)
        {
            foreach (DataRow dr in rcp1.Rows)
            {
                try
                {
                    DniPracyRcp1.Add(new DzienPracy
                    {
                        KodPracownika = (string)dr[0],
                        Data = Convert.ToDateTime(dr[1]),
                        GodzinaWejscia = TimeSpan.Parse(dr[2].ToString()),
                        GodzinaWyjscia = TimeSpan.Parse(dr[3].ToString())
                    });
                }
                catch (Exception ex)
                {

                    Console.WriteLine("błąd w lini: " + licznikWierszyRcp1.ToString() + " " + ex.Message);
                }
                licznikWierszyRcp1++;
            }          
        }

        private static void DodajPracownikow( List<DzienPracy> DniPracy, List<Pracownik> Pracownicy, Pracownik pracownik, string kodPoprzedniegoPracownika)
        {
            pracownik.DniPracy = new List<DzienPracy>();
            foreach (var dzienPracy in DniPracy)
            {
                if (kodPoprzedniegoPracownika == dzienPracy.KodPracownika)
                {
                    PrzypiszDzienPracy(pracownik, dzienPracy);
                }
                else
                {
                    pracownik.KodPracownika = kodPoprzedniegoPracownika;
                    Pracownicy.Add(pracownik);
                    pracownik = new Pracownik();
                    pracownik.DniPracy = new List<DzienPracy>();
                    PrzypiszDzienPracy(pracownik, dzienPracy);
                }
                kodPoprzedniegoPracownika = dzienPracy.KodPracownika;
            }         
        }

        private static void PrzypiszDzienPracy(Pracownik pracownik, DzienPracy dzienPracyRcp)
        {
            try
            {
                DzienPracy dzienPracy = new DzienPracy
                {
                    KodPracownika = dzienPracyRcp.KodPracownika,
                    Data = dzienPracyRcp.Data,
                    GodzinaWejscia = dzienPracyRcp.GodzinaWejscia,
                    GodzinaWyjscia = dzienPracyRcp.GodzinaWyjscia
                };
                pracownik.DniPracy.Add(dzienPracy);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Nie udało dodać dnia pracy - " + String.Format("{0:MM/dd/yyyy}", dzienPracyRcp.Data));
            }
           
        }


        // konwertowanie Csv na DataTable
        public static DataTable KonwertujCsvDoDataTable(string sciezka)
        {
            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(sciezka))
            {
                string[] naglowki = sr.ReadLine().Split(';');
                foreach (string naglowek in naglowki)
                {
                    dt.Columns.Add(naglowek);
                }
                while (!sr.EndOfStream)
                {
                    string[] linie = sr.ReadLine().Split(';');
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < naglowki.Length; i++)
                    {
                        dr[i] = linie[i];
                    }
                    dt.Rows.Add(dr);
                }

            }
            return dt;
        }
    }
}
