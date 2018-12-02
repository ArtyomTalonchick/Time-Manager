using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace TimeManager
{
    public delegate void NoteEventHandler(object s, EventArgs e);

    public class NoteView : ContentView
	{
        private Switch switcher;
        private Entry entry;
        private Button button;

        public event NoteEventHandler Clicked;

        public bool ValueOfSwitch
        {
            get => switcher.IsToggled;
            set => switcher.IsToggled = value;
        }
        public string Text
        {
            get => entry.Text;
            set => entry.Text = value;
        }

        public NoteView ()
		{
            InitializeComponent();            
		}

        private void InitializeComponent()
        {
            switcher = new Switch { HorizontalOptions = LayoutOptions.Start };
            entry = new Entry { HorizontalOptions=LayoutOptions.Fill };
            button = new Button { Image = "drawable/close_30_30.png", HeightRequest = 40, WidthRequest = 40, BackgroundColor=Color.Transparent, HorizontalOptions=LayoutOptions.End };
            switcher.Toggled += (s, e) => switcher_Toggled();
            button.Clicked += (s, e) => Clicked?.Invoke(s, e);

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.Children.Add(switcher, 0, 0);
            grid.Children.Add(entry, 1, 0);
            grid.Children.Add(button, 2, 0);

            Content = grid;
        }

        public override string ToString() => Text;

        private void switcher_Toggled()
        {
            if (switcher.IsToggled == true) 
            {
                entry.IsEnabled = false;
                button.IsEnabled = false;
            }
            else
            {
                entry.IsEnabled = true;
                button.IsEnabled = true;
            }
        }
    }
}