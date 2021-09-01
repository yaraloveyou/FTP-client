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
using System.Windows.Shapes;

namespace Ftp_client
{
    /// <summary>
    /// Конструктор для создания новых файлов или дерикторий
    /// </summary>
    public class Item
    {
        private string _Type;
        private string _Name;
        private string _Role;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public string Role
        {
            get { return _Role; }
            set { _Role = value; }
        }

        public string Type
        {
            get { return _Type;  }
            set { _Type = value; }
        }
    }

    /// <summary>
    /// Логика взаимодействия для CreateNewFolderOrFile.xaml
    /// </summary>
    public partial class CreateNewFolderOrFile : Window
    {
        public Item item = new Item();

        /// <summary>
        /// Объект класса CreateNewFolderOrFile
        /// </summary>
        public CreateNewFolderOrFile()
        {
            InitializeComponent();
            txtNewFolder.Focus();
        }

        /// <summary>
        /// Событие нажатия на кнопку для создания файла или каталога
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            item.Name = txtNewFolder.Text;
            if((item.Name[0] != '/' || item.Name[0] != '\\') && item.Type == "d")
            {
                item.Name = '/' + item.Name;
            }
            Close();
        }

        /// <summary>
        /// событие нажатия на кнопку для отмены создания файла или каталога
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Item item = new Item();
            item.Role = null;
            item.Type = null;
            item.Name = null;
            Close();
        }

        /// <summary>
        /// Класс для создания директорий на сервере
        /// </summary>
        public class CreateFolder : CreateNewFolderOrFile
        {
            /// <summary>
            /// объект класса CreateFolder
            /// </summary>
            public CreateFolder()
            {
                Title = "Создать каталог";
                type.Content = "Введите имя создаваемого каталога";
                txtNewFolder.Text = "/Новый каталог";
                txtNewFolder.Select(1, txtNewFolder.Text.Length - 1);
                item.Type = "d";
                item.Role = "drwxr-xr-x";
            }
        }

        /// <summary>
        /// Класс для создания файлов на сервере
        /// </summary>
        public class CreateFile : CreateNewFolderOrFile
        {
            /// <summary>
            /// объект класса CreateFile
            /// </summary>
            public CreateFile()
            {
                Title = "Создать пустой файл";
                type.Content = "Введите имя создаваемого файла";
                item.Role = "-rw-r--r--";
                item.Type = "-";
            }
        }

        /// <summary>
        /// класс для создания директорий на клиенте
        /// </summary>
        public class CreateFolderOnPC : CreateNewFolderOrFile
        {
            /// <summary>
            /// Объект класса CreateFolderOnPC
            /// </summary>
            public CreateFolderOnPC()
            {
                Title = "Создать каталог";
                type.Content = "Введите имя создаваемого каталога";
                txtNewFolder.Text = "/Новый каталог";
                txtNewFolder.Select(1, txtNewFolder.Text.Length - 1);
            }
        }

        /// <summary>
        /// класс для создания файлов на клиенте
        /// </summary>
        public class CreateFileOnPC : CreateNewFolderOrFile
        {
            /// <summary>
            /// объект класса CreateFileOnPC
            /// </summary>
            public CreateFileOnPC()
            {
                Title = "Создать пустой файл";
                type.Content = "Введите имя создаваемого файла";
            }
        }
    }
}
