
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
            List<DzienPracy> DniPracyRcp1 = new List<DzienPracy>();
            List<DzienPracy> DniPracyRcp2 = new List<DzienPracy>();
            List<Pracownik> Pracownicy = new List<Pracownik>();
            TimeSpan ts = TimeSpan.Parse(String.Format("{0:00:00}","13:43"));
            //Pobieranie danych z Excela
            DataTable rcp1 = ConvertCsvToDataTable("../../RcpFiles/rcp1.csv");
            DataTable rcp2 = ConvertCsvToDataTable("../../RcpFiles/rcp2.CSV");
            int licznikWierszyRcp1 = 0;
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
            for (int i = 0; i < rcp2.Rows.Count-1; i++)                     
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
            DniPracyRcp1.Count();
            DniPracyRcp2.Count();
            Console.ReadLine();
            Console.ReadKey();

            //string kodPoprzedniegoPracownika = string.Empty;
            //foreach (var dzienPracyRcp1 in DniPracyRcp1)
            //{
            //    )
            //    {

            //    }
            //    Pracownicy.Add(new Pracownik
            //    {
            //        KodPracownika = dzienPracyRcp1.KodPracownika
            //    });
            //    if (kodPoprzedniegoPracownika == dzienPracyRcp1.KodPracownika)
            //    {
            //        Pracownicy.Where(p => p.KodPracownika == dzienPracyRcp1.KodPracownika).SingleOrDefault().DniPracy.a;
            //    }
            //    Pracownicy.Add(new Pracownik
            //    {
            //        DniPracy = DniPracyRcp1.Where(p => p.KodPracownika == )
            //    })
            //        kodPoprzedniegoPracownika = dzienPracyRcp1.KodPracownika;
            //}
            //foreach (var item in Pracownicy)
            //{
            //    Pracownicy.Add(new Pracownik
            //    {
            //        DniPracy = DniPracyRcp1.Where(p => p.KodPracownika == )
            //    })
            //    Console.WriteLine();
            //}
        }
      


      
        public static DataTable ConvertCsvToDataTable(string filePath)
        {
            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(filePath))
            {
                string[] headers = sr.ReadLine().Split(';');
                foreach (string header in headers)
                {
                    dt.Columns.Add(header);
                }
                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split(';');
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dr[i] = rows[i];
                    }
                    dt.Rows.Add(dr);
                }

            }
            return dt;
        }
    }
}
