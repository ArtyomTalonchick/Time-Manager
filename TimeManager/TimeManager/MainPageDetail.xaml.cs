using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TimeManager
{
[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPageDetail : ContentPage
    {       
        public MainPageDetail()
        {
            InitializeComponent();
            TimeLabel.Text = (DateTime.Now).ToShortTimeString();
            DateLabel.Text = (DateTime.Today).ToLongDateString();
        }
    }
}