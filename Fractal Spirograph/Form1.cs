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

        Circle sun;
        Circle end;
        Circle begin;
        Circle last = new Circle(0, 0, 0, 0);
        bool endDraw = false;

        const int k = -4;
        float shortest = 50.0F;
        bool drawCircles = false;
        string dateAndTimeWhenProgramStarted;

        List<PVector> path = new List<PVector>();

        public static float map(float value, float valueMin, float valueMax, float targetMin, float targetMax)
        {
            return (float)Math.Floor((((value - valueMin) / (double)(valueMax - valueMin))) * (targetMax - targetMin) + targetMin);
        }

        class Circle
        {
            public PVector pos;
            public double r;
            public double angle;
            public double speed;
            public int n;
            public Circle parent;
            public Circle child;

            public Circle(float x, float y, double r, int n) : this(x, y, r, n, null)
            {
                
            }

            public Circle(float x, float y, double r, int n, Circle parent)
            {
                pos = new PVector(x, y);
                this.r = r;
                this.n = n;
                this.parent = parent;
                angle = 0.0;
                speed = (Math.Pow(-10, n - 1)  * Math.PI / 180.0 / (Math.Pow(2.8, Math.Abs(k - 1))));
            }

            public Circle addChild()
            {
                double newr = r / 3.0F;
                double newx = pos.x + r + newr;
                double newy = pos.y;
                child = new Circle((float)newx, (float)newy, newr, n + 1, this);
                return child;
            }

            public void update()
            {
                if(parent != null)
                {
                    angle += speed;
                    double rsum = r + parent.r;
                    pos.x = parent.pos.x + (float)(rsum * Math.Cos(angle));
                    pos.y = parent.pos.y + (float)(rsum * Math.Sin(angle));
                }
            }

            public void show()
            {
                g.DrawEllipse(Pens.Gray, pos.x - (float)r, pos.y - (float)r, (float)r * 2.0F, (float)r * 2.0F);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dateAndTimeWhenProgramStarted = DateTime.Today.ToString("yyyy-MM-dd") + "_" + DateTime.Now.ToString("HH-mm-ss");
            pictureBox1.Width = Form1.ActiveForm.Width - 16;
            pictureBox1.Height = Form1.ActiveForm.Height - 39;
            width = pictureBox1.Width;
            height = pictureBox1.Height;
            bmp = new Bitmap(width, height);
            g = Graphics.FromImage(bmp);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Color.Black);
            pictureBox1.Image = bmp;

            sun = new Circle(width / 2, height / 2, 150, 0);
            Circle current = sun;
            for(int i = 0; i < 5; i++)
            {
                current = current.addChild();
            }
            begin = new Circle(current.pos.x, current.pos.y, current.r, current.n);
        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            Circle current = sun;
            while(current != null)
            {
                current.update();
                end = current;
                current = current.child;
            }


            path.Add(new PVector(end.pos.x, end.pos.y));
            if (frameCount > 0)
            {
                if(drawCircles)
                {
                    g.Clear(Color.Black);
                    current = sun;
                    while (current != null)
                    {
                        current.show();
                        current = current.child;
                    }
                    for (int i = 1; i < path.Count; i++)
                    {
                        g.DrawLine(Pens.MediumPurple, path[i - 1].x, path[i - 1].y, path[i].x, path[i].y);
                    }
                }
                else
                {
                    g.DrawLine(Pens.MediumPurple, last.pos.x, last.pos.y, end.pos.x, end.pos.y);
                }

                if (frameCount % 3 == 0)
                {
                    pictureBox1.Image = bmp;
                    pictureBox1.Update();
                }
                //g.DrawString(shortest.ToString(), DefaultFont, Brushes.White, width - 100, height - 40);
            }

            

            last.pos = end.pos.copy();
            if(PVector.dist(begin.pos, end.pos) < 4.0F)
            {
                shortest = (float)sun.child.angle % (float)(Math.PI * 2.0F);
            }
            if(endDraw && sun.child.angle % (Math.PI * 2.0F) > 0.01F && sun.child.angle % (Math.PI * 2.0F) < Math.PI * 2.0F - 0.01F)
            {
                timer1.Stop();
                bmp.Save(".\\Images\\" + dateAndTimeWhenProgramStarted + "_SpirographImage.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                g.DrawString("Done", DefaultFont, Brushes.White, width - 100, height - 40);
                pictureBox1.Image = bmp;
                pictureBox1.Update();
            }
            if (frameCount > 400 && (sun.child.angle % (Math.PI * 2.0F) < 0.2F || sun.child.angle % (Math.PI * 2.0F) > Math.PI * 2.0F - 0.2F) && PVector.dist(begin.pos, end.pos) < 4.0F)
            {
                endDraw = true;
            }

            frameCount++;
        }
    }
}
