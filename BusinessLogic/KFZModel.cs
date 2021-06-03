using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{

//    public enum KFZ_TYP
//    {
//        Limusine, 
//        Caprio,
//        Kleinwagen
//    }
    public class KFZModel
    {
        bool IsC { get; set; }
        public long Idkfz { get; set; }
        public string FahrgestellNr { get; set; }
        public int Leistung { get; set; }
        public string Typ { get; set; }
        public string Kennzeichnen { get; set; }



        public KFZModel()
        {
            


        }



    }
}

