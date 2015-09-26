using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp.Extensions;

namespace HSVFilter
{
    public partial class Form1 : Form
    {
        Thread _cameraThread;
        VideoCapture cap = VideoCapture.FromCamera(0);

        private int minBound = 0, maxBound = 255;

        public Form1()
        {
            InitializeComponent();

            _cameraThread = new Thread(CaptureCameraCallback);
            _cameraThread.Start();
        }

        private void CaptureCameraCallback()
        {
            while (Window.WaitKey(10) < 0)
            {
                using (var image = new Mat())
                {
                    cap.Read(image);
                    if (image.Empty())
                        break;

                    pictureBox1.Image = GetChannel(image).ToBitmap();
                }
            }
        }

        private Mat GetChannel(Mat input)
        {
            using (var hsvImage = new Mat())
            {
                //input.ConvertTo(input, -1, 1.0, -250);
                //input.ConvertTo(input, -1, 1000.0, 0);

                Cv2.CvtColor(input, hsvImage, ColorConversion.RgbToHsv);

                var hsvImage_channels = new Mat[3];
                Cv2.Split(hsvImage, out hsvImage_channels);

                Cv2.InRange(hsvImage_channels[2], minBound, maxBound, hsvImage_channels[2]);
                GC.Collect();


                return hsvImage_channels[2];
                //return input;
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            minBound = trackBar1.Value;
            label1.Text = minBound.ToString();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            maxBound = trackBar2.Value;
            label2.Text = maxBound.ToString();
        }
    }
}
