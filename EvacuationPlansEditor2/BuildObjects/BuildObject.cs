using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Collections;
using System.Windows.Media;
using System.Xml;

namespace EvacuationPlansEditor2
{
    public abstract class BuildObject:IEnumerable
    {
        public string Name { get; set; }
        public Shape VisualObject { get; set; }
        public double ThinknessVisualObject 
        { 
            get 
            { 
                return VisualObject.StrokeThickness; 
            } 
            set 
            { 
                VisualObject.StrokeThickness = value; 
            } 
        }

        //new
        public virtual bool IsSelect
        {
            get
            {
                return ColorVisualObject == Brushes.Black ? false : true;
            }

            set
            {
                if (value)
                {
                    ColorVisualObject = Brushes.Red;
                }
                else
                {
                    ColorVisualObject = Brushes.Black;
                }  

                VisualObject.Stroke = ColorVisualObject;
            }
        }

        public Brush ColorVisualObject 
        { 
            get 
            { 
                return VisualObject.Stroke; 
            } 
            set 
            { 
                VisualObject.Stroke = value; 
            } 
        }

        public virtual IEnumerator GetEnumerator()
        {
            yield return VisualObject;
        }

        public virtual bool IsUIElementInBuildObject(UIElement element)
        {
            return VisualObject == element;
        }

        //Методы для сохраниения и загрузки

        public abstract void SaveToFile(XmlTextWriter xmlOut);

        public abstract void LoadFromFile(XmlTextReader xmlIn);

    }
}
