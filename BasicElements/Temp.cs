using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Linq;
using System.Globalization;


/*namespace logic
{
    [DefaultPropertyAttribute("Element")]
    public abstract class Element : ICloneable, IDisposable
    {
        [CategoryAttribute("Property"), DescriptionAttribute("Name")]
        public string Name { get; set; }
        [CategoryAttribute("Value"), DescriptionAttribute("Current value")]
        abstract public double Value {get;set;}
        protected double _value;
        public Guid guid;
        

        public bool Shared = false;
        public bool Enabled = true;

        bool disposed = false;

        public Element()
        {
            guid = Guid.NewGuid();
            
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        public Element(string elementName)
        {
            Name = elementName;
        }

        virtual public object Clone()
        {
            return this.MemberwiseClone();
        }
        
        virtual public void SetValue(double value)
        {
            Value = value;
        }
        virtual public void SetValue(bool value)
        {
            Value = Convert.ToDouble(value);
        }
        virtual public int GetImageIndex()
        {
            return 0;
        }
        public double GetAsDouble()
        {
            return Value;
        }
        public int GetAsInt()
        {
            return Convert.ToInt32(Value);
        }

        public bool GetAsBool()
        {
            return Convert.ToBoolean(Value);
        }
        public string GetName()
        {
            return Name;
        }

        virtual public bool Update(double v)
        {
            return false;
        }
        public override string ToString()
        {
            return Name;
        }
        virtual public XElement GetXElements()
        {
            return null;
        }
        static public Element FromXElement(XElement xelement, SimData simData)
        {
            Element el = null;
            
            if (xelement.Attribute("Type").Value == "SimConnect")
            {
                el = new SimConnectElement(xelement.Attribute("Name").Value);
                ((SimConnectElement)el).simVariable = xelement.Attribute("SimConnectVariable").Value;
                ((SimConnectElement)el).simUnit = xelement.Attribute("SimConnectUnit").Value;
                ((SimConnectElement)el).IOSetting = Convert.ToBoolean(xelement.Attribute("IOSetting").Value);
            }
            if (xelement.Attribute("Type").Value == "SimConnectEvent")
            {
                el = new SimConnectEvent(xelement.Attribute("Name").Value);
                ((SimConnectEvent)el).simEvent = xelement.Attribute("EventName").Value;
                //((SCEventElement)el).ElemntEventValue = Convert.ToInt32(xelement.Attribute("ElementEventValue").Value);
                ((SimConnectEvent)el).SendEveryChange = Convert.ToBoolean(xelement.Attribute("SendEveryChange").Value);
            }
            if (xelement.Attribute("Type").Value == "Variable")
            {
                el = new VariableElement(xelement.Attribute("Name").Value);

            }
            if (xelement.Attribute("Type").Value == "Constant")
            {
                el = new ConstantElement(xelement.Attribute("Name").Value);
                ((ConstantElement)el).Value = Convert.ToDouble(xelement.Attribute("Value").Value, new CultureInfo("En-Us"));
            }
            if (xelement.Attribute("Type").Value == "UniBoxSwitch")
            {
                el = new UniboxSwitchElement(xelement.Attribute("Name").Value);
                ((UniboxSwitchElement)el).UniBox = Convert.ToInt32(xelement.Attribute("UniBox").Value);
                ((UniboxSwitchElement)el).Port = Convert.ToInt32(xelement.Attribute("Port").Value);
                ((UniboxSwitchElement)el).Position = Convert.ToInt32(xelement.Attribute("Position").Value);
                ((UniboxSwitchElement)el).IOSetting = Convert.ToBoolean(xelement.Attribute("IOSetting").Value);
            }
            if (xelement.Attribute("Type").Value == "UniBoxMagnetometr")
            {
                el = new UniboxMagnetometrElement(xelement.Attribute("Name").Value);
                ((UniboxMagnetometrElement)el).UniBox = Convert.ToInt32(xelement.Attribute("UniBox").Value);
                ((UniboxMagnetometrElement)el).Branch = Convert.ToInt32(xelement.Attribute("Branch").Value);
                ((UniboxMagnetometrElement)el).Position = Convert.ToInt32(xelement.Attribute("Position").Value);

            }
            if (xelement.Attribute("Type").Value == "UniBoxForces")
            {
                el = new UniboxForcesElement(xelement.Attribute("Name").Value);
                ((UniboxForcesElement)el).UniBox = Convert.ToInt32(xelement.Attribute("UniBox").Value);
                ((UniboxForcesElement)el).Position = Convert.ToInt32(xelement.Attribute("Position").Value);

            }
            if (xelement.Attribute("Type").Value == "UniBoxEncoder")
            {
                el = new UniboxEncoderElement(xelement.Attribute("Name").Value);
                ((UniboxEncoderElement)el).UniBox = Convert.ToInt32(xelement.Attribute("UniBox").Value);
                ((UniboxEncoderElement)el).Branch = Convert.ToInt32(xelement.Attribute("Branch").Value);
                ((UniboxEncoderElement)el).Position = Convert.ToInt32(xelement.Attribute("Position").Value);

            }
            if (xelement.Attribute("Type").Value == "SimData") 
            {
                SimDataElement.SIMDATATYPE t = (SimDataElement.SIMDATATYPE)Enum.Parse(typeof(SimDataElement.SIMDATATYPE), xelement.Attribute("SimDataType").Value.ToString(), true);
                el = new SimDataElement(xelement.Attribute("Name").Value, t , Convert.ToInt32(xelement.Attribute("Position").Value), simData);

            }
            if (xelement.Attribute("guid") != null)
                el.guid = Guid.Parse(xelement.Attribute("guid").Value);
            return el;
        }
        static public Element[] FromXElements(XElement[] xelements, SimData simData)
        {
            Element[] elements = new Element[0];
            foreach (var xelement in xelements)
            {
                Element el=FromXElement(xelement, simData);
               
                if (el != null)
                {
                    Array.Resize(ref elements, elements.Count() + 1);
                    elements[elements.Count() - 1] = el;
                }

            }
            return elements;
        }
    }

    public class SimConnectElement : Element
    {
       
        [CategoryAttribute("Property"), DescriptionAttribute("Input or Output settings. True = Input")]
        public bool IOSetting { get; set; }

        public SimConnectElement(string elementName)
        {
            Name = elementName;
            
        }

        public override double Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }


        override public object Clone()
        {
            return this.MemberwiseClone();
        }
        override public int GetImageIndex()
        {
            return 1;
        }
        public void SetSimVariable(string variableName)
        {
            simVariable = variableName;
        }
        override public XElement GetXElements()
        {
            return new XElement("Element", new XAttribute("Name", Name), new XAttribute("Type", "SimConnect"), new XAttribute("IOSetting", IOSetting), new XAttribute("SimConnectVariable", simVariable), new XAttribute("SimConnectUnit", simUnit), new XAttribute("guid",guid));
        }
       
    }

    public class SimConnectEvent : Element
    {
        [CategoryAttribute("Property"), DescriptionAttribute("Event Name")]
        public string simEvent { get; set; }
        [CategoryAttribute("Property"), DescriptionAttribute("Unused")]
        private int ElemntEventValue { get; set; }
        [CategoryAttribute("Value"), DescriptionAttribute("The event send if the Value!=OldValue")]
        public double OldValue  { get; set; }
        [CategoryAttribute("Property"), DescriptionAttribute("If true send event when value is changed, if false send whne change from 0 to 1")]
        public bool SendEveryChange { get; set; }
        
        public SimConnectEvent(string elementName)
        {
            Name = elementName;

        }

        public override double Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }
        
        override public object Clone()
        {
            return this.MemberwiseClone();
        }
        override public int GetImageIndex()
        {
            return 2;
        }
        public void SetSimVariable(string simEventName)
        {
            simEvent = simEventName;
        }
        override public XElement GetXElements()
        {
            return new XElement("Element", new XAttribute("Name", Name), new XAttribute("Type", "SimConnectEvent"), new XAttribute("EventName", simEvent), new XAttribute("ElementEventValue", ElemntEventValue), new XAttribute("SendEveryChange", SendEveryChange), new XAttribute("guid",guid));
        }

    }

    public class ConstantElement : Element
    {
        public ConstantElement(string elementName)
        {
            Name = elementName;
            
        }

        public override double Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }


        override public object Clone()
        {
            return this.MemberwiseClone();
        }
        public void SetSimVariable(string variableName)
        {
            
        }
        override public XElement GetXElements()
        {
            return new XElement("Element", new XAttribute("Name", Name), new XAttribute("Type", "Constant"), new XAttribute("Value", Value), new XAttribute("guid", guid));
        }
        override public int GetImageIndex()
        {
            return 3;
        }
    }
    
    public class VariableElement : Element
    {
        public VariableElement(string elementName)
        {
            Name = elementName;
            
        }
        public override double Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        override public object Clone()
        {
            return this.MemberwiseClone();
        }
        override public XElement GetXElements()
        {
            return new XElement("Element", new XAttribute("Name", Name), new XAttribute("Type", "Variable"), new XAttribute("guid", guid));
        }
        override public int GetImageIndex()
        {
            return 4;
        }
    }

    public class UniboxSwitchElement : Element
    {
        [CategoryAttribute("Property"), DescriptionAttribute("Unibox Number (0-X)")]
        public int UniBox { get; set; }
        [CategoryAttribute("Property"), DescriptionAttribute("IOBUS port (0-4)")]
        public int Port { get; set; }
        [CategoryAttribute("Property"), DescriptionAttribute("IOBUS signal (0-15)")]
        public int Position { get; set; }
        [CategoryAttribute("Property"), DescriptionAttribute("Input or Output settings. True = Input")]
        public bool IOSetting { get; set; }

        public UniboxSwitchElement(string elementName)
        {
            Name = elementName;         
        }
        public override double Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        override public object Clone()
        {
            return this.MemberwiseClone();
        }
        override public int GetImageIndex()
        {
            return 5;
        }
        override public XElement GetXElements()
        {
            return new XElement("Element", new XAttribute("Name", Name), new XAttribute("Type", "UniBoxSwitch"), new XAttribute("IOSetting", IOSetting), new XAttribute("UniBox", UniBox), new XAttribute("Port", Port), new XAttribute("Position", Position), new XAttribute("guid", guid));
        }

    }
    public class UniboxMagnetometrElement : Element
    {
        [CategoryAttribute("Property"), DescriptionAttribute("Unibox Number (0-X)")]
        public int UniBox { get; set; }
        [CategoryAttribute("Property"), DescriptionAttribute("IOBUS branch (0-2)")]
        public int Branch { get; set; }
        [CategoryAttribute("Property"), DescriptionAttribute("IOBUS signal (0-6)")]
        public int Position { get; set; }
      
        public UniboxMagnetometrElement(string elementName)
        {
            Name = elementName;
        }
        public override double Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        override public object Clone()
        {
            return this.MemberwiseClone();
        }
        override public int GetImageIndex()
        {
            return 6;
        }
        override public XElement GetXElements()
        {
            return new XElement("Element", new XAttribute("Name", Name), new XAttribute("Type", "UniBoxMagnetometr"), new XAttribute("UniBox", UniBox), new XAttribute("Branch", Branch), new XAttribute("Position", Position), new XAttribute("guid", guid));
        }

    }

    public class UniboxEncoderElement : Element
    {
        [CategoryAttribute("Property"), DescriptionAttribute("Unibox Number (0-X)")]
        public int UniBox { get; set; }
        [CategoryAttribute("Property"), DescriptionAttribute("IOBUS branch (0-1)")]
        public int Branch { get; set; }
        [CategoryAttribute("Property"), DescriptionAttribute("IOBUS encoder (0-7)")]
        public int Position { get; set; }

        public UniboxEncoderElement(string elementName)
        {
            Name = elementName;
        }
        public override double Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        override public object Clone()
        {
            return this.MemberwiseClone();
        }
        override public int GetImageIndex()
        {
            return 6;
        }
        override public XElement GetXElements()
        {
            return new XElement("Element", new XAttribute("Name", Name), new XAttribute("Type", "UniBoxEncoder"), new XAttribute("UniBox", UniBox), new XAttribute("Branch", Branch), new XAttribute("Position", Position), new XAttribute("guid",guid));
        }

    }
    public class UniboxForcesElement : Element
    {
        [CategoryAttribute("Property"), DescriptionAttribute("Unibox Number (0-X)")]
        public int UniBox { get; set; }
        [CategoryAttribute("Property"), DescriptionAttribute("IOBUS signal (0-5)")]
        public int Position { get; set; }

        public UniboxForcesElement(string elementName)
        {
            Name = elementName;
        }
        public override double Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        override public object Clone()
        {
            return this.MemberwiseClone();
        }
        override public int GetImageIndex()
        {
            return 5;
        }
        override public XElement GetXElements()
        {
            return new XElement("Element", new XAttribute("Name", Name), new XAttribute("Type", "UniBoxForces"), new XAttribute("UniBox", UniBox), new XAttribute("Position", Position), new XAttribute("guid", guid));
        }

    }

    public class SimDataElement : Element
    {
        public enum SIMDATATYPE
        {
            SWITCH,
            BANK,
        }
        [CategoryAttribute("Property"), DescriptionAttribute("Set type of element")]
        public SIMDATATYPE SimDataType { get; set; }
        [CategoryAttribute("Property"), DescriptionAttribute("Position sim data")]
        public int Position { get; set; }
        private SimData _simData;

        public SimDataElement(string elementName, SIMDATATYPE type, int position, SimData simData)
        {
            Name = elementName;
            SimDataType = type;
            Position = position;
            _simData = simData;

        }
        public override double Value
        {
            get
            {
                if (SimDataType == SIMDATATYPE.SWITCH)
                    return _simData.data.switches[Position];
                else
                    return _simData.data.bank[Position];
            }
            set
            {
                if (SimDataType == SIMDATATYPE.SWITCH)
                    _simData.data.switches[Position] = Convert.ToByte(value);
                else
                    _simData.data.bank[Position] = value;
                _value = value;
            }
        }

        override public object Clone()
        {
            return this.MemberwiseClone();
        }
        override public XElement GetXElements()
        {
            return new XElement("Element", new XAttribute("Name", Name), new XAttribute("Type", "SimData"), new XAttribute("SimDataType", SimDataType), new XAttribute("Position", Position), new XAttribute("guid", guid));
        }
        override public int GetImageIndex()
        {
            return 4;
        }
    }
}*/
