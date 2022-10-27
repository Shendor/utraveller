using Microsoft.Phone.Maps.Toolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UTraveller.EventMap
{
    public static class MapPushPinDependency
    {
        public static readonly DependencyProperty ItemsSourceProperty =
                DependencyProperty.RegisterAttached(
                 "ItemsSource", typeof(IEnumerable), typeof(MapPushPinDependency),
                 new PropertyMetadata(OnPushPinPropertyChanged));

        private static void OnPushPinPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement uie = (UIElement)d;
            var pushpin = MapExtensions.GetChildren((Microsoft.Phone.Maps.Controls.Map)uie).OfType<MapItemsControl>().FirstOrDefault();
            pushpin.ItemsSource = (IEnumerable)e.NewValue;
        }


        #region Getters and Setters

        public static IEnumerable GetItemsSource(DependencyObject obj)
        {
            return (IEnumerable)obj.GetValue(ItemsSourceProperty);
        }

        public static void SetItemsSource(DependencyObject obj, IEnumerable value)
        {
            obj.SetValue(ItemsSourceProperty, value);
        }

        #endregion
    }
}
