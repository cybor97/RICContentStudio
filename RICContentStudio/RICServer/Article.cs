using System.IO;
using System.Text;
using System.Xml;
using System.Net;

namespace RICContentStudio
{
    public class Article
	{
		public int ID{ get; set; }

		public string Title{ get; set; }

		public string Text{ get; set; }

		public string Images{ get; set; }

		public string Category{ get; set; }

		public string Tags{ get; set; }

		static WebClient Client{ get { return new WebClient (); } }

		public static Article Parse (string xml)
		{
			var result = new Article ();
			using (var reader = XmlReader.Create (new MemoryStream (Encoding.UTF8.GetBytes (xml))))
				while (!reader.EOF)
					if (reader.IsStartElement ("Article"))
					{
						int id;
						if (int.TryParse (reader.GetAttribute ("ID"), out id))
							result.ID = id;
						result.Title = reader.GetAttribute ("Title");
						result.Category = reader.GetAttribute ("Category");
						result.Tags = reader.GetAttribute ("Tags");
						reader.Read ();
					}
					else if (reader.IsStartElement ("Text"))
						result.Text = reader.ReadElementContentAsString ();
					else if (reader.IsStartElement ("Images"))
						result.Images = reader.ReadElementContentAsString ();
					else
						reader.Read ();
			return result;
		}

		public override string ToString ()
		{
			var stream = new MemoryStream ();
			using (var writer = XmlWriter.Create (stream, new XmlWriterSettings { 
				Indent = true, 
				OmitXmlDeclaration = true 
			}))
			{
				writer.WriteStartElement ("Article");
				writer.WriteAttributeString ("ID", ID.ToString ());
				writer.WriteAttributeString ("Title", Title);
				writer.WriteAttributeString ("Category", Category);
				writer.WriteAttributeString ("Tags", Tags);
				writer.WriteElementString ("Text", Text);
				writer.WriteElementString ("Images", Images);
				writer.WriteEndElement ();
			}
			return Encoding.UTF8.GetString (stream.GetBuffer ());
		}
	}
}