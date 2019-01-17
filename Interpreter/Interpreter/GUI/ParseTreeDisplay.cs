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
            Console.WriteLine("OnExpose");
            DrawingArea drawingArea = this.tree;
            Cairo.Context ctx = Gdk.CairoHelper.Create(drawingArea.GdkWindow);

            Cairo.PointD point = new Cairo.PointD()
            {
                X = 150.0,
                Y = 100.0,
            };
            ctx.SelectFontFace("Sans", Cairo.FontSlant.Normal, Cairo.FontWeight.Normal);
            ctx.SetFontSize(15.0);

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
            //Console.WriteLine("DisplayTree");
            int width = 0;
            int height = 0;

            this.GetSize(out width, out height);

            double y = (level * 50) + 15;

            double x = 0;

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
                int childrenLength = n.Children().Count;
                for (int i = 0; i < childrenLength; i++)
                {
                    CoreParser.Parser.AST.Node node = n.Children()[i];
                    y = ((level+1) * 20) + 15;

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
                    ctx.MoveTo(x, y);
                    ctx.ShowText(node.Token.token);
                }
            }


        }
    }
}
