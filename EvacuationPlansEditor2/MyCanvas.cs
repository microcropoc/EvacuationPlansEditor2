using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace EvacuationPlansEditor2
{
    public enum Side { Right, Left, Up, Down}

    public class MyCanvas:Canvas
    {

        public Tools SelectTool { get; set; }

        public BuildObject SelectBuildObject { get; set; }
        //переделать
        public SettingToolForVisualObject SettingTool { get; set; }
        //new --------------------------------------------------------
        public TransformGroup TransformGroupForUIElement { get; set; }

        public ScaleTransform ScaleTransformForUIElement { get; set; }

        public MyCanvas()
        {
            TransformGroupForUIElement = new TransformGroup();
            ScaleTransformForUIElement = new ScaleTransform();
            TransformGroupForUIElement.Children.Add(ScaleTransformForUIElement);
        }
        //-----------------------------------------------------------------

        #region MouseEvents

        //Нажатие
        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            Point mousePosition = e.GetPosition(this);

            //With scale
            mousePosition = new Point((mousePosition.X / ScaleTransformForUIElement.ScaleX), (mousePosition.Y / ScaleTransformForUIElement.ScaleY));

            switch(SelectTool)
            {

                #region Line

                case Tools.Line:

                    if (SelectBuildObject != null)
                        return;
                    SelectBuildObject = new LineWall("Стена")
                    {
                        ThinknessVisualObject = SettingTool.Thinckness,
                        FirstPoint = mousePosition,
                        EndPoint = mousePosition,
                    };

                    Program.CurrentProject.ListBuildObject.Add(SelectBuildObject);
                    this.AddBuildObject(SelectBuildObject);

                    break;

                #endregion

                #region PolyLine

                case Tools.PolyLine:

                     if (SelectBuildObject == null)
                     {
                         SelectBuildObject = new PolyLineWall("ЛоманаяСтена")
                         {
                             ThinknessVisualObject = SettingTool.Thinckness,
                             FirstPoint = mousePosition,
                         };

                         (SelectBuildObject as PolyLineWall).Points.Add(mousePosition);
                         Program.CurrentProject.ListBuildObject.Add(SelectBuildObject);
                         this.AddBuildObject(SelectBuildObject);
                     }
                     else
                         if (SelectBuildObject is PolyLineWall)
                         {
                             (SelectBuildObject as PolyLineWall).Points.Add(mousePosition);
                         }

                     break;

                #endregion

                #region Cricle

                case Tools.Cricle:
                    if(SelectBuildObject==null)
                    {
                        SelectBuildObject = new EllipseWall("Круговая стена")
                        {
                            ThinknessVisualObject = SettingTool.Thinckness
                        };

                        EllipseGeometry ellipseGeometry = (SelectBuildObject as EllipseWall).EllipseObject;
                        ellipseGeometry.Center = mousePosition;
                        ellipseGeometry.RadiusX = SelectBuildObject.ThinknessVisualObject;
                        ellipseGeometry.RadiusY = SelectBuildObject.ThinknessVisualObject;

                        Program.CurrentProject.ListBuildObject.Add(SelectBuildObject);
                        this.AddBuildObject(SelectBuildObject);
                        
                    }
                    break;
                #endregion

                #region Rectangle

                case Tools.Rectangle:
                    if (SelectBuildObject == null)
                    {
                        SelectBuildObject = new RectangleWall("Прямоугольная стена")
                        {
                            ThinknessVisualObject = SettingTool.Thinckness
                        };

                        RectangleGeometry rectangleGeometry = (SelectBuildObject as RectangleWall).RectangleObject;
                        rectangleGeometry.Rect = new Rect(mousePosition, mousePosition);

                        Program.CurrentProject.ListBuildObject.Add(SelectBuildObject);
                        this.AddBuildObject(SelectBuildObject);

                    }
                    break;

                #endregion

                #region ImportImage

                case Tools.ImportImg:

                    if(SelectBuildObject is RectangleWithImage)
                    {
                        RectangleWithImage rectangleWithImage = SelectBuildObject as RectangleWithImage;

                        rectangleWithImage.RectangleObject.Rect = new Rect(mousePosition, mousePosition);
                        rectangleWithImage.ImgDraw.Rect = rectangleWithImage.RectangleObject.Rect;

                        Program.CurrentProject.ListBuildObject.Add(SelectBuildObject);
                        this.AddBuildObject(SelectBuildObject);

                        Canvas.SetLeft(rectangleWithImage.ImportImage, mousePosition.X);
                        Canvas.SetTop(rectangleWithImage.ImportImage, mousePosition.Y);
                    }

                    break;

                #endregion

                #region PathEvacuacion

                case Tools.PathEvacuacion:

                    if (SelectBuildObject == null)
                     {
                         SelectBuildObject = new EvacPath("Путь эвакуации", "PointerEvacTool")
                         {
                             ThinknessVisualObject = SettingTool.Thinckness,
                             FirstPoint = mousePosition,
                         };

                         (SelectBuildObject as EvacPath).Points.Add(mousePosition);
                         Program.CurrentProject.ListBuildObject.Add(SelectBuildObject);
                         this.AddBuildObject(SelectBuildObject);
                     }
                     else
                        if (SelectBuildObject is EvacPath)
                         {
                             (SelectBuildObject as EvacPath).AddPointWithPointer(mousePosition);

                            //обновление
                             this.RemoveBuildObject(SelectBuildObject);
                             this.AddBuildObject(SelectBuildObject);
                         }

                     break;

                #endregion

                #region Exit
                //доработать
                case Tools.Exit:
                    if(SelectBuildObject==null)
                    {
                        SelectBuildObject = new RectangleWithImage("Выход","ExitManIcon");
                        RectangleWithImage rectWithImg = (SelectBuildObject as RectangleWithImage);
                        double widthRect=SettingTool.Thinckness;
                        rectWithImg.RectangleObject.Rect = new Rect(new Point(mousePosition.X - widthRect, mousePosition.Y - widthRect), new Point(mousePosition.X + widthRect, mousePosition.Y + widthRect));
                        rectWithImg.ImgDraw.Rect = rectWithImg.RectangleObject.Rect;

                        Program.CurrentProject.ListBuildObject.Add(SelectBuildObject);
                        this.AddBuildObject(SelectBuildObject);

                        Canvas.SetLeft(rectWithImg.ImportImage, (mousePosition.X - widthRect)*ScaleTransformForUIElement.ScaleX);
                        Canvas.SetTop(rectWithImg.ImportImage, (mousePosition.Y - widthRect) * ScaleTransformForUIElement.ScaleY);
                    }
                    break;

                #endregion


            }
        }

        //Движение
        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if(SelectBuildObject==null)            
                return;

            Point mousePosition = e.GetPosition(this);

            //With scale
            mousePosition = new Point((mousePosition.X / ScaleTransformForUIElement.ScaleX), (mousePosition.Y / ScaleTransformForUIElement.ScaleY));

            switch (SelectTool)
            {

                #region Line

                case Tools.Line:

                    if (SelectBuildObject is LineWall)
                    {
                        LineWall lineWall = (SelectBuildObject as LineWall);
                        lineWall.EndPoint = new Point(Math.Abs(mousePosition.X - lineWall.FirstPoint.X) > 10 ? mousePosition.X : lineWall.FirstPoint.X, Math.Abs(mousePosition.Y  - lineWall.FirstPoint.Y) > 10 ? mousePosition.Y  : lineWall.FirstPoint.Y);
                    }

                    break;

                #endregion

                #region PolyLine

                case Tools.PolyLine:

                        if (SelectBuildObject is PolyLineWall)
                        {
                           PolyLineWall polyLineWall= (SelectBuildObject as PolyLineWall);
                           polyLineWall.EndPoint = new Point(Math.Abs(mousePosition.X - polyLineWall.Points[polyLineWall.Points.Count - 2].X) > 10 ? mousePosition.X : polyLineWall.Points[polyLineWall.Points.Count - 2].X, Math.Abs(mousePosition.Y - polyLineWall.Points[polyLineWall.Points.Count - 2].Y) > 10 ? mousePosition.Y : polyLineWall.Points[polyLineWall.Points.Count - 2].Y);
                        }

                break;

                #endregion

                #region Cricle

                case Tools.Cricle:

                if (SelectBuildObject is EllipseWall)
                {
                    EllipseGeometry ellipseGeometry = (SelectBuildObject as EllipseWall).EllipseObject;
                    ellipseGeometry.RadiusX = Math.Sqrt(Math.Pow(ellipseGeometry.Center.Y - mousePosition.Y, 2) + Math.Pow(ellipseGeometry.Center.X - mousePosition.X, 2));
                    ellipseGeometry.RadiusY = ellipseGeometry.RadiusX;

                }

                    break;

                #endregion

                #region Rectangle

                case Tools.Rectangle:
                    if(SelectBuildObject is RectangleWall)
                    {
                        RectangleGeometry rectangleGeometry = (SelectBuildObject as RectangleWall).RectangleObject;
                        rectangleGeometry.Rect = new Rect(rectangleGeometry.Rect.Location, mousePosition);
                    }
                    break;

                #endregion

                #region ImportImage

                case Tools.ImportImg:

                    if (SelectBuildObject is RectangleWithImage)//костыль
                    if ((SelectBuildObject as RectangleWithImage).ImgDraw.Rect!=Rect.Empty)
                    {
                        RectangleGeometry rectangleGeometry = (SelectBuildObject as RectangleWithImage).RectangleObject;
                        ImageDrawing imgDraw = (SelectBuildObject as RectangleWithImage).ImgDraw;
                        rectangleGeometry.Rect = new Rect(rectangleGeometry.Rect.Location, mousePosition);
                        imgDraw.Rect = rectangleGeometry.Rect;
                    }

                    break;

                #endregion

                #region PathEvacuation

                case Tools.PathEvacuacion:


                    if (SelectBuildObject is EvacPath)
                    {
                        EvacPath evacPath = (SelectBuildObject as EvacPath);
                        evacPath.EndPoint = new Point(Math.Abs(mousePosition.X - evacPath.Points[evacPath.Points.Count - 2].X) > 10 ? mousePosition.X : evacPath.Points[evacPath.Points.Count - 2].X, Math.Abs(mousePosition.Y - evacPath.Points[evacPath.Points.Count - 2].Y) > 10 ? mousePosition.Y : evacPath.Points[evacPath.Points.Count - 2].Y);
                    }

                    break;

                #endregion

            }
        }

        //Отпускание
        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (SelectBuildObject == null)
                return;

        //    Point mousePosition = e.GetPosition(this);

            switch (SelectTool)
            {

                case Tools.Line:
                case Tools.Cricle:
                case Tools.Rectangle:
                case Tools.Exit:

                    if (SelectBuildObject is BuildObject)
                    {
                        SelectBuildObject = null;
                    }

                    break;

                case Tools.ImportImg:
                    if (SelectBuildObject is BuildObject)//костыль
                        if ((SelectBuildObject as RectangleWithImage).ImgDraw.Rect != Rect.Empty)
                    {
                        SelectBuildObject = null;
                    }
                    break;
            }
        }

        //Нажатие правой клавиши
        protected override void OnMouseRightButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);

            Point mousePosition = e.GetPosition(this);

            switch (SelectTool)
            {

                #region PolyLine

                case Tools.PolyLine:


                        SelectBuildObject = null;
                    

                    break;

                #endregion

                #region PathEvacuation

                case Tools.PathEvacuacion:


                    SelectBuildObject = null;

                    break;

                #endregion
            }
        }

        #endregion


        //посмотреть мб изменить
        public void AddBuildObject(BuildObject addBuildObject)
        {
            foreach (UIElement element in addBuildObject)
            {
                

                if (element.RenderTransform is TransformGroup)
                {
                    if (!(element.RenderTransform as TransformGroup).Children.Any(p => p == ScaleTransformForUIElement))
                    {
                        //костыль, для смещения изображений при увеличении или уменьшении объектов
                        if (element is Image)
                        {
                            Canvas.SetLeft(element, Canvas.GetLeft(element) * ScaleTransformForUIElement.ScaleX);
                            Canvas.SetTop(element, Canvas.GetTop(element) * ScaleTransformForUIElement.ScaleY);
                            
                        }

                        (element.RenderTransform as TransformGroup).Children.Add(ScaleTransformForUIElement);
                    }
                        
                }
                else
                    element.RenderTransform = TransformGroupForUIElement;
                Children.Add(element);
            }

        }

        public void RemoveBuildObject(BuildObject addBuildObject)
        {
            foreach (UIElement element in addBuildObject)
            {
                Children.Remove(element);
            }

        }

        //--------------------------------------------------------------------------------

        //Методы перемещения и увеличения объектов-----

        #region MoveObjectMethod

        //Увеличение/Уменьшение

        #region Zoom
        public void ZoomInc(double step)
        {
            ScaleTransformForUIElement.ScaleX += step;
            ScaleTransformForUIElement.ScaleY += step;

            foreach(UIElement element in this.Children)
            {
                //костыль, для смещения изображений при увеличении или уменьшении объектов
                if (element is Image)
                {
                    Canvas.SetLeft(element, Canvas.GetLeft(element) / (ScaleTransformForUIElement.ScaleX - step) * ScaleTransformForUIElement.ScaleX);
                    Canvas.SetTop(element, Canvas.GetTop(element) / (ScaleTransformForUIElement.ScaleY - step) * ScaleTransformForUIElement.ScaleY);

                }
            }
        }

        public void ZoomDec(double step)
        {
            ScaleTransformForUIElement.ScaleX -= step;
            ScaleTransformForUIElement.ScaleY -= step;

            foreach (UIElement element in this.Children)
            {
                //костыль, для смещения изображений при увеличении или уменьшении объектов
                if (element is Image)
                {
                    Canvas.SetLeft(element, Canvas.GetLeft(element) / (ScaleTransformForUIElement.ScaleX + step) * ScaleTransformForUIElement.ScaleX);
                    Canvas.SetTop(element, Canvas.GetTop(element) / (ScaleTransformForUIElement.ScaleY + step) * ScaleTransformForUIElement.ScaleY);

                }
            }
        }

        #endregion

        //Перемещение объектов
        public void MoveObjects(Side sideMove, double step)
        {
            foreach (UIElement element in this.Children)
            {
                if (element is Line)
                {
                    Line line = element as Line;

                    #region SelectSide

                    switch (sideMove)
                    {
                        case Side.Right:

                            line.X1 += step;
                            line.X2 += step;

                            break;

                        case Side.Left:

                            line.X1 -= step;
                            line.X2 -= step;

                            break;

                        case Side.Up:

                            line.Y1 -= step;
                            line.Y2 -= step;

                            break;

                        case Side.Down:

                            line.Y1 += step;
                            line.Y2 += step;

                            break;
                    }

                    #endregion

                }
                else
                    if (element is Polyline)
                    {
                        Polyline polyline = element as Polyline;
                        for (int i = 0; i < polyline.Points.Count; i++)
                        {

                            #region SelectSide

                            switch (sideMove)
                            {
                                case Side.Right:

                                    polyline.Points[i] = new Point(polyline.Points[i].X + step, polyline.Points[i].Y);

                                    break;

                                case Side.Left:

                                    polyline.Points[i] = new Point(polyline.Points[i].X - step, polyline.Points[i].Y);

                                    break;

                                case Side.Up:

                                    polyline.Points[i] = new Point(polyline.Points[i].X, polyline.Points[i].Y - step);

                                    break;

                                case Side.Down:

                                    polyline.Points[i] = new Point(polyline.Points[i].X, polyline.Points[i].Y + step);

                                    break;
                            }

                            #endregion

                            
                        }
                    }
                    else
                        if (element is Path)
                        {
                            Path path = element as Path;
                            if (path.Data is RectangleGeometry)
                            {
                                RectangleGeometry rg = (path.Data as RectangleGeometry);

                                #region SelectSide

                                switch (sideMove)
                                {
                                    case Side.Right:

                                        rg.Rect = new Rect(new Point(rg.Rect.Location.X + step, rg.Rect.Location.Y), new Point(rg.Rect.BottomRight.X + step, rg.Rect.BottomRight.Y));

                                        break;

                                    case Side.Left:

                                        rg.Rect = new Rect(new Point(rg.Rect.Location.X - step, rg.Rect.Location.Y), new Point(rg.Rect.BottomRight.X - step, rg.Rect.BottomRight.Y));

                                        break;

                                    case Side.Up:

                                        rg.Rect = new Rect(new Point(rg.Rect.Location.X, rg.Rect.Location.Y - step), new Point(rg.Rect.BottomRight.X, rg.Rect.BottomRight.Y - step));

                                        break;

                                    case Side.Down:

                                        rg.Rect = new Rect(new Point(rg.Rect.Location.X, rg.Rect.Location.Y + step), new Point(rg.Rect.BottomRight.X, rg.Rect.BottomRight.Y + step));

                                        break;
                                }

                                #endregion
  
                            }
                            else
                                if (path.Data is EllipseGeometry)
                                {
                                    EllipseGeometry eg = (path.Data as EllipseGeometry);

                                    #region SelectSide

                                    switch (sideMove)
                                    {
                                        case Side.Right:

                                            eg.Center = new Point((path.Data as EllipseGeometry).Center.X + step, (path.Data as EllipseGeometry).Center.Y);

                                            break;

                                        case Side.Left:

                                            eg.Center = new Point((path.Data as EllipseGeometry).Center.X - step, (path.Data as EllipseGeometry).Center.Y);

                                            break;

                                        case Side.Up:

                                            eg.Center = new Point((path.Data as EllipseGeometry).Center.X, (path.Data as EllipseGeometry).Center.Y - step);

                                            break;

                                        case Side.Down:

                                            eg.Center = new Point((path.Data as EllipseGeometry).Center.X, (path.Data as EllipseGeometry).Center.Y + step);

                                            break;
                                    }

                                    #endregion

                                    
                                }

                        }
                        else
                            if (element is Image)
                            {
                                #region SelectSide

                                switch (sideMove)
                                {
                                    case Side.Right:

                                        Canvas.SetLeft(element, Canvas.GetLeft(element) + step*ScaleTransformForUIElement.ScaleX);

                                        break;

                                    case Side.Left:

                                        Canvas.SetLeft(element, Canvas.GetLeft(element) - step * ScaleTransformForUIElement.ScaleX);

                                        break;

                                    case Side.Up:

                                        Canvas.SetTop(element, Canvas.GetTop(element) - step * ScaleTransformForUIElement.ScaleY);

                                        break;

                                    case Side.Down:

                                        Canvas.SetTop(element, Canvas.GetTop(element) + step * ScaleTransformForUIElement.ScaleY);

                                        break;
                                }

                                #endregion

                                
                            }
            }
        }

        #endregion
        //---------------------------------------------

    }
}
