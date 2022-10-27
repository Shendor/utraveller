using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Shapes;
using System.Windows.Media;
using System.ComponentModel;

namespace UTraveller.Walkthrough
{
    public partial class WalkthroughPagination : UserControl
    {
        public WalkthroughPagination()
        {
            InitializeComponent();

            foreach (var element in paginationPanel.Children)
            {
                ((Ellipse)element).Stroke = App.AppProperties.MainColor;
            }
        }


        public int CurrentPage
        {
            get { return 0; }
            set
            {
                if (paginationPanel.Children.Count > value && value >= 0)
                {
                    foreach (var ellipse in paginationPanel.Children)
                    {
                        ((Ellipse)ellipse).Fill = null;
                    }

                    var element = paginationPanel.Children[value];
                    ((Ellipse)element).Fill = App.AppProperties.MainColor;
                }
            }
        }
    }
}
