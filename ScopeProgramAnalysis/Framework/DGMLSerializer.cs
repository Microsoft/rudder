﻿using Backend.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;

namespace Backend.Serialization
{
    public static class MyDGMLSerializer
    {
        public static string Serialize(SimplePointsToGraph ptg)
        {
            using (var stringWriter = new StringWriter())
            using (var xmlWriter = new XmlTextWriter(stringWriter))
            {
                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.WriteStartElement("DirectedGraph");
                xmlWriter.WriteAttributeString("xmlns", "http://schemas.microsoft.com/vs/2009/dgml");
                xmlWriter.WriteStartElement("Nodes");

                foreach (var variable in ptg.Roots)
                {
                    var label = variable.Name;

                    xmlWriter.WriteStartElement("Node");
                    xmlWriter.WriteAttributeString("Id", label);
                    xmlWriter.WriteAttributeString("Label", label);
                    xmlWriter.WriteAttributeString("Shape", "None");
                    xmlWriter.WriteEndElement();
                }

                foreach (var node in ptg.Nodes)
                {
                    var nodeId = Convert.ToString(node.Id);
                    var label = DGMLSerializer.Serialize(node);

                    xmlWriter.WriteStartElement("Node");
                    xmlWriter.WriteAttributeString("Id", nodeId);
                    xmlWriter.WriteAttributeString("Label", label);

                    if (node.Kind == PTGNodeKind.Null)
                    {
                        xmlWriter.WriteAttributeString("Background", "Yellow");
                    }
                    else if (node.Kind == PTGNodeKind.Delegate)
                    {
                        xmlWriter.WriteAttributeString("Background", "Cyan");
                    }
                    else if (node.Kind == PTGNodeKind.Parameter)
                    {
                        xmlWriter.WriteAttributeString("Background", "Red");
                        xmlWriter.WriteAttributeString("StrokeDashArray", "6,6");
                    }
                    else if (node.Kind == PTGNodeKind.Unknown)
                    {
                        xmlWriter.WriteAttributeString("Background", "#FFB445");
                        xmlWriter.WriteAttributeString("StrokeDashArray", "6,6");
                    }

                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("Links");

                foreach (var variable in ptg.Roots)
                {
                    var sourceId = variable.Name;
                    foreach (var target in ptg.GetTargets(variable))
                    {
                        var targetId = Convert.ToString(target.Id);

                        xmlWriter.WriteStartElement("Link");
                        xmlWriter.WriteAttributeString("Source", sourceId);
                        xmlWriter.WriteAttributeString("Target", targetId);
                        xmlWriter.WriteEndElement();
                    }
                }

                foreach (var node in ptg.Nodes)
                {
                    var sourceId = Convert.ToString(node.Id);

                    foreach (var targetMap in ptg.GetTargets(node))
                    {
                        foreach (var target in targetMap.Value)
                        {
                            var targetId = Convert.ToString(target.Id);
                            var label = targetMap.Key.Name;

                            xmlWriter.WriteStartElement("Link");
                            xmlWriter.WriteAttributeString("Source", sourceId);
                            xmlWriter.WriteAttributeString("Target", targetId);
                            xmlWriter.WriteAttributeString("Label", label);
                            xmlWriter.WriteEndElement();
                        }
                    }

                }

                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("Styles");
                xmlWriter.WriteStartElement("Style");
                xmlWriter.WriteAttributeString("TargetType", "Node");

                xmlWriter.WriteStartElement("Setter");
                xmlWriter.WriteAttributeString("Property", "FontFamily");
                xmlWriter.WriteAttributeString("Value", "Consolas");
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Setter");
                xmlWriter.WriteAttributeString("Property", "NodeRadius");
                xmlWriter.WriteAttributeString("Value", "5");
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Setter");
                xmlWriter.WriteAttributeString("Property", "MinWidth");
                xmlWriter.WriteAttributeString("Value", "0");
                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
                xmlWriter.Flush();
                return stringWriter.ToString();
            }
        }

    }
}
