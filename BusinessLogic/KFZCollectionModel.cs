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

                            if (kfzfromDB[i].Idkfz != kfzliste[i].Idkfz ||
                                kfzfromDB[i].Kennzeichnen != kfzliste[i].Kennzeichnen ||
                                kfzfromDB[i].Leistung != kfzliste[i].Leistung ||
                                kfzfromDB[i].Typ != kfzliste[i].Typ ||
                                kfzfromDB[i].FahrgestellNr != kfzliste[i].FahrgestellNr)
                            {
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
                            var list1 = kfzliste.Where(i => !kfzfromDB.Contains(i)).ToList();
                            var list2 = kfzfromDB.Where(i => !kfzliste.Contains(i)).ToList();
                           bool r  =  Enumerable.SequenceEqual(list1, list2);
                            if (KFZStateChanged != null)
                                {
                                    //KFZStateChanged(E_kfzstate.eKFZDeleted, list1[i]);
                                }
                   
                            // for (int i = 0; i < kfzliste.Count; i++)
                            // {
                            //     if (kfzfromDB[i].Idkfz != kfzliste[i].Idkfz)
                            //     {
                            //         if (KFZStateChanged != null)
                            //         {
                            //             KFZStateChanged(E_kfzstate.eKFZDeleted, kfzfromDB[i]);
                            //         }
                            //     }
                            //}
                        }
                        if (kfzliste.Count < kfzfromDB.Count) // insert
                        {
                            for (int i = 0; i < kfzfromDB.Count; i++)
                            {
                                var id = kfzfromDB[i].Idkfz;
                                foreach (KFZCT item in kfzliste)
                                {
                                    if (item.Idkfz != id)
                                    {
                                        if (KFZStateChanged != null)
                                        {
                                            KFZStateChanged(E_kfzstate.eKFZNew, kfzfromDB[i]);
                                        }
                                    }

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
