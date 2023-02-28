/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using LockheedMartin.Prepar3D.SimConnect;
using System.Runtime.InteropServices;
using System.IO.MemoryMappedFiles;
using System.IO;
using System.Runtime.Remoting;
using System.Reflection;


namespace logic
{
    public class SignalConvertor
    {
        public struct InterTheoremMemElement
        {
            public string Name;
            public double Data;
        }

        public List<Type> paradigmaTypes = new List<Type>();

        public SignalConvertor()
        {
            Init();
            LoadConnectionTypes(@".\");
        }

        public void LoadConnectionTypes(string path)
        {
            DirectoryInfo dllDirectory = new DirectoryInfo(path);
            FileInfo[] dlls = dllDirectory.GetFiles("*.par");
            foreach (FileInfo dllFileInfo in dlls)
            {
                Assembly assembly = Assembly.LoadFrom(dllFileInfo.FullName);
                paradigmaTypes.AddRange(assembly.GetTypes());
            }
        }
        
        public void Init()
        {
       
        }

        public SimData simData = new SimData();
       
        public SharedElements sharedElements = new SharedElements();
        
        public List<Theorem> theorems=new List<Theorem>();
        
        public string ProjectDirectory { get; set; }

        public void AddTheorem(Theorem theorem)
        {
            theorems.Add(theorem);
        }
        
        public void SaveToXML(ref XElement xroot)
        {
            ExportToXML(ref xroot, theorems.ToArray());
            
        }
        
        public void ExportToXML(ref XElement xroot, Theorem[] theoremsForExport)
        {
            List<XElement> xTheorems = new List<XElement>();
            foreach (var theorem in theoremsForExport)
            {
                List<XElement> xElements = new List<XElement>();
                foreach (var element in theorem.Elements)
                {
                    if (element.Shared == false)
                        xElements.Add(element.GetXElements());
                    else
                        xElements.Add(new XElement("Element", new XAttribute("Name", element.Name), new XAttribute("Type", "Shared"), new XAttribute("Index", sharedElements.items.IndexOf(element))));
                }
                List<XElement> xParadigmas = new List<XElement>();
                foreach (var paradigma in theorem.paradigma)
                {
                    xParadigmas.Add(paradigma.GetXElements());
                }
                xTheorems.Add(new XElement("theorem", new XAttribute("Name", theorem.Name), new XAttribute("guid",theorem.guid), xElements.ToArray(), xParadigmas.ToArray()));
            }

            XElement xml = new XElement("Theorems",  xTheorems.ToArray(), simData.GetDescriptionToXML(), sharedElements.getSharedElementsXML());
            xroot.Add(xml);
            //xml.Save(fileName);
        }
        
        public void LoadFromXML(XElement root)
        {
            double version;
            if (root.Attribute("Ver") != null)
                version = Convert.ToDouble(root.Attribute("Ver").Value, new System.Globalization.CultureInfo("en-US"));
            else
                version = 1.0;

            XElement xml = root.Element("Theorems");

            if (xml == null)
                xml = root;

            // Shared elements
            XElement sharedXElements = xml.Element("SharedElements");
            sharedElements = new SharedElements();
            sharedElements.LoadSharedElementsFromXML(sharedXElements, simData);

            // SimData list
            XElement simdataXElements = xml.Element("SimConnectData");
            simData = new SimData();
            simData.CreateDescriptionFromXML(simdataXElements);

            //Theorems
            XElement[] theoremXElements = xml.Elements("theorem").ToArray();
            foreach (var theoremXElement in theoremXElements)
            {
               

                Theorem th = new Theorem(theoremXElement.Attribute("Name").Value);
                if (theoremXElement.Attribute("guid") != null)
                    th.guid = Guid.Parse(theoremXElement.Attribute("guid").Value);
              
                AddTheorem(th);
                XElement[] elementXElements = theoremXElement.Elements("Element").ToArray();
                foreach (var xElement in elementXElements)
                {
                    if (xElement.Attribute("Type").Value == "Shared")
                    {
                        string name = xElement.Attribute("Name").Value.ToString();
                        int index = Convert.ToInt32(xElement.Attribute("Index").Value);
                        th.AddElement(sharedElements.items[index]);
                    }
                    else
                        th.Elements.Add(Element.FromXElement(xElement, simData));
                }         
                
                XElement[] paradigmaXElements = theoremXElement.Elements("Paradigma").ToArray();
                foreach (var xpar in paradigmaXElements)
                {
                    Type type = paradigmaTypes.Find(i => i.Name == xpar.Attribute("Type").Value.ToString());
                    if (type != null)
                    {
                        Paradigma par = (Paradigma)Activator.CreateInstance(type);
                        par.FromXElements(xpar, th, version);
                        if (version < 0)
                        {
                            par.Prepare();
                        }
                        th.paradigma.Add(par);

                    }
                    else
                    {
                        version = 0.1;
                    }
                   
                }
              }
          }

        public void Process()
        {
            foreach (var th in theorems)
            {
                th.Process();
            }
        }
    }
}*/
