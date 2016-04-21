using System.Text;
using System.IO;
using System.Xml;

namespace RICContentStudio
{
	public class RICRequest
	{
		public string Type { get; set; }

		public string In { get; set; }

		public string By { get; set; }

		public string Data { get; set; }

		public override string ToString ()
		{
			var stream = new MemoryStream ();
			using (var writer = XmlWriter.Create (stream))
			{
				writer.WriteStartElement ("Request");
				writer.WriteElementString ("Type", Type);
				writer.WriteElementString ("In", In);
				writer.WriteElementString ("By", By);
				if (Type == "ADD" || Type == "SET")
				{
					writer.WriteStartElement ("Data");
					writer.WriteRaw (Data.Replace(((char)0).ToString(),""));
					writer.WriteEndElement ();
				}
				else writer.WriteElementString ("Data", Data);
				writer.WriteEndElement ();
			}
			return Encoding.UTF8.GetString (stream.GetBuffer ());
		}
	}
}
