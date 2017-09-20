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
    class RectangleWall:BuildObject
    {
        public RectangleGeometry RectangleObject { get; private set; }

        public RectangleWall(string name)
        {
            base.Name = name;
            RectangleObject = new RectangleGeometry();
            VisualObject = new Path(){Stroke = Brushes.Black, Data=RectangleObject};
        }

        public RectangleWall(XmlTextReader xmlIn): this("")
        {
            LoadFromFile(xmlIn);
        }

        public override void SaveToFile(XmlTextWriter xmlOut)
        {
            xmlOut.WriteStartElement("RectangleWall");
            xmlOut.WriteAttributeString("Name", base.Name);
            xmlOut.WriteAttributeString("Location", RectangleObject.Rect.Location.X.ToString() + ':' + RectangleObject.Rect.Location.Y.ToString());
            xmlOut.WriteAttributeString("BottomRight", RectangleObject.Rect.BottomRight.X.ToString() + ':' + RectangleObject.Rect.BottomRight.Y.ToString());
            xmlOut.WriteAttributeString("Stroke", base.ColorVisualObject.ToString());
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
                RectangleObject.Rect = new Rect(new Point(Convert.ToDouble(xmlIn.GetAttribute("Location").Split(':')[0]), Convert.ToDouble(xmlIn.GetAttribute("Location").Split(':')[1])), new Point(Convert.ToDouble(xmlIn.GetAttribute("BottomRight").Split(':')[0]), Convert.ToDouble(xmlIn.GetAttribute("BottomRight").Split(':')[1])));
                
            }
            catch
            {

            }
        }
    }
}
