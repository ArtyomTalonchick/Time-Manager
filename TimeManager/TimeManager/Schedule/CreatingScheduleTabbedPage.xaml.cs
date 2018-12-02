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
    public partial class CreatingScheduleTabbedPage : TabbedPage
    {
        public CreatingScheduleTabbedPage ()
        {
            InitializeComponent();
            Children.Add(new ChangeTimeTablePage { Title = "Изменить день" });
            Children.Add(new AddDayPatternPage { Title = "Шаблон дня" });

        }
    }
}