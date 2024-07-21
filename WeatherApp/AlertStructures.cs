using System.Text.Json.Serialization;

namespace WeatherApp;

public struct Feature
{
	public string id, type;
	public Geometry geometry;
	public Properties properties;
}

public struct Geometry
{
	public string type;
	public double[][][] coordinates;
}

public struct Properties
{
	[JsonPropertyName("@id")]
	public string atId;
	[JsonPropertyName("@type")]
	public string
		atType,
		id,
		areaDesc;
	public Geocode geocode;
	public string[] affectedZones;
	public Reference[] refrences;
	public string
		sent,
		effective,
		onset,
		expires,
		ends,
		status,
		messageType,
		category,
		severity,
		certainty,
		urgency;
	[JsonPropertyName("event")]
	public string _event;
	public string
		sender,
		senderName,
		headline,
		description,
		instruction,
		response;
	public Parameters parameters;
}

public struct Geocode
{
	public string[] SAME, UGC;
}

public struct Reference
{
	[JsonPropertyName("@id")]
	public string atId;
	public string
		identifier,
		sender,
		sent;
}

public struct Parameters
{
	public string[]
		AWIPSidentifier,
		WMOidentifier,
		NWSheadline,
		BLOCKCHANNEL;
	[JsonPropertyName("EAS-ORG")]
	public string[] EAS_ORG;
	public string[]
		VTEC,
		eventEndingTime,
		expiredReferences;
}