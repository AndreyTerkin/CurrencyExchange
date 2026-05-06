using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Microsoft.Extensions.Configuration;

namespace CurrencyWorker;

public record CbrCurrencyEntry(string Code, string Name, decimal Rate);

public class CbrXmlParser(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    ILogger<CbrXmlParser> logger)
{
    private static readonly XmlSchemaSet SchemaSet = BuildSchemaSet();

    private const string Xsd = """
        <?xml version="1.0" encoding="utf-8"?>
        <xs:schema xmlns:xs="http://www.w3.org/2001/XMLSchema">
          <xs:element name="ValCurs">
            <xs:complexType>
              <xs:sequence>
                <xs:element name="Valute" minOccurs="0" maxOccurs="unbounded">
                  <xs:complexType>
                    <xs:sequence>
                      <xs:element name="NumCode" type="xs:string"/>
                      <xs:element name="CharCode" type="xs:string"/>
                      <xs:element name="Nominal" type="xs:string"/>
                      <xs:element name="Name" type="xs:string"/>
                      <xs:element name="Value" type="xs:string"/>
                      <xs:element name="VunitRate" type="xs:string" minOccurs="0"/>
                    </xs:sequence>
                    <xs:anyAttribute processContents="skip"/>
                  </xs:complexType>
                </xs:element>
              </xs:sequence>
              <xs:anyAttribute processContents="skip"/>
            </xs:complexType>
          </xs:element>
        </xs:schema>
        """;

    private static XmlSchemaSet BuildSchemaSet()
    {
        var set = new XmlSchemaSet();
        set.Add("", XmlReader.Create(new StringReader(Xsd)));
        set.Compile();
        return set;
    }

    public async Task<IReadOnlyList<CbrCurrencyEntry>> FetchRatesAsync(CancellationToken ct)
    {
        string url = configuration["Cbr:Url"]
            ?? throw new InvalidOperationException("Cbr:Url is not configured.");

        HttpClient client = httpClientFactory.CreateClient("cbr");
        string xml = await client.GetStringAsync(url, ct);

        XDocument doc = XDocument.Parse(xml);

        var validationErrors = new List<string>();
        doc.Validate(SchemaSet, (_, e) => validationErrors.Add(e.Message));
        if (validationErrors.Count > 0)
            throw new InvalidOperationException(
                $"CBR XML schema validation failed: {string.Join("; ", validationErrors)}");

        var entries = new List<CbrCurrencyEntry>();

        foreach (XElement valute in doc.Root?.Elements("Valute") ?? [])
        {
            string? code = valute.Element("CharCode")?.Value;
            string? name = valute.Element("Name")?.Value;
            string? nominalStr = valute.Element("Nominal")?.Value;
            string? valueStr = valute.Element("Value")?.Value;

            if (code is null || name is null || nominalStr is null || valueStr is null)
                continue;

            if (!int.TryParse(nominalStr, out int nominal) || nominal == 0)
                continue;

            string normalized = valueStr.Replace(',', '.');
            if (!decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value))
                continue;

            entries.Add(new CbrCurrencyEntry(code, name, value / nominal));
        }

        logger.LogInformation("Parsed {Count} currencies from CBR", entries.Count);
        return entries;
    }
}
