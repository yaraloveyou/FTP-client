using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using System.Net;
using System.Diagnostics;

namespace Ftp_client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    ///

    public partial class MainWindow : Window
    {
        private List<TaskManager> taskManager = new List<TaskManager>();
        private List<Transfers> transfersFailed = new List<Transfers>();
        private Connection connection = new Connection();
        private FtpWebRequest ftpWebRequest;
        private FtpWebResponse ftpWebResponse;
        private DirectoryInfo drives;
        private bool transition = false;
        private string pathOnPC = "C:/";
        private string lastName = null;

        /// <summary>
        /// Объект класса MainWindow
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            openFolderPC();
            listViewOnServer.IsEnabled = false;
        }

        /// <summary>
        /// Выгрузка папок с FTP-сервера, а так же переход по подкатолагам
        /// </summary>
        /// <returns></returns>
        private bool openFolderServer()
        {
            try
            {
                ftpWebRequest = (FtpWebRequest)WebRequest.Create(connection.FullPath);
                ftpWebRequest.Credentials = new NetworkCredential(connection.Login, connection.Password);
                ftpWebRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                listViewOnServer.Items.Clear();
                ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                Stream stream = ftpWebResponse.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                treeFiles(reader.ReadToEnd());
                ftpWebResponse.Close();
                stream.Close();
                txtServer.Text = connection.FullPath;
                transition = true;
                return true;
            }
            catch (Exception ex)
            {
                status.Text += $"Ошибка:\t{ex.Message}\n";
                return false;
            }
        }

        /// <summary>
        /// Выгрузка папок с локального сервера(компьютера пользователя). а так же переход по ним
        /// Подсчет: количества занимаемого места (при наличии файлов); каталогов (при наличии); файлов (приналичии)
        /// </summary>
        private void openFolderPC()
        {
            int countFolder = 0;
            int countFiles = 0;
            long countBytes = 0;
            listViewOnPC.Items.Clear();
            txtPC.Text = pathOnPC;
            try
            {
                drives = new DirectoryInfo(pathOnPC);
                listViewOnPC.Items.Add(new PCFolder() { Name = "..", ImageSource = @"/images/folder.png" });

                foreach (var item in drives.GetDirectories())
                {
                    countFolder++;
                    listViewOnPC.Items.Add(new PCFolder() { Name = item.ToString(), ImageSource = @"/images/folder.png", Type = "Папка с файлами", LastChange = item.LastWriteTime.ToString() });
                }

                foreach (var item in drives.GetFiles())
                {
                    countFiles++;
                    countBytes += item.Length;
                    listViewOnPC.Items.Add(new PCFolder() { Name = item.ToString(), ImageSource = @"/images/file.png", Type = item.Extension, LastChange = item.LastWriteTime.ToString(), Size = item.Length.ToString() });
                }
                transition = true;
            }
            catch (Exception ex) 
            {
                status.Text +=  $"Ошибка:\t{ex.Message}\n";
            }
            string resultFolder = null;
            string resultFiles = null;
            if(countFiles > 0)
            {
                char lastNumber = countFiles.ToString()[countFiles.ToString().Length - 1];
                if(lastNumber == '1')
                {
                    resultFiles = statusOnPC.Text = $"{countFiles} файл. Общий размер: {countBytes} байт";
                }
                else if(lastNumber == '2' || lastNumber == '3' || lastNumber == '4')
                {
                    resultFiles = statusOnPC.Text = $"{countFiles} файла. Общий размер: {countBytes} байт";
                }
                else
                {
                    resultFiles = statusOnPC.Text = $"{countFiles} файлов. Общий размер: {countBytes} байт";
                }
            }
            if (countFolder > 0)
            {
                char lastNumber = countFolder.ToString()[countFolder.ToString().Length - 1];
                if (lastNumber == '1')
                {
                    resultFolder = statusOnPC.Text = $"{countFolder} каталог";
                }
                else if (lastNumber == '2' || lastNumber == '3' || lastNumber == '4')
                {
                    resultFolder = statusOnPC.Text = $"{countFolder} каталога";
                }
                else
                {
                    resultFolder = statusOnPC.Text = $"{countFolder} каталогов";
                }
            }
            if(countFolder > 0 && countFiles > 0)
            {
                statusOnPC.Text = resultFiles.Insert(resultFiles.IndexOf('.'), " и " + resultFolder);
            }
            else if (countFolder == 0 && countFiles == 0)
            {
                statusOnPC.Text = "Пустой каталог";
            }
        }

        /// <summary>
        /// Подключение к FTP-серверу при помощи IP сервера, открытого порта для FTP протокола,
        /// логина и пароля при необходимости
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connect_Click(object sender, RoutedEventArgs e)
        {
            if (host.Text.Length != 0 && port.Text.Length != 0)
            {
                connection.Host = host.Text.Trim();
                connection.Port = port.Text.Trim();
                connection.Login = login.Text;
                connection.Password = password.Password;
                connection.FullPath = $"ftp://{connection.Host}:{connection.Port}/";
                //allPath.Add(connection.FullPath);
                if (openFolderServer())
                {
                    status.Text = "Статус:\tУспешное подключение\n";
                    listViewOnServer.IsEnabled = true;
                }
                else
                {
                    statusOnServer.Text = "Нет соединения";
                }
            }
        }

        /// <summary>
        /// Определение файлов(папка, файл) и их отображение
        /// </summary>
        /// <param name="files"></param>
        private void treeFiles(string files)
        {
            ServerFolder ServerFolder = new ServerFolder();
            ListViewItem item = new ListViewItem();
            item.DataContext = ServerFolder;
            listViewOnServer.Items.Add(new ServerFolder() { Name = "..", ImageSource = @"/images/folder.png"});
            string[] fileName = files.Split(new char[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);
            string[] pars;
            string imageSource = null;
            string size = null;
            int countFiles = 0;
            int countFolder = 0;
            int countBytes = 0;
            for (int i = 0; i < fileName.Length; i++)
            {
                pars = parsingFiles(fileName[i]);
                if (pars[0][0] == 'd')
                {
                    imageSource = @"/images/folder.png";
                    countFolder++;
                    size = null;
                }
                else if (pars[0][0] == '-')
                {
                    imageSource = @"/images/file.png";
                    size = pars[2];
                    countBytes += Convert.ToInt32(pars[2]);
                    countFiles++;
                }
                listViewOnServer.Items.Add(new ServerFolder() { Name = pars[1], ImageSource = imageSource, Role = pars[0], Size = size });
            }
            listViewOnServer.Items.Refresh();
            string resultFolder = null;
            string resultFiles = null;
            if (countFiles > 0)
            {
                char lastNumber = countFiles.ToString()[countFiles.ToString().Length - 1];
                if (lastNumber == '1')
                {
                    resultFiles = statusOnServer.Text = $"{countFiles} файл. Общий размер: {countBytes} байт";
                }
                else if (lastNumber == '2' || lastNumber == '3' || lastNumber == '4')
                {
                    resultFiles = statusOnServer.Text = $"{countFiles} файла. Общий размер: {countBytes} байт";
                }
                else
                {
                    resultFiles = statusOnServer.Text = $"{countFiles} файлов. Общий размер: {countBytes} байт";
                }
            }
            if (countFolder > 0)
            {
                char lastNumber = countFolder.ToString()[countFolder.ToString().Length - 1];
                if (lastNumber == '1')
                {
                    resultFolder = statusOnServer.Text = $"{countFolder} каталог";
                }
                else if (lastNumber == '2' || lastNumber == '3' || lastNumber == '4')
                {
                    resultFolder = statusOnServer.Text = $"{countFolder} каталога";
                }
                else
                {
                    resultFolder = statusOnServer.Text = $"{countFolder} каталогов";
                }
            }
            if (countFolder > 0 && countFiles > 0)
            {
                statusOnServer.Text = resultFiles.Insert(resultFiles.IndexOf('.'), " и " + resultFolder);
            }
            else if (countFolder == 0 && countFiles == 0)
            {
                statusOnServer.Text = "Пустой каталог";
            }
        }

        /// <summary>
        /// Парсинг файлов из FTP-сервера для определения прав и названий
        /// </summary>
        /// <param name="array"> строка с файлом с их размером и правами </param>
        /// <returns></returns>
        private string[] parsingFiles(string array)
        {
            string[] pars = new string[3];
            pars[0] = array.Substring(0, 10);
            string[] split = array.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for(int i = 8; i++ < split.Length;)
            {
                if (split.Length >= i + 1)
                {
                    if (split[i] != " ")
                    {
                        split[8] += ' ' + split[i];
                    }
                }
                else
                {
                    break;
                }
            }
            pars[1] = split[8].Remove(split[8].Length - 1, 1);
            pars[2] = split[4];
            return pars;
        }

        /// <summary>
        /// Установка директории на FPT-сервер
        /// </summary>
        /// <param name="pathFolder"> путь до закачиваемой директории </param>
        /// <param name="uploadPath"> путь для заказчки директории </param>
        /// <param name="item"> необязательный параметр с конструктором директорий и файлов на клиенте </param>
        public void uploadFolder(string pathFolder, string uploadPath, PCFolder item = null)
        {
            try
            {
                ftpWebRequest = (FtpWebRequest)WebRequest.Create(uploadPath);
                ftpWebRequest.Credentials = new NetworkCredential(connection.Login, connection.Password);
                string[] files = Directory.GetFiles(pathFolder, "*.*");
                string[] subDirs = Directory.GetDirectories(pathFolder);
                ftpWebRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();

                foreach (var itemDirs in subDirs)
                {
                    uploadPath = $"{uploadPath}{itemDirs.Remove(0, itemDirs.LastIndexOf('\\')).Replace("\\", "//")}";
                    pathFolder = itemDirs.Replace('\\', '/');
                    uploadFolder(pathFolder, uploadPath);
                }

                foreach (var itemFiles in files)
                {
                    downloadFileOnServer(itemFiles.Replace('\\', '/').Replace(txtPC.Text, ""));
                }
                if (item != null)
                {
                    listTransferSuccess.Items.Add(new Transfers() { ImageSource = item.ImageSource, PathOnPC = $"{pathFolder}/{item.Name}", SideTransmission = "-->", PathOnServer = uploadPath, Size = item.Size, Time = DateTime.Now });
                }
            }
            catch (Exception ex)
            {
                status.Text += $"Ошибка:\t{ex.Message}\n";
                if (item != null)
                {
                    listTransferFailed.Items.Add(new Transfers() { ImageSource = item.ImageSource, PathOnPC = $"{pathFolder}/{item.Name}", SideTransmission = "-->", PathOnServer = uploadPath, Size = item.Size, Time = DateTime.Now });
                    transfersFailed.Add(new Transfers() { ImageSource = item.ImageSource, PathOnPC = $"{pathFolder}/{item.Name}", SideTransmission = "-->", PathOnServer = uploadPath, Size = item.Size, Time = DateTime.Now });
                }
            }
        }

        /// <summary>
        /// установка директории на клиент
        /// </summary>
        /// <param name="url"> путь до устанавливаемой директории </param>
        /// <param name="localPath"> путь установки дериктории </param>
        /// <param name="item"> необязательный параметр с конструктором передачи файлов в задании </param>
        /// <param name="itemServer"> необязательный параметр с конструктором директорий и файлов на сервере </param>
        private void downloadFolder(string url, string localPath, TaskManager item = null, ServerFolder itemServer = null)
        {
            try
            {
                ftpWebRequest = (FtpWebRequest)WebRequest.Create(url);
                ftpWebRequest.Credentials = new NetworkCredential(connection.Login, connection.Password);
                ftpWebRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                List<string> lines = new List<string>();
                ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                Stream responseStream = ftpWebResponse.GetResponseStream();
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    while (!reader.EndOfStream)
                    {
                        lines.Add(reader.ReadLine());
                    }
                }

                foreach (var itemLines in lines)
                {
                    string[] tokens = itemLines.Split(new char[] { ' ' }, 9, StringSplitOptions.RemoveEmptyEntries);
                    string name = tokens[8];
                    string persmissions = tokens[0];
                    string localFilePath = System.IO.Path.Combine(localPath, name);
                    string fileUrl = url + name;
                    if (persmissions[0] == 'd')
                    {
                        if (!Directory.Exists(localFilePath))
                        {
                            Directory.CreateDirectory(localFilePath);
                        }
                        downloadFolder(fileUrl + "/", localFilePath, item);
                    }
                    else
                    {
                        ftpWebRequest = (FtpWebRequest)WebRequest.Create(fileUrl);
                        ftpWebRequest.Credentials = new NetworkCredential(connection.Login, connection.Password);
                        ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;

                        FileStream downloadFiles = new FileStream(localFilePath, FileMode.Create, FileAccess.ReadWrite);

                        ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                        responseStream = ftpWebResponse.GetResponseStream();

                        int size = 0;
                        byte[] file = new byte[10240];

                        while ((size = responseStream.Read(file, 0, 1024)) > 0)
                        {
                            downloadFiles.Write(file, 0, size);
                        }
                    }
                }
                if(itemServer != null)
                {
                    listTransferSuccess.Items.Add(new Transfers() { ImageSource = itemServer.ImageSource, PathOnPC = localPath, SideTransmission = "<--", PathOnServer = url, Size = itemServer.Size, Time = DateTime.Now });
                }
            }
            catch (Exception ex)
            {
                status.Text += $"Ошибка:\t{ex.Message}\n";
                if(item != null)
                {
                    listTransferFailed.Items.Add(new Transfers() { ImageSource = item.ImageSource, PathOnPC = item.PathOnPC, SideTransmission = "<--", PathOnServer = item.PathOnServer, Size = item.Size, Time = DateTime.Now });
                    transfersFailed.Add(new Transfers() { ImageSource = item.ImageSource, PathOnPC = item.PathOnPC, SideTransmission = "<--", PathOnServer = item.PathOnServer, Size = item.Size, Time = DateTime.Now });
                }
                else if(itemServer != null)
                {
                    listTransferFailed.Items.Add(new Transfers() { ImageSource = itemServer.ImageSource, PathOnPC = $"{localPath}/{itemServer.Name}", SideTransmission = "<--", PathOnServer = connection.FullPath, Size = itemServer.Size, Time = DateTime.Now });
                    transfersFailed.Add(new Transfers() { ImageSource = itemServer.ImageSource, PathOnPC = $"{localPath}/{itemServer.Name}", SideTransmission = "<--", PathOnServer = connection.FullPath, Size = itemServer.Size, Time = DateTime.Now });
                }
            }
        }

        /// <summary>
        /// Скачивание файла с Ftp-сервера на локальный сервер(компьютер пользователя)
        /// </summary>
        /// <param name="fileName"> наименование устанавливаемого файла </param>
        /// <param name="item"> необязательный параметр с конструктором директорий и файлов на сервере </param>
        private void downloadFileOnPc(string fileName, ServerFolder item = null )
        {
            FileStream downloadFiles = null;
            Stream responseStream = null;
            try
            {
                ftpWebRequest = (FtpWebRequest)WebRequest.Create($"{connection.FullPath}/{fileName}");
                ftpWebRequest.Credentials = new NetworkCredential(connection.Login, connection.Password);
                ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                
                downloadFiles = new FileStream($"{pathOnPC}/{fileName}", FileMode.Create, FileAccess.ReadWrite);

                ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                responseStream = ftpWebResponse.GetResponseStream();

                int size = 0;
                byte[] file = new byte[10240];

                while ((size = responseStream.Read(file, 0, 1024)) > 0)
                {
                    downloadFiles.Write(file, 0, size);
                }
                status.Text += $"Статус:\tФайл {fileName} был успешно установлен\n";
                if (item != null)
                {
                    listTransferSuccess.Items.Add(new Transfers() { ImageSource = item.ImageSource, PathOnPC = pathOnPC, SideTransmission = "<--", PathOnServer = $"{connection.FullPath}/", Size = item.Size, Time = DateTime.Now });
                }
            }
            catch (Exception ex)
            {
                status.Text += $"Ошибка:\t{ex.Message}\n";
                if (item != null)
                {
                    listTransferFailed.Items.Add(new Transfers() { ImageSource = item.ImageSource, PathOnPC = pathOnPC, SideTransmission = "<--", PathOnServer = $"{connection.FullPath}/", Size = item.Size, Time = DateTime.Now });
                    transfersFailed.Add(new Transfers() { ImageSource = item.ImageSource, PathOnPC = pathOnPC, SideTransmission = "<--", PathOnServer = $"{connection.FullPath}/{item.Name}", Size = item.Size, Time = DateTime.Now });
                }
            }
            if (downloadFiles != null && responseStream != null)
            {
                downloadFiles.Close();
                responseStream.Close();
            }
            openFolderPC();
        }

        /// <summary>
        /// Установка файлов на сервер
        /// </summary>
        /// <param name="fileName"> наименование устанавливаемого файла </param>
        /// <param name="item"> необязательный параметр с конструктором директорий и файлов на клиенте </param>
        public void downloadFileOnServer(string fileName, PCFolder item = null)
        {
            try
            {
                FileStream uploadFile = null;
                ftpWebRequest = (FtpWebRequest)WebRequest.Create($"{connection.FullPath}/{fileName}");
                ftpWebRequest.Credentials = new NetworkCredential(connection.Login, connection.Password);
                ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;
                uploadFile = new FileStream($"{pathOnPC}/{fileName}", FileMode.Open, FileAccess.Read);
                byte[] fileToBytes = new byte[uploadFile.Length];
                uploadFile.Read(fileToBytes, 0, fileToBytes.Length);
                uploadFile.Close();

                Stream writer = ftpWebRequest.GetRequestStream();
                writer.Write(fileToBytes, 0, fileToBytes.Length);
                writer.Close();
                openFolderServer();
                status.Text += $"Статус:\tФайл ${fileName} успешно передан\n";
                if(item != null)
                {
                    listTransferSuccess.Items.Add(new Transfers() { ImageSource = item.ImageSource, PathOnPC = pathOnPC, SideTransmission = "-->", PathOnServer = $"{connection.FullPath}/{item.Name}", Size = item.Size, Time = DateTime.Now });
                }
            }
            catch (Exception ex)
            {
                status.Text += $"Ошибка:\t{ex.Message}\n";
                if (item != null)
                {
                    listTransferFailed.Items.Add(new Transfers() { ImageSource = item.ImageSource, PathOnPC = pathOnPC, SideTransmission = "-->", PathOnServer = $"{connection.FullPath}/{item.Name}", Size = item.Size, Time = DateTime.Now });
                    transfersFailed.Add(new Transfers() { ImageSource = item.ImageSource, PathOnPC = pathOnPC, SideTransmission = "-->", PathOnServer = $"{connection.FullPath}/{item.Name}", Size = item.Size, Time = DateTime.Now });
                }
            }
        }

        /// <summary>
        /// Изменение пути для отображения файлов FTP-сервера или локального сервера (компьютер пользователя)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txt_KeyDown(object sender, KeyEventArgs e)
        {
            transition = false;
            if (e.Key == Key.Enter)
            {
                TextBox textBox = (TextBox)sender;
                switch (textBox.Name)
                {
                    case "txtPC":
                        lastName = pathOnPC;
                        pathOnPC = textBox.Text;
                        openFolderPC();
                        if (transition)
                        {
                            textBox.Text = pathOnPC;
                            return;
                        }
                        else
                        {
                            MessageBox.Show($"Директория {pathOnPC} не найдена");
                            pathOnPC = lastName;
                            textBox.Text = lastName;
                            textBox.SelectionStart = lastName.Length;
                            openFolderPC();
                        }

                        break;
                    case "txtServer":
                        lastName = connection.FullPath;
                        connection.FullPath = $"{textBox.Text}";
                        openFolderServer();
                        if (transition)
                        {
                            textBox.Text = connection.FullPath;
                            return;
                        }
                        else
                        {
                            connection.FullPath = lastName;
                            openFolderServer();
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Установка, если файл или переход по папкам, если директория на сервере
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewItemOnServer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listViewOnServer.SelectedItems.Count > 0)
            {
                ServerFolder item = (ServerFolder)listViewOnServer.SelectedItem;
                if (item == null) return;
                if (item.Name == ".." && connection.FullPath.Length != $"ftp://{connection.Host}:{connection.Port}/".Length)
                {
                    connection.FullPath = connection.FullPath.Remove(connection.FullPath.LastIndexOf("/"), connection.FullPath.Length - connection.FullPath.LastIndexOf("/"));
                    openFolderServer();
                }
                if (item.Role != null && item.Role[0] == 'd')
                {
                    connection.FullPath += $"/{item.Name}";
                    status.Text += $"Статус:\tСписок каталогов '{connection.FullPath}' извлечен\n";
                    openFolderServer();
                }
                if (item.Role != null && item.Role[0] == '-')
                {
                    downloadFileOnPc(item.Name);
                    //Stream downloadFiles = ftpWebResponse.GetResponseStream();
                }
            }
        }

        /// <summary>
        /// Установка, если файл или переход по папкам, если дериктория на клиенте
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewItemOnPC_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PCFolder item = (PCFolder)listViewOnPC.SelectedItem;
            if (item == null) return;
            if (item.Size == null)
            {
                if (item.Name != "..")
                {
                    pathOnPC += "/" + item.Name;
                    openFolderPC();
                }
                else if (txtPC.Text.Contains("C://") || txtPC.Text.Contains("C:/"))
                {
                    pathOnPC = pathOnPC.Remove(pathOnPC.LastIndexOf("/"), pathOnPC.Length - pathOnPC.LastIndexOf("/"));
                    openFolderPC();
                }
            }
            if(Convert.ToInt64(item.Size) > 0)
            {
                downloadFileOnServer(item.Name, item);
            }
        }

        /// <summary>
        /// cоздание директории на сервере
        /// </summary>
        /// <param name="fileName"> наименование создаваемой директории </param>
        private void createNewFolderOnServer(string fileName)
        {
            try
            {
                ftpWebRequest = (FtpWebRequest)WebRequest.Create($"{connection.FullPath}{fileName}/");
                ftpWebRequest.Credentials = new NetworkCredential(connection.Login, connection.Password);
                ftpWebRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();

                status.Text += $"Статус:\tКаталог {fileName} успешно создан";
            }
            catch (Exception ex) 
            {
                status.Text += $"Ошибка\t{ex.Message}";
            }
        }

        /// <summary>
        /// cоздание файла на сервере
        /// </summary>
        /// <param name="fileName"></param>
        private void createNewFileOnServer(string fileName)
        {
            ftpWebRequest = (FtpWebRequest)WebRequest.Create($"{connection.FullPath}{fileName}");
            ftpWebRequest.Credentials = new NetworkCredential(connection.Login, connection.Password);
            ftpWebRequest.Method = WebRequestMethods.Ftp.AppendFile;
            ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();

            status.Text += $"Статус:\tФайл {fileName} успешно создан";
        }

        /// <summary>
        /// Обновление коллекций дерикторий и файлов на клиенте и на сервере
        /// </summary>
        private void refresh()
        {
            openFolderPC();
            openFolderServer();
        }

        /// <summary>
        /// Удаление директорий и файлов на сервере
        /// </summary>
        /// <param name="item">  конструктор директорий и файлов на сервере </param>
        private void deleteOnServer(ServerFolder item)
        {
            try
            {
                ftpWebRequest = (FtpWebRequest)WebRequest.Create($"{connection.FullPath}/{item.Name}");
                ftpWebRequest.Credentials = new NetworkCredential(connection.Login, connection.Password);
                if (item.Role[0] == 'd')
                {
                    ftpWebRequest.Method = WebRequestMethods.Ftp.RemoveDirectory;
                }
                else if (item.Role[0] == '-')
                {
                    ftpWebRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                }
                ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                openFolderServer();

                if (item.Role[0] == 'd')
                {
                    status.Text += $"Статус:\tКаталог '{item.Name}' успешно удален\n";
                }
                else if (item.Role[0] == '-')
                {
                    status.Text += $"Статус:\tФайл '{item.Name}' успешно удален\n";
                }
            }
            catch (Exception ex)
            {
                status.Text += $"Ошибка:\t{ex.Message}\n";
            }
        }

        /// <summary>
        /// событие нажатия на пункт контекстного меню на клиенте
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemOnPC_Click(object sender, RoutedEventArgs e)
        {
            CreateNewFolderOrFile create;
            PCFolder selectedItem = (PCFolder)listViewOnPC.SelectedItem;
            MenuItem menuItem = (MenuItem)sender;
            string image = null;

            if (selectedItem != null)
            {
                if (selectedItem.Size == null)
                {
                    contextPCInputInFolder.IsEnabled = true;
                    image = "/images/folder.png";
                }
                else if (Convert.ToInt32(selectedItem.Size) >= 0)
                {
                    contextPCInputInFolder.IsEnabled = false;
                    image = "/images/file.png";
                }
            }

            switch (menuItem.Name.Remove(0, 9))
            {
                case "Download":
                    if (selectedItem.Size != null)
                    {
                        downloadFileOnServer('/' + selectedItem.Name, selectedItem);
                    }
                    else
                    {
                        uploadFolder($"{pathOnPC}/{selectedItem.Name}", $"{connection.FullPath}/{selectedItem.Name}", selectedItem);
                    }
                    break;
                case "InputInFolder":
                    pathOnPC += '/' + selectedItem.Name;
                    openFolderPC();
                    break;
                case "Open":
                        Process.Start(pathOnPC + "/" + selectedItem.Name);
                    break;
                case "CreateFolder":
                    create = new CreateNewFolderOrFile.CreateFolder();
                    create.ShowDialog();
                    if (create.item.Name == null) { return; }
                    if (!Directory.Exists(pathOnPC + '/' + create.item.Name))
                    {
                        Directory.CreateDirectory(pathOnPC + '/' + create.item.Name);
                    }
                    status.Text += $"Статус:\tДиректория {create.item.Name} успешно создана\n";
                    openFolderPC();
                    break;
                case "CreateFile":
                    create = new CreateNewFolderOrFile.CreateFile();
                    create.ShowDialog();
                    FileStream createFile = new FileStream(pathOnPC + '/' + create.item.Name, FileMode.Create, FileAccess.ReadWrite);
                    openFolderPC();
                    break;
                case "Refresh":
                    refresh();
                    break;
                case "Delete":
                    try
                    {

                        if (selectedItem.Size == null)
                        {
                            Directory.Delete(pathOnPC + '/' + selectedItem.Name);
                            status.Text += $"Статус:\tДиректория {selectedItem.Name} успешно удалена\n";
                        }
                        else
                        {
                            File.Delete(pathOnPC + '/' + selectedItem.Name);
                            status.Text += $"Статус:\tФайл {selectedItem.Name} успешно удалена\n";
                        }
                        openFolderPC();
                    }
                    catch(Exception ex) 
                    {
                        status.Text += $"Ошибка\t{ex.Message}\n";
                    }
                    break;
                case "CopyPath":
                    Clipboard.SetText(pathOnPC + '/' + selectedItem.Name);
                    break;
                case "AddInTask":
                    taskManager.Add(new TaskManager() { ImageSource = image, PathOnPC = $"{pathOnPC}/{selectedItem.Name}", SideTransmission = "-->", PathOnServer = connection.FullPath, Size = selectedItem.Size  });
                    listInTask.Items.Add(new TaskManager() { ImageSource = image, PathOnPC = $"{pathOnPC}/{selectedItem.Name}", SideTransmission = "-->", PathOnServer = connection.FullPath, Size = selectedItem.Size });
                    break;
            }
        }

        /// <summary>
        /// событие нажатие на пункт контекстного меню на клиенте
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemOnServer_Click(object sender, RoutedEventArgs e)
        {
            ServerFolder selectedItem = (ServerFolder)listViewOnServer.SelectedItem;
            MenuItem menuItem = (MenuItem)sender;
            CreateNewFolderOrFile create;
            string image = null;

            if (selectedItem != null)
            {
                if (selectedItem.Role[0] == 'd')
                {
                    contextServerInputInFolder.IsEnabled = true;
                    image = "/images/folder.png";
                }
                else if (selectedItem.Role[0] == '-')
                {
                    contextServerInputInFolder.IsEnabled = false;
                    image = "/images/file.png";
                }
            }

            switch (menuItem.Name.Replace("contextServer", " ").Remove(0, 1))
            {
                case "Download":
                    if (selectedItem.Role[0] == 'd')
                    {
                        Directory.CreateDirectory($"{pathOnPC}/{selectedItem.Name}");
                        downloadFolder($"{connection.FullPath}/{selectedItem.Name}/", $"{pathOnPC}/{selectedItem.Name}", null, selectedItem);
                    }
                    else if (selectedItem.Role[0] == '-')
                    {
                        downloadFileOnPc(selectedItem.Name, selectedItem);
                    }
                    openFolderPC();
                    break;
                case "InputInFolder":
                    connection.FullPath += $"/{selectedItem.Name}";
                    status.Text += $"Статус:\tСписок каталогов '{connection.FullPath}' извлечен\n";
                    openFolderServer();
                    break;
                case "CreateFolder":
                    create = new CreateNewFolderOrFile.CreateFolder();
                    create.ShowDialog();
                    if (create.item.Name == null) { return; }
                    createNewFolderOnServer(create.item.Name);
                    openFolderServer();
                    break;
                case "CreateFile":
                    create = new CreateNewFolderOrFile.CreateFile();
                    create.ShowDialog();
                    if (create.item.Name == null) { return; }
                    createNewFileOnServer(create.item.Name);
                    openFolderServer();
                    break;
                case "Refresh":
                    refresh();
                    break;
                case "Delete":
                    deleteOnServer(selectedItem);
                    break;
                case "CopyPath":
                    Clipboard.SetText(connection.FullPath + '/' + selectedItem.Name);
                    break;
                case "AddInTask":
                    taskManager.Add(new TaskManager() { ImageSource = image, PathOnPC = pathOnPC, SideTransmission = "<--", PathOnServer = $"{connection.FullPath}/{selectedItem.Name}", Size = selectedItem.Size });
                    listInTask.Items.Add(new TaskManager() { ImageSource = image, PathOnPC = pathOnPC, SideTransmission = "<--", PathOnServer = $"{connection.FullPath}/{selectedItem.Name}", Size = selectedItem.Size });
                    break;
            }
        }

        /// <summary>
        /// Работа с контекстным меню на сервере
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            ServerFolder selectedItem = (ServerFolder)listViewOnServer.SelectedItem;
            listViewOnServer.UnselectAll();
            if (selectedItem == null || selectedItem.Name == "..")
            {
                contextServerCopyPath.IsEnabled = false;
                contextServerInputInFolder.IsEnabled = false;
                contextServerDownload.IsEnabled = false;
                contextServerDelete.IsEnabled = false;
                contextServerCopyPath.IsEnabled = false;
                return;
            }
            else
            {
                contextServerCopyPath.IsEnabled = true;
                contextServerInputInFolder.IsEnabled = true;
                contextServerDownload.IsEnabled = true;
                contextServerDelete.IsEnabled = true;
                contextServerCopyPath.IsEnabled = true;
                listViewOnServer.SelectedItem = selectedItem;
            }
            
            if (selectedItem.Role[0] == '-')
            {
                contextServerInputInFolder.IsEnabled = false;
            }
            else if (selectedItem.Role[0] == 'd')
            {
                contextServerInputInFolder.IsEnabled = true;
            }
        }

        /// <summary>
        /// работа с диспетчером задач
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void execute_Click(object sender, RoutedEventArgs e)
        {
            string lastPathOnServer = connection.FullPath;
            string lastPathOnPC = pathOnPC;

            foreach (var item in taskManager)
            {
                if (item.Size == null)
                {
                    if (item.SideTransmission == "<--")
                    {
                        string folder = item.PathOnServer.Substring(item.PathOnServer.LastIndexOf('/'), item.PathOnServer.Length - item.PathOnServer.LastIndexOf('/'));
                        Directory.CreateDirectory($"{pathOnPC}/{folder}");
                        downloadFolder(item.PathOnServer, item.PathOnPC + '/' + folder, item);
                    }
                    if (item.SideTransmission == "-->")
                    {
                        string folder = item.PathOnPC.Substring(item.PathOnPC.LastIndexOf('/'), item.PathOnPC.Length - item.PathOnPC.LastIndexOf('/'));
                        uploadFolder(item.PathOnPC, $"{item.PathOnServer}/{folder}");
                    }
                }
                else if (item.Size != null)
                {
                    if (item.SideTransmission == "<--")
                    {
                        connection.FullPath = item.PathOnServer.Substring(0, item.PathOnServer.LastIndexOf('/'));
                        string folder = item.PathOnServer.Substring(item.PathOnServer.LastIndexOf('/'), item.PathOnServer.Length - item.PathOnServer.LastIndexOf('/'));
                        downloadFileOnPc(folder);
                    }
                    if (item.SideTransmission == "-->")
                    {
                        pathOnPC = item.PathOnPC.Substring(0, item.PathOnPC.LastIndexOf('/'));
                        string folder = item.PathOnPC.Substring(item.PathOnPC.LastIndexOf('/'), item.PathOnPC.Length - item.PathOnPC.LastIndexOf('/'));
                        downloadFileOnServer(folder);
                    }
                }
            }
            taskManager.Clear();
            listInTask.Items.Clear();
            connection.FullPath = lastPathOnServer;
            pathOnPC = lastPathOnPC;
        }

        /// <summary>
        /// удаление элементов из диспетчера задач
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteAll_Click(object sender, RoutedEventArgs e)
        {
            taskManager.Clear();
            listInTask.Items.Clear();
            listInTask.Items.Refresh();
        }

        /// <summary>
        /// удаление конкретного элемента из диспетчера задач
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteSpecific_Click(object sender, RoutedEventArgs e)
        {
            TaskManager selectedItem = (TaskManager)listInTask.SelectedItem;
            foreach(TaskManager item in taskManager)
            {
                if(item.PathOnPC == selectedItem.PathOnPC && item.PathOnServer == selectedItem.PathOnServer && item.Size == selectedItem.Size && item.SideTransmission == selectedItem.SideTransmission)
                {
                    taskManager.Remove(item);
                    listInTask.Items.Remove(item);
                    break;
                }
            }
            listInTask.Items.Refresh();
        }

       /// <summary>
       /// удаление всех элементов из неудавшихся загрузок
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void deleteAllTransferFailed_Click(object sender, RoutedEventArgs e)
        {
            listTransferFailed.Items.Clear();
            listTransferFailed.Items.Refresh();
        }

        /// <summary>
        /// удаление конкретного элемента из неудавшихся загрузок
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteSpecificTransferFailed_Click(object sender, RoutedEventArgs e)
        {
            listTransferFailed.Items.Remove((Transfers)listTransferFailed.SelectedItem);
            listTransferFailed.Items.Refresh();
        }

        /// <summary>
        /// удаление всех элементов из успешных загрузок
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteAllTransferSuccess_Click(object sender, RoutedEventArgs e)
        {
            listTransferSuccess.Items.Clear();
            listTransferSuccess.Items.Refresh();
        }

        /// <summary>
        /// удаление конкретного элемента из успешных загрузок
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteSpecificTransferSuccess_Click(object sender, RoutedEventArgs e)
        {
            listTransferSuccess.Items.Remove((Transfers)listTransferSuccess.SelectedItem);
            listTransferSuccess.Items.Refresh();
        }
    }
}
