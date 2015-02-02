using System;
using System.Drawing;
using Microsoft.Kinect;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.IO;
using Media = System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;

namespace ImaginativeUniversal
{
    public static class KinectImageExtensions
    {
        /// <summary>
        /// Deletes the object.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns></returns>
        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);


        /// <summary>
        /// Converts Kinect byte array to Bitmap.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="format">The pixel format.</param>
        /// <returns></returns>
        public static Bitmap ToBitmap(this byte[] data, int width, int height
            , PixelFormat format)
        {
            var bitmap = new Bitmap(width, height, format);

            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.WriteOnly,
                bitmap.PixelFormat);
            Marshal.Copy(data, 0, bitmapData.Scan0, data.Length);
            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }

        /// <summary>
        /// Converts Kinect byte array to Bitmap.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public static Bitmap ToBitmap(this short[] data, int width, int height
            , PixelFormat format)
        {
            var bitmap = new Bitmap(width, height, format);

            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.WriteOnly,
                bitmap.PixelFormat);
            Marshal.Copy(data, 0, bitmapData.Scan0, data.Length);
            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }

        /// <summary>
        /// Convert Kinect byte array to BitmapSource.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="format">The format.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        public static Media.Imaging.BitmapSource ToBitmapSource(this byte[] data
            , Media.PixelFormat format, int width, int height)
        {
            return Media.Imaging.BitmapSource.Create(width, height, 96, 96
                , format, null, data, width * format.BitsPerPixel / 8);
        }

        public static Media.Imaging.BitmapSource ToBitmapSource(this short[] data
        , Media.PixelFormat format, int width, int height)
        {
            return Media.Imaging.BitmapSource.Create(width, height, 96, 96
                , format, null, data, width * format.BitsPerPixel / 8);
        }

        // bitmap methods

        /// <summary>
        /// Gets Bitmap image from Kinect ColorImageFrame.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public static Bitmap ToBitmap(this ColorImageFrame image, PixelFormat format)
        {
            if (image == null || image.PixelDataLength == 0)
                return null;
            var data = new byte[image.PixelDataLength];
            image.CopyPixelDataTo(data);
            return data.ToBitmap(image.Width, image.Height
                , format);
        }

        /// <summary>
        /// Gets Bitmap image from Kinect DepthImageFrame.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public static Bitmap ToBitmap(this DepthImageFrame image, PixelFormat format)
        {
            if (image == null || image.PixelDataLength == 0)
                return null;
            var data = new short[image.PixelDataLength];
            image.CopyPixelDataTo(data);
            return data.ToBitmap(image.Width, image.Height
                , format);
        }

        /// <summary>
        /// Gets Bitmap image from Kinect ColorImageFrame.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns></returns>
        public static Bitmap ToBitmap(this ColorImageFrame image)
        {
            return image.ToBitmap(PixelFormat.Format32bppRgb);
        }

        public static Bitmap ToBitmap(this DepthImageFrame image)
        {
            return image.ToBitmap(PixelFormat.Format16bppRgb565);
        }

        // bitmapsource methods

        /// <summary>
        /// Gets Bitmap image from Kinect ColorImageFrame.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns></returns>
        public static Media.Imaging.BitmapSource ToBitmapSource(this ColorImageFrame image)
        {
            if (image == null || image.PixelDataLength == 0)
                return null;
            var data = new byte[image.PixelDataLength];
            image.CopyPixelDataTo(data);
            return data.ToBitmapSource(Media.PixelFormats.Bgr32, image.Width, image.Height);
        }

        /// <summary>
        /// Gets Bitmap image from Kinect DepthImageFrame.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns></returns>
        public static Media.Imaging.BitmapSource ToBitmapSource(this DepthImageFrame image)
        {
            if (image == null || image.PixelDataLength == 0)
                return null;
            var data = new short[image.PixelDataLength];
            image.CopyPixelDataTo(data);
            return data.ToBitmapSource(Media.PixelFormats.Bgr555, image.Width, image.Height);
        }

        /// <summary>
        /// Converts Kinect byte array to transparent BitmapSource.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        public static Media.Imaging.BitmapSource ToTransparentBitmapSource(this byte[] data
            , int width, int height)
        {
            return data.ToBitmapSource(Media.PixelFormats.Bgra32, width, height);
        }


        /// <summary>
        /// Converts Bitmap to BitmapSource.
        /// </summary>
        /// <param name="bitmap">The bitmap image.</param>
        /// <returns></returns>
        public static Media.Imaging.BitmapSource ToBitmapSource(this Bitmap bitmap)
        {
            if (bitmap == null) return null;
            IntPtr ptr = bitmap.GetHbitmap();
            var source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
            ptr,
            IntPtr.Zero,
            Int32Rect.Empty,
            Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            DeleteObject(ptr);
            return source;
        }

        /// <summary>
        /// Converts BitmapSource to Bitmap.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static Bitmap ToBitmap(this Media.Imaging.BitmapSource source)
        {
            Bitmap bitmap;
            using (MemoryStream outStream = new MemoryStream())
            {
                var enc = new Media.Imaging.PngBitmapEncoder();
                enc.Frames.Add(Media.Imaging.BitmapFrame.Create(source));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }
            return bitmap;
        }


        /// <summary>
        /// Renders all active players detected by Kinect sensor.  Kinect color, depth and skeleton streams must be enabled.
        /// </summary>
        /// <param name="sensor">The Kinect sensor.</param>
        /// <returns></returns>
        public static WriteableBitmap RenderActivePlayer(this KinectSensor sensor)
        {
            return sensor.RenderActivePlayer(Color.Transparent, Color.White);
        }

        /// <summary>
        /// Renders all active players detected by Kinect sensor.  Kinect color, depth and skeleton streams must be enabled.
        /// </summary>
        /// <param name="sensor">The Kinect sensor.</param>
        /// <param name="foreColor">A foreground color for player outline.  The default transparent uses original rgb colors.</param>
        /// <returns></returns>
        public static WriteableBitmap RenderActivePlayer(this KinectSensor sensor, Color foreColor)
        {
            return sensor.RenderActivePlayer(sensor.DepthStream.FrameWidth, sensor.DepthStream.FrameHeight, foreColor, Color.White);
        }

        /// <summary>
        /// Renders all active players detected by Kinect sensor.  Kinect color, depth and skeleton streams must be enabled.
        /// </summary>
        /// <param name="sensor">The Kinect sensor.</param>
        /// <param name="foreColor">A foreground color for player outline.  Transparent uses original rgb colors.</param>
        /// <param name="backgroundColor">A background color.  Transparent uses original rgb colors.  White is the default.</param>
        /// <returns></returns>
        public static WriteableBitmap RenderActivePlayer(this KinectSensor sensor, Color foreColor, Color backgroundColor)
        {
            return sensor.RenderActivePlayer(sensor.DepthStream.FrameWidth, sensor.DepthStream.FrameHeight, foreColor, backgroundColor);
        }

        /// <summary>
        /// Renders all active players detected by Kinect sensor.  Kinect color, depth and skeleton streams must be enabled.
        /// </summary>
        /// <param name="sensor">The Kinect sensor.</param>
        /// <param name="depthWidth">Width of the depth stream image.</param>
        /// <param name="depthHeight">Height of the depth stream image.</param>
        /// <returns></returns>
        public static WriteableBitmap RenderActivePlayer(this KinectSensor sensor, int depthWidth, int depthHeight)
        {
            return sensor.RenderActivePlayer(sensor.DepthStream.FrameWidth, sensor.DepthStream.FrameHeight, Color.Transparent, Color.White);
        }

        /// <summary>
        /// Renders all active players detected by Kinect sensor.  Kinect color, depth and skeleton streams must be enabled.
        /// </summary>
        /// <param name="sensor">The Kinect sensor.</param>
        /// <param name="depthWidth">Width of the depth stream image.</param>
        /// <param name="depthHeight">Height of the depth stream image.</param>
        /// <param name="foreColor">A foreground color for player outline.  Transparent uses original rgb colors.</param>
        /// <returns></returns>
        public static WriteableBitmap RenderActivePlayer(this KinectSensor sensor, int depthWidth, int depthHeight, Color foreColor)
        {
            return sensor.RenderActivePlayer(sensor.DepthStream.FrameWidth, sensor.DepthStream.FrameHeight, foreColor, Color.White);
        }


        /// <summary>
        /// Renders all active players detected by Kinect sensor.  Kinect color, depth and skeleton streams must be enabled.
        /// </summary>
        /// <param name="sensor">The Kinect sensor.</param>
        /// <param name="depthWidth">Width of the depth stream image.</param>
        /// <param name="depthHeight">Height of the depth stream image.</param>
        /// <param name="foreColor">A foreground color for player outline.  Transparent uses original rgb colors.</param>
        /// <param name="backgroundColor">A background color.  Transparent uses original rgb colors.  White is the default.</param>
        /// <returns></returns>
        public static WriteableBitmap RenderActivePlayer(this KinectSensor sensor, int depthWidth, int depthHeight, Color foreColor, Color backgroundColor)
        {
            if (sensor.DepthStream.IsEnabled == false || sensor.SkeletonStream.IsEnabled == false || sensor.ColorStream.IsEnabled == false)
                throw new InvalidOperationException("Open all the data streams before calling this method.");

            WriteableBitmap target = new WriteableBitmap(depthWidth, depthHeight, 96, 96, Media.PixelFormats.Bgra32, null);
            var targetRect = new System.Windows.Int32Rect(0, 0, depthWidth, depthHeight);

            bool isTransparent = (foreColor == Color.Transparent);
            bool isBackgroundTransparent = (backgroundColor == Color.Transparent);
            bool showBackground = (backgroundColor != Color.White);
            byte R = foreColor.R;
            byte G = foreColor.G;
            byte B = foreColor.B;
            byte backR = backgroundColor.R;
            byte backG = backgroundColor.G;
            byte backB = backgroundColor.B;
            DepthImagePixel[] depthBits = new DepthImagePixel[sensor.DepthStream.FramePixelDataLength];
            ColorImagePoint[] colorCoordinates = new ColorImagePoint[sensor.DepthStream.FramePixelDataLength];
            byte[] colorBits = new byte[sensor.ColorStream.FramePixelDataLength];
            byte[] output = new byte[depthWidth * depthHeight * 4];
            int colorStride = sensor.ColorStream.FrameBytesPerPixel * sensor.ColorStream.FrameWidth;

            sensor.AllFramesReady += (s, e) =>
            {
                using (var depthFrame = e.OpenDepthImageFrame())
                using (var colorFrame = e.OpenColorImageFrame())
                {
                    if (depthFrame != null && colorFrame != null)
                    {

                        depthFrame.CopyDepthImagePixelDataTo(depthBits);
                        colorFrame.CopyPixelDataTo(colorBits);

                        if (colorCoordinates == null)
                            colorCoordinates =
                            new ColorImagePoint[depthFrame.PixelDataLength];

                        sensor.CoordinateMapper.MapDepthFrameToColorFrame(depthFrame.Format, depthBits,
                                                        colorFrame.Format,
                                                        colorCoordinates);

                        Parallel.For(0, depthFrame.Height, depthRowIndex =>
                        {
                            for (int depthImageColumnIndex = 0; depthImageColumnIndex < depthFrame.Width; depthImageColumnIndex++)
                            {
                                var depthIndex = depthImageColumnIndex + (depthRowIndex * depthFrame.Width);
                                var outputIndex = depthIndex * 4;
                                var playerIndex = depthBits[depthIndex].PlayerIndex;

                                var colorPoint = colorCoordinates[depthIndex];

                                var colorPixelIndex = (colorPoint.X * colorFrame.BytesPerPixel) +
                                                    (colorPoint.Y * colorStride);

                                if ((playerIndex > 0 && isTransparent) || (playerIndex == 0 && isBackgroundTransparent))
                                {
                                    output[outputIndex] = colorBits[colorPixelIndex + 0];
                                    output[outputIndex + 1] = colorBits[colorPixelIndex + 1];
                                    output[outputIndex + 2] = colorBits[colorPixelIndex + 2];
                                }
                                else if (playerIndex > 0)
                                {
                                    output[outputIndex] = B;
                                    output[outputIndex + 1] = G;
                                    output[outputIndex + 2] = R;
                                }
                                else if (showBackground)
                                {
                                    output[outputIndex] = backB;
                                    output[outputIndex + 1] = backG;
                                    output[outputIndex + 2] = backR;
                                }
                                //alpha channel
                                output[outputIndex + 3] = (playerIndex > 0 || showBackground) ? (byte)255 : (byte)0;

                            }
                        });
                        target.WritePixels(targetRect, output,
                                        depthFrame.Width * 4, 0);

                    }

                }

            };
            return target;
        }

        /// <summary>
        /// Renders the predator thermal vision view.
        /// </summary>
        /// <param name="sensor">The Kinect sensor.</param>
        /// <returns></returns>
        public static WriteableBitmap RenderPredatorView(this KinectSensor sensor)
        {
            if (sensor.DepthStream.IsEnabled == false || sensor.SkeletonStream.IsEnabled == false || sensor.ColorStream.IsEnabled == false)
                throw new InvalidOperationException("Open all the data streams before calling this method.");

            var depthWidth = sensor.DepthStream.FrameWidth;
            var depthHeight = sensor.DepthStream.FrameHeight;

            WriteableBitmap target = new WriteableBitmap(depthWidth, depthHeight, 96, 96, Media.PixelFormats.Bgra32, null);
            var targetRect = new System.Windows.Int32Rect(0, 0, depthWidth, depthHeight);

            DepthImagePixel[] depthBits = new DepthImagePixel[sensor.DepthStream.FramePixelDataLength];
            ColorImagePoint[] colorCoordinates = new ColorImagePoint[sensor.DepthStream.FramePixelDataLength];
            byte[] colorBits = new byte[sensor.ColorStream.FramePixelDataLength];
            byte[] output = new byte[depthWidth * depthHeight * 4];
            int colorStride = sensor.ColorStream.FrameBytesPerPixel * sensor.ColorStream.FrameWidth;

            sensor.AllFramesReady += (s, e) =>
            {
                using (var depthFrame = e.OpenDepthImageFrame())
                using (var colorFrame = e.OpenColorImageFrame())
                {
                    if (depthFrame != null && colorFrame != null)
                    {

                        depthFrame.CopyDepthImagePixelDataTo(depthBits);
                        colorFrame.CopyPixelDataTo(colorBits);

                        if (colorCoordinates == null)
                            colorCoordinates =
                            new ColorImagePoint[depthFrame.PixelDataLength];

                        sensor.CoordinateMapper.MapDepthFrameToColorFrame(depthFrame.Format, depthBits,
                                                        colorFrame.Format,
                                                        colorCoordinates);

                        Parallel.For(0, depthFrame.Height, depthRowIndex =>
                        {
                            for (int depthImageColumnIndex = 0; depthImageColumnIndex < depthFrame.Width; depthImageColumnIndex++)
                            {
                                var depthIndex = depthImageColumnIndex + (depthRowIndex * depthFrame.Width);
                                var playerIndex = depthBits[depthIndex].PlayerIndex;
                                var depth = depthBits[depthIndex].Depth;
                                var colorPoint = colorCoordinates[depthIndex];
                                var colorPixelIndex = (colorPoint.X * colorFrame.BytesPerPixel) +
                                                    (colorPoint.Y * colorStride);

                                var outputIndex = depthIndex * 4;


                                if (playerIndex > 0)
                                {
                                    output[outputIndex] = 0;
                                    output[outputIndex + 1] = Convert.ToByte(depth % 255);
                                    output[outputIndex + 2] = Convert.ToByte(depth % 120 + 134);
                                    output[outputIndex + 3] = 255;
                                }

                                if (playerIndex == 0)
                                {
                                    var b = colorBits[colorPixelIndex];
                                    output[outputIndex] = b < (byte)100.0 ? (byte)0 : b;
                                    output[outputIndex + 1] = b < (byte)100.0 ? (byte)0 : (byte)70;
                                    output[outputIndex + 2] = 0;
                                    output[outputIndex + 3] = 250;
                                }

                            }
                        });
                        target.WritePixels(targetRect, output,
                                        depthFrame.Width * 4, 0);

                    }

                }

            };
            return target;
        }

        /// <summary>
        /// Renders the player skeleton.
        /// </summary>
        /// <param name="sensor">The Kinect sensor.</param>
        /// <param name="jointThickness">The thickness of drawn joints.</param>
        /// <param name="trackedBoneThickness">The tracked bone thickness.</param>
        /// <param name="inferredBoneThickness">The inferred bone thickness.</param>
        /// <param name="bodyCenterThickness">The thickness of the position only center point.</param>
        /// <returns></returns>
        public static Media.DrawingImage RenderPlayerSkeleton(this KinectSensor sensor, double jointThickness = 3, double trackedBoneThickness = 6, double inferredBoneThickness = 1, double bodyCenterThickness = 10)
        {
            return RenderPlayerSkeleton(sensor, Color.Black, jointThickness, trackedBoneThickness, inferredBoneThickness, bodyCenterThickness);
        }

        /// <summary>
        /// Renders the player skeleton.
        /// </summary>
        /// <param name="sensor">The Kinect sensor.</param>
        /// <param name="backgroundColor">Color of the background.  Default is Black.</param>
        /// <param name="jointThickness">The thickness of drawn joints.</param>
        /// <param name="trackedBoneThickness">The tracked bone thickness.</param>
        /// <param name="inferredBoneThickness">The inferred bone thickness.</param>
        /// <param name="bodyCenterThickness">The thickness of the position only center point.</param>
        /// <returns></returns>
        public static Media.DrawingImage RenderPlayerSkeleton(this KinectSensor sensor, Color backgroundColor, double jointThickness = 3, double trackedBoneThickness = 6, double inferredBoneThickness = 1, double bodyCenterThickness = 10)
        {
            return RenderPlayerSkeleton(sensor, backgroundColor, Color.FromArgb(255, 68, 192, 68), Color.Green, Color.Yellow, Color.Gray, Color.Blue, jointThickness, trackedBoneThickness, inferredBoneThickness, bodyCenterThickness);
        }

        /// <summary>
        /// Renders the player skeleton.
        /// </summary>
        /// <param name="sensor">The Kinect sensor.</param>
        /// <param name="backgroundColor">Color of the background.  Default is Black.</param>
        /// <param name="trackedJointColor">Color of the tracked joint.</param>
        /// <param name="trackedBoneColor">Color of the tracked bone.</param>
        /// <param name="jointThickness">The thickness of drawn joints.</param>
        /// <param name="trackedBoneThickness">The tracked bone thickness.</param>
        /// <param name="inferredBoneThickness">The inferred bone thickness.</param>
        /// <param name="bodyCenterThickness">The thickness of the position only center point.</param>
        /// <returns></returns>
        public static Media.DrawingImage RenderPlayerSkeleton(this KinectSensor sensor, Color backgroundColor, Color trackedJointColor, Color trackedBoneColor, double jointThickness = 3, double trackedBoneThickness = 6, double inferredBoneThickness = 1, double bodyCenterThickness = 10)
        {
            return RenderPlayerSkeleton(sensor, backgroundColor, trackedJointColor, trackedBoneColor, Color.Green, Color.Gray, Color.Blue, jointThickness, trackedBoneThickness, inferredBoneThickness, bodyCenterThickness);
        }

        /// <summary>
        /// Renders the player skeleton.
        /// </summary>
        /// <param name="sensor">The Kinect sensor.</param>
        /// <param name="backgroundColor">Color of the background.  Default is Black.</param>
        /// <param name="trackedJointColor">Color of the tracked joint.</param>
        /// <param name="trackedBoneColor">Color of the tracked bone.</param>
        /// <param name="inferredJointColor">Color of the inferred joint.</param>
        /// <param name="inferredBoneColor">Color of the inferred bone.</param>
        /// <param name="jointThickness">The thickness of drawn joints.</param>
        /// <param name="trackedBoneThickness">The tracked bone thickness.</param>
        /// <param name="inferredBoneThickness">The inferred bone thickness.</param>
        /// <param name="bodyCenterThickness">The thickness of the position only center point.</param>
        /// <returns></returns>
        public static Media.DrawingImage RenderPlayerSkeleton(this KinectSensor sensor, Color backgroundColor, Color trackedJointColor, Color trackedBoneColor, Color inferredJointColor, Color inferredBoneColor, double jointThickness = 3, double trackedBoneThickness = 6, double inferredBoneThickness = 1, double bodyCenterThickness = 10)
        {
            return RenderPlayerSkeleton(sensor, backgroundColor, trackedJointColor, trackedBoneColor, inferredJointColor, inferredBoneColor, Color.Blue, jointThickness, trackedBoneThickness, inferredBoneThickness, bodyCenterThickness);
        }

        /// <summary>
        /// Renders the player skeleton.
        /// </summary>
        /// <param name="sensor">The Kinect sensor.</param>
        /// <param name="backgroundColor">Color of the background.  Default is Black.</param>
        /// <param name="trackedJointColor">Color of the tracked joint.</param>
        /// <param name="trackedBoneColor">Color of the tracked bone.</param>
        /// <param name="inferredJointColor">Color of the inferred joint.</param>
        /// <param name="inferredBoneColor">Color of the inferred bone.</param>
        /// <param name="centerPointColor">Color of the center point when tracking state is position only.</param>
        /// <param name="jointThickness">The thickness of drawn joints.</param>
        /// <param name="trackedBoneThickness">The tracked bone thickness.</param>
        /// <param name="inferredBoneThickness">The inferred bone thickness.</param>
        /// <param name="bodyCenterThickness">The thickness of the position only center point.</param>
        /// <returns></returns>
        public static Media.DrawingImage RenderPlayerSkeleton(this KinectSensor sensor, Color backgroundColor, Color trackedJointColor, Color trackedBoneColor, Color inferredJointColor, Color inferredBoneColor, Color centerPointColor, double jointThickness = 3, double trackedBoneThickness = 6, double inferredBoneThickness = 1, double bodyCenterThickness = 10)
        {
            if (sensor.SkeletonStream.IsEnabled == false)
                throw new InvalidOperationException("Open skeleton stream before calling this method.");

            float renderWidth = 640.0f;
            float renderHeight = 480.0f;
            var centerPointBrush = new Media.SolidColorBrush(Media.Color.FromArgb(centerPointColor.A, centerPointColor.R, centerPointColor.G, centerPointColor.B));
            var trackedJointBrush = new Media.SolidColorBrush(Media.Color.FromArgb(trackedJointColor.A, trackedJointColor.R, trackedJointColor.G, trackedJointColor.B));
            var inferredJointBrush = new Media.SolidColorBrush(Media.Color.FromArgb(inferredJointColor.A, inferredJointColor.R, inferredJointColor.G, inferredJointColor.B));
            var trackedBonePen = new Media.Pen(new Media.SolidColorBrush(Media.Color.FromArgb(trackedBoneColor.A, trackedBoneColor.R, trackedBoneColor.G, trackedBoneColor.B)), trackedBoneThickness);
            var inferredBonePen = new Media.Pen(new Media.SolidColorBrush(Media.Color.FromArgb(inferredBoneColor.A, inferredBoneColor.R, inferredBoneColor.G, inferredBoneColor.B)), inferredBoneThickness);
            var drawingGroup = new Media.DrawingGroup();
            var imageSource = new Media.DrawingImage(drawingGroup);
            Skeleton[] skeletons = new Skeleton[sensor.SkeletonStream.FrameSkeletonArrayLength];

            sensor.SkeletonFrameReady += (s, e) =>
                {
                    using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
                    {
                        if (skeletonFrame == null)
                            return;

                        skeletonFrame.CopySkeletonDataTo(skeletons);
                    }

                    using (Media.DrawingContext dc = drawingGroup.Open())
                    {
                        // Draw a transparent background to set the render size
                        dc.DrawRectangle(new Media.SolidColorBrush(Media.Color.FromArgb(backgroundColor.A, backgroundColor.R, backgroundColor.G, backgroundColor.B))
                            , null, new Rect(0.0, 0.0, renderWidth, renderHeight));


                        foreach (Skeleton skel in skeletons)
                        {
                            if (skel == null || skel.TrackingState == SkeletonTrackingState.NotTracked)
                                continue;

                            if (skel.TrackingState == SkeletonTrackingState.Tracked)
                            {
                                DrawBonesAndJoints(sensor, skel, dc, jointThickness, inferredBonePen, inferredJointBrush, trackedBonePen, trackedJointBrush);
                            }
                            else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                            {
                                dc.DrawEllipse(
                                centerPointBrush,
                                null,
                                sensor.SkeletonPointToScreen(skel.Position),
                                bodyCenterThickness,
                                bodyCenterThickness);
                            }
                        }


                        // prevent drawing outside of our render area
                        drawingGroup.ClipGeometry = new Media.RectangleGeometry(new Rect(0.0, 0.0, renderWidth, renderHeight));
                    }
                };

            return imageSource;
        }

        private static void DrawBonesAndJoints(KinectSensor sensor, Skeleton skeleton, Media.DrawingContext drawingContext, double JointThickness
            , System.Windows.Media.Pen inferredBonePen, System.Windows.Media.Brush inferredJointBrush
            , System.Windows.Media.Pen trackedBonePen, System.Windows.Media.Brush trackedJointBrush)
        {
            // Render Torso
            DrawBone(sensor, skeleton, drawingContext, JointType.Head, JointType.ShoulderCenter, inferredBonePen, inferredJointBrush, trackedBonePen, trackedJointBrush);
            DrawBone(sensor, skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft, inferredBonePen, inferredJointBrush, trackedBonePen, trackedJointBrush);
            DrawBone(sensor, skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight, inferredBonePen, inferredJointBrush, trackedBonePen, trackedJointBrush);
            DrawBone(sensor, skeleton, drawingContext, JointType.ShoulderCenter, JointType.Spine, inferredBonePen, inferredJointBrush, trackedBonePen, trackedJointBrush);
            DrawBone(sensor, skeleton, drawingContext, JointType.Spine, JointType.HipCenter, inferredBonePen, inferredJointBrush, trackedBonePen, trackedJointBrush);
            DrawBone(sensor, skeleton, drawingContext, JointType.HipCenter, JointType.HipLeft, inferredBonePen, inferredJointBrush, trackedBonePen, trackedJointBrush);
            DrawBone(sensor, skeleton, drawingContext, JointType.HipCenter, JointType.HipRight, inferredBonePen, inferredJointBrush, trackedBonePen, trackedJointBrush);

            // Left Arm
            DrawBone(sensor, skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft, inferredBonePen, inferredJointBrush, trackedBonePen, trackedJointBrush);
            DrawBone(sensor, skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft, inferredBonePen, inferredJointBrush, trackedBonePen, trackedJointBrush);
            DrawBone(sensor, skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft, inferredBonePen, inferredJointBrush, trackedBonePen, trackedJointBrush);

            // Right Arm
            DrawBone(sensor, skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight, inferredBonePen, inferredJointBrush, trackedBonePen, trackedJointBrush);
            DrawBone(sensor, skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight, inferredBonePen, inferredJointBrush, trackedBonePen, trackedJointBrush);
            DrawBone(sensor, skeleton, drawingContext, JointType.WristRight, JointType.HandRight, inferredBonePen, inferredJointBrush, trackedBonePen, trackedJointBrush);

            // Left Leg
            DrawBone(sensor, skeleton, drawingContext, JointType.HipLeft, JointType.KneeLeft, inferredBonePen, inferredJointBrush, trackedBonePen, trackedJointBrush);
            DrawBone(sensor, skeleton, drawingContext, JointType.KneeLeft, JointType.AnkleLeft, inferredBonePen, inferredJointBrush, trackedBonePen, trackedJointBrush);
            DrawBone(sensor, skeleton, drawingContext, JointType.AnkleLeft, JointType.FootLeft, inferredBonePen, inferredJointBrush, trackedBonePen, trackedJointBrush);

            // Right Leg
            DrawBone(sensor, skeleton, drawingContext, JointType.HipRight, JointType.KneeRight, inferredBonePen, inferredJointBrush, trackedBonePen, trackedJointBrush);
            DrawBone(sensor, skeleton, drawingContext, JointType.KneeRight, JointType.AnkleRight, inferredBonePen, inferredJointBrush, trackedBonePen, trackedJointBrush);
            DrawBone(sensor, skeleton, drawingContext, JointType.AnkleRight, JointType.FootRight, inferredBonePen, inferredJointBrush, trackedBonePen, trackedJointBrush);

            // Render Joints
            foreach (Joint joint in skeleton.Joints)
            {
                Media.Brush drawBrush = null;

                if (joint.TrackingState == JointTrackingState.Tracked)
                {
                    drawBrush = trackedJointBrush;
                }
                else if (joint.TrackingState == JointTrackingState.Inferred)
                {
                    drawBrush = inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, sensor.SkeletonPointToScreen(joint.Position), JointThickness, JointThickness);
                }
            }
        }

        private static void DrawBone(KinectSensor sensor, Skeleton skeleton, Media.DrawingContext drawingContext
            , JointType jointType0, JointType jointType1
            , Media.Pen inferredBonePen, Media.Brush inferredJointBrush
            , Media.Pen trackedBonePen, Media.Brush trackedJointBrush)
        {
            Joint joint0 = skeleton.Joints[jointType0];
            Joint joint1 = skeleton.Joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == JointTrackingState.NotTracked ||
                joint1.TrackingState == JointTrackingState.NotTracked)
            {
                return;
            }

            // Don't draw if both points are inferred
            if (joint0.TrackingState == JointTrackingState.Inferred &&
                joint1.TrackingState == JointTrackingState.Inferred)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Media.Pen drawPen = inferredBonePen;
            if (joint0.TrackingState == JointTrackingState.Tracked && joint1.TrackingState == JointTrackingState.Tracked)
            {
                drawPen = trackedBonePen;
            }

            drawingContext.DrawLine(drawPen, sensor.SkeletonPointToScreen(joint0.Position), sensor.SkeletonPointToScreen(joint1.Position));
        }

        private static System.Windows.Point SkeletonPointToScreen(this KinectSensor sensor, SkeletonPoint skelpoint)
        {
            // Convert point to depth space.  
            // We are not using depth directly, but we do want the points in our 640x480 output resolution.
            DepthImagePoint depthPoint = sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            return new System.Windows.Point(depthPoint.X, depthPoint.Y);
        }


    }
}
