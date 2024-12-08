using Infragistics.Win;
using Infragistics.Win.Misc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CustomControls.JControls
{
    class JUButton : UltraButton
    {
        //Fields
        private int borderSize = 0;
        private int borderRadius = 0;
        private Color borderColor = Color.PaleVioletRed;
        private Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();

        //Properties
        [Category("JControls 확장")]
        public int BorderSize
        {
            get { return borderSize; }
            set
            {
                borderSize = value;
                this.Invalidate();
            }
        }

        [Category("JControls 확장")]
        public int BorderRadius
        {
            get { return borderRadius; }
            set
            {
                borderRadius = value;
                this.Invalidate();
            }
        }

        [Category("JControls 확장")]
        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = value;
                this.Invalidate();
            }
        }

        [Category("JControls 확장")]
        public Color BackgroundColor
        {
            get { return this.BackColor; }
            set { this.BackColor = value; }
        }

        [Category("JControls 확장")]
        public Color TextColor
        {
            get { return this.ForeColor; }
            set { this.ForeColor = value; }
        }

        //Constructor
        public JUButton()
        {
            //this.FlatStyle = FlatStyle.Flat;
            //this.FlatAppearance.BorderSize = 0;
            this.Size = new Size(150, 40);
            //this.BackColor = Color.MediumSlateBlue;
            //this.ForeColor = Color.White;
            this.Appearance.BackColor = Color.MediumSlateBlue; 
            this.Appearance.ForeColor = Color.White;
            this.Resize += new EventHandler(Button_Resize);

            // don't display a focus rect when the control gets focus
            this.ShowFocusRect = false;

            // allow the control to get focus
            this.AcceptsFocus = true;

            // do not show the outline around the control
            // when it is the default button
            this.ShowOutline = false;



            CreateShapeImage(this);
        }

        //Methods
        private GraphicsPath GetFigurePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            float curveSize = radius * 2F;

            path.StartFigure();
            path.AddArc(rect.X, rect.Y, curveSize, curveSize, 180, 90);
            path.AddArc(rect.Right - curveSize, rect.Y, curveSize, curveSize, 270, 90);
            path.AddArc(rect.Right - curveSize, rect.Bottom - curveSize, curveSize, curveSize, 0, 90);
            path.AddArc(rect.X, rect.Bottom - curveSize, curveSize, curveSize, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            Rectangle rectSurface = this.ClientRectangle;
            Rectangle rectBorder = Rectangle.Inflate(rectSurface, -borderSize, -borderSize);
            int smoothSize = 2;
            if (borderSize > 0)
                smoothSize = borderSize;

            if (borderRadius > 2) //Rounded button
            {
                using (GraphicsPath pathSurface = GetFigurePath(rectSurface, borderRadius))
                using (GraphicsPath pathBorder = GetFigurePath(rectBorder, borderRadius - borderSize))
                using (Pen penSurface = new Pen(this.Parent.BackColor, smoothSize))
                using (Pen penBorder = new Pen(borderColor, borderSize))
                {
                    pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    //Button surface
                    this.Region = new Region(pathSurface);
                    //Draw surface border for HD result
                    pevent.Graphics.DrawPath(penSurface, pathSurface);

                    //Button border                    
                    if (borderSize >= 1)
                        //Draw control border
                        pevent.Graphics.DrawPath(penBorder, pathBorder);
                }
            }
            else //Normal button
            {
                pevent.Graphics.SmoothingMode = SmoothingMode.None;
                //Button surface
                this.Region = new Region(rectSurface);
                //Button border
                if (borderSize >= 1)
                {
                    using (Pen penBorder = new Pen(borderColor, borderSize))
                    {
                        penBorder.Alignment = PenAlignment.Inset;
                        pevent.Graphics.DrawRectangle(penBorder, 0, 0, this.Width - 1, this.Height - 1);
                    }
                }
            }
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            this.Parent.BackColorChanged += new EventHandler(Container_BackColorChanged);
        }
        private void Container_BackColorChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }
        private void Button_Resize(object sender, EventArgs e)
        {
            if (borderRadius > this.Height)
                borderRadius = this.Height;
        }

        private void CreateShapeImage(Infragistics.Win.Misc.UltraButton button)
        {
            // create a bitmap that will be used to provide the shape
            // of the button.
            Bitmap bitmap = new Bitmap(100, 40);

            Pen blackPen = new Pen(Color.Black, 3);
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            // Create start and sweep angles on ellipse.
            float startAngle = 45.0F;
            float sweepAngle = 270.0F;

            // create a temporary graphics object so we can render into it
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                // draw the background in white. whatever color
                // is in the lower left hand pixel will be assumed
                // to be transparent
                g.Clear(Color.White);

                // draw our circle in a different color
                g.DrawArc(blackPen, rect, startAngle, sweepAngle);

                // make sure to fill it in or the only displayed
                // part of the button will be the outline of the
                // circle
                //g.FillEllipse(Brushes.Black, 0, 0, 99, 99);
            }

            // set the shape
            button.ShapeImage = bitmap;

            // autosize to the shape image
            button.AutoSize = true;
        }
    }
}
