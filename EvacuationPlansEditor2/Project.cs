using System;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Xml;
using System.Windows;

namespace EvacuationPlansEditor2
{
    public class Project
    {
        public string Name { get; set; }

        public ObservableCollection<BuildObject> ListBuildObject { get; set; }

        public Project(string name)
        {
            ListBuildObject = new ObservableCollection<BuildObject>();
            Name = name;
        }

        public Project(XmlTextReader xmlIn) : this("")
        {
            LoadFromFile(xmlIn);
        }

        public void SaveToFile(XmlTextWriter xmlOut)
        {
            xmlOut.Formatting = Formatting.Indented;

            //начало документа
            xmlOut.WriteStartDocument();
            xmlOut.WriteStartElement("Project");
            xmlOut.WriteAttributeString("Name", Name);

            //список строительных элементов

            foreach (BuildObject obj in ListBuildObject)
            {
                obj.SaveToFile(xmlOut);
            }

            //-----------------------------

            xmlOut.WriteEndElement();
            xmlOut.WriteEndDocument();


        }

        public void LoadFromFile(XmlTextReader xmlIn)
        {
            ListBuildObject.Clear();
            //setting xmlIn
            xmlIn.WhitespaceHandling = WhitespaceHandling.None;
            xmlIn.MoveToContent();
            if (xmlIn.Name != "Project")
                throw new ArgumentException("Incorrect File Format.");

            Name = xmlIn.GetAttribute("Name");
            

            //цикл для чтения тегов элементов проекта
            #region ReadElementTag

            do
            {
                if (!xmlIn.Read())
                    break;
                if (xmlIn.NodeType == XmlNodeType.EndElement)
                    continue;

                switch (xmlIn.Name)
                {
                    case "LineWall":
                        LineWall lineWall = new LineWall(xmlIn);
                        ListBuildObject.Add(lineWall);
                        break;

                    case "PolyLineWall":
                        PolyLineWall polyLineWall = new PolyLineWall(xmlIn);
                        ListBuildObject.Add(polyLineWall);
                        break;

                    case "EvacPath":
                        EvacPath evacPath = new EvacPath(xmlIn);
                        ListBuildObject.Add(evacPath);
                        break;

                    case "EllipseWall":
                        EllipseWall ellipseWall = new EllipseWall(xmlIn);
                        ListBuildObject.Add(ellipseWall);
                        break;

                    case "RectangleWall":
                        RectangleWall rectangleWall = new RectangleWall(xmlIn);
                        ListBuildObject.Add(rectangleWall);
                        break;

                    case "RectangleWithImage":
                        RectangleWithImage rectangleWithImage = new RectangleWithImage(xmlIn);
                        ListBuildObject.Add(rectangleWithImage);
                        break;

                }
            } while (!xmlIn.EOF);

            #endregion

            //--------------
        }

    }
}
