using NetTopologySuite.Features;
using Newtonsoft.Json;
using System.IO;

namespace NTS_BufferingTest
{
    class Program
    {
        static void Main(string[] args)
        {
            NetTopologySuite.IO.GeoJsonSerializer serialize = new NetTopologySuite.IO.GeoJsonSerializer();
            var inputFile1 = File.OpenText("source1.geojson");
            var inputFile2 = File.OpenText("source2.geojson");
            var features1 = serialize.Deserialize<FeatureCollection>(new JsonTextReader(inputFile1));
            var features2 = serialize.Deserialize<FeatureCollection>(new JsonTextReader(inputFile2));

            var bufferedFeatures = new FeatureCollection();
            foreach(var feature in features1.Features)
            {
                var op = new NetTopologySuite.Operation.Buffer.BufferOp(feature.Geometry);
                var geometry = op.GetResultGeometry(0.02);

                var bufferedFeature = new Feature(geometry, new AttributesTable());

                bufferedFeatures.Add(bufferedFeature);
            }

            var intersections = new FeatureCollection();
            foreach(var feature in bufferedFeatures.Features)
            {
                foreach(var feature2 in features2.Features)
                {
                    var intersection = feature.Geometry.Intersection(feature2.Geometry);
                    intersections.Add(new Feature(intersection, new AttributesTable()));
                }
            }
      
            using (var outputFile = new StreamWriter(File.OpenWrite("buffered.geojson")))
            {
                serialize.Serialize(outputFile, bufferedFeatures);
            }
            using (var outputFile = new StreamWriter(File.OpenWrite("intersections.geojson")))
            {
                serialize.Serialize(outputFile, intersections);
            }
        }
    }
}
