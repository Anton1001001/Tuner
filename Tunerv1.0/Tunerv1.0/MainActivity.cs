using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Button = Xamarin.Forms.Button;

namespace Tunerv1._0
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
        }
    }
    public partial class MainPage : ContentPage
    {
        private StackLayout buttonLayout;

        public MainPage()
        {

            // Create top layout with six buttons initially
            buttonLayout = CreateButtonLayout(6);

            // Create middle layout for drawing space
            var drawingSpace = new BoxView
            {
                BackgroundColor = Color.White,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };

            // Create bottom layout with two comboboxes
            var comboBoxLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.End,
                Padding = new Thickness(10, 5),
                Children =
                {
                    CreateComboBox(1),
                    CreateComboBox(2)
                }
            };

            // Create main layout
            var mainLayout = new StackLayout
            {
                Children =
                {
                    buttonLayout,
                    drawingSpace,
                    comboBoxLayout
                }
            };

            Content = mainLayout;
        }

        private StackLayout CreateButtonLayout(int numberOfButtons)
        {
            var layout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Start,
                Padding = new Thickness(10, 5)
            };

            for (int i = 0; i < numberOfButtons; i++)
            {
                layout.Children.Add(new Button { Text = $"Button {i + 1}" });
            }

            return layout;
        }

        private Picker CreateComboBox(int index)
        {
            var comboBox = new Picker
            {
                Title = $"ComboBox {index}",
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            // Add options to the combobox
            comboBox.Items.Add("Option 1");
            comboBox.Items.Add("Option 2");
            comboBox.Items.Add("Option 3");

            // Handle selection change event
            comboBox.SelectedIndexChanged += (sender, args) =>
            {

                var selectedOption = comboBox.Items[comboBox.SelectedIndex];
                UpdateButtonLayout(selectedOption);
            };

            return comboBox;
        }

        private void UpdateButtonLayout(string selectedOption)
        {
            // Determine number of buttons based on the selected option
            int numberOfButtons;
            switch (selectedOption)
            {
                case "Option 1":
                    numberOfButtons = 3;
                    break;
                case "Option 2":
                    numberOfButtons = 5;
                    break;
                case "Option 3":
                    numberOfButtons = 8;
                    break;
                default:
                    numberOfButtons = 6;
                    break;
            }

            // Remove existing buttons from layout
            buttonLayout.Children.Clear();

            // Add new buttons to layout
            buttonLayout = CreateButtonLayout(numberOfButtons);
        }
    }
}
