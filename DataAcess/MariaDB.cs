using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonType;
using Mk.DBConnector;
using DataAcess.Events;
using System.Data;

namespace DataAcess
{
    public class MariaDB : IODBcs
    {
        bool _connection;
        long typ;

        DBAdapter _dbsql;
        public event InfosMessageEventHandler infoMessage;
        public event ErrorMessageEH ErrorMessage;

        public MariaDB()
        {
            connection();
        }
        public bool connection()
        {

            _dbsql = new DBAdapter(DatabaseType.MySql, Instance.NewInstance, "localhost", 3306, "kfzmysql", "root", "", "MySql.log");
            _dbsql.Adapter.LogFile = true;
            try
            {
                _connection = true;

            }
            catch (Exception)
            {

                _connection = false;
            }
            return _connection;
        }

        public List<KFZCT> getKFZList()
        {
            List<KFZTYP> kFZTYPs = getKFZTyp();

            List<KFZCT> daten = new List<KFZCT>();
            // string sql = string.Format("SELECT * FROM kfz;");
            string sql = string.Format("SELECT * FROM kfz ;");
            DataTable dt = _dbsql.Adapter.GetDataTable(sql);

            foreach (DataRow r in dt.Rows)
            {
                KFZCT k = new KFZCT();
                k.Idkfz = long.Parse(r[0].ToString());
                k.FahrgestellNr = Convert.ToString(r[1]);
                k.Kennzeichnen = r[2].ToString();
                k.Leistung = Convert.ToInt32(r[3]);
                k.Typ = r[4].ToString();

                daten.Add(k);
            }
            return daten;


        }


        public List<KFZCT> getkfzlist2()
        {
            //List<KFZTYP> kFZTYPs = getKFZTyp();

            List<KFZCT> daten = new List<KFZCT>();
            // string sql = string.Format("SELECT * FROM kfz;");
            string sql = string.Format("SELECT kfz.FahrgestellNr, kfz.idkfz, kfz.Kennzeichen, kfz.Leistung, kfztyp.Typ FROM kfz inner join kfztyp on kfz.Typ = kfztyp.idkfztyp; "); ;
            DataTable dt = _dbsql.Adapter.GetDataTable(sql);

            foreach (DataRow r in dt.Rows)
            {
                KFZCT k = new KFZCT();
                k.Idkfz = long.Parse(r["idkfz"].ToString());
                k.FahrgestellNr = Convert.ToString(r[1]);
                k.Kennzeichnen = r[2].ToString();
                k.Leistung = Convert.ToInt32(r[3]);
                k.Typ = r[4].ToString();

                daten.Add(k);
            }
            return daten;



        }
        public List<KFZTYP> getKFZTyp()
        {
            List<KFZTYP> kfztyp = new List<KFZTYP>();
            string sql2 = string.Format("SELECT * FROM kfztyp");
            DataTable dt2 = _dbsql.Adapter.GetDataTable(sql2);
            foreach (DataRow r2 in dt2.Rows)
            {
                KFZTYP k2 = new KFZTYP();
                k2.idkfztyp = long.Parse(r2[0].ToString());
                k2.typ = r2[1].ToString();
                kfztyp.Add(k2);
            }
            return kfztyp;
        }



        public bool delKFZList()
        {

            string sql = string.Format("DELETE FROM kfz Where idkfz = 2;");

            try
            {
                _dbsql.Adapter.ExecuteSQL(sql);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool delKFZbyID(long id)
        {
            string sql = string.Format($"DELETE FROM kfz Where idkfz = {id};");

            try
            {
                _dbsql.Adapter.ExecuteSQL(sql);
                infoMessage?.Invoke($"{id} erfolgreich gelöscht");
                return true;
            }
            catch (Exception ex)
            {
                if (ErrorMessage != null)
                {
                    ErrorMessage($"{id}" + ex.Message);
                }
                return false;
            }

        }

       
        public void insertKFZList(KFZCT neukfz)
        {
            kfztyp(neukfz);
            string sql = string.Format($"INSERT INTO `kfz`(`idkfz`, `FahrgestellNr`, `Kennzeichen`, `Leistung`, `Typ`) VALUES('{neukfz.Idkfz}','{neukfz.FahrgestellNr}','{neukfz.Kennzeichnen}','{neukfz.Leistung}','{typ}');");
            try
            {
                _dbsql.Adapter.ExecuteSQL(sql);
                infoMessage?.Invoke($" erfolgreich gelöscht");
               
            }
            catch (Exception ex)
            {
                if (ErrorMessage != null)
                {
                    ErrorMessage(ex.Message);
                }
               
            }

        }

        public void kfztyp(KFZCT neukfz)
        {
            List<KFZTYP> kfztyp = new List<KFZTYP>();
            string sql = string.Format($"SELECT * FROM kfztyp;");
            
            DataTable dt = _dbsql.Adapter.GetDataTable(sql);
            foreach (DataRow r2 in dt.Rows)
            {
                KFZTYP k2 = new KFZTYP();
                k2.idkfztyp = long.Parse(r2[0].ToString());
                k2.typ = r2[1].ToString();
                kfztyp.Add(k2);

            }

            foreach (var item in kfztyp)
            {
                if (item.typ == neukfz.Typ)
                {
                    typ = item.idkfztyp;
                }
             
            }
          

        }

        void IODBcs.updateKFZList(KFZCT updatekfz)
        {
            kfztyp(updatekfz);
            string sql = string.Format($"UPDATE `kfz` SET  `FahrgestellNr`= '{updatekfz.FahrgestellNr}', `Kennzeichen` = '{updatekfz.Kennzeichnen}', `Leistung` = '{updatekfz.Leistung}', `Typ` = '{typ}' WHERE (`idkfz` = '{updatekfz.Idkfz}');");
            try
            {
                _dbsql.Adapter.ExecuteSQL(sql);
                infoMessage?.Invoke($" erfolgreich gelöscht");

            }
            catch (Exception ex)
            {
                if (ErrorMessage != null)
                {
                    ErrorMessage(ex.Message);
                }

            }
        }
    }

}