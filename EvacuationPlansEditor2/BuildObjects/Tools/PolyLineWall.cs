using System;
using System.Collections;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
using System.Xml;

namespace EvacuationPlansEditor2
{
    class PolyLineWall : BuildObject
    {
        public PointCollection Points { get { return (VisualObject as Polyline).Points; } private set { } }

        public Point FirstPoint 
        { 
            get 
            {
                if (Points.Count > 0)
                    return Points[0];
                throw new Exception();
            } 

            set
            {
                if (Points.Count == 0)
                    Points.Add(value);
                else
                    Points[0] = value;
            }
        }

        public Point EndPoint
        {
            get
            {
                if (Points.Count > 0)
                    return Points[Points.Count-1];
                throw new Exception();
            }

            set
            {
                if (Points.Count > 0)
                    Points[Points.Count - 1] = value;
                else
                    throw new Exception();
            }
        }

        public PolyLineWall(string name)
        {
            base.Name = name;
            VisualObject = new Polyline() { Stroke = Brushes.Black};
        }

        //new
        public PolyLineWall(XmlTextReader xmlIn):this("")
        {
            LoadFromFile(xmlIn);
        }

        public override IEnumerator GetEnumerator()
        {
            yield return VisualObject;
        }

        public override void SaveToFile(XmlTextWriter xmlOut)
        {
            xmlOut.WriteStartElement("PolyLineWall");
            xmlOut.WriteAttributeString("Name", base.Name);

            string pointsValue=string.Empty;
            foreach(Point p in this.Points)
            {
                pointsValue += p.X.ToString() + ':' + p.Y.ToString() + ';';
            }
            xmlOut.WriteAttributeString("Points", pointsValue);

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

                this.Points.Clear();

                string pointsValue = xmlIn.GetAttribute("Points");
                pointsValue = pointsValue.Substring(0, pointsValue.Length - 1);
                string[] noParsePoints = pointsValue.Split(';');

                foreach(string selectS in noParsePoints)
                {
                    this.Points.Add(new Point(Convert.ToDouble(selectS.Split(':')[0]),Convert.ToDouble(selectS.Split(':')[1])));
                }

            }
            catch
            {

            }
        }
    }

}
