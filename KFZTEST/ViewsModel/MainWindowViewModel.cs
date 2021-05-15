using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandHelper;
using System.ComponentModel;
using BusinessLogic;
using BusinessLogic.Events;
using System.Windows.Input;
using CommonType;
using System.Collections.ObjectModel;
using System.Windows;

namespace KFZTEST.ViewsModel
{
    class MainWindowViewModel : INotifyPropertyChanged  // -> 
    {
        public event PropertyChangedEventHandler PropertyChanged;
        string _infomsg;
        public string Infosmessage
        {
            get { return _infomsg; }
            set
            {
                _infomsg = value;

                if (_infomsg != null)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Infosmessage"));
                }
            }

        }
        KFZCollectionModel _kfzCollModel; // Backend field, variable

        KFZModel _selectedItem;  // Feld


        public ObservableCollection<KFZModel> kfzOC { get; set; } = new ObservableCollection<KFZModel>();
        public KFZModel SelectedItem // methode
        {
            get { return _selectedItem; }

            set
            {
                _selectedItem = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedItem")); // Selected muss Property Changed
            }
        }


        private void _kfzCollModel_KFZReady(List<KFZModel> list)
        {
            kfzOC.Clear();
            foreach (var item in list) // thong qua variable _kfzCollModel lay list tu KFZCModell
            {
                kfzOC.Add(item);
            }
        }



        public ICommand GetAllDataCommand { get; set; }
        public ICommand DeleteAllDataCommand { get; set; }
        public ICommand InsertDataCommand { get; set; }
        public ICommand UpdateDataCommand { get; set; }
        public ICommand _viewLoadCommand { get; set; }




        public MainWindowViewModel()
        {


            _kfzCollModel = new KFZCollectionModel();
            _kfzCollModel.KFZStateChanged += KFZUpdate;
            _kfzCollModel.KFZReady += _kfzCollModel_KFZReady;
            _kfzCollModel.Infos += _kfzCollModel_Infos;
            GetAllDataCommand = new RelayCommand(c => GetAllData());
            DeleteAllDataCommand = new RelayCommand(c => DeleteKFZData());
            InsertDataCommand = new RelayCommand(c => InsertKFZData());
            UpdateDataCommand = new RelayCommand(c => UpdateKFZData());


        }

        private void KFZUpdate(E_kfzstate kfzs, KFZCT k)
        {


            switch (kfzs)
            {
                case E_kfzstate.eKFZNew:
                    Infosmessage = $"{kfzs}: {k.Idkfz}, Kennzeichen {k.Kennzeichnen} insert";
                    break;
                case E_kfzstate.eKFZChanged:
                    Infosmessage = $"{kfzs}: Das KFZ Mit ID {k.Idkfz} hat ihren Daten geändert";
                    break;
                case E_kfzstate.eKFZDeleted:
                    Infosmessage = $"{kfzs}:Das KFZ Mit ID {k.Idkfz} wurde gelöscht";
                    break;
                default:
                    break;
            }
        }
        private void _kfzCollModel_Infos(string msg)
        {
            Infosmessage = msg;
        }


        private void UpdateKFZData()
        {
            KFZModel neukfz = new KFZModel();
            neukfz.Kennzeichnen = SelectedItem.Kennzeichnen;
            neukfz.Idkfz = SelectedItem.Idkfz;
            neukfz.Leistung = SelectedItem.Leistung;
            neukfz.Typ = SelectedItem.Typ;
            neukfz.FahrgestellNr = SelectedItem.FahrgestellNr;
            _kfzCollModel.updatekfz(neukfz);

        }
        private void InsertKFZData()
        {
            KFZModel neukfz = new KFZModel();
            neukfz.Kennzeichnen = SelectedItem.Kennzeichnen;
            neukfz.Idkfz = SelectedItem.Idkfz;
            neukfz.Leistung = SelectedItem.Leistung;
            neukfz.Typ = SelectedItem.Typ;
            neukfz.FahrgestellNr = SelectedItem.FahrgestellNr;


            _kfzCollModel.insertkfz(neukfz);
        }
        private void GetAllData()
        {


            _kfzCollModel.getKFZfromDB();

            //kfzOC.Clear();
            //foreach (var item in _kfzCollModel.Kfzlist) // thong qua variable _kfzCollModel lay list tu KFZCModell
            //{
            //    kfzOC.Add(item);
            //}
        }



        private void DeleteAllData()
        {


            bool result;
            result = _kfzCollModel.delKFZfromDB();

            if (result == true)
            {
                GetAllData();
                MessageBox.Show("Sucess");
            }
            else
            { MessageBox.Show("Unsucess"); }
        }


        private void DeleteKFZData()
        {
            _kfzCollModel.delKFZbyID(SelectedItem.Idkfz);
            kfzOC.Remove(SelectedItem);


        }

    }
}