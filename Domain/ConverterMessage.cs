using MyClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public static class ConverterMessage
    {
        //преобразование структуры в массив байтов
        public static byte[] StructToByte(MyMessage message)
        {
            // опредеделяем длинну структуры
            var sizeMessege = Marshal.SizeOf(message);

            // создаем массив заданной длинны структуры
            var byteArr = new byte[sizeMessege];

            // создаем пустой указатель
            var ptrMessage = IntPtr.Zero;

            try
            {
                // выделяем память из неуправляемой памяти проццеса
                // из кучи
                ptrMessage = Marshal.AllocHGlobal(sizeMessege);

                // маршализуем данные структуры из стека в кучу
                Marshal.StructureToPtr(message, ptrMessage,
                   true);


                // Копируем данные структуры в кучу
                Marshal.Copy(ptrMessage, byteArr, 0, sizeMessege);

            }
            finally
            {
                // в любом случае освобождаем память кучи
                Marshal.FreeHGlobal(ptrMessage);
            }
            return byteArr;
        }

        // преобразование байт массива в строку
        public static MyMessage ByteToStruct(byte[] byteArr)
        {
            var message = new MyMessage();

            // определяем размер структуры
            var sizeMassege = Marshal.SizeOf(message);

            // создаем пустой указатель
            var ptrMessage = IntPtr.Zero;

            try
            {
                // выделяем память из неуправляемой памяти проццеса
                // из кучи
                ptrMessage = Marshal.AllocHGlobal(sizeMassege);

                Marshal.Copy(byteArr, 0, ptrMessage, sizeMassege);

                // конвертируем массив байтов в структуру
                message = (MyMessage)Marshal.PtrToStructure(ptrMessage, message.GetType());
            }
            finally
            {
                Marshal.FreeHGlobal(ptrMessage);
            }
            return message;

        }

        
    }
}
