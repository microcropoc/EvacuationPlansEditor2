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
    class EllipseWall : BuildObject
    {
        public EllipseGeometry EllipseObject { get; private set; }

        public EllipseWall(string name)
        {
            base.Name = name;
            EllipseObject = new EllipseGeometry();
            VisualObject = new Path(){Stroke = Brushes.Black,Data=EllipseObject};
        }

        public EllipseWall(XmlTextReader xmlIn):this("")
        {
            LoadFromFile(xmlIn);
        }

        public override void SaveToFile(XmlTextWriter xmlOut)
        {
            xmlOut.WriteStartElement("EllipseWall");
            xmlOut.WriteAttributeString("Name", base.Name);
            xmlOut.WriteAttributeString("Center", EllipseObject.Center.X.ToString() + ':' + EllipseObject.Center.Y.ToString());
            xmlOut.WriteAttributeString("RadiusX", EllipseObject.RadiusX.ToString());
            xmlOut.WriteAttributeString("RadiusY", EllipseObject.RadiusY.ToString());
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
                EllipseObject.Center = new Point(Convert.ToDouble(xmlIn.GetAttribute("Center").Split(':')[0]), Convert.ToDouble(xmlIn.GetAttribute("Center").Split(':')[1]));
                EllipseObject.RadiusX = Convert.ToDouble(xmlIn.GetAttribute("RadiusX"));
                EllipseObject.RadiusY = Convert.ToDouble(xmlIn.GetAttribute("RadiusY"));
            }
            catch
            {

            }
        }
    }
}
