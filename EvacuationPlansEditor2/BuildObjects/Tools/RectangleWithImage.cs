using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace EvacuationPlansEditor2
{
    class RectangleWithImage : RectangleWall
    {
        public Image ImportImage { get; private set; }

        public ImageDrawing ImgDraw {get; private set;}

        public string ResourceName { get; set; }

        //forImport
        public RectangleWithImage(string name,ImageSource imageSourse):base(name)
        {
            ImportImage = new Image();
            ImgDraw = new ImageDrawing();
            ImgDraw.ImageSource = imageSourse;
            DrawingImage drawImage = new DrawingImage(ImgDraw);
            ImportImage.Source = drawImage;
            base.ThinknessVisualObject = 1;
        }

        //forWithResource
        public RectangleWithImage(string name, string resourceName): base(name)
        {
            ImportImage = new Image();
            ImgDraw = new ImageDrawing();
            FrameworkElement seasherResource = new FrameworkElement();
            ImgDraw.ImageSource = (seasherResource.FindResource(resourceName) as Image).Source;
            ResourceName = resourceName;
            DrawingImage drawImage = new DrawingImage(ImgDraw);
            ImportImage.Source = drawImage;
            base.ThinknessVisualObject = 1;
        }

        public RectangleWithImage(XmlTextReader xmlIn): base("")
        {
            ImportImage = new Image();
            ImgDraw = new ImageDrawing();
            LoadFromFile(xmlIn);
            DrawingImage drawImage = new DrawingImage(ImgDraw);
            ImportImage.Source = drawImage;
            base.ThinknessVisualObject = 1;
        }
        //-------------

        public override IEnumerator GetEnumerator()
        {
            yield return VisualObject;
            yield return ImportImage;
        }

        public override bool IsUIElementInBuildObject(UIElement element)
        {
            return base.IsUIElementInBuildObject(element)||ImportImage==element;
        }

        public override void SaveToFile(XmlTextWriter xmlOut)
        {
            xmlOut.WriteStartElement("RectangleWithImage");
            xmlOut.WriteAttributeString("Name", base.Name);
            //new
            xmlOut.WriteAttributeString("ResourceName",string.IsNullOrEmpty(ResourceName)?"none":ResourceName);
            //------------
            xmlOut.WriteAttributeString("Location", RectangleObject.Rect.Location.X.ToString() + ':' + RectangleObject.Rect.Location.Y.ToString());
            xmlOut.WriteAttributeString("BottomRight", RectangleObject.Rect.BottomRight.X.ToString() + ':' + RectangleObject.Rect.BottomRight.Y.ToString());
            xmlOut.WriteAttributeString("Stroke", base.ColorVisualObject.ToString());
            xmlOut.WriteAttributeString("StrokeThickness", base.ThinknessVisualObject.ToString());
            xmlOut.WriteEndElement();
        } 

        public override void LoadFromFile(XmlTextReader xmlIn)
        {
            //base.LoadFromFile(xmlIn)
            base.Name = xmlIn.GetAttribute("Name");
            base.ColorVisualObject = Brushes.Black.ToString() == xmlIn.GetAttribute("Stroke") ? Brushes.Black : Brushes.Gray;
            base.ThinknessVisualObject = Convert.ToDouble(xmlIn.GetAttribute("StrokeThickness"));
            RectangleObject.Rect = new Rect(new Point(Convert.ToDouble(xmlIn.GetAttribute("Location").Split(':')[0]), Convert.ToDouble(xmlIn.GetAttribute("Location").Split(':')[1])), new Point(Convert.ToDouble(xmlIn.GetAttribute("BottomRight").Split(':')[0]), Convert.ToDouble(xmlIn.GetAttribute("BottomRight").Split(':')[1])));
            //------------------
            //new
            FrameworkElement seasherResource = new FrameworkElement();
            ImgDraw.ImageSource = (seasherResource.FindResource(xmlIn.GetAttribute("ResourceName")) as Image).Source;
            ResourceName = xmlIn.GetAttribute("ResourceName");
            //--------------------
            ImgDraw.Rect = base.RectangleObject.Rect;
            Canvas.SetLeft(ImportImage, ImgDraw.Rect.Location.X);
            Canvas.SetTop(ImportImage, ImgDraw.Rect.Location.Y);
            
        }
    }
}
