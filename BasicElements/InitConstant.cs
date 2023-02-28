using logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace BasicElements
{
    public class InitConstant : Constant
    {
        [LabPropertyAttribute(LabPropertyAttribute.AttType.NONVISUAL)]
        public double InitValue { get; set; }

        
        public override XElement ToXML(ElementPropertyAttribute.AttType saveType)
        {
            Value = InitValue;
            return base.ToXML(saveType);
        }

        public override int Init(object InitData)
        {
            base.Init(InitData);
            Value = InitValue;
			return 0;
        }
    }
}
