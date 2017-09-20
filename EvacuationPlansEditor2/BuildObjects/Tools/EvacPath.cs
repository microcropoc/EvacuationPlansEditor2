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
using System.Windows.Shapes;
using System.Xml;

namespace EvacuationPlansEditor2
{
	class EvacPath:BuildObject
	{
		List<Image> listPointers;
		Image templateImg;

		public string ResourceName { get; set; }


		//пересмортеть
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
					return Points[Points.Count - 1];
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

		public EvacPath(string name,string resourceName)
		{
			base.Name = name;
			VisualObject = new Polyline() { Stroke = Brushes.Green };
			listPointers = new List<Image>();
			FrameworkElement seasherResource = new FrameworkElement();
			templateImg = seasherResource.FindResource(resourceName) as Image;
			ResourceName = resourceName;
		}

		public EvacPath(XmlTextReader xmlIn)
		{
			VisualObject = new Polyline() { Stroke = Brushes.Green };
			listPointers = new List<Image>();
			LoadFromFile(xmlIn);
		}

		//добавление нового узла
		public bool AddPointWithPointer(Point point)
		{
			if (Points.Count < 2)
				return false;

			double rotateAngle;
			Vector normalizeVector;

			//вычисление угла поворота
			#region CalculationAngleRotatePointer

			Vector firstVector = new Vector(Points[Points.Count - 2].X, Points[Points.Count - 2].Y);
			Vector endVector = new Vector(Points[Points.Count - 1].X, Points[Points.Count - 1].Y);
			normalizeVector = endVector - firstVector;
			normalizeVector.Normalize();
			rotateAngle = Math.Atan2(normalizeVector.Y, normalizeVector.X) * (180 / Math.PI);

			#endregion

			//создание стрелки по средством клонирования шаблонного изображения
			Image cloneTemplate = new Image();
			cloneTemplate.Source = templateImg.Source.Clone();

			//определение параметров
			cloneTemplate.Height = this.ThinknessVisualObject * 4;
			cloneTemplate.Width = this.ThinknessVisualObject * 4;

			TransformGroup tg = new TransformGroup();
			tg.Children.Add(new RotateTransform(rotateAngle + 90, cloneTemplate.Width / 2, cloneTemplate.Height / 2));

			cloneTemplate.RenderTransform = tg;

			Canvas.SetLeft(cloneTemplate, EndPoint.X - cloneTemplate.Width / 2);
			Canvas.SetTop(cloneTemplate, EndPoint.Y - cloneTemplate.Height / 2);

			listPointers.Add(cloneTemplate);

			Points.Add(point);

			return true;
		}

		public override IEnumerator GetEnumerator()
		{
			yield return VisualObject;
			foreach(UIElement element in listPointers)
			{
				yield return element;
			}
		}

		public override bool IsUIElementInBuildObject(UIElement element)
		{
			return base.IsUIElementInBuildObject(element)||listPointers.Any(p=>p==element);
		}

		public override bool IsSelect
		{
			get
			{
				return ColorVisualObject == Brushes.Green ? false : true;
			}

			set
			{
				if (value)
				{
					ColorVisualObject = Brushes.Red;
					VisualObject.Stroke = ColorVisualObject;
					//newTestColorImage
					foreach(Image img in listPointers)
					{
						((((img.Source as DrawingImage).Drawing as ImageDrawing).ImageSource as DrawingImage).Drawing as GeometryDrawing).Brush=Brushes.Red;
					}
					//-------
				}
				else
				{
					ColorVisualObject = Brushes.Green;
					VisualObject.Stroke = ColorVisualObject;
					//newTestColorImage
					foreach (Image img in listPointers)
					{
						((((img.Source as DrawingImage).Drawing as ImageDrawing).ImageSource as DrawingImage).Drawing as GeometryDrawing).Brush = Brushes.Green;
					}
					//-------
				}
			}
		}

		public override void SaveToFile(XmlTextWriter xmlOut)
		{
			xmlOut.WriteStartElement("EvacPath");
			xmlOut.WriteAttributeString("Name", base.Name);
			//new
			xmlOut.WriteAttributeString("ResourceName", string.IsNullOrEmpty(ResourceName) ? "none" : ResourceName);
			//------------

			string pointsValue = string.Empty;
			foreach (Point p in this.Points)
			{
				pointsValue += p.X.ToString() + ':' + p.Y.ToString() + ';';
			}
			xmlOut.WriteAttributeString("Points", pointsValue);

			xmlOut.WriteAttributeString("Stroke", base.ColorVisualObject.ToString());
			xmlOut.WriteAttributeString("StrokeThickness", base.ThinknessVisualObject.ToString());
			xmlOut.WriteEndElement();
		}

		//работает, упростить по возможности!
		public override void LoadFromFile(XmlTextReader xmlIn)
		{
			try
			{
				base.Name = xmlIn.GetAttribute("Name");
				base.ColorVisualObject = Brushes.Black.ToString() == xmlIn.GetAttribute("Stroke") ? Brushes.Black : Brushes.Green;
				base.ThinknessVisualObject = Convert.ToDouble(xmlIn.GetAttribute("StrokeThickness"));

				//new
				FrameworkElement seasherResource = new FrameworkElement();
				templateImg = seasherResource.FindResource(xmlIn.GetAttribute("ResourceName")) as Image;
				ResourceName = xmlIn.GetAttribute("ResourceName");
				//--------------------

				this.Points.Clear();

				string pointsValue = xmlIn.GetAttribute("Points");
				pointsValue = pointsValue.Substring(0, pointsValue.Length - 1);
				string[] noParsePoints = pointsValue.Split(';');
				//отделение первой и последней точки
				Point firstPoint = new Point(Convert.ToDouble(noParsePoints[0].Split(':')[0]), Convert.ToDouble(noParsePoints[0].Split(':')[1]));
				Point twoPoint = new Point(Convert.ToDouble(noParsePoints[1].Split(':')[0]), Convert.ToDouble(noParsePoints[1].Split(':')[1]));
				

				List<string> noParseAddPointWith =new List<string>();
				for (int i = 2; i < noParsePoints.Length ; i++)
					noParseAddPointWith.Add(noParsePoints[i]);

				//----------------------------------------------------------------

				//добавление начальной точки
				this.FirstPoint = firstPoint;
				this.Points.Add(twoPoint);

				//добавление узлов со стрелкой
				foreach (string selectS in noParseAddPointWith)
				{
					this.AddPointWithPointer(new Point(Convert.ToDouble(selectS.Split(':')[0]), Convert.ToDouble(selectS.Split(':')[1])));
				}				

			}
			catch
			{

			}
		}
	}
}
