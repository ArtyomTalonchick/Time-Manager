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
    public partial class TimeTable : ContentPage
    {
        private Grid noteGrid;
        private Button outOfNoteButton;
        private Editor noteEditor;
        private ToolbarItem changeTb;

        private TimeItems timeItems { get; set; }

        public TimeTable(TimeItems _timeItems)
        {
            InitializeComponent();
            InitializeNote();
            timeItems = _timeItems;

            changeTb = new ToolbarItem
            {
                Text = "Изменить",
                Order = ToolbarItemOrder.Primary,
                Priority = 0,
            };
            changeTb.Clicked += async (s,e) => await Navigation.PushAsync(new ChangeTimeTablePage(timeItems));
            ToolbarItems.Add(changeTb);


            ListOfTimeItem.ItemsSource = timeItems;                    
            ListOfTimeItem.ItemTemplate = new DataTemplate(typeof(TimeItemView));
            ListOfTimeItem.ItemSelected += (s, e) =>
            {
                ItemEditor.BindingContext = ((TimeItem)ListOfTimeItem.SelectedItem);
                ItemEditor.SetBinding(Editor.TextProperty, "Note");
            };



        }

        private void InitializeNote()
        {
            noteEditor = new Editor();
            noteEditor.BindingContext = ItemEditor;
            noteEditor.SetBinding(Editor.TextProperty, "Text");
            outOfNoteButton = new Button { Text = "Ок" };
            outOfNoteButton.Clicked += (s, e) =>
            {
                this.Content = GridOfTimeTable;
                Title = "Расписание";
                ToolbarItems.Add(changeTb);
            };
            noteGrid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength (10, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength (1, GridUnitType.Star) },
                }
            };
            noteGrid.Children.Add(noteEditor, 0, 0);
            noteGrid.Children.Add(outOfNoteButton, 0, 1);
        }

        private void ItemEditor_Focused(object s, EventArgs e)
        {
            Title = ((TimeItem)ListOfTimeItem.SelectedItem).Name;
            ToolbarItems.Clear();
            this.Content = noteGrid;
        }
    }
}