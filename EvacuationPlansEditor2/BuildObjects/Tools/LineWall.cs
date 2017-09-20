using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
using System.Xml;

namespace EvacuationPlansEditor2
{
    class LineWall:BuildObject
    {
        public Point FirstPoint 
        { 
            get 
            {
                return new Point(((VisualObject as Line).X1), ((VisualObject as Line).Y1)); 
            }

            set
            {
                (VisualObject as Line).X1 = value.X;
                (VisualObject as Line).Y1 = value.Y;
            }
        }

        public Point EndPoint
        {
            get
            {
                return new Point(((VisualObject as Line).X2), ((VisualObject as Line).Y2));
            }

            set
            {
                (VisualObject as Line).X2 = value.X;
                (VisualObject as Line).Y2 = value.Y;
            }
        }

        public LineWall(string name)
        {
            base.Name = name;
            VisualObject = new Line() {Stroke=Brushes.Black};
        }

        public LineWall(string name,Point firstPosition,Point endPosition):this(name)
        {
            FirstPoint = firstPosition;
            EndPoint = endPosition;
        }

        //new
        public LineWall(XmlTextReader xmlIn) : this("")
        {
            LoadFromFile(xmlIn);
        }

        public override void SaveToFile(XmlTextWriter xmlOut)
        {
            xmlOut.WriteStartElement("LineWall");
            xmlOut.WriteAttributeString("Name", base.Name);
            xmlOut.WriteAttributeString("FirstPoint", FirstPoint.X.ToString()+':'+FirstPoint.Y.ToString());
            xmlOut.WriteAttributeString("EndPoint", EndPoint.X.ToString() + ':' + EndPoint.Y.ToString());
            xmlOut.WriteAttributeString("Stroke",base.ColorVisualObject.ToString());
            xmlOut.WriteAttributeString("StrokeThickness", base.ThinknessVisualObject.ToString());
            xmlOut.WriteEndElement();


        }

        public override void LoadFromFile(XmlTextReader xmlIn)
        {
            try
            {
                base.Name = xmlIn.GetAttribute("Name");
                base.ColorVisualObject = Brushes.Black.ToString() == xmlIn.GetAttribute("Stroke") ? Brushes.Black : Brushes.Gray;
                base.ThinknessVisualObject = Convert.ToDouble(xmlIn.GetAttribute("StrokeThickness"));
                FirstPoint = new Point(Convert.ToDouble(xmlIn.GetAttribute("FirstPoint").Split(':')[0]), Convert.ToDouble(xmlIn.GetAttribute("FirstPoint").Split(':')[1]));
                EndPoint = new Point(Convert.ToDouble(xmlIn.GetAttribute("EndPoint").Split(':')[0]), Convert.ToDouble(xmlIn.GetAttribute("EndPoint").Split(':')[1]));
            }
            catch
            {

            }
        }
    }

}
