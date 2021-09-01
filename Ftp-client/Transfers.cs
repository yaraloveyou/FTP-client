using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ftp_client
{
    /// <summary>
    /// Конструктор для успешных или неудавшихся загрузок
    /// </summary>
    class Transfers
    {
        private string _PathOnPC;
        private string _SideTransmission;
        private string _PathOnServer;
        private string _Size;
        private string _Condition;
        private string _Type;
        private DateTime _Time;

        public string ImageSource { get; set; }

        public string PathOnPC
        {
            get { return _PathOnPC; }
            set { _PathOnPC = value; }
        }
        public string SideTransmission
        {
            get { return _SideTransmission; }
            set { _SideTransmission = value; }
        }
        public string PathOnServer
        {
            get { return _PathOnServer; }
            set { _PathOnServer = value; }
        }
        public string Size
        {
            get { return _Size; }
            set { _Size = value; }
        }
        public string Condition
        {
            get { return _Condition; }
            set { _Condition = value; }
        }

        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        public DateTime Time
        {
            get { return _Time; }
            set { _Time = value; }
        }
    }
}
