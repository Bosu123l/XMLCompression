using System;
using System.Collections.Generic;
using System.Xml;

namespace ExiLibary
{
    public class Code
    {
        private readonly string _textToCompress;
        private readonly List<string> _compressed;

        public Code(string textToCompress)
        {
            _textToCompress = textToCompress;
            _compressed = new List<string>();
        }

        public string CodeToExi()
        {
            _compressed.Clear();
            CodeXml(_textToCompress);

            return string.Join(Environment.NewLine, _compressed);//jezeli nie chce  ze enterami to wpisuje 1 arg jako ""
        }

        private void CodeXml(string readedXml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(readedXml);

            MakeExiNode(doc);
            _compressed.Add(ConstantsMarks.ED);
        }

        private void MakeExiNode(XmlNode doc)
        {
            foreach (XmlNode node in doc.ChildNodes)
            {
                if (node.Name.Equals(ConstantsMarks.xml))
                {
                    _compressed.Add(ConstantsMarks.SD);
                  
                }
                if (node.Name.Equals(ConstantsMarks.comment))
                {
                    _compressed.Add(string.Format("{0}({1})", ConstantsMarks.SC, node.Value));
                }
                else if (!node.Name.Equals(ConstantsMarks.text) && !node.Name.Equals(ConstantsMarks.xml)) //)
                {
                    _compressed.Add(string.Format("{0}({1})", ConstantsMarks.SE, node.Name));
                    if (node.Attributes != null && node.Attributes.Count > 0)
                    {
                        foreach (XmlAttribute atribute in node.Attributes)
                        {
                            _compressed.Add(string.Format("{0}({1}:{2})", ConstantsMarks.AT, atribute.LocalName, atribute.Value));
                        }
                    }
                }
                else
                {
                    if (node.Value.Contains(Environment.NewLine))
                    {
                        var noNewLine = node.Value.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                        _compressed.Add(string.Format("{0}({1})", ConstantsMarks.CH, string.Join("", noNewLine)));
                    }
                    else
                    {
                        if (!node.Name.Equals(ConstantsMarks.xml))
                        {
                            _compressed.Add(string.Format("{0}({1})", ConstantsMarks.CH, node.Value));
                        }
                  
                    }
                }

                if (node.ChildNodes.Count > 0)
                {
                    MakeExiNode(node);
                    _compressed.Add(ConstantsMarks.EE);
                }
            }
        }
    }
}