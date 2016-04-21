using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace RICContentStudio
{
	public class RICResponse
	{
		public List<object> Data { get; set; }

		RICResponse ()
		{
			Data = new List<object> ();
		}

		public static RICResponse Parse (string xml)
		{
			var result = new RICResponse ();
			using (var reader = XmlReader.Create (new MemoryStream (Encoding.UTF8.GetBytes (xml))))
				while (!reader.EOF)
					if (reader.IsStartElement ("Article"))
						result.Data.Add (Article.Parse (reader.ReadOuterXml ()));
					else if (reader.IsStartElement ("Text"))
						result.Data.Add (reader.ReadElementContentAsString ());
					else reader.Read ();
			return result;
		}

		public static RICResponse Text (string text)
		{
			return new RICResponse{ Data = new List<object> (new []{ text }) };
		}
    }
}

