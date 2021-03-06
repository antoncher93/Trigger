﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
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

            writer.WritePropertyName("user_id");
            writer.WriteValue(tel.UserId);

            writer.WritePropertyName("telemetry");
            writer.WriteStartArray();

            foreach (var beac in tel)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(beac.Address); // Beacon

                writer.WriteStartArray();
                foreach (var s in beac)
                {

                    writer.WriteValue(s.ToCompact(offset));
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
                            else if (string.Equals(j.Name, "user_id", StringComparison.InvariantCultureIgnoreCase))
                            {
                                result.UserId = j.Value.Value<string>();
                            }
                            else if (string.Equals(j.Name, "telemetry", StringComparison.InvariantCultureIgnoreCase))
                            {
                                foreach (JProperty bi in j.Value.Children<JObject>().Children())
                                {
                                    BeaconData beacon = BeaconData.FromAddress(bi.Name);

                                    foreach (JValue s in bi.Value)
                                    {
                                        BeaconItem item = BeaconItem.FromCompact(s.Value<long>(), offset);
                                        beacon.Add(item);
                                    }

                                    result.Add(beacon);
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
