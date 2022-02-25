using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Drawing.Drawing2D;

namespace trig_functions
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // The image used for the graph.
        private Bitmap GraphImage;

        // Graph.
        private void btnGraph_Click(object sender, EventArgs e)
        {
            GraphImage = new Bitmap(
                picGraph.ClientSize.Width,
                picGraph.ClientSize.Height);
            using (Graphics gr = Graphics.FromImage(GraphImage))
            {
                gr.Clear(Color.White);
                gr.SmoothingMode = SmoothingMode.AntiAlias;

                using (Pen thin_pen = new Pen(Color.Purple, 0))
                {
                    // Get the bounds.
                    double xmin = double.Parse(txtXmin.Text) * Math.PI;
                    double xmax = double.Parse(txtXmax.Text) * Math.PI;
                    double ymin = double.Parse(txtYmin.Text);
                    double ymax = double.Parse(txtYmax.Text);

                    // Scale to make the area fit the PictureBox.
                    RectangleF world_coords = new RectangleF(
                        (float)xmin, (float)ymax,
                        (float)(xmax - xmin),
                        (float)(ymin - ymax));
                    PointF[] device_coords =
                    {
                        new PointF(0, 0),
                        new PointF(picGraph.ClientSize.Width, 0),
                        new PointF(0, picGraph.ClientSize.Height),
                    };
                    gr.Transform = new Matrix(world_coords, device_coords);

                    // Draw the X-axis.
                    // Start at the multiple of Pi < xmin.
                    double start_x = Math.PI * ((int)(xmin - 1));
                    gr.DrawLine(thin_pen, (float)xmin, 0, (float)xmax, 0);
                    float dy = (float)((ymax - ymin) / 30.0);
                    for (double x = start_x; x <= xmax; x += Math.PI)
                    {
                        gr.DrawLine(thin_pen, (float)x, -2 * dy, (float)x, 2 * dy);
                    }
                    for (double x = start_x + Math.PI / 2.0; x <= xmax; x += Math.PI)
                    {
                        gr.DrawLine(thin_pen, (float)x, -dy, (float)x, dy);
                    }

                    // Draw the Y-axis.
                    // Start at the multiple of 1 < ymin.
                    double start_y = (int)ymin - 1;
                    gr.DrawLine(thin_pen, 0, (float)ymin, 0, (float)ymax);
                    float dx = (float)((xmax - xmin) / 60.0);
                    for (double y = start_y; y <= ymax; y += 1.0)
                    {
                        gr.DrawLine(thin_pen, -2 * dx, (float)y, 2 * dx, (float)y);
                    }
                    for (double y = start_y + 0.5; y <= ymax; y += 1.0)
                    {
                        gr.DrawLine(thin_pen, -dx, (float)y, dx, (float)y);
                    }

                    // Draw vertical asymptotes.
                    thin_pen.DashPattern = new float[] { 5, 5 };
                    for (double x = start_x + Math.PI / 2.0; x <= xmax; x += Math.PI)
                    {
                        gr.DrawLine(thin_pen, (float)x, (float)ymin, (float)x, (float)ymax);
                    }

                    // Draw horizontal limits for sine and cosine.
                    gr.DrawLine(thin_pen, (float)xmin, 1, (float)xmax, 1);
                    gr.DrawLine(thin_pen, (float)xmin, -1, (float)xmax, -1);
                    thin_pen.DashStyle = DashStyle.Solid;
                    
                    // See how big a pixel is before scaling.
                    Matrix inverse = gr.Transform;
                    inverse.Invert();
                    PointF[] pixel_pts =
                    {
                        new PointF(0, 0),
                        new PointF(1, 0),
                    };
                    inverse.TransformPoints(pixel_pts);
                    dx = pixel_pts[1].X - pixel_pts[0].X;

                    // Sine.
                    List<PointF> sine_points = new List<PointF>();
                    for (float x = (float)xmin; x <= xmax; x += dx)
                    {
                        sine_points.Add(new PointF(x, (float)Math.Sin(x)));
                    }
                    thin_pen.Color = Color.Red;
                    gr.DrawLines(thin_pen, sine_points.ToArray());

                    // Cosine.
                    List<PointF> cosine_points = new List<PointF>();
                    for (float x = (float)xmin; x <= xmax; x += dx)
                    {
                        cosine_points.Add(new PointF(x, (float)Math.Cos(x)));
                    }
                    thin_pen.Color = Color.Green;
                    gr.DrawLines(thin_pen, cosine_points.ToArray());

                    // Tangent.
                    List<PointF> tangent_points = new List<PointF>();
                    double old_value = Math.Tan(xmin);
                    thin_pen.Color = Color.Blue;
                    for (float x = (float)xmin; x <= xmax; x += dx)
                    {
                        // See if we're at a discontinuity.
                        double new_value = Math.Tan(x);
                        if ((Math.Abs(new_value - old_value) > 10) &&
                            (Math.Sign(new_value) != Math.Sign(old_value)))
                        {
                            if (tangent_points.Count > 1)
                                gr.DrawLines(thin_pen, tangent_points.ToArray());
                            tangent_points = new List<PointF>();
                        }
                        else
                        {
                            tangent_points.Add(new PointF(x, (float)Math.Tan(x)));
                        }
                    }
                    if (tangent_points.Count > 1)
                        gr.DrawLines(thin_pen, tangent_points.ToArray());
                }
            }

            // Display the result.
            picGraph.Image = GraphImage;
        }
    }
}
