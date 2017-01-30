using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SteamManifestReader
{
    /// <summary>
    /// Generates an FCIV conform XML
    ///<?xml version="1.0" encoding="utf-8"?>
    ///<FCIV>
    ///    <FILE_ENTRY>
    ///        <name> </name>
    ///        <MD5> </MD5>
    ///        <SHA1> </SHA1>
    ///    </FILE_ENTRY>
    ///</FCIV>	
    /// </summary>
    class FCIVExport
    {
        private XmlWriter _writer = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename">filename or path</param>
        public FCIVExport(string filename)
        {
            if(!string.IsNullOrEmpty(filename))
                Initialize(filename);
        }
        
        private void Initialize(string filename)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            _writer = XmlWriter.Create(filename, settings);

            _writer.WriteStartDocument();

            // FCIV start
            _writer.WriteStartElement("FCIV");
        }

        public void AddEntry(string fileOrPath, string sha1hash)
        {
            if (_writer == null)
                return;

            // FILE_ENTRY start
            _writer.WriteStartElement("FILE_ENTRY");

            // name start & end
            _writer.WriteStartElement("name");
            _writer.WriteString(fileOrPath);
            _writer.WriteEndElement();

            // SHA1 start & end
            _writer.WriteStartElement("SHA1");
            _writer.WriteString(sha1hash);
            _writer.WriteEndElement();

            // FILE_ENTRY end
            _writer.WriteEndElement();
        }

        public void Finalize()
        {
            if (_writer == null)
                return;

            // FCIV end
            _writer.WriteEndElement();

            _writer.WriteEndDocument();

            _writer.Close();
            _writer = null;
        }
    }
}
