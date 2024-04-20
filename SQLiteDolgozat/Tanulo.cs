using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteDolgozat
{
    internal class Tanulo
    {
        string nev;
        string nem;
        int pontszam;
        string szak;

        public Tanulo(string nev, string nem, int pontszam, string szak)
        {
            this.Nev = nev;
            this.Nem = nem;
            this.Pontszam = pontszam;
            this.Szak = szak;
        }

        public string Nev { get; set; }
        public string Nem { get; set; }
        public int Pontszam { get; set; }
        public string Szak { get; set; }
    }
}
