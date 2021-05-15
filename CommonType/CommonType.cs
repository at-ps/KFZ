using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonType
{  /// <summary>
   ///Struktur KFZ 
   /// </summary>
    public class KFZCT
    {
        public long Idkfz { get; set; }
        public string FahrgestellNr { get; set; }
        public int Leistung { get; set; }
        public string Typ { get; set; }
        public string Kennzeichnen { get; set; }


        public KFZCT()
        {
            
        }

      
        
    }
}
