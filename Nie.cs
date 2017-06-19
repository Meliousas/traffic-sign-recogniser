using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Flann;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.XFeatures2D;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TrafficSignRecogniser
{
    public partial class Nie : Form
    {
        List<Image<Bgr, byte>> model= new List<Image<Bgr, byte>>();
        Image<Bgr, byte> current;
        private SURF detector = new SURF(300);


        public Nie()
        {
            InitializeComponent();
            foreach (string f in System.IO.Directory.GetFiles("model"))
            {
                try
                {
                    Image<Bgr, byte> img = new Image<Bgr,byte>(f);
                    model.Add(img);

                }
                catch
                {
                    // Out of Memory Exceptions are thrown in Image.FromFile if you pass in a non-image file.
                }
            }
            //var item = model[model.Count - 1].Resize(pictureBox2.Width, pictureBox2.Height, Inter.Linear);
            //pictureBox2.Image = item.ToBitmap();
          
        }

        private void recognise() {
            foreach (var image in model)
            {
                using (Mat modelImage = image.Mat)
                {
                    VectorOfKeyPoint modelKeyPoints;
                    VectorOfVectorOfDMatch matches = new VectorOfVectorOfDMatch();
                    Mat mask;
                    Mat homography;

                   find(modelImage,out modelKeyPoints, matches, out mask, out homography);

                    if (homography != null) {
                        Image<Bgr, byte> itemx = modelImage.ToImage< Bgr, byte>();

                        //display found sign in another pictureBox
                        var item = itemx.Resize(pictureBox2.Width, pictureBox2.Height, Inter.Linear);
                        pictureBox2.Image = item.ToBitmap();
                    }
                }
            }
        }

        private void find(Mat modelImage, out VectorOfKeyPoint modelKeyPoints, VectorOfVectorOfDMatch matches, out Mat mask, out Mat homography) {

            int k = 2;
            double uniquenessThreshold = 0.60;

            homography = null;

            VectorOfKeyPoint currentKeyPoints = new VectorOfKeyPoint();
            Mat currentDescriptors = new Mat();
            detector.DetectAndCompute(current, null, currentKeyPoints, currentDescriptors, false);

            modelKeyPoints = new VectorOfKeyPoint();
            Mat modelDescriptors = new Mat();
            detector.DetectAndCompute(modelImage, null, modelKeyPoints, modelDescriptors, false);

            LinearIndexParams ip = new LinearIndexParams();
            SearchParams sp = new SearchParams();
            DescriptorMatcher matcher = new FlannBasedMatcher(ip, sp);

            matcher.Add(modelDescriptors);

            matcher.KnnMatch(currentDescriptors, matches, k, null);
            mask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
            mask.SetTo(new MCvScalar(255));
            Features2DToolbox.VoteForUniqueness(matches, uniquenessThreshold, mask);

            int nonZeroCount = CvInvoke.CountNonZero(mask);
            if (nonZeroCount >= 4)
            {
                nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, currentKeyPoints, matches, mask, 1.5, 20);
                if (nonZeroCount >= 4)
                    homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints, currentKeyPoints, matches, mask, 2);
            }

        }
   
        private void Nie_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == DialogResult.OK)
            {

                Image<Bgr, Byte> myImage = new Image<Bgr, byte>(openFile.FileName);
                var tempImage = myImage.Resize(pictureBox1.Width, pictureBox1.Height, Emgu.CV.CvEnum.Inter.Linear);
                Image<Gray, Byte> hsvImage = tempImage.Convert<Gray, Byte>();
                pictureBox1.Image = tempImage.ToBitmap();
                current = tempImage;

            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                pictureBox2.Show();
                recognise();
            }
            else {
                pictureBox2.Hide();
            }
            



        }
    }
}
