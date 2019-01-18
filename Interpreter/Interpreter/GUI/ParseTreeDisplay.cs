using System;
using Gtk;
using CoreParser;
namespace GUI
{
    public partial class ParseTreeDisplay : Gtk.Window
    {
        DrawingArea tree = new DrawingArea();

        CoreParser.Parser.AST.Node ast;

        public ParseTreeDisplay(CoreParser.Parser.AST.Node node) :
                base(Gtk.WindowType.Toplevel)
        {

            this.ast = node;

            //Create draw area
            tree.ExposeEvent += OnExpose;

            this.Add(tree);
            this.Title = "Parse Tree";
            //this.Build();
        }

        void OnExpose(object sender, EventArgs eventArgs)
        {
            DrawingArea drawingArea = this.tree;
            // Get drawing context
            Cairo.Context ctx = Gdk.CairoHelper.Create(drawingArea.GdkWindow);

            // Set initial point
            Cairo.PointD point = new Cairo.PointD()
            {
                X = 150.0,
                Y = 100.0,
            };

            //Setup text style
            ctx.SelectFontFace("Sans", Cairo.FontSlant.Normal, Cairo.FontWeight.Normal);
            ctx.SetFontSize(15.0);


            // Set current and next point
            Cairo.PointD currentPoint = new Cairo.PointD
            {
                X = 200,
                Y = 125,
            };

            Cairo.PointD newPoint = new Cairo.PointD
            {
                X = 125,
                Y = 225,
            };
            this.DisplayTree(this.ast, ctx, currentPoint, 0, true, false);
        }


        private void DisplayTree(CoreParser.Parser.AST.Node n, Cairo.Context ctx, Cairo.PointD currentPoint, int level, bool left, bool leftTree)
        {
            // Init width and height
            int width = 0;
            int height = 0;

            // Get window size
            this.GetSize(out width, out height);


            // Set y value
            double y = (level * 50) + 15;

            // initialise x value
            double x = 0;


            // Calculate x value
            if (level == 0)
            {
                x = (width) - 50 ;
                ctx.MoveTo(x, y);
                //ctx.ShowText(n.Token);
            }
            else
            {
                if (leftTree)
                {
                    x = currentPoint.X - 50.0;
                } else {
                    x = currentPoint.X + 45.0;
                }
            }
            if (n.HasChildren())
            {
                // Get node's children
                int childrenLength = n.Children().Count;
                for (int i = 0; i < childrenLength; i++)
                {
                    // loop over children and calc level
                    CoreParser.Parser.AST.Node node = n.Children()[i];
                    y = ((level+1) * 20) + 15;

                    // Calculate left or right
                    if(i % 2 == 0)
                    {
                        x = x - 50.0;

                        if (node.HasChildren())
                        {
                            DisplayTree(node, ctx, new Cairo.PointD
                            {
                                X = x+45,
                                Y = y,
                            }, level + 1, true, true);
                        }

                    }
                    else
                    {
                        x = x + 100.0;

                        if (node.HasChildren())
                        {
                            DisplayTree(node, ctx, new Cairo.PointD
                            {
                                X = x+65,
                                Y = y,
                            }, level + 1, true, true);
                        }

                    }
                    // Render the node's value to the window
                    ctx.MoveTo(x, y);
                    ctx.ShowText(node.Token.token);
                }
            }


        }
    }
}
