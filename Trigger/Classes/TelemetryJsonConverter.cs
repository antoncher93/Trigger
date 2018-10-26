using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Trigger.Beacons;
using Trigger.Classes.Beacons;
using Trigger.Signal;

namespace Trigger.Classes
{
    public class TelemetryJsonConverter : JsonConverter<Telemetry>
    {
        public override void WriteJson(JsonWriter writer, Telemetry tel, JsonSerializer serializer)
        {
            DateTime offset = tel.MinDateTime;

            writer.Formatting = Formatting.Indented;

            writer.WriteStartObject();

            writer.WritePropertyName("type");
            writer.WriteValue(tel.Type);

            writer.WritePropertyName("data");
            writer.WriteStartObject();


            writer.WritePropertyName("timeoffset");
            writer.WriteValue(offset.Ticks);

            writer.WritePropertyName("telemetry");
            writer.WriteStartArray();

            foreach (var a in tel)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(a.AccessPointUid); // Access point

                writer.WriteStartArray();
                foreach (var b in a.Beacons)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName(b.Address); // Beacon mac address

                    writer.WriteStartArray();
                    foreach (var bi in b)
                    {
                        writer.WriteValue(bi.ToCompact(offset));
                        //writer.WriteStartObject();
                        //writer.WritePropertyName(bi.Time.ToString());
                        //writer.WriteValue(Math.Abs(bi.Rssi));
                        //writer.WriteEndObject();
                    }
                    writer.WriteEndArray();

                    writer.WriteEndObject();
                }
                writer.WriteEndArray();

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        public override Telemetry ReadJson(JsonReader reader, Type objectType, Telemetry existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject pobjrop = JObject.Load(reader);

            Telemetry result = new Telemetry();
            DateTime offset = new DateTime();

            foreach (JProperty jo in pobjrop.Children<JProperty>())
            {
                if (jo.Type == JTokenType.Property)
                {
                    if (string.Equals(jo.Name, "type", StringComparison.InvariantCultureIgnoreCase))
                    {
                        result.Type = (Enums.TelemetryType)jo.Value.Value<int>();
                    }
                    else if (string.Equals(jo.Name, "data", StringComparison.InvariantCultureIgnoreCase))
                    {
                        foreach (JProperty j in jo.Value.Children<JProperty>())
                        {
                            if (string.Equals(j.Name, "timeoffset", StringComparison.InvariantCultureIgnoreCase))
                            {
                                offset = new DateTime().AddTicks(j.Value.Value<long>());
                            }
                            else if (string.Equals(j.Name, "telemetry", StringComparison.InvariantCultureIgnoreCase))
                            {
                                foreach (JProperty ap in j.Value.Children<JObject>().Children())
                                {
                                    AccessPointData point = new AccessPointData { AccessPointUid = ap.Name };

                                    foreach (JProperty b in ap.Value.Children<JObject>().Children())
                                    {
                                        BeaconData beacon = new BeaconData { Address = b.Name };

                                        foreach (JToken bi in b.Value.Children<JToken>())
                                        {
                                            BeaconItem item = BeaconItem.FromCompact(bi.Value<long>(), offset);
                                            beacon.Add(item);
                                        }

                                        point.Beacons.Add(beacon);
                                    }

                                    result.Add(point);
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
