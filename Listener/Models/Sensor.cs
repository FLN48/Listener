using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener
{
    public enum TypeSensor: byte
    {
        Temperature = 1,
        Humidity = 2,
        Pressure = 3
    }
    internal class Sensor
    {
        public TypeSensor m_TypeSensor { get; set; }
        public double m_ValueSensor { get; set; }
        public Sensor(TypeSensor _TypeSensor, double _ValueSensor) 
        {
            m_TypeSensor = _TypeSensor;
            m_ValueSensor = _ValueSensor;
        }
    }
}
