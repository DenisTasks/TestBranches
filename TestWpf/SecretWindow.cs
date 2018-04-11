using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViewModel.Interfaces;

namespace TestWpf
{
    public partial class SecretWindow :  Window , IView
    {
        public SecretWindow()
        {
            InitializeComponent();
        }
        

        public IViewModel ViewModel
        {
            get
            {
                return DataContext as IViewModel;
            }
            set
            {
                DataContext = value;
            }
        }
    }
}

