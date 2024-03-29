﻿// -----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using System.Windows.Controls;

namespace FaceTrackingBasics
{
    using System;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Kinect;
    using Microsoft.Kinect.Toolkit;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly int Bgr32BytesPerPixel = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;
        private readonly KinectSensorChooser sensorChooser = new KinectSensorChooser();
        private WriteableBitmap colorImageWritableBitmap;
        private byte[] colorImageData;
        private ColorImageFormat currentColorImageFormat = ColorImageFormat.Undefined;
        public static int m_angleSliderY = 90;
        public static int m_angleSliderX = 90;
        readonly ICollection<SerialPort> m_ports = new List<SerialPort>();
        private readonly Thread m_servoThread;

        public static int m_angleSliderA = 90;
        public static int m_angleSliderB = 90;

        public MainWindow()
        {
            InitializeComponent();

            var faceTrackingViewerBinding = new Binding("Kinect") { Source = sensorChooser };
            faceTrackingViewer.SetBinding(FaceTrackingViewer.KinectProperty, faceTrackingViewerBinding);

            sensorChooser.KinectChanged += SensorChooserOnKinectChanged;

            sensorChooser.Start();

            // Get a list of serial port names.
            var portNames = SerialPort.GetPortNames();

            Debug.WriteLine("The following serial ports were found:");

            // Display each port name to the console. 
            foreach (var port in portNames)
            {
                Debug.WriteLine(port);
                m_ports.Add(new SerialPort(port));
            }
            m_servoThread = new Thread(RunServo);
            m_servoThread.Start();
        }

        public static void setAngleX(double value)
        {
            m_angleSliderX = ((int)(value+0.5));
            Debug.WriteLine("x: " + m_angleSliderX);
        }

        public static void setAngleY(double value)
        {
            m_angleSliderY = (int)(value+0.5)-13;
            Debug.WriteLine("y: " + m_angleSliderY);
        }

        public static void setAngleA(double value)
        {
            m_angleSliderA = ((int)(value + 0.5));
            Debug.WriteLine("a: " + m_angleSliderA);
        }

        public static void setAngleB(double value)
        {
            m_angleSliderB = (int)(value + 0.5) - 13;
            Debug.WriteLine("b: " + m_angleSliderB);
        }

        private void AngleChangeX(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            m_angleSliderX = (int)((Slider)sender).Value;
            m_angleSliderA = (int)((Slider)sender).Value;
        }

        private void AngleChangeY(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            m_angleSliderY = (int)((Slider)sender).Value;
            m_angleSliderB = (int)((Slider)sender).Value;
        }

        // 4 way range about 30-175
        // 2 way range about 11-167
        private void RunServo()
        {
            const int minAngleX = 40;
            const int maxAngleX = 140;
            const int minAngleY = 40;
            const int maxAngleY = 90;

            const int minAngleA = 40;
            const int maxAngleA = 140;
            const int minAngleB = 40;
            const int maxAngleB = 90;

            foreach (var serialPort in m_ports)
            {
                serialPort.Open();
            }
            var currentValueX = m_angleSliderX;
            var currentValueY = m_angleSliderY;
            var currentValueA = m_angleSliderA;
            var currentValueB = m_angleSliderB;
            while (true)
            {

                
                
                if (currentValueY < minAngleY) currentValueY = minAngleY;                
                else if (currentValueY > maxAngleY) currentValueY = maxAngleY;

                if (currentValueA < minAngleA) currentValueA = minAngleA;
                else if (currentValueA > maxAngleA) currentValueA = maxAngleA;

                if (currentValueB < minAngleB) currentValueB = minAngleB;
                else if (currentValueB > maxAngleB) currentValueB = maxAngleB;
                                
                foreach (var serialPort in m_ports)
                {
                    if (currentValueX != m_angleSliderX)
                    {
                        if (m_angleSliderX < minAngleX) serialPort.Write("X" + minAngleX);
                        else if (m_angleSliderX > maxAngleX) serialPort.Write("X" + maxAngleX);
                        else serialPort.Write("X" + m_angleSliderX);
                        currentValueX = m_angleSliderX;
                        WaitForResponse(serialPort);                        
                    }
                    if (currentValueY != m_angleSliderY)
                    {
                        if (m_angleSliderY < minAngleY) serialPort.Write("Y" + minAngleY);
                        else if (m_angleSliderY > maxAngleY) serialPort.Write("X" + maxAngleY);
                        else serialPort.Write("Y" + m_angleSliderY);
                        currentValueY = m_angleSliderY;
                        WaitForResponse(serialPort);  
                    }
                    if (currentValueA != m_angleSliderA)
                    {
                        if (m_angleSliderA < minAngleA) serialPort.Write("A" + minAngleA);
                        else if (m_angleSliderA > maxAngleA) serialPort.Write("A" + maxAngleA);
                        else serialPort.Write("A" + m_angleSliderA);
                        currentValueA = m_angleSliderA;
                        WaitForResponse(serialPort);  
                    }
                    if (currentValueB != m_angleSliderB)
                    {
                        if (m_angleSliderB < minAngleB) serialPort.Write("B" + minAngleB);
                        else if (m_angleSliderB > maxAngleB) serialPort.Write("B" + maxAngleB);
                        else serialPort.Write("B" + m_angleSliderB);
                        currentValueB = m_angleSliderB;
                        WaitForResponse(serialPort);  
                    }
                }

            }
        }

       

        private void WaitForResponse(SerialPort port)
        {
            var buffer = new byte[1];
            var received = 0;
            while (received < 1)
                received += port.Read(buffer, 0, 1);
        }


        private void SensorChooserOnKinectChanged(object sender, KinectChangedEventArgs kinectChangedEventArgs)
        {
            KinectSensor oldSensor = kinectChangedEventArgs.OldSensor;
            KinectSensor newSensor = kinectChangedEventArgs.NewSensor;

            if (oldSensor != null)
            {
                oldSensor.AllFramesReady -= KinectSensorOnAllFramesReady;
                oldSensor.ColorStream.Disable();
                oldSensor.DepthStream.Disable();
                oldSensor.DepthStream.Range = DepthRange.Default;
                oldSensor.SkeletonStream.Disable();
                oldSensor.SkeletonStream.EnableTrackingInNearRange = false;
                oldSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
            }

            if (newSensor != null)
            {
                try
                {
                    newSensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                    newSensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
                    try
                    {
                        // This will throw on non Kinect For Windows devices.
                        newSensor.DepthStream.Range = DepthRange.Near;
                        newSensor.SkeletonStream.EnableTrackingInNearRange = true;
                    }
                    catch (InvalidOperationException)
                    {
                        newSensor.DepthStream.Range = DepthRange.Default;
                        newSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    }

                    newSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                    newSensor.SkeletonStream.Enable();
                    newSensor.AllFramesReady += KinectSensorOnAllFramesReady;
                }
                catch (InvalidOperationException)
                {
                    // This exception can be thrown when we are trying to
                    // enable streams on a device that has gone away.  This
                    // can occur, say, in app shutdown scenarios when the sensor
                    // goes away between the time it changed status and the
                    // time we get the sensor changed notification.
                    //
                    // Behavior here is to just eat the exception and assume
                    // another notification will come along if a sensor
                    // comes back.
                }
            }
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            m_servoThread.Abort();
            foreach (var serialPort in m_ports)
            {
                serialPort.Close();
            }
            sensorChooser.Stop();
            faceTrackingViewer.Dispose();
        }

        private void KinectSensorOnAllFramesReady(object sender, AllFramesReadyEventArgs allFramesReadyEventArgs)
        {
            using (var colorImageFrame = allFramesReadyEventArgs.OpenColorImageFrame())
            {
                if (colorImageFrame == null)
                {
                    return;
                }

                // Make a copy of the color frame for displaying.
                var haveNewFormat = this.currentColorImageFormat != colorImageFrame.Format;
                if (haveNewFormat)
                {
                    this.currentColorImageFormat = colorImageFrame.Format;
                    this.colorImageData = new byte[colorImageFrame.PixelDataLength];
                    this.colorImageWritableBitmap = new WriteableBitmap(
                        colorImageFrame.Width, colorImageFrame.Height, 96, 96, PixelFormats.Bgr32, null);
                    ColorImage.Source = this.colorImageWritableBitmap;
                }

                colorImageFrame.CopyPixelDataTo(this.colorImageData);
                this.colorImageWritableBitmap.WritePixels(
                    new Int32Rect(0, 0, colorImageFrame.Width, colorImageFrame.Height),
                    this.colorImageData,
                    colorImageFrame.Width * Bgr32BytesPerPixel,
                    0);
            }
        }
    }
}
