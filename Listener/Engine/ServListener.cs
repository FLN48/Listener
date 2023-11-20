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
        public ServListener(string _IP, string _Port)
        {
            m_IP = _IP;
            m_Port = _Port;
        }
        public bool Init()
        {
            try
            {
                m_TcpClient = new TcpClient();
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
                    Helpers.WriteOut("Сервер уже запущен\r\n", false);
                }
                m_Work = true;
                m_TcpClient = new TcpClient();
                await m_TcpClient.ConnectAsync(m_IP, Convert.ToInt32(m_Port));
                Helpers.WriteOut("Сервер запущен\r\n",false);
                var stream = m_TcpClient.GetStream();
                while (m_Work)
                {
                    ModelData modelData = new ModelData(stream);
                    for (int i=0; i < modelData.m_Types.Count;i++)
                    {
                        Sensor sensor = new Sensor((TypeSensor)modelData.m_Types[i], modelData.m_Values[i]);
                        sensors.Add(sensor);
                        Helpers.WriteOut($"Получены данные о сенсоре {Enum.GetName(typeof(TypeSensor), sensor.m_TypeSensor)}\r\n" +
                            $"Значение: {sensor.m_ValueSensor}\r\n",false);
                    }
                    Helpers.WriteOut("///////////////////////////////////////////////\r\n",false);
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
            return true;
        }
        public bool Info()
        {

            int i = 1;
            Helpers.WriteOut($"Данные датчика {Enum.GetName(typeof(TypeSensor), TypeSensor.Temperature)}\r\n",false);
            foreach (double d in Helpers.MovingAverage(sensors
                .Where(s => s.m_TypeSensor == TypeSensor.Temperature)
                .Select(s => s.m_ValueSensor).ToList(),
                sensors.FindAll(s => s.m_TypeSensor == TypeSensor.Temperature).Count))
            {
                Helpers.WriteOut($"{i++}: {d}\r\n");
            }
            Helpers.WriteOut("\r\n");

            i = 1;
            Helpers.WriteOut($"Данные датчика {Enum.GetName(typeof(TypeSensor), TypeSensor.Humidity)}\r\n");
            foreach (double d in Helpers.MovingAverage(sensors
                .Where(s => s.m_TypeSensor == TypeSensor.Humidity)
                .Select(s => s.m_ValueSensor).ToList(),
                sensors.FindAll(s => s.m_TypeSensor == TypeSensor.Humidity).Count))
            {
                Helpers.WriteOut($"{i++}: {d}\r\n");
            }
            Helpers.WriteOut("\r\n");
            i = 1;
            Helpers.WriteOut($"Данные датчика {Enum.GetName(typeof(TypeSensor), TypeSensor.Pressure)}\r\n");
            foreach (double d in Helpers.MovingAverage(sensors
                .Where(s => s.m_TypeSensor == TypeSensor.Pressure)
                .Select(s => s.m_ValueSensor).ToList(),
                sensors.FindAll(s => s.m_TypeSensor == TypeSensor.Pressure).Count))
            {
                Helpers.WriteOut($"{i++}: {d}\r\n");
            }
            Helpers.WriteOut("\r\n");

            return true;
        }
        public bool Statistics()
        {
            if (m_Work)
            {
                Helpers.WriteOut("$Сервер активен\r\n");
            }
            else
            {
                Helpers.WriteOut("$Сервер не активен\r\n");
            }
            Helpers.WriteOut($"Количество полученный сообщений: {m_CountMessage}\r\n");
            return true;
        }
    }
}
