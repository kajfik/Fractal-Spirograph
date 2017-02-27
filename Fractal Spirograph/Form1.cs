using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using PVectorLibrary;

namespace Fractal_Spirograph
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        static Bitmap bmp;
        static Graphics g;
        static int width, height;
        static Random rnd = new Random();
        static long frameCount = 0;
        List<PVector> path = new List<PVector>();

        Circle sun;
        Circle end;

        const int k = -4;

        class Circle
        {
            public float x;
            public float y;
            public float r;
            public float angle;
            public float speed;
            public int n;
            public Circle parent;
            public Circle child;

            public Circle(float x, float y, float r, int n) : this(x, y, r, n, null)
            {
                
            }

            public Circle(float x, float y, float r, int n, Circle parent)
            {
                this.x = x;
                this.y = y;
                this.r = r;
                this.n = n;
                this.parent = parent;
                angle = 0;
                speed = (float)(Math.Pow(k, n - 1) * Math.PI / 180.0F) / 10.0F;
            }

            public Circle addChild()
            {
                float newr = r / 3.0F;
                float newx = x + r + newr;
                float newy = y;
                child = new Circle(newx, newy, newr, n + 1, this);
                return child;
            }

            public void update()
            {
                if(parent != null)
                {
                    angle += speed;
                    float rsum = r + parent.r;
                    x = parent.x + rsum * (float)Math.Cos(angle);
                    y = parent.y + rsum * (float)Math.Sin(angle);
                }
            }

            public void show()
            {
                g.DrawEllipse(Pens.Gray, x - r, y - r, r * 2.0F, r * 2.0F);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Width = Form1.ActiveForm.Width - 16;
            pictureBox1.Height = Form1.ActiveForm.Height - 39;
            width = pictureBox1.Width;
            height = pictureBox1.Height;
            bmp = new Bitmap(width, height);
            g = Graphics.FromImage(bmp);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Color.White);
            pictureBox1.Image = bmp;

            sun = new Circle(width / 2, height / 2, 150, 0);
            Circle current = sun;
            for(int i = 0; i < 10; i++)
            {
                current = current.addChild();
            }
            end = current;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            g.Clear(Color.Black);

            Circle current = sun;
            while(current != null)
            {
                current.update();
                //current.show();
                current = current.child;
            }

            path.Add(new PVector(end.x, end.y));

            if (frameCount % 5 == 0)
            {
                for (int i = 1; i < path.Count; i++)
                {
                    g.DrawLine(Pens.MediumPurple, path[i - 1].x, path[i - 1].y, path[i].x, path[i].y);
                }
                pictureBox1.Image = bmp;
                pictureBox1.Update();
            }

            if(frameCount > 50 && sun.child.angle % (Math.PI * 2.0F) < 0.01F)
            {
                timer1.Stop();
                bmp.Save("Spirograph.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            }

            frameCount++;
        }
    }
}
