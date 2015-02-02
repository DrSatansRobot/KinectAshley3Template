using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using Microsoft.Kinect;
using ImaginativeUniversal;
using System.ComponentModel;

namespace KinectApp3Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region private members

        private KinectSensor _sensor;
        byte[] _colorBits;
        DepthImagePixel[] _depthBits;
        Skeleton[] _skeletons;

        public MainWindow()
        {
            this.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
            InitializeComponent();
        }

        #endregion

        #region kinect setup
        private void InitSensor(KinectSensor _sensor)
        {
            if (_sensor == null)
                return;

            _sensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(sensor_ColorFrameReady);
            _sensor.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(sensor_DepthFrameReady);
            _sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(sensor_SkeletonFrameReady);
            _sensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(sensor_AllFramesReady);

            _sensor.ColorStream.Enable();
            _sensor.DepthStream.Enable();
            _sensor.SkeletonStream.Enable();
            _sensor.Start();

            this.canvas.Source = _sensor.RenderActivePlayer();
        }

        private void DeInitSensor(KinectSensor _sensor)
        {
            if (_sensor == null) return;

            _sensor.AllFramesReady -= sensor_AllFramesReady;
            _sensor.SkeletonFrameReady -= sensor_SkeletonFrameReady;
            _sensor.DepthFrameReady -= sensor_DepthFrameReady;
            _sensor.ColorFrameReady -= sensor_ColorFrameReady;

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (a, b) =>
            {
                _sensor.Stop();

                _sensor.SkeletonStream.Disable();
                _sensor.DepthStream.Disable();
                _sensor.ColorStream.Disable();

                _sensor = null;
            };
            bw.RunWorkerAsync();

        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            KinectSensor.KinectSensors.StatusChanged += new EventHandler<StatusChangedEventArgs>(KinectSensors_StatusChanged);
            _sensor = KinectSensor.KinectSensors.FirstOrDefault();
            InitSensor(_sensor);
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DeInitSensor(_sensor);
        }

        void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            if (e.Status == KinectStatus.Disconnected)
            {
                if (_sensor != null)
                {
                    DeInitSensor(_sensor);
                }
            }

            if (e.Status == KinectStatus.Connected)
            {
                _sensor = e.Sensor;
                InitSensor(_sensor);
            }
        }

        #endregion

        #region kinect stream handlers

        void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame frame = e.OpenColorImageFrame())
            {
                if (frame == null)
                    return;

                if (_colorBits == null)
                //{
                    _colorBits = new byte[frame.PixelDataLength];
                    frame.CopyPixelDataTo(_colorBits);
                    this.canvas.Source =_colorBits.ToBitmapSource(PixelFormats.Bgr32, 640, 480);
                //}

                //throw new NotImplementedException();
            }
        }

        void sensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame frame = e.OpenDepthImageFrame())
            {
                if (frame == null)
                    return;

                if (_depthBits == null)
                //{
                    _depthBits = new DepthImagePixel[frame.PixelDataLength];
                    frame.CopyDepthImagePixelDataTo(_depthBits);
                //}

                //throw new NotImplementedException();
            }
        }

        void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame == null)
                    return;

                if (_skeletons == null)
                {
                    _skeletons = new Skeleton[frame.SkeletonArrayLength];
                    frame.CopySkeletonDataTo(_skeletons);
                }

                //throw new NotImplementedException();
            }
        }


        void sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            //throw new NotImplementedException();
        }

        #endregion
    }
}
