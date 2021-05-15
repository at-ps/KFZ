using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonType;

namespace DataAcess
{
  public  interface IODBcs
    {
        bool connection();
        bool delKFZbyID(long id);
        bool delKFZList();
        void insertKFZList(KFZCT neukfz);
        void  updateKFZList(KFZCT updatekfz);

        List<KFZCT> getKFZList();  // Methode  lay daten tu DB
        List<KFZCT> getkfzlist2();
    }
}
