using System;
using CommonType;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Events // ohne class
{
    public enum E_kfzstate
    {
        eKFZNew,
        eKFZChanged,
        eKFZDeleted

    }

    public delegate void KFZReadyEventHandler(List<KFZModel> list);
    public delegate void InfoEventHandler(string msg);
    public delegate void KFZStateChangedEventHandler(E_kfzstate kfzs, KFZCT kfz);
}
