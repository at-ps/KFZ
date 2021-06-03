using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAcess;
using CommonType;
using BusinessLogic.Events;
using System.ComponentModel;
using System.Data;

namespace BusinessLogic
{
    public class KFZCollectionModel
    {
        private BackgroundWorker _bwThread;
        IODBcs dbAcesssql;
        public List<KFZModel> Kfzlist { get; set; } = new List<KFZModel>(); // property
        public event KFZReadyEventHandler KFZReady;
        public KFZReadyEventHandler KFZReady2;
        public event InfoEventHandler Infos;
        public event KFZStateChangedEventHandler KFZStateChanged;

        public KFZCollectionModel()
        {
            dbAcesssql = new MariaDB();
            _bwThread = new BackgroundWorker();
            _bwThread.DoWork += ThreadMethode;
            _bwThread.WorkerSupportsCancellation = true;
            _bwThread.RunWorkerAsync();

            //getKFZfromDB();


        }

        public void ThreadMethode(object sender, DoWorkEventArgs e)
        {
            //Daten von DB holen
            // Daten mit KfZliste vergleichen
            //KFZStatechangedEvent werfen
            while (true)
            {
                List<KFZCT> kfzfromDB = dbAcesssql.getkfzlist2();

                if (Kfzlist.Count > 0)
                {
                    List<KFZCT> kfzliste = new List<KFZCT>();

                    foreach (var item in Kfzlist)
                    {
                        KFZCT kfzCT = new KFZCT
                        {
                            FahrgestellNr = item.FahrgestellNr,
                            Kennzeichnen = item.Kennzeichnen,
                            Idkfz = item.Idkfz,
                            Leistung = item.Leistung,
                            Typ = item.Typ

                        }; kfzliste.Add(kfzCT);

                    }

                    if (kfzliste.Count == kfzfromDB.Count)
                    {

                        for (int i = 0; i < kfzliste.Count; i++)
                        {

                            if (!kfzliste[i].IsEqual(kfzfromDB[i]))
                            {
                                //KFZCT vw = new KFZCT();
                                //vw.Typ = "VW POLO";
                                
                                //KFZCT ford = new KFZCT();
                                //ford.Typ = "Ford Ranger";
                                //vw.IsEqual(ford);

                                //vw.ConsoleWriteTyp(); // -> vw;
                                //                      //  Console.WriteLine(this.Typ);
                                //Console.WriteLine(vw.Typ);


                                //ford.ConsoleWriteTyp(); // -> ford;

                                //int leben = 42;
                                //int result = auto.multiply(2, 3);
                                //Console.Write(result);

                                if (KFZStateChanged != null)
                                {
                                    KFZStateChanged(E_kfzstate.eKFZChanged, kfzfromDB[i]);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (kfzliste.Count > kfzfromDB.Count) // Daten im DB wurde gelöscht
                        {


                            //kfzliste: 1,8,9
                            //kfzfromDB: 1,8,9,10

                            for (int i = 0; i < kfzliste.Count; i++)
                            {
                                bool kfzfromDBcontains = false;
                                for (int y = 0; y < kfzfromDB.Count; y++)
                                {
                                    if (kfzfromDB[y].Idkfz == kfzliste[i].Idkfz)
                                    {
                                        kfzfromDBcontains = true;
                                    }
                                }
                                if (!kfzfromDBcontains && KFZStateChanged != null)
                                {
                                    KFZStateChanged(E_kfzstate.eKFZDeleted, kfzliste[i]);
                                }
                            }
                        }
                        if (kfzliste.Count < kfzfromDB.Count) // insert
                        {
                            for (int i = 0; i < kfzfromDB.Count; i++)
                            {
                                bool kfzlistscontain = false;
                                for (int y = 0; y < kfzliste.Count; y++)
                                {
                                    if (kfzfromDB[i].Idkfz == kfzliste[y].Idkfz)
                                    {
                                        kfzlistscontain = true;
                                    }
                                }
                                if ( !kfzlistscontain && KFZStateChanged != null)
                                {
                                    KFZStateChanged(E_kfzstate.eKFZNew, kfzfromDB[i]);
                                }
                            }
                        }
                    }
                }
                System.Threading.Thread.Sleep(5000);

            }
        }


        public void getKFZfromDB()
        {

            List<KFZCT> kfz = dbAcesssql.getkfzlist2();  //KFZ von CommonType  List<KFZCT> => Variable
            Kfzlist.Clear();
            foreach (var item in kfz)
            {
                KFZModel km = new KFZModel
                {
                    FahrgestellNr = item.FahrgestellNr,
                    Kennzeichnen = item.Kennzeichnen,
                    Idkfz = item.Idkfz,
                    Leistung = item.Leistung,
                    Typ = item.Typ
                };  // tao Objekt 

                Kfzlist.Add(km);

                if (KFZReady != null)
                {
                    KFZReady(Kfzlist);
                }


            }
        }


        public bool delKFZfromDB()
        {
            return dbAcesssql.delKFZList();

        }

        public bool delKFZbyID(long id)
        {
            return dbAcesssql.delKFZbyID(id);

        }

        public void insertkfz(KFZModel kFZ)
        {
            KFZCT neukfz = new KFZCT();
            neukfz.Idkfz = kFZ.Idkfz;
            neukfz.Kennzeichnen = kFZ.Kennzeichnen;
            neukfz.Leistung = kFZ.Leistung;
            neukfz.FahrgestellNr = kFZ.FahrgestellNr;
            neukfz.Typ = kFZ.Typ;
            dbAcesssql.insertKFZList(neukfz);
        }

        public void updatekfz(KFZModel kFZ)
        {
            KFZCT updatekfz = new KFZCT();
            updatekfz.Idkfz = kFZ.Idkfz;
            updatekfz.Kennzeichnen = kFZ.Kennzeichnen;
            updatekfz.Leistung = kFZ.Leistung;
            updatekfz.FahrgestellNr = kFZ.FahrgestellNr;
            updatekfz.Typ = kFZ.Typ;
            dbAcesssql.updateKFZList(updatekfz);
        }
    }
}
