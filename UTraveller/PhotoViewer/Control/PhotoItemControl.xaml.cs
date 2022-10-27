using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace UTraveller.PhotoViewer.Control
{
    public partial class PhotoItemControl : FlipViewItem, IDisposable
    {
        private static readonly double DELTA = 0.001;
        private static readonly double MaxScale = 5;

        private double scale = 1.0;
        private double minScale;
        private double coercedScale;
        private double originalScale;
        private Size viewportSize;
        private bool pinching;
        private bool isViewportMinHeightSet;
        private Point screenMidpoint;
        private Point relativeMidpoint;
        private BitmapImage bitmap;
        private FlipView parent;
        private PageOrientation pageOrientation;

        public PhotoItemControl()
        {
            InitializeComponent();
            imageLoadStatusText.Visibility = Visibility.Collapsed;
        }


        public void DescriptionVisibility(bool isVisible)
        {
            if (scale <= minScale)
            {
                if (isVisible)
                {
                    descriptionTextBlock.Visibility = Visibility.Visible;
                }
                else
                {
                    descriptionTextBlock.Visibility = Visibility.Collapsed;
                }
            }
        }


        /// <summary> 
        /// Initialize image after its source was set
        /// </summary> 
        public void InitializeImage(PageOrientation pageOrientation, bool isRemote = false)
        {
            this.pageOrientation = pageOrientation;
            bitmap = (BitmapImage)image.Source;
            if (isRemote)
            {
                imageLoadProgress.Visibility = Visibility.Visible;
            }
            if (bitmap != null)
            {
                isViewportMinHeightSet = false;
                // Set scale to the minimum, and then save it. 
                scale = 0;
                CoerceScale(true);
                scale = coercedScale;
                ResizeImage(true);
            }
        }


        public void FailInitialization()
        {
            imageLoadStatusText.Visibility = Visibility.Visible;
        }


        public void Dispose()
        {
            isViewportMinHeightSet = false;
            image.Source = null;
            parent = null;
            if (bitmap != null)
            {
                bitmap.UriSource = null;
                bitmap = null;
            }
        }


        public void ChangePageOrientation(PageOrientation pageOrientation)
        {
            isViewportMinHeightSet = false;
            InitializeImage(pageOrientation);
        }


        public void ImageLoaded(object sender, RoutedEventArgs e)
        {
            InitializeImage(pageOrientation);
           /* if (bitmap != null && !bitmap.UriSource.OriginalString.Equals(string.Empty))
            {
                imageLoadProgress.Visibility = Visibility.Visible;
            }*/
        }


        private void ImageOpened(object sender, RoutedEventArgs e)
        {
            imageLoadProgress.Visibility = Visibility.Collapsed;
            imageLoadStatusText.Visibility = Visibility.Collapsed;
            InitializeImage(pageOrientation);
        }


        private void ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            imageLoadProgress.Visibility = Visibility.Collapsed;
            imageLoadStatusText.Visibility = Visibility.Visible;
        }


        /// <summary> 
        /// Either the user has manipulated the image or the size of the viewport has changed. We only 
        /// care about the size. 
        /// </summary> 
        void viewport_ViewportChanged(object sender, System.Windows.Controls.Primitives.ViewportChangedEventArgs e)
        {
            Size newSize = new Size(viewport.Viewport.Width, viewport.Viewport.Height);
            if (bitmap != null && newSize != viewportSize)
            {
                if (!pinching)
                {
                    isViewportMinHeightSet = false;
                }
                viewportSize = newSize;
                CoerceScale(true);
                ResizeImage(true);
            }
        }

        /// <summary> 
        /// Handler for the ManipulationStarted event. Set initial state in case 
        /// it becomes a pinch later. 
        /// </summary> 
        void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            parent = this.Parent as FlipView;
            //parent.IsLocked = !(minScale <= scale + DELTA);
            isViewportMinHeightSet = true;
            pinching = false;
            originalScale = scale;
        }

        /// <summary> 
        /// Handler for the ManipulationDelta event. It may or may not be a pinch. If it is not a  
        /// pinch, the ViewportControl will take care of it. 
        /// </summary> 
        /// <param name="sender"></param> 
        /// <param name="e"></param> 
        void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (e.PinchManipulation != null)
            {
                //parent.IsLocked = true;
                e.Handled = true;

                if (scrollViewer.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled)
                {
                    descriptionTextBlock.Visibility = System.Windows.Visibility.Collapsed;
                    viewport.Height = scrollViewer.ActualHeight;
                    scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                }

                if (!pinching)
                {
                    pinching = true;
                    Point center = e.PinchManipulation.Original.Center;
                    relativeMidpoint = new Point(center.X / image.ActualWidth, center.Y / image.ActualHeight);

                    var xform = image.TransformToVisual(viewport);
                    screenMidpoint = xform.Transform(center);
                }

                scale = originalScale * e.PinchManipulation.CumulativeScale;

                CoerceScale(false);
                ResizeImage(false);
            }
            else if (pinching)
            {
                pinching = false;
                originalScale = scale = coercedScale;
            }
        }

        /// <summary> 
        /// The manipulation has completed (no touch points anymore) so reset state. 
        /// </summary> 
        void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            pinching = false;
            scale = coercedScale;
            if (scale <= minScale)
            {
                System.Diagnostics.Debug.WriteLine(scale);
                isViewportMinHeightSet = false;
                SetViewportMinHeight();
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            }
        }


        /// <summary> 
        /// Adjust the size of the image according to the coerced scale factor. Optionally 
        /// center the image, otherwise, try to keep the original midpoint of the pinch 
        /// in the same spot on the screen regardless of the scale. 
        /// </summary> 
        /// <param name="center"></param> 
        void ResizeImage(bool center)
        {
            if (coercedScale != 0)
            {
                double newWidth = canvas.Width = Math.Round(bitmap.PixelWidth * coercedScale);
                double newHeight = canvas.Height = Math.Round(bitmap.PixelHeight * coercedScale);

                xform.ScaleX = xform.ScaleY = coercedScale;

                viewport.Bounds = new Rect(0, 0, newWidth, newHeight);

                if (center)
                {
                    viewport.SetViewportOrigin(
                        new Point(
                            Math.Round((newWidth - ActualWidth) / 2),
                            Math.Round((newHeight - ActualHeight) / 2)
                            ));
                }
                else
                {
                    Point newImgMid = new Point(newWidth * relativeMidpoint.X, newHeight * relativeMidpoint.Y);
                    Point origin = new Point(newImgMid.X - screenMidpoint.X, newImgMid.Y - screenMidpoint.Y);
                    viewport.SetViewportOrigin(origin);
                }
                SetViewportMinHeight();
            }
        }


        private void SetViewportMinHeight()
        {
            if (!isViewportMinHeightSet && bitmap.PixelHeight > 0)
            {
                var screenWidth = Application.Current.Host.Content.ActualWidth;
                var screenHeight = Application.Current.Host.Content.ActualHeight;

                if (pageOrientation == PageOrientation.Portrait || pageOrientation == PageOrientation.PortraitDown ||
                    pageOrientation == PageOrientation.PortraitUp || pageOrientation == PageOrientation.None)
                {
                    if (bitmap.PixelHeight > bitmap.PixelWidth)
                    {
                        viewport.MinHeight = (bitmap.PixelHeight > screenHeight) ? screenHeight : bitmap.PixelHeight;
                    }
                    else
                    {
                        viewport.MinHeight = (int)(bitmap.PixelHeight * scale) + 30;
                    }
                }
                else
                {
                    if (bitmap.PixelHeight > bitmap.PixelWidth)
                    {
                        viewport.MinHeight = (bitmap.PixelHeight > screenWidth) ? screenWidth : bitmap.PixelHeight;
                    }
                    else
                    {
                        int minHeight = (int)(bitmap.PixelHeight * scale) + 30;
                        viewport.MinHeight = (minHeight > screenWidth) ? screenWidth : minHeight;
                    }
                }
                viewport.Height = viewport.MinHeight + 30;
                isViewportMinHeightSet = true;
            }
        }

        /// <summary> 
        /// Coerce the scale into being within the proper range. Optionally compute the constraints  
        /// on the scale so that it will always fill the entire screen and will never get too big  
        /// to be contained in a hardware surface. 
        /// </summary> 
        /// <param name="recompute">Will recompute the min max scale if true.</param> 
        private void CoerceScale(bool recompute)
        {
            if (recompute && viewport != null)
            {
                // Calculate the minimum scale to fit the viewport 
                double minX = ActualWidth / bitmap.PixelWidth;
                double minY = ActualHeight / bitmap.PixelHeight;

                minScale = Math.Min(minX, minY);
            }

            coercedScale = Math.Min(MaxScale, Math.Max(scale, minScale));
        }
    }
}
