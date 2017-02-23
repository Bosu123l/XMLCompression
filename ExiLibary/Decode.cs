using System;
using System.Collections.Generic;

namespace ExiLibary
{
    public class Decode
    {
        private readonly string _textToDecode;
        private readonly Stack<string> _rememberedNodeNames;
        private string decompressedXML;

        public Decode(string textToDecode)
        {
            _textToDecode = textToDecode;
            _rememberedNodeNames = new Stack<string>();
            decompressedXML = string.Empty;
        }

        public string DecodeToXml()
        {
            decompressedXML = string.Empty;
            var toCommpressCollection = _textToDecode.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            foreach (string line in toCommpressCollection)
            {
                if (line.Equals(ConstantsMarks.SD))
                {
                    decompressedXML += Helper.AddWithNewLine(ConstantsMarks.mainRoot);
                }
                if (line.Contains(ConstantsMarks.SE))
                {
                    string nodeName = line.Split('(', ')')[1];

                    string node = string.Format("<{0}>", nodeName);

                    decompressedXML += Helper.AddWithNewLine(node);
                    _rememberedNodeNames.Push(nodeName);
                }
                if (line.Contains(ConstantsMarks.CM))
                {
                    string commentaryValue = string.Format("<!--{0}-->", line.Split('(', ')')[1]);

                    decompressedXML += Helper.AddWithNewLine(commentaryValue);
                }
                if (line.Contains(ConstantsMarks.CH))
                {
                    decompressedXML = Helper.RemoveLastChar(decompressedXML, 2);

                    string nodeValue = line.Split('(', ')')[1];
                    decompressedXML += nodeValue;
                }
                if (line.Contains(ConstantsMarks.EE))
                {
                    string closeNode = string.Format("</{0}>", _rememberedNodeNames.Pop());
                    decompressedXML += Helper.AddWithNewLine(closeNode);
                }
                if (line.Contains(ConstantsMarks.AT))
                {
                    var typeAtribute = (line.Split('(', ')')[1]).Split(':');

                    if (typeAtribute.Length > 1)
                    {
                        decompressedXML = Helper.RemoveLastChar(decompressedXML, 3);
                        var atribute = string.Format(" {0}=\"{1}\">", typeAtribute[0], typeAtribute[1]);

                        decompressedXML += Helper.AddWithNewLine(atribute);
                    }
                }
                if (line.Contains(ConstantsMarks.ED))
                {
                    if (_rememberedNodeNames.Count > 0)
                    {
                        string closeNode = string.Format("</{0}>", _rememberedNodeNames.Pop());
                        decompressedXML += Helper.AddWithNewLine(closeNode);
                    }
                }
            }

            return Helper.PrintXml(decompressedXML);
        }
    }
}