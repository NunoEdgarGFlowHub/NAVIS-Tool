﻿/////////////////////////////////////////////////////////////////////////////////
//                                                                             //
//    Copyright © 2015  Juan P. Dominguez-Morales                              //
//                                                                             //        
//    This file is part of Neuromorphic Auditory Visualizer Tool (NAVIS Tool). //
//                                                                             //
//    NAVIS Tool is free software: you can redistribute it and/or modify       //
//    it under the terms of the GNU General Public License as published by     //
//    the Free Software Foundation, either version 3 of the License, or        //
//    (at your option) any later version.                                      //
//                                                                             //
//    NAVIS Tool is distributed in the hope that it will be useful,            //
//    but WITHOUT ANY WARRANTY; without even the implied warranty of           //
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the              //
//    GNU General Public License for more details.                             //
//                                                                             //
//    You should have received a copy of the GNU General Public License        //
//    along with NAVIS Tool.  If not, see <http://www.gnu.org/licenses/>.      //
//                                                                             //
/////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;

namespace NAVIS
{
    /// <summary>
    /// Disparity between cochleas window
    /// </summary>
    public partial class Difference : Window
    {
        BitmapImage image;
        aedat16 c16 = MainWindow.aedatObject16;
        aedat32 c32 = MainWindow.aedatObject32;
        bool isLoaded = false;

        int maxValue;

        /// <summary>
        /// Disparity between cochleas empty constructor
        /// </summary>
        public Difference()
        {
            InitializeComponent();

            #region Lines to prevent memory leaks in WPF
            image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            image.UriSource = new Uri(@"component/../archivobmpDiff.png", UriKind.RelativeOrAbsolute);
            image.EndInit();
            Img_Difference.Source = image;
            #endregion

            Img_Difference.Width = image.Width;
            Img_Difference.Height = image.Height;

            this.SizeToContent = SizeToContent.WidthAndHeight;

            if (MainWindow.settings.MainS.eventSize == 16)
            {
                SB_maxValue.Maximum = aedat16.maxValSonogram * 2;
                SB_maxValue.Value = aedat16.maxValSonogram;
                maxValue = aedat16.maxValSonogram;
            }
            else if (MainWindow.settings.MainS.eventSize == 32)
            {
                SB_maxValue.Maximum = aedat32.maxValSonogram * 2;
                SB_maxValue.Value = aedat32.maxValSonogram;
                maxValue = aedat32.maxValSonogram;
            }
            isLoaded = true;
        }

        /// <summary>
        /// Updates the chart with the new maxValue (saturation) on load
        /// </summary>
        private void SB_maxValue_valueChanged(object sender, EventArgs e)
        {
            if (isPressed != true && isLoaded == true)
            {
                this.Cursor = Cursors.Wait;

                image = null;

                if (MainWindow.settings.MainS.eventSize == 16)
                {
                    c16.generateDisparity("archivobmpDiff", SB_maxValue.Value);
                }
                else if (MainWindow.settings.MainS.eventSize == 32)
                {
                    c32.generateDisparity("archivobmpDiff", SB_maxValue.Value);
                }

                BitmapImage _image = new BitmapImage();
                _image.BeginInit();
                _image.CacheOption = BitmapCacheOption.None;
                _image.CacheOption = BitmapCacheOption.OnLoad;
                _image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                _image.UriSource = new Uri(@"component/../archivobmpDiff.png", UriKind.RelativeOrAbsolute);
                _image.EndInit();
                Img_Difference.Source = _image;
                this.Cursor = Cursors.Arrow;
            }
        }

        bool isPressed = false;
        private void SB_maxValue_dragStartedScroll(object sender, EventArgs e)
        {
            isPressed = true;
        }

        /// <summary>
        /// Updates the chart with the new maxValue (saturation) when the user scrolls the scrollbar
        /// </summary>
        private void SB_maxValue_dragCompletedScroll(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Wait;

            isPressed = false;

            image = null;

            if (MainWindow.settings.MainS.eventSize == 16)
            {
                c16.generateDisparity("archivobmpDiff", SB_maxValue.Value);
            }
            else if (MainWindow.settings.MainS.eventSize == 32)
            {
                c32.generateDisparity("archivobmpDiff", SB_maxValue.Value);
            }

            BitmapImage _image = new BitmapImage();
            _image.BeginInit();
            _image.CacheOption = BitmapCacheOption.None;
            _image.CacheOption = BitmapCacheOption.OnLoad;
            _image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            _image.UriSource = new Uri(@"component/../archivobmpDiff.png", UriKind.RelativeOrAbsolute);
            _image.EndInit();
            Img_Difference.Source = _image;

            this.Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Resets the chart to the default value.
        /// </summary>
        private void Btn_resetToDefault_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.settings.MainS.eventSize == 16)
            {
                SB_maxValue.Value = maxValue;
            }
            else if (MainWindow.settings.MainS.eventSize == 32)
            {
                SB_maxValue.Value = maxValue;
            }
        }

        /// <summary>
        /// Saves an image with the disparity between cochleae chart information
        /// </summary>
        private void Btn_saveImage_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Png Image (.png)|*.png";

            sfd.Title = "Select path to save the image";
            sfd.ShowDialog();

            if (sfd.FileName != "")
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)Img_Difference.Source));
                using (FileStream stream = new FileStream(sfd.FileName, FileMode.Create))
                    encoder.Save(stream);

                InfoWindow iw = new InfoWindow("Success!", "Image saved successfuly");
                iw.ShowDialog();
            }
        }

        private void Btn_saveCSV_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV file (.csv)|*.csv";

            sfd.Title = "Select path to save the csv file";
            sfd.ShowDialog();

            if (sfd.FileName != "")
            {
                if (MainWindow.settings.MainS.eventSize == 16)
                {
                    c16.generateDisparityCSV(sfd.FileName);
                }
                else if (MainWindow.settings.MainS.eventSize == 32)
                {
                    c32.generateDisparityCSV(sfd.FileName);
                }
                InfoWindow iw = new InfoWindow("Success!", "CSV file saved successfuly");
                iw.ShowDialog();
            }
        }
    }
}
