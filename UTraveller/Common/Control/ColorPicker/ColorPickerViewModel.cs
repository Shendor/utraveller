using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using UTraveller.Common.Message;
using UTraveller.Common.ViewModel;
using UTraveller.Service.Api;

namespace UTraveller.Common.Control
{
    public class ColorPickerViewModel : BaseViewModel
    {
        private IAppPropertiesService appPropertiesService;
        private IUserService userService;

        public ColorPickerViewModel(IAppPropertiesService appPropertiesService, IUserService userService)
        {
            this.appPropertiesService = appPropertiesService;
            this.userService = userService;

            ChooseColorCommand = new ActionCommand(ChooseColor);

            MessengerInstance.Register<ColorChangedMessage>(this, OnColorChanged);
        }

        public void Initialize()
        {
            ColorPalette = new List<SolidColorBrush>();

            var currentUser = userService.GetCurrentUser();
            var properties = appPropertiesService.GetPropertiesForUser(currentUser.Id);

            ColorPalette.Add(new SolidColorBrush(Color.FromArgb(255, 42, 136, 76)));
            ColorPalette.Add(new SolidColorBrush(Color.FromArgb(255, 172, 40, 64)));
            ColorPalette.Add(new SolidColorBrush(Color.FromArgb(255, 87, 150, 212)));
            ColorPalette.Add(new SolidColorBrush(Color.FromArgb(255, 189, 96, 64)));
            ColorPalette.Add(new SolidColorBrush(Color.FromArgb(255, 30, 30, 30)));
            ColorPalette.Add(new SolidColorBrush(Color.FromArgb(255, 38, 42, 50)));
            ColorPalette.Add(new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)));
            ColorPalette.Add(new SolidColorBrush(Color.FromArgb(255, 232, 85, 176))); 

            if (properties.Limitation.IsExtendedColors)
            {
                ColorPalette.Add(new SolidColorBrush(Color.FromArgb(255, 104, 196, 219)));
                ColorPalette.Add(new SolidColorBrush(Color.FromArgb(255, 182, 121, 201)));
                ColorPalette.Add(new SolidColorBrush(Color.FromArgb(255, 176, 206, 99)));
                ColorPalette.Add(new SolidColorBrush(Color.FromArgb(255, 61, 62, 64)));
                ColorPalette.Add(new SolidColorBrush(Color.FromArgb(255, 227, 223, 214)));
                ColorPalette.Add(new SolidColorBrush(Color.FromArgb(255, 60, 61, 77)));
                ColorPalette.Add(new SolidColorBrush(Color.FromArgb(255, 240, 204, 84)));
                ColorPalette.Add(new SolidColorBrush(Color.FromArgb(255, 242, 109, 48)));
                ColorPalette.Add(new SolidColorBrush(Color.FromArgb(255, 219, 118, 190)));
            }
        }


        public override void Cleanup()
        {
            ColorPalette.Clear();
            ColorPalette = null;
        }


        public ICollection<SolidColorBrush> ColorPalette
        {
            get;
            private set;
        }

        public ICommand ChooseColorCommand
        {
            get;
            private set;
        }


        public object Token
        {
            get;
            private set;
        }


        public Color Color
        {
            get;
            set;
        }


        public void ChooseColor()
        {
            ChooseColor(Color);
        }


        public void ChooseColor(Color color)
        {
            Color = color;
            MessengerInstance.Send<ColorChosenMessage>(new ColorChosenMessage(new SolidColorBrush(Color)), Token);
        }


        private void OnColorChanged(ColorChangedMessage message)
        {
            Color = message.Object;
            Token = message.Token;
        }

    }
}
