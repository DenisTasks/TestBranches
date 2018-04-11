using GalaSoft.MvvmLight;
using Model.Entities;
using Model.ModelService;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ViewModel.Interfaces;

namespace ViewModel.Print
{
    public class PrintViewModel : ViewModelBase, IViewModel
    {
        private ObservableCollection<Appointment> _appointments;

        public PrintViewModel()
        {
            FillTable();
        }

        public ObservableCollection<Appointment> Appointments
        {
            get => _appointments;
            private set
            {
                if (value != _appointments)
                {
                    _appointments = value;
                    base.RaisePropertyChanged();
                }
            }
        }

        public void FillTable()
        {
           using(var uow = new UnitOfWork())
           {
                Appointments = new ObservableCollection<Appointment>(uow.Appointments.Get());
           }
        }



        private void PrintBtn_Click(object sender, RoutedEventArgs e)

        {

            PrintDialog printDialog = new PrintDialog();


            if (printDialog.ShowDialog() == true)

            {

                //printDialog.PrintVisual(grid, "My First Print Job");

            }
        }

        

    }

}

