using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows;

namespace EvacuationPlansEditor2
{
    public enum Tools { None, SelectObjects, MoveObjects, Line, PolyLine, Cricle, Rectangle, PathEvacuacion, ImportImg, Exit }
    public static class Program
    {
        public static string PathFolder { get; set; }
        public static string PathProject { get; set; }

        public static Project CurrentProject { get; set; }

        static Program()
        {
            PathFolder = AppDomain.CurrentDomain.BaseDirectory;
        }

        //реализовано
        public static bool NewProject(string Name)
        {
                Microsoft.Win32.SaveFileDialog createDlg = new Microsoft.Win32.SaveFileDialog();

                #region SettingSaveFileDialog

                createDlg.InitialDirectory = PathFolder;
                createDlg.FileName = Name;
                createDlg.Title = "Создание нового проекта";
                createDlg.DefaultExt = ".epp";
                createDlg.Filter = "Evacuation Plan Project (.epp)|*.epp";

                #endregion
               
                if ((bool)createDlg.ShowDialog())
                {

                    //создание файла на диске
                    #region CreateFileOnDisk
                    try
                    {
                        using (FileStream fs = new FileStream(createDlg.FileName, FileMode.Create))
                        {
                            using (XmlTextWriter xmlOut = new XmlTextWriter(fs, Encoding.Unicode))
                            {
                                Project newProject = new Project(Name);
                                newProject.ListBuildObject.Add(new LineWall("Стена", new Point(10, 10), new Point(100, 100)));
                                newProject.SaveToFile(xmlOut);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        WindowMsgError errorFileNotExist = new WindowMsgError("Ошибка", "Тест не создан");
                        errorFileNotExist.ShowDialog();
                        return false;
                    }

                    #endregion

                    return OpenProject(createDlg.FileName);

                }

            return false;
        }
        //реализовано
        public static bool OpenProject(string path="")
        {
            if (path == "")
            {
                Microsoft.Win32.OpenFileDialog openDlg = new Microsoft.Win32.OpenFileDialog();

                #region SettingOpenFileDialog

                openDlg.InitialDirectory = PathFolder;
                openDlg.Title = "Загрузка файла";
                openDlg.FileName = "";
                openDlg.DefaultExt = ".epp";
                openDlg.Filter = "Evacuation Plan Project (.epp)|*.epp";

                #endregion

                if ((bool)openDlg.ShowDialog()!=true)
                {
                    return false;
                }
                PathProject = openDlg.FileName;
                path = PathProject;
            }

            if(File.Exists(path))
            {
                PathProject = path;

                using(FileStream fs=new FileStream(path,FileMode.Open))
                {
                    using(XmlTextReader xmlIn=new XmlTextReader(fs))
                    {
                        try
                        {
                            CurrentProject = new Project(xmlIn);
                        }
                        catch
                        {
                            WindowMsgError windowError = new WindowMsgError("Ошибка при открытии проекта","Неверный формат файла");
                            windowError.ShowDialog();
                            return false;
                        }
                    }
                }

                return true;
            }
            else
            {
                WindowMsgError errorFileNotFound = new WindowMsgError("Ошибка при открытии проекта", "Неверный путь к тесту");
                errorFileNotFound.ShowDialog();
                return false;
            }
        }
        //реализовано
        public static bool SaveOrSaveAsProject(string path = "")
        {
            //false если save
            bool saveOrSaveAs=false;

            if (path == "")
            {
                if (CurrentProject != null)
                {
                    Microsoft.Win32.SaveFileDialog saveDlg = new Microsoft.Win32.SaveFileDialog();

                    #region SettingSaveFileDialog

                    saveDlg.InitialDirectory = PathFolder;
                    saveDlg.FileName = CurrentProject.Name;
                    saveDlg.Title = "Создание нового проекта";
                    saveDlg.DefaultExt = ".epp";
                    saveDlg.Filter = "Evacuation Plan Project (.epp)|*.epp";

                    #endregion

                    if ((bool)saveDlg.ShowDialog() != true)
                    {
             
                        return false;
                    }

                    PathProject = saveDlg.FileName;
                    path = PathProject;
                    saveOrSaveAs = true;
                }
            }

            if(saveOrSaveAs || File.Exists(path))
            {
                PathProject = path;

                //сохранение проекта на диске
                #region CreateProjectOnDisk
                try
                {
                    using (FileStream fs = new FileStream(path, FileMode.Create))
                    {
                        using (XmlTextWriter xmlOut = new XmlTextWriter(fs, Encoding.Unicode))
                        {
                            CurrentProject.SaveToFile(xmlOut);
                        }
                    }
                }
                catch (Exception)
                {
                    WindowMsgError errorFileNotExist = new WindowMsgError("Ошибка", "Проект не создан");
                    errorFileNotExist.ShowDialog();
                    return false;
                }

                #endregion

                return true;
            }

            return false;
        }
    }
}
