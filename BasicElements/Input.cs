using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Globalization;

namespace logic
{
    public class Input : Element
    {
        [ElementPropertyAttribute(ElementPropertyAttribute.AttType.REMOTE | ElementPropertyAttribute.AttType.SAVE)]
        [LabPropertyAttribute(LabPropertyAttribute.AttType.VISUAL)]
        public override double Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                base.Value = value;
            }
        }

        public override Type GetWrapperType()
        {
            return null;
        }

      
    }
}
