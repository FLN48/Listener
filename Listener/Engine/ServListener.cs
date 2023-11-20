using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Listener
{
    internal class ServListener
    {
        public Exception m_LastException { get; set; }
        private string m_IP { get; set; }
        private string m_Port { get; set;}
        private bool m_Work { get; set; } = false;
        private int m_CountMessage { get; set; } = 0;
        private TcpClient m_TcpClient { get; set; }
        private List<Sensor> sensors { get; set; } = new List<Sensor>();
        int outCol, outRow, outHeight = 100;
        public ServListener(string _IP, string _Port)
        {
            m_IP = _IP;
            m_Port = _Port;
        }
        public bool Init()
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                m_LastException = ex;
                return false;
            }
        }
        public async void Start()
        {
            try
            {
                if (m_Work)
                {
                    WriteOut("Сервер уже запущен\r\n", false);
                }
                outCol =0;
                outRow = 0;
                m_Work = true;
                m_TcpClient = new TcpClient();
                await m_TcpClient.ConnectAsync(m_IP, Convert.ToInt32(m_Port));
                WriteOut("Сервер запущен\r\n",false);
                var stream = m_TcpClient.GetStream();
                while (m_Work)
                {
                    ModelData modelData = new ModelData(stream);
                    for (int i=0; i < modelData.m_Types.Count;i++)
                    {
                        Sensor sensor = new Sensor((TypeSensor)modelData.m_Types[i], modelData.m_Values[i]);
                        sensors.Add(sensor);
                        WriteOut($"Получены данные о сенсоре {Enum.GetName(typeof(TypeSensor), sensor.m_TypeSensor)}\r\n" +
                            $"Значение: {sensor.m_ValueSensor}\r\n",false);
                    }
                    WriteOut("///////////////////////////////////////////////\r\n",false);
                    m_CountMessage++;
                }
            }
            catch (Exception ex)
            {
                m_Work = false;
                m_LastException = ex;
            }
            finally 
            {              
                m_TcpClient.Close();
            }
        }
        public bool Stop()
        {
            m_Work = false;
            m_TcpClient.Close();

            WriteOut($"Введите команду из списка\r\n" +
                    $"start\r\n" +
                    $"stop\r\n" +
                    $"info\r\n" +
                    $"statistics\r\n" +
                    $"exit\r\n");
            return true;
        }
        public bool Info()
        {

            int i = 1;
            WriteOut($"Данные датчика {Enum.GetName(typeof(TypeSensor), TypeSensor.Temperature)}\r\n",false);
            foreach (double d in Helpers.MovingAverage(sensors
                .Where(s => s.m_TypeSensor == TypeSensor.Temperature)
                .Select(s => s.m_ValueSensor).ToList(),
                sensors.FindAll(s => s.m_TypeSensor == TypeSensor.Temperature).Count))
            {
                WriteOut($"{i++}: {d}\r\n");
            }
            WriteOut("\r\n");

            i = 1;
            WriteOut($"Данные датчика {Enum.GetName(typeof(TypeSensor), TypeSensor.Humidity)}\r\n");
            foreach (double d in Helpers.MovingAverage(sensors
                .Where(s => s.m_TypeSensor == TypeSensor.Humidity)
                .Select(s => s.m_ValueSensor).ToList(),
                sensors.FindAll(s => s.m_TypeSensor == TypeSensor.Humidity).Count))
            {
                WriteOut($"{i++}: {d}\r\n");
            }
            WriteOut("\r\n");
            i = 1;
            WriteOut($"Данные датчика {Enum.GetName(typeof(TypeSensor), TypeSensor.Pressure)}\r\n");
            foreach (double d in Helpers.MovingAverage(sensors
                .Where(s => s.m_TypeSensor == TypeSensor.Pressure)
                .Select(s => s.m_ValueSensor).ToList(),
                sensors.FindAll(s => s.m_TypeSensor == TypeSensor.Pressure).Count))
            {
                WriteOut($"{i++}: {d}\r\n");
            }
            WriteOut("\r\n");

            return true;
        }
        public bool Statistics()
        {
            if (m_Work)
            {
                WriteOut("$Сервер активен\r\n");
            }
            else
            {
                WriteOut("$Сервер не активен\r\n");
            }
            WriteOut($"Количество полученный сообщений: {m_CountMessage}\r\n");
            return true;
        }
        private void WriteOut(string msg, bool appendNewLine = false)
        {
            int inCol, inRow;
            inCol = Console.CursorLeft;
            inRow = Console.CursorTop;

            int outLines = getMsgRowCount(outCol, msg) + (appendNewLine ? 1 : 0);
            int outBottom = outRow + outLines;
            if (outBottom > outHeight)
                outBottom = outHeight;
            if (inRow <= outBottom)
            {
                int scrollCount = outBottom - inRow + 1;
                Console.MoveBufferArea(0, inRow, Console.BufferWidth, 1, 0, inRow + scrollCount);
                inRow += scrollCount;
            }
            if (outRow + outLines > outHeight)
            {
                int scrollCount = outRow + outLines - outHeight;
                Console.MoveBufferArea(0, scrollCount, Console.BufferWidth, outHeight - scrollCount, 0, 0);
                outRow -= scrollCount;
                Console.SetCursorPosition(outCol, outRow);
            }
            Console.SetCursorPosition(outCol, outRow);
            if (appendNewLine)
                Console.WriteLine(msg);
            else
                Console.Write(msg);
            outCol = Console.CursorLeft;
            outRow = Console.CursorTop;
            Console.SetCursorPosition(inCol, inRow);
        }

        private int getMsgRowCount(int startCol, string msg)
        {
            string[] lines = msg.Split('\n');
            int result = 0;
            foreach (string line in lines)
            {
                result += (startCol + line.Length) / Console.BufferWidth;
                startCol = 0;
            }
            return result + lines.Length - 1;
        }
    }
}
