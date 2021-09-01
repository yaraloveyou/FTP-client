using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ftp_client
{
    /// <summary>
    /// Конструктор для списка каталогов и файлов на клиенте
    /// </summary>
    public class PCFolder
    {
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public string Type { get; set; }
        public string Size { get; set; }
        public string LastChange { get; set; }

        public string ImageSource { get; set; }
    }

    /// <summary>
    /// Конструктор для списка каталогов и файлов на сервере
    /// </summary>
    public class ServerFolder
    {
        private string _Role;
        private string _Name;
        private string _Size;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public string ImageSource { get; set; }
        public string Role
        {
            get { return _Role; }
            set { _Role = value; }
        }
        public string Size
        {
            get { return _Size; }
            set { _Size = value; }
        }
    }
}
