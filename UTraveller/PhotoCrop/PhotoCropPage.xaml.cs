using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Nokia.Graphics.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Windows.Storage.Streams;
using UTraveller.PhotoCrop.ViewModel;
using Ninject;
using UTraveller.PhotoCrop.Message;
using UTraveller.Common.Control;

namespace UTraveller.PhotoCrop
{
    public partial class PhotoCropPage : BasePhoneApplicationPage
    {
        private double scale = 1.0;
        private bool pinching = false;
        private Point relativeCenter;
        private PhotoCropViewModel viewModel;

        public PhotoCropPage()
        {
            InitializeComponent();
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                viewModel = DataContext as PhotoCropViewModel;
                if (viewModel != null)
                {
                    viewModel.Cleanup();
                    Image.Source = null;
                }
            }
        }


        protected override void OnNavigatedTo(NavigationEventArgs navigationArgs)
        {
            base.OnNavigatedTo(navigationArgs);

            if (viewModel == null)
            {
                viewModel = App.IocContainer.Get<PhotoCropViewModel>();
                DataContext = viewModel;
            }
            viewModel.Initialize();
            ConfigureViewport();
        }


        private void ApplyCropButtonClick(object sender, EventArgs e)
        {
            GeneralTransform transform = Crop.TransformToVisual(Image);

            var topLeftWindowsPoint = transform.Transform(new Point(0, 0));
            topLeftWindowsPoint.X /= scale;
            topLeftWindowsPoint.Y /= scale;

            var bottomRightWindowsPoint = transform.Transform(new Point(Crop.Width, Crop.Height));
            bottomRightWindowsPoint.X /= scale;
            bottomRightWindowsPoint.Y /= scale;

            viewModel.ApplyCrop(topLeftWindowsPoint, bottomRightWindowsPoint);

            Close();
        }


        private void Viewport_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (viewModel != null)
            {
                ConfigureViewport();
            }
        }


        private void ConfigureViewport()
        {
            if (viewModel.ImageWidth < viewModel.ImageHeight)
            {
                scale = Crop.Width / viewModel.ImageWidth;
            }
            else
            {
                scale = Crop.Height / viewModel.ImageHeight;
            }

            Image.Width = viewModel.ImageWidth * scale;
            Image.Height = viewModel.ImageHeight * scale;
            Image.Margin = new Thickness()
            {
                Left = Math.Max(0, (ContentPanel.ActualWidth - Crop.Width) / 2),
                Right = Math.Max(0, (ContentPanel.ActualWidth - Crop.Width) / 2),
                Top = Math.Max(0, (ContentPanel.ActualHeight - Crop.Height) / 2),
                Bottom = Math.Max(0, (ContentPanel.ActualHeight - Crop.Height) / 2)
            };

            Viewport.Bounds = new Rect(0, 0, Image.Width + Image.Margin.Left + Image.Margin.Right, Image.Height + Image.Margin.Top + Image.Margin.Bottom);
            Viewport.SetViewportOrigin(new Point(
                Viewport.Bounds.Width / 2 - Crop.Width / 2,
                Viewport.Bounds.Height / 2 - Crop.Height / 2));
        }

        private void Viewport_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            if (pinching)
            {
                e.Handled = true;

                CompletePinching();
            }
        }

        private void Viewport_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (e.PinchManipulation != null)
            {
                e.Handled = true;

                if (!pinching)
                {
                    pinching = true;

                    relativeCenter = new Point(
                        e.PinchManipulation.Original.Center.X / Image.Width,
                        e.PinchManipulation.Original.Center.Y / Image.Height);
                }

                double w, h;

                if (viewModel.ImageWidth < viewModel.ImageHeight)
                {
                    w = viewModel.ImageWidth * scale * e.PinchManipulation.CumulativeScale;
                    w = Math.Max(Crop.Width, w);
                    w = Math.Min(w, viewModel.ImageWidth);
                    w = Math.Min(w, 4096);

                    h = w * viewModel.ImageHeight / viewModel.ImageWidth;

                    if (h > 4096)
                    {
                        var scaler = 4096.0 / h;
                        h *= scaler;
                        w *= scaler;
                    }
                }
                else
                {
                    h = viewModel.ImageHeight * scale * e.PinchManipulation.CumulativeScale;
                    h = Math.Max(Crop.Height, h);
                    h = Math.Min(h, viewModel.ImageHeight);
                    h = Math.Min(h, 4096);

                    w = h * viewModel.ImageWidth / viewModel.ImageHeight;

                    if (w > 4096)
                    {
                        var scaler = 4096.0 / w;
                        w *= scaler;
                        h *= scaler;
                    }
                }

                Image.Width = w;
                Image.Height = h;

                Viewport.Bounds = new Rect(0, 0, w + Image.Margin.Left + Image.Margin.Right, h + Image.Margin.Top + Image.Margin.Bottom);

                GeneralTransform transform = Image.TransformToVisual(Viewport);
                Point p = transform.Transform(e.PinchManipulation.Original.Center);

                double x = relativeCenter.X * w - p.X + Image.Margin.Left;
                double y = relativeCenter.Y * h - p.Y + Image.Margin.Top;

                if (w < viewModel.ImageWidth && h < viewModel.ImageHeight)
                {
                    //System.Diagnostics.Debug.WriteLine("Viewport.ActualWidth={0} .ActualHeight={1} Origin.X={2} .Y={3} Image.Width={4} .Height={5}",
                    //    Viewport.ActualWidth, Viewport.ActualHeight, x, y, Image.Width, Image.Height);

                    Viewport.SetViewportOrigin(new Point(x, y));
                }
            }
            else if (pinching)
            {
                e.Handled = true;

                CompletePinching();
            }
        }

        private void Viewport_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (pinching)
            {
                e.Handled = true;

                CompletePinching();
            }
        }

        private void CompletePinching()
        {
            pinching = false;

            double sw = Image.Width / viewModel.ImageWidth;
            double sh = Image.Height / viewModel.ImageHeight;

            scale = Math.Min(sw, sh);
        }

        private void Viewport_ManipulationStateChanged(object sender, ManipulationStateChangedEventArgs e)
        {
            // Viewport sometimes does not animate correctly back to the content area,
            // so here we fix it if that happens

            if (Viewport.ManipulationState == ManipulationState.Idle)
            {
                if (Viewport.Viewport.X < 0)
                {
                    Viewport.SetViewportOrigin(new Point(0, Viewport.Viewport.Y));
                }
                else if (Viewport.Viewport.X > Image.Width - Viewport.Viewport.Width)
                {
                    Viewport.SetViewportOrigin(new Point(Image.Width - Viewport.Width, Viewport.Viewport.Y));
                }

                if (Viewport.Viewport.Y < 0)
                {
                    Viewport.SetViewportOrigin(new Point(Viewport.Viewport.X, 0));
                }
                else if (Viewport.Viewport.Y > Image.Height - Viewport.Viewport.Height)
                {
                    Viewport.SetViewportOrigin(new Point(Viewport.Viewport.X, Image.Height - Viewport.Height));
                }
            }
        }

        private void CloseButtonClick(object sender, EventArgs e)
        {
            Close();
        }
    }
}