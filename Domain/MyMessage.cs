using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MyClient
{
    // структура для отправки через сокет со служебной информацией
    // 1 байт и строкой для последующего расскодирования
    // если  1 байт равен 4 - значит отправляется ключ, если 7, то сообшение
    public struct MyMessage
    {
        public byte ServiseInfo;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 148)]
        public byte[] Message = new byte[148];

        public MyMessage()
        {

        }
    }
}
