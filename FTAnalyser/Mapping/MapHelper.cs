using FTAnalyzer.Forms;
using FTAnalyzer.Utilities;
using NetTopologySuite.Geometries;
using SharpMap;
using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Forms;
using SharpMap.Layers;
using SharpMap.Rendering;
using SharpMap.Rendering.Decoration.ScaleBar;
using SharpMap.Styles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace FTAnalyzer.Mapping
{
    public class MapHelper
    {
        static MapHelper instance;
        readonly FamilyTree ft = FamilyTree.Instance;

        MapHelper()
        {
        }

        public static MapHelper Instance
        {
            get
            {
                if (instance == null)
                    instance = new MapHelper();
                return instance;
            }
        }

        public static void SetScaleBar(MapBox mapBox1)
        {
            if (Properties.MappingSettings.Default.HideScaleBar)
            {
                if (mapBox1.Map.Decorations.Count > 0)
                    mapBox1.Map.Decorations.RemoveAt(0);
            }
            else
            {
                ScaleBar scalebar = new ScaleBar
                {
                    BackgroundColor = Color.White,
                    RoundedEdges = true
                };
                mapBox1.Map.Decorations.Add(scalebar);
            }
            mapBox1.Refresh();
        }

        public static void MnuHideScaleBar_Click(ToolStripMenuItem mnuHideScaleBar, MapBox mapBox1)
        {
            Properties.MappingSettings.Default.HideScaleBar = mnuHideScaleBar.Checked;
            Properties.MappingSettings.Default.Save();
            SetScaleBar(mapBox1);
        }

        public static Envelope GetExtents(FeatureDataTable table)
        {
            Envelope bbox = new Envelope();
            Envelope empty = new Envelope();
            foreach (FeatureDataRow row in table)
            {
                foreach (Coordinate c in row.Geometry.Coordinates)
                {
                    if (c != null)
                        bbox.ExpandToInclude(c);
                }
                var x = (Envelope)row["ViewPort"];
                Console.WriteLine(x.ToString());
                if (x.MaxX == 0 && x.MaxY == 0 )
                    Console.WriteLine("we have zeos");
                else
                    bbox.ExpandToInclude(x);
            }
            Envelope expand;
            if (bbox.Centre == null)
                expand = new Envelope(-25000000, 25000000, -17000000, 17000000);
            else
            {
                expand = new Envelope(bbox.TopLeft(), bbox.BottomRight());
                expand.ExpandBy(bbox.Width * 0.1d);
            }
            return expand;
        }

        public List<MapLocation> AllMapLocations
        {
            get
            {
                List<MapLocation> result = new List<MapLocation>();
                foreach (Individual ind in FamilyTree.Instance.AllIndividuals)
                {
                    foreach (Fact f in ind.AllFacts)
                        if (f.Location.IsGeoCoded(false))
                            result.Add(new MapLocation(ind, f, f.FactDate));
                }
                return result;
            }
        }

        public static List<MapLocation> YearMapLocations(FactDate when, int limit)
        {
            List<MapLocation> result = new List<MapLocation>();
            foreach (Individual ind in FamilyTree.Instance.AllIndividuals)
            {
                if (ind.IsAlive(when) && ind.GetMaxAge(when) < FactDate.MAXYEARS)
                {
                    Fact fact = ind.BestLocationFact(when, limit);
                    FactLocation loc = fact.Location;
                    if (loc.IsGeoCoded(false))
                        result.Add(new MapLocation(ind, fact, when));
                    else
                    {
                        int startlevel = loc.Level - 1;
                        for (int level = startlevel; level > FactLocation.UNKNOWN; level--)
                        {
                            loc = loc.GetLocation(level);
                            if (loc.IsGeoCoded(false))
                            {
                                result.Add(new MapLocation(ind, fact, loc, when));
                                break;
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}
