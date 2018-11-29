using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace TimeManager
{
	public class TimeItemView : ViewCell
    {
       
        public TimeItemView ()
		{
            InitializeView();

        }

        public void InitializeView()
        {
            var startLb = new Label { TextColor = Color.FromHex("F44336"), FontSize = 16, HorizontalOptions = LayoutOptions.Center };
            startLb.SetBinding(Label.TextProperty, "Start");
            var finishLb = new Label { TextColor = Color.FromHex("42A5F5"), FontSize = 12, HorizontalOptions = LayoutOptions.Center };
            finishLb.SetBinding(Label.TextProperty, "Finish");
            var nameLb = new Label { TextColor = Color.FromHex("424242"), FontSize = 32, VerticalOptions = LayoutOptions.Center };
            nameLb.SetBinding(Label.TextProperty, "Name");
            var grid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength (1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength (6, GridUnitType.Star) },
                },
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength (1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength (1, GridUnitType.Star) },
                },
            };
            grid.Children.Add(startLb, 0, 0);
            grid.Children.Add(finishLb, 0, 1);
            grid.Children.Add(nameLb, 1, 0);
            Grid.SetRowSpan(nameLb, 2);

            View = grid;
        }
    }
}