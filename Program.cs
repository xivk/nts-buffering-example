using NetTopologySuite.Features;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTS_BufferingTest
{
    class Program
    {
        static void Main(string[] args)
        {
            NetTopologySuite.IO.GeoJsonSerializer serialize = new NetTopologySuite.IO.GeoJsonSerializer();
            var inputFile = File.OpenText("map.geojson");
            var features = serialize.Deserialize<FeatureCollection>(new JsonTextReader(inputFile));

            var bufferedFeatures = new FeatureCollection();
            foreach(var feature in features.Features)
            {
                var op = new NetTopologySuite.Operation.Buffer.BufferOp(feature.Geometry);
                var geometry = op.GetResultGeometry(0.02);

                var bufferedFeature = new Feature(geometry, new AttributesTable());

                bufferedFeatures.Add(bufferedFeature);
            }
      
            using (var outputFile = new StreamWriter(File.OpenWrite("buffered.geojson")))
            {
                serialize.Serialize(outputFile, bufferedFeatures);
            }
        }
    }
}
