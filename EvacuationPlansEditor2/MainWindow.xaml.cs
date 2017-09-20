using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace EvacuationPlansEditor2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SettingToolForVisualObject currentSettingTool;
        public MainWindow()
        {
            InitializeComponent();
            gridWorkArea.IsEnabled = false;

            menuSave.IsEnabled = false;
            menuSaveAs.IsEnabled = false;
        }

        bool ViewOpenCurrentProjectInWindow()
        {
            if (Program.CurrentProject == null)
                return false;

            myCanvasWork.Children.Clear();
          //  myCanvasWork.ScaleTransformForUIElement = new ScaleTransform();

            gridListAndTreeObjects.DataContext = Program.CurrentProject.ListBuildObject;

            //заполнение полотна строительными объектами

            foreach (BuildObject obj in Program.CurrentProject.ListBuildObject)
            {
                myCanvasWork.AddBuildObject(obj);
            }


            #region BindingProjectNameWithTitle

            Binding titleBinding = new Binding();
            titleBinding.Source = Program.CurrentProject;
            titleBinding.Path = new PropertyPath("Name");
            SetBinding(Window.TitleProperty, titleBinding);

            #endregion

            gridWorkArea.IsEnabled = true;

            //Выбор инструмента для выделения объектов (SelectObject)(Будем использовать этот инструмент по умолчанию)
            btnSelectObject_Click(btnSelectObject, new RoutedEventArgs());

            menuSave.IsEnabled = true;
            menuSaveAs.IsEnabled = true;

            return true;
        }

        #region Events

        #region ButtonToolsEvents

        //Navigation
        private void btnSelectObject_Click(object sender, RoutedEventArgs e)
        {
            myCanvasWork.SelectTool = Tools.SelectObjects;
            //adding settingFrame
            currentSettingTool = null;
            gridSettingSelectTool.Children.Clear();
        }

        private void btnMoveObjects_Click(object sender, RoutedEventArgs e)
        {
            myCanvasWork.SelectTool = Tools.MoveObjects;
            //adding settingFrame
            currentSettingTool = null;
            gridSettingSelectTool.Children.Clear();
        }


        //Tools
        private void btnLine_Click(object sender, RoutedEventArgs e)
        {
            myCanvasWork.SelectTool = Tools.Line;
            //adding settingFrame
            gridSettingSelectTool.Children.Clear();
            currentSettingTool = new SettingToolForVisualObject("Стена");
            gridSettingSelectTool.Children.Add(currentSettingTool);
            myCanvasWork.SettingTool = currentSettingTool;
        }

        private void btnPolyLine_Click(object sender, RoutedEventArgs e)
        {
            myCanvasWork.SelectTool = Tools.PolyLine;
            //adding settingFrame
            gridSettingSelectTool.Children.Clear();
            currentSettingTool = new SettingToolForVisualObject("Ломаная Стена");
            gridSettingSelectTool.Children.Add(currentSettingTool);
            myCanvasWork.SettingTool = currentSettingTool;
        }

        private void btnCricle_Click(object sender, RoutedEventArgs e)
        {
            myCanvasWork.SelectTool = Tools.Cricle;
            //adding settingFrame
            gridSettingSelectTool.Children.Clear();
            currentSettingTool = new SettingToolForVisualObject("Круговая Стена");
            gridSettingSelectTool.Children.Add(currentSettingTool);
            myCanvasWork.SettingTool = currentSettingTool;
        }

        private void btnRectangle_Click(object sender, RoutedEventArgs e)
        {
            myCanvasWork.SelectTool = Tools.Rectangle;
            //adding settingFrame
            gridSettingSelectTool.Children.Clear();
            currentSettingTool = new SettingToolForVisualObject("Прямоугольная Стена");
            gridSettingSelectTool.Children.Add(currentSettingTool);
            myCanvasWork.SettingTool = currentSettingTool;
        }

        private void btnPathEvacuacion_Click(object sender, RoutedEventArgs e)
        {
            myCanvasWork.SelectTool = Tools.PathEvacuacion;
            //adding settingFrame
            gridSettingSelectTool.Children.Clear();
            currentSettingTool = new SettingToolForVisualObject("Путь эвакуации");
            gridSettingSelectTool.Children.Add(currentSettingTool);
            myCanvasWork.SettingTool = currentSettingTool;
        }

        private void btnExitMan_Click(object sender, RoutedEventArgs e)
        {
            myCanvasWork.SelectTool = Tools.Exit;
            //adding settingFrame
            gridSettingSelectTool.Children.Clear();
            currentSettingTool = new SettingToolForVisualObject("Знак выхода");
            currentSettingTool.groupBoxThinckness.Header = "Ширина";
            gridSettingSelectTool.Children.Add(currentSettingTool);
            myCanvasWork.SettingTool = currentSettingTool;

        }


        #endregion

        #region MenuEvents

        //-----------------------------------------------------------------

        private void menuNew_Click(object sender, RoutedEventArgs e)
        {
            //тест
            WindowCreateProject createProjectWinsow = new WindowCreateProject();

            if (createProjectWinsow.ShowDialog() == true)
            {
                if (!ViewOpenCurrentProjectInWindow())
                {
                    WindowMsgError windowError = new WindowMsgError("Ошибка", "Ошибка создания проекта");
                }
            }
        }

        private void menuOpen_Click(object sender, RoutedEventArgs e)
        {
            if (Program.OpenProject())
            {
                if (!ViewOpenCurrentProjectInWindow())
                {
                    WindowMsgError windowError = new WindowMsgError("Ошибка", "Ошибка открытия проекта");
                }
            }
        }

        private void menuSave_Click(object sender, RoutedEventArgs e)
        {
            Program.SaveOrSaveAsProject(Program.PathProject);
        }

        private void menuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            Program.SaveOrSaveAsProject();
        }

        //=================================================================

        private void menuImport_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openDialog = new Microsoft.Win32.OpenFileDialog();
            openDialog.DefaultExt = ".png";
            openDialog.Filter = "Image documents (.png)|*.png";

            if (openDialog.ShowDialog() == true)
            {
                string fileName = openDialog.FileName;
                myCanvasWork.SelectBuildObject = new RectangleWithImage("Прямоугольная область с изображением", new BitmapImage(new Uri(@fileName, UriKind.Absolute)));
                myCanvasWork.SelectTool = Tools.ImportImg;
            }
        }

        private void menuExport_Click(object sender, RoutedEventArgs e)
        {

            //из интернета

            #region InternetVersion


            Rect bounds = VisualTreeHelper.GetDescendantBounds(myCanvasWork);
            double dpi = 96d*3;


            RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width*3, (int)bounds.Height*3, dpi, dpi, System.Windows.Media.PixelFormats.Default);


            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(myCanvasWork);
                dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            }

            rtb.Render(dv);
            //cохранение в файл
            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(rtb));

            #region SaveDialog

            //настройка и вызов SaveDialog
            Microsoft.Win32.SaveFileDialog saveDlg = new Microsoft.Win32.SaveFileDialog();
            saveDlg.InitialDirectory = Program.PathFolder;
            saveDlg.FileName = Program.CurrentProject.Name + "ExportImage";
            saveDlg.Title = "Создание нового проекта";
            saveDlg.DefaultExt = ".png";
            saveDlg.Filter = "Portable Network Graphics (.png)|*.png";

            if ((bool)saveDlg.ShowDialog() == true)
            {

                using (Stream stm = File.Create(saveDlg.FileName))
                {
                    png.Save(stm);
                }

            }
            else
            {
                WindowMsgError errorWindow = new WindowMsgError("Ошибка экспорта", "Изображение не сохранено");
            }

            #endregion





            #endregion

        }

        //=================================================================

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            //пока
            Close();
        }

        //-----------------------------------------------------------------

        #endregion

        #region OtherEvents

        private void listViewBuildObjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listViewBuildObjects.SelectedItems.Count == 0)
                return;
            //убирает выделение с ранее выделенных элементов
            foreach (BuildObject selectItem in listViewBuildObjects.Items)
            {
                if (selectItem.IsSelect)
                {
                    selectItem.IsSelect = false;
                }
            }
            //если выбрано более одного элемента, выделить на полотне красным цветом, иначе (если выбран один)
            if (listViewBuildObjects.SelectedItems.Count > 1)
            {
                foreach (BuildObject selectItem in listViewBuildObjects.SelectedItems)
                {

                    selectItem.IsSelect = true;

                }
            }
            else
            {
                (listViewBuildObjects.SelectedItem as BuildObject).IsSelect = true;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (listViewBuildObjects.SelectedItems.Count == 0)
                    return;

                myCanvasWork.RemoveBuildObject(listViewBuildObjects.SelectedItem as BuildObject);
                Program.CurrentProject.ListBuildObject.Remove((listViewBuildObjects.SelectedItem as BuildObject));
            }

            #region MoveObjects

            
                switch (e.Key)
                {
                    case Key.Q:

                        #region Zoom+

                        myCanvasWork.ZoomInc(0.1);

                        #endregion

                        break;

                    case Key.E:

                        #region Zoom-

                        myCanvasWork.ZoomDec(0.1);

                        #endregion

                        break;

                    case Key.D:

                        #region LeftMove

                        myCanvasWork.MoveObjects(Side.Left, 2);

                        #endregion

                        break;

                    case Key.A:

                        #region RightMove

                        myCanvasWork.MoveObjects(Side.Right, 2);

                        #endregion

                        break;

                    case Key.S:

                        #region UpMove

                        myCanvasWork.MoveObjects(Side.Up, 2);

                        #endregion

                        break;

                    case Key.W:

                        #region DownMove

                        myCanvasWork.MoveObjects(Side.Down, 2);

                        #endregion

                        break;
                }

            #endregion
            //--------------------------------------
        }

        private void myCanvasWork_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            //выделение нажатого элемента
            #region SelectElement
            if (listViewBuildObjects.SelectedItems.Count > 0)
            {
                foreach (BuildObject selectItem in listViewBuildObjects.Items)
                {
                    if (selectItem.IsSelect)
                    {
                        selectItem.IsSelect = false;
                    }
                }

                listViewBuildObjects.SelectedItems.Clear();
            }

            MyCanvas currentCanvas = sender as MyCanvas;
            Point mousePosition = e.GetPosition(currentCanvas);
            if (currentCanvas.SelectTool == Tools.SelectObjects)
            {

                HitTestResult result = VisualTreeHelper.HitTest(currentCanvas, mousePosition);
                if (result.VisualHit is UIElement)
                {
                    BuildObject clickObject = Program.CurrentProject.ListBuildObject.FirstOrDefault(p => p.IsUIElementInBuildObject(result.VisualHit as UIElement));
                    if (clickObject != null)
                    {
                        clickObject.IsSelect = true;
                        listViewBuildObjects.SelectedItems.Add(clickObject);
                    }

                }
            }
            #endregion
        }

        #endregion

        //ПЕРЕПИСАТЬ!!!!!!!!!!
        #region TestContextMenuForListView

        private void listViewContectMenu_HideOrShow_Click(object sender, RoutedEventArgs e)
        {
            string str = (sender as MenuItem).Header.ToString();
            if (str == "Скрыть")
            {
                foreach(UIElement element in (listViewBuildObjects.SelectedItem as BuildObject))
                {
                    element.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                foreach (UIElement element in (listViewBuildObjects.SelectedItem as BuildObject))
                {
                    element.Visibility = Visibility.Visible;
                }
            }
        }

        private void listViewBuildObjects_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            


        }

        private void listViewBuildObjects_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            //переписать!!!
            if (listViewBuildObjects.SelectedItems.Count == 0)
                listViewBuildObjects.ContextMenu.Visibility = Visibility.Hidden;
            //-------------

            if (listViewBuildObjects.SelectedItem is BuildObject)
            {
                foreach (UIElement element in myCanvasWork.Children)
                {
                    
                    if ((listViewBuildObjects.SelectedItem as BuildObject).IsUIElementInBuildObject(element))
                    {
                        if (element.IsVisible)
                        {
                            listViewContectMenu_HideOrShow.Header = "Скрыть";
                        }
                        else
                        {
                            listViewContectMenu_HideOrShow.Header = "Показать";
                        }
                    }
                }   
            }
        }

        #endregion


        #endregion





    }
}
