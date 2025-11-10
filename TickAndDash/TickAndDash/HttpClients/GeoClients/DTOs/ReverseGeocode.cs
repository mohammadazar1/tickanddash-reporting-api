using System.Collections.Generic;

namespace TickAndDash.HttpClients.GeocodingClient.DTOs
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Datasource
    {
        public string Sourcename { get; set; }
        public string Amenity { get; set; }
        public string Residential { get; set; }
        public string Osm_type { get; set; }
        public int Osm_id { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public string Addresstype { get; set; }
        public string Display_name { get; set; }
        public string Continent { get; set; }
    }

    public class Rank
    {
        public double Popularity { get; set; }
    }

    public class Properties
    {
        public string Name { get; set; }
        public Datasource Datasource { get; set; }
        public string Street { get; set; }
        public string Suburb { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string State { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
        public string Country_code { get; set; }
        public double Lon { get; set; }
        public double Lat { get; set; }
        public string Result_type { get; set; }
        public double Distance { get; set; }
        public string Formatted { get; set; }
        public string Address_line1 { get; set; }
        public string Address_line2 { get; set; }
        public Rank Rank { get; set; }
    }

    public class Geometry
    {
        public string Type { get; set; }
        public List<double> Coordinates { get; set; }
    }

    public class Feature
    {
        public string Type { get; set; }
        public Properties Properties { get; set; }
        public List<double> Bbox { get; set; }
        public Geometry Geometry { get; set; }
    }

    public class ReverseGeocode
    {
        public string Type { get; set; }
        public List<Feature> Features { get; set; }
    }

    public class RegisterUserDto
    {
        public string Name { get; set; } = "";
        public string Location { get; set; } = "Palestine";
        public string MSISDN { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int CountryId { get; set; } = 1;
        public string Email { get; set; }
        public bool CanUseBalance { get; set; } = true;
        public int FlowId { get; set; } = 3;
    }
}
