using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ftp_client
{
    /// <summary>
    /// Конструктор для создания подключения к серверу
    /// </summary>
    class Connection
    {
        private string _Host;
        private string _Login;
        private string _Password;
        private bool _SSL = false;
        private string _Port;
        private string _FullPath;

        public string Host
        {
            get { return _Host; }
            set { _Host = value; }
        }

        public string FullPath
        {
            get { return _FullPath; }
            set { _FullPath = value; }
        }

        public string Login
        {
            get { return _Login; }
            set { _Login = value; }
        }

        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        public string Port
        {
            get { return _Port; }
            set { _Port = value; }
        }
        public bool SSL
        {
            get { return _SSL; }
            set { _SSL = value; }
        }
    }
}
