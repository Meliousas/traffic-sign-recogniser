using Emgu.CV;
using Emgu.CV.Structure;
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


        public Nie()
        {
            InitializeComponent();
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
                var tempImage = myImage.Resize(480, 320, Emgu.CV.CvEnum.Inter.Linear);
                Image<Gray, Byte> hsvImage = tempImage.Convert<Gray, Byte>();
                pictureBox1.Image = tempImage.ToBitmap();

            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
