﻿// Copyright 2008 - William Dollins   
// SQL Server 2008 by William Dollins (dollins.bill@gmail.com)   
// Based on Oracle provider by Humberto Ferreira (humbertojdf@gmail.com)   
//   
// Date 2007-11-28   
//   
// This file is part of    
// is free software; you can redistribute it and/or modify   
// it under the terms of the GNU Lesser General Public License as published by   
// the Free Software Foundation; either version 2 of the License, or   
// (at your option) any later version.   
//   
// is distributed in the hope that it will be useful,   
// but WITHOUT ANY WARRANTY; without even the implied warranty of   
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the   
// GNU Lesser General Public License for more details.   
  
// You should have received a copy of the GNU Lesser General Public License   
// along with  if not, write to the Free Software   
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA    
  
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using NetTopologySuite.Geometries;


namespace SharpMap.Data.Providers
{
    /// <summary>
    /// Possible spatial object types on SqlServer
    /// </summary>
    public enum SqlServerSpatialObjectType
    {
        /// <summary>
        /// Geometry
        /// </summary>
        Geometry,

        /// <summary>
        /// Geography
        /// </summary>
        Geography,
    }

    /// <summary>   
    /// Method used to determine extents of all features
    /// </summary>
    public enum SqlServer2008ExtentsMode
    {
        /// <summary>
        /// Reads through all features in the table to determine extents
        /// </summary>
        QueryIndividualFeatures,

        /// <summary>
        /// Directly reads the bounds of the spatial index from the system tables (very fast, but does not take <see cref="SqlServer2008.DefinitionQuery"/> into account)
        /// </summary>
        SpatialIndex,

        /// <summary>
        /// Uses the EnvelopeAggregate aggregate function introduced in SQL Server 2012
        /// </summary>
        EnvelopeAggregate
    }

    /// <summary>   
    /// SQL Server 2008 data provider   
    /// </summary>   
    /// <remarks>   
    /// <para>This provider was developed against the SQL Server 2008 November CTP. The platform may change significantly before release.</para>   
    /// <example>   
    /// Adding a datasource to a layer:   
    /// <code lang="C#">   
    /// Layers.VectorLayer myLayer = new Layers.VectorLayer("My layer");   
    /// string ConnStr = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=myDB;Data Source=myServer\myInstance";   
    /// myLayer.DataSource = new Data.Providers.SqlServer2008(ConnStr, "myTable", "GeomColumn", "OidColumn");   
    /// </code>   
    /// </example>   
    /// <para>SQL Server 2008 provider by Bill Dollins (dollins.bill@gmail.com). Based on the Oracle provider written by Humberto Ferreira.</para>   
    /// </remarks>   
    [Serializable]
    public class SqlServer2008 : BaseProvider
    {
        /// <summary>   
        /// Initializes a new connection to SQL Server   
        /// </summary>   
        /// <param name="connectionStr">Connectionstring</param>   
        /// <param name="tablename">Name of data table</param>   
        /// <param name="geometryColumnName">Name of geometry column</param>   
        /// <param name="oidColumnName">Name of column with unique identifier</param>   
        public SqlServer2008(string connectionStr, string tablename, string geometryColumnName, string oidColumnName)
            : this(connectionStr, tablename, geometryColumnName, oidColumnName, SqlServerSpatialObjectType.Geometry)
        {
        }

        /// <summary>   
        /// Initializes a new connection to SQL Server   
        /// </summary>   
        /// <param name="connectionStr">Connectionstring</param>   
        /// <param name="tablename">Name of data table</param>   
        /// <param name="geometryColumnName">Name of geometry column</param>   
        /// <param name="oidColumnName">Name of column with unique identifier</param>   
        /// <param name="spatialObjectType">The type of the spatial object to use for spatial queries</param>
        public SqlServer2008(string connectionStr, string tablename, string geometryColumnName, string oidColumnName,
            SqlServerSpatialObjectType spatialObjectType)
            : this(connectionStr, tablename, geometryColumnName, oidColumnName, spatialObjectType, false)
        {
        }

        /// <summary>   
        /// Initializes a new connection to SQL Server   
        /// </summary>   
        /// <param name="connectionStr">Connectionstring</param>   
        /// <param name="tablename">Name of data table</param>   
        /// <param name="geometryColumnName">Name of geometry column</param>   
        /// <param name="oidColumnName">Name of column with unique identifier</param>   
        /// <param name="spatialObjectType">The type of the spatial object to use for spatial queries</param>
        /// <param name="useSpatialIndexExtentAsExtent">If true, the bounds of the spatial index is used for the GetExtents() method which heavily increases performance instead of reading through all features in the table</param>
        public SqlServer2008(string connectionStr, string tablename, string geometryColumnName, string oidColumnName,
            SqlServerSpatialObjectType spatialObjectType, bool useSpatialIndexExtentAsExtent)
            : this(
                connectionStr, tablename, geometryColumnName, oidColumnName, spatialObjectType,
                useSpatialIndexExtentAsExtent, 0)
        {
        }

        /// <summary>   
        /// Initializes a new connection to SQL Server   
        /// </summary>   
        /// <param name="connectionStr">Connectionstring</param>   
        /// <param name="tablename">Name of data table</param>   
        /// <param name="geometryColumnName">Name of geometry column</param>   
        /// <param name="oidColumnName">Name of column with unique identifier</param>   
        /// <param name="spatialObjectType">The type of the spatial object to use for spatial queries</param>
        /// <param name="useSpatialIndexExtentAsExtent">If true, the bounds of the spatial index is used for the GetExtents() method which heavily increases performance instead of reading through all features in the table</param>
        /// <param name="srid">The spatial reference id</param>
        public SqlServer2008(string connectionStr, string tablename, string geometryColumnName, string oidColumnName,
            SqlServerSpatialObjectType spatialObjectType, bool useSpatialIndexExtentAsExtent, int srid)
        {
            ConnectionString = connectionStr;

            ParseTablename(tablename);

            GeometryColumn = geometryColumnName;
            ObjectIdColumn = oidColumnName;
            _spatialObjectType = spatialObjectType;
            switch (spatialObjectType)
            {
                case SqlServerSpatialObjectType.Geometry:
                    _spatialObject = "geometry";
                    break;
                //case SqlServerSpatialObjectType.Geography:
                default:
                    _spatialObject = "geography";
                    break;
            }

            _extentsMode = (useSpatialIndexExtentAsExtent
                ? SqlServer2008ExtentsMode.SpatialIndex
                : SqlServer2008ExtentsMode.QueryIndividualFeatures);

            SRID = srid;
        }

        /// <summary>
        /// Method to parse TableSchema and Table from a (fully qualified) tablename
        /// </summary>
        /// <param name="tablename">The table name</param>
        protected void ParseTablename(string tablename)
        {
            bool open = false;
            var sb = new StringBuilder(tablename.Length);
            var lc = char.MinValue;

            foreach (var c in tablename)
            {
                switch (c)
                {
                    case '[':
                        if (open)
                            throw new ArgumentException("tablename");
                        open = true;
                        break;
                    case ']':
                        if (!open)
                            throw new ArgumentException("tablename");
                        open = false;
                        break;
                    case '.':
                        if (lc == char.MinValue)
                            throw new ArgumentException("tablename");
                        if (open)
                            sb.Append(c);
                        else
                        {
                            if (string.IsNullOrEmpty(TableSchema))
                                TableSchema = sb.ToString();
                            else
                                TableSchema += "." + sb;
                            sb.Clear();

                        }
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
                lc = c;
            }

            if (open)
                throw new ArgumentException("tablename");

            Table = sb.ToString();
        }

        /// <summary>   
        /// Initializes a new connection to SQL Server   
        /// </summary>   
        /// <param name="connectionStr">Connectionstring</param>   
        /// <param name="tablename">Name of data table</param>   
        /// <param name="oidColumnName">Name of column with unique identifier</param>   
        public SqlServer2008(string connectionStr, string tablename, string oidColumnName)
            : this(connectionStr, tablename, "shape", oidColumnName, SqlServerSpatialObjectType.Geometry)
        {
        }

        /// <summary>   
        /// Initializes a new connection to SQL Server   
        /// </summary>   
        /// <param name="connectionStr">Connectionstring</param>   
        /// <param name="tablename">Name of data table</param>   
        /// <param name="oidColumnName">Name of column with unique identifier</param>
        /// <param name="spatialObjectType">The type of the spatial object to use for spatial queries</param>
        public SqlServer2008(string connectionStr, string tablename, string oidColumnName,
            SqlServerSpatialObjectType spatialObjectType)
            : this(connectionStr, tablename, "shape", oidColumnName, spatialObjectType)
        {
        }

        private SqlServer2008ExtentsMode _extentsMode;

        /// <summary>
        /// Gets or sets the method used in the <see cref="GetExtents"/> method.
        /// </summary>
        public SqlServer2008ExtentsMode ExtentsMode
        {
            get { return _extentsMode; }
            set { _extentsMode = value; }
        }

        /// <summary>   
        /// Connectionstring   
        /// </summary>   
        public string ConnectionString
        {
            get { return ConnectionID; }
            set { ConnectionID = value; }
        }

        private string _table;

        /// <summary>   
        /// Data table name   
        /// </summary>   
        public string Table
        {
            get { return _table; }
            set { _table = value; }
        }

        private string _schema;

        /// <summary>   
        /// Data table schema   
        /// </summary>   
        public string TableSchema
        {
            get { return _schema; }
            set { _schema = value; }
        }

        /// <summary>
        /// Gets a value indicating the qualified schema table name in square brackets
        /// </summary>
        protected string QualifiedTable
        {
            get
            {
                var sb = new StringBuilder();
                if (string.IsNullOrEmpty(TableSchema))
                    sb.AppendFormat("[{0}].", TableSchema);
                sb.AppendFormat("[{0}]", Table);
                return sb.ToString();
            }
        }

        private string _geometryColumn;

        /// <summary>   
        /// Name of geometry column   
        /// </summary>   
        public string GeometryColumn
        {
            get { return _geometryColumn; }
            set { _geometryColumn = value; }
        }

        private string _objectIdColumn;

        /// <summary>   
        /// Name of column that contains the Object ID   
        /// </summary>   
        public string ObjectIdColumn
        {
            get { return _objectIdColumn; }
            set { _objectIdColumn = value; }
        }

        private bool _makeValid;

        /// <summary>
        /// Gets/Sets whether all <see cref="NetTopologySuite.Geometries"/> passed to SqlServer2008 should be made valid using this function.
        /// </summary>
        public Boolean ValidateGeometries
        {
            get { return _makeValid; }
            set { _makeValid = value; }
        }

        private String MakeValidString
        {
            get { return _makeValid ? ".MakeValid()" : String.Empty; }
        }

        private readonly SqlServerSpatialObjectType _spatialObjectType;
        private readonly string _spatialObject;

        /// <summary>
        /// Spatial object type for  
        /// </summary>
        public SqlServerSpatialObjectType SpatialObjectType
        {
            get { return _spatialObjectType; }
        }


        private int _maxDop;

        /// <summary>
        /// If set, sends an Option MaxDop to the SQL-Server to override the Parallel Execution of indexes
        /// This can be used if Spatial indexes are not used on SQL-Servers with many processors.
        /// 
        /// MaxDop = 0 // Default behaviour
        /// MaxDop = 1 // Suppress Parallel execution of Queryplan
        /// MaxDop = [2..n] // Use X cores in in execution plan
        /// </summary>
        public int MaxDop
        {
            get { return _maxDop; }
            set { _maxDop = value; }
        }

        /// <summary>
        /// Function to transform <see cref="MaxDop"/> to sql for the query
        /// </summary>
        /// <returns>MAXDOP option striong</returns>
        protected string GetExtraOptions()
        {
            if (_maxDop != 0)
            {
                return "OPTION (MAXDOP " + _maxDop + ")";
            }
            else
            {
                return null;
            }
        }


        /// <summary>   
        /// Returns geometries within the specified bounding box   
        /// </summary>   
        /// <param name="bbox"></param>   
        /// <returns></returns>   
        public override Collection<Geometry> GetGeometriesInView(Envelope bbox)
        {
            var features = new Collection<Geometry>();
            using (var conn = new SqlConnection(ConnectionString))
            {
                //Get bounding box string   
                string strBbox = GetBoxFilterStr(bbox);

                string strSql = "SELECT g." + GeometryColumn + ".STAsBinary() ";
                strSql += " FROM " + QualifiedTable + " g " + BuildTableHints() + " WHERE ";

                if (!string.IsNullOrEmpty(_definitionQuery))
                    strSql += DefinitionQuery + " AND ";

                strSql += strBbox;

                string extraOptions = GetExtraOptions();
                if (!string.IsNullOrEmpty(extraOptions))
                    strSql += " " + extraOptions;

                using (var command = new SqlCommand(strSql, conn))
                {
                    conn.Open();
                    using (SqlDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            if (dr[0] != DBNull.Value)
                            {
                                var geom = Converters.WellKnownBinary.GeometryFromWKB.Parse((byte[]) dr[0], Factory);
                                if (geom != null)
                                {
                                    if (_spatialObjectType == SqlServerSpatialObjectType.Geography) FlipXY(geom);
                                    features.Add(geom);
                                }
                            }
                        }
                    }
                    conn.Close();
                }
            }
            return features;
        }

        /// <summary>   
        /// Returns the geometry corresponding to the Object ID   
        /// </summary>   
        /// <param name="oid">Object ID</param>   
        /// <returns>geometry</returns>   
        public override Geometry GetGeometryByID(uint oid)
        {
            Geometry geom = null;
            using (var conn = new SqlConnection(ConnectionString))
            {
                string strSql = "SELECT g." + GeometryColumn + ".STAsBinary() FROM " + QualifiedTable + " g WHERE " +
                                ObjectIdColumn + "='" + oid + "'";
                conn.Open();
                using (var command = new SqlCommand(strSql, conn))
                {
                    using (SqlDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            if (dr[0] != DBNull.Value)
                            {
                                geom = Converters.WellKnownBinary.GeometryFromWKB.Parse((byte[]) dr[0], Factory);
                                if (_spatialObjectType == SqlServerSpatialObjectType.Geography) FlipXY(geom);
                            }
                        }
                    }
                }
                conn.Close();
            }
            return geom;
        }

        private static void FlipXY(Geometry geom)
        {
            var coords = geom.Coordinates;
            for (var i = 0; i < coords.Length; i++)
            {
                var x = coords[i].X;
                coords[i].X = coords[i].Y;
                coords[i].Y = x;
            }
            geom.GeometryChanged();
        }

        /// <summary>   
        /// Returns geometry Object IDs whose bounding box intersects 'bbox'   
        /// </summary>   
        /// <param name="bbox"></param>   
        /// <returns></returns>   
        public override Collection<uint> GetObjectIDsInView(Envelope bbox)
        {
            var objectlist = new Collection<uint>();
            using (var conn = new SqlConnection(ConnectionString))
            {
                //Get bounding box string   
                var strBbox = GetBoxFilterStr(bbox);

                string strSql = "SELECT g." + ObjectIdColumn + " ";
                strSql += "FROM " + QualifiedTable + " g " + BuildTableHints() + " WHERE ";

                if (!string.IsNullOrEmpty(_definitionQuery))
                    strSql += DefinitionQuery + " AND ";

                strSql += strBbox;

                string extraOptions = GetExtraOptions();
                if (!string.IsNullOrEmpty(extraOptions))
                    strSql += " " + extraOptions;


                using (var command = new SqlCommand(strSql, conn))
                {
                    conn.Open();
                    using (var dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            if (dr[0] != DBNull.Value)
                            {
                                uint id = Convert.ToUInt32(dr[0]);
                                objectlist.Add(id);
                            }
                        }
                    }
                    conn.Close();
                }
            }
            return objectlist;
        }

        /// <summary>   
        /// Returns the box filter string needed in SQL query   
        /// </summary>   
        /// <param name="bbox"></param>   
        /// <returns></returns>   
        protected string GetBoxFilterStr(Envelope bbox)
        {
            //geography::STGeomFromText('LINESTRING(47.656 -122.360, 47.656 -122.343)', 4326);   
            if (_spatialObjectType == SqlServerSpatialObjectType.Geography)
                bbox = new Envelope(bbox.MinY, bbox.MaxY, bbox.MinX, bbox.MaxY);
            var bboxText = Converters.WellKnownText.GeometryToWKT.Write(Factory.ToGeometry(bbox)); // "";   
            //string whereClause = GeometryColumn + ".STIntersects(geometry::STGeomFromText('" + bboxText + "', " + SRID + ")" + MakeValidString + ") = 1";   
            string whereClause = String.Format("{0}{1}.STIntersects({4}::STGeomFromText('{2}', {3})) = 1",
                GeometryColumn, MakeValidString, bboxText, SRID, _spatialObject);
            return whereClause; // strBbox;   
        }

        /// <summary>   
        /// Returns the features that intersects with 'geom'   
        /// </summary>   
        /// <param name="geom"></param>   
        /// <param name="ds">FeatureDataSet to fill data into</param>   
        protected override void OnExecuteIntersectionQuery(Geometry geom, FeatureDataSet ds)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string strGeom = _spatialObject + "::STGeomFromText('" + geom.AsText() + "', #SRID#)";

                strGeom = strGeom.Replace("#SRID#", SRID > 0 ? SRID.ToString(CultureInfo.InvariantCulture) : "0");
                strGeom = GeometryColumn + ".STIntersects(" + strGeom + ") = 1";

                string strSql = "SELECT g.* , g." + GeometryColumn + ".STAsBinary() As sharpmap_tempgeometry FROM " +
                                QualifiedTable + " g " + BuildTableHints() + " WHERE ";

                if (!string.IsNullOrEmpty(_definitionQuery))
                    strSql += DefinitionQuery + " AND ";

                strSql += strGeom;

                string extraOptions = GetExtraOptions();
                if (!string.IsNullOrEmpty(extraOptions))
                    strSql += " " + extraOptions;


                using (var adapter = new SqlDataAdapter(strSql, conn))
                {
                    conn.Open();
                    adapter.Fill(ds);
                    conn.Close();
                    if (ds.Tables.Count > 0)
                    {
                        var fdt = new FeatureDataTable(ds.Tables[0]);
                        foreach (System.Data.DataColumn col in ds.Tables[0].Columns)
                            if (col.ColumnName != GeometryColumn && col.ColumnName != "sharpmap_tempgeometry")
                                fdt.Columns.Add(col.ColumnName, col.DataType, col.Expression);
                        foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
                        {
                            FeatureDataRow fdr = fdt.NewRow();
                            foreach (System.Data.DataColumn col in ds.Tables[0].Columns)
                                if (col.ColumnName != GeometryColumn && col.ColumnName != "sharpmap_tempgeometry")
                                    fdr[col.ColumnName] = dr[col];
                            var tmpGeom =
                                Converters.WellKnownBinary.GeometryFromWKB.Parse((byte[]) dr["sharpmap_tempgeometry"],
                                    Factory);
                            if (tmpGeom != null && _spatialObjectType == SqlServerSpatialObjectType.Geography)
                            {
                                FlipXY(tmpGeom);
                                tmpGeom.GeometryChanged();
                            }
                            fdr.Geometry = tmpGeom;
                            fdt.AddRow(fdr);
                        }
                        ds.Tables.Add(fdt);
                    }
                }
            }
        }

        /*
       /// <summary>   
       /// Convert WellKnownText to linestrings   
       /// </summary>   
       /// <param name="wkt"></param>   
       /// <returns></returns>   
       private LineString WktToLineString(string wkt)   
       {   
           LineString line = new LineString();   
           wkt = wkt.Substring(wkt.LastIndexOf('(') + 1).Split(')')[0];   
           string[] strPoints = wkt.Split(',');   
           foreach (string strPoint in strPoints)   
           {   
               string[] coord = strPoint.Split(' ');   
               line.Vertices.Add(new Point(double.Parse(coord[0], Map.NumberFormatEnUs), double.Parse(coord[1], Map.NumberFormatEnUs)));   
           }   
           return line;   
       }
        */

        /// <summary>   
        /// Returns the number of features in the dataset   
        /// </summary>   
        /// <returns>number of features</returns>   
        public override int GetFeatureCount()
        {
            int count;
            using (var conn = new SqlConnection(ConnectionString))
            {
                var strSql = "SELECT COUNT(*) FROM " + QualifiedTable;
                if (!string.IsNullOrEmpty(_definitionQuery))
                    strSql += " WHERE " + DefinitionQuery;
                using (var command = new SqlCommand(strSql, conn))
                {
                    conn.Open();
                    count = (int) command.ExecuteScalar();
                    conn.Close();
                }
            }
            return count;
        }

        #region IProvider Members   

        private string _definitionQuery;

        /// <summary>   
        /// Definition query used for limiting dataset   
        /// </summary>   
        public string DefinitionQuery
        {
            get { return _definitionQuery; }
            set { _definitionQuery = value; }
        }

        /// <summary>   
        /// Gets a collection of columns in the dataset   
        /// </summary>   
        public System.Data.DataColumnCollection Columns
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>   
        /// Returns a datarow based on a RowID   
        /// </summary>   
        /// <param name="rowId"></param>   
        /// <returns>datarow</returns>   
        public override FeatureDataRow GetFeature(uint rowId)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string strSql = "select g.* , g." + GeometryColumn + ".STAsBinary() As sharpmap_tempgeometry from " +
                                QualifiedTable + " g WHERE " + ObjectIdColumn + "=" + rowId + "";
                using (var adapter = new SqlDataAdapter(strSql, conn))
                {
                    var ds = new System.Data.DataSet();
                    conn.Open();
                    adapter.Fill(ds);
                    conn.Close();
                    if (ds.Tables.Count > 0)
                    {
                        var fdt = new FeatureDataTable(ds.Tables[0]);
                        foreach (System.Data.DataColumn col in ds.Tables[0].Columns)
                            if (col.ColumnName != GeometryColumn && col.ColumnName != "sharpmap_tempgeometry")
                                fdt.Columns.Add(col.ColumnName, col.DataType, col.Expression);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            System.Data.DataRow dr = ds.Tables[0].Rows[0];
                            FeatureDataRow fdr = fdt.NewRow();
                            foreach (System.Data.DataColumn col in ds.Tables[0].Columns)
                                if (col.ColumnName != GeometryColumn && col.ColumnName != "sharpmap_tempgeometry")
                                    fdr[col.ColumnName] = dr[col];
                            var tmpGeom =
                                Converters.WellKnownBinary.GeometryFromWKB.Parse((byte[]) dr["sharpmap_tempgeometry"],
                                    Factory);
                            if (tmpGeom != null && _spatialObjectType == SqlServerSpatialObjectType.Geography)
                            {
                                FlipXY(tmpGeom);
                                tmpGeom.GeometryChanged();
                            }
                            fdr.Geometry = tmpGeom;
                            return fdr;
                        }
                        return null;
                    }
                    return null;
                }
            }
        }

        /// <summary>   
        /// Boundingbox of dataset   
        /// </summary>   
        /// <returns>boundingbox</returns>   
        public override Envelope GetExtents()
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string sql;
                switch (_extentsMode)
                {
                    case SqlServer2008ExtentsMode.SpatialIndex:
                        sql =
                            "select bounding_box_xmin,bounding_box_xmax,bounding_box_ymin,bounding_box_ymax from sys.spatial_index_tessellations where object_id  = (select object_id from sys.tables where name = '" +
                            _table + "' and type_desc = 'USER_TABLE')";

                        using (var command = new SqlCommand(sql, conn))
                        {
                            //Geometry geom = null;   
                            using (var dr = command.ExecuteReader())
                            {
                                if (dr.Read())
                                {
                                    return new Envelope(
                                        Convert.ToDouble(dr["bounding_box_xmin"]),
                                        Convert.ToDouble(dr["bounding_box_xmax"]),
                                        Convert.ToDouble(dr["bounding_box_ymin"]),
                                        Convert.ToDouble(dr["bounding_box_ymax"]));
                                }
                            }
                        }
                        break;

                    case SqlServer2008ExtentsMode.QueryIndividualFeatures:

                        if (_spatialObjectType == SqlServerSpatialObjectType.Geography)
                        {
                            // The geography datatype does not have the STEnvelope method. If using SQL2012, EnvelopeAggregate provides an alternative
                            throw new NotSupportedException("STEnvelope does not work with geography!");
                        }

                        //string strSQL = "SELECT g." + GeometryColumn + ".STEnvelope().STAsText() FROM " + Table + " g ";   
                        sql = String.Format("SELECT g.{0}{1}.STEnvelope().STAsText() FROM {2} g ",
                            GeometryColumn, MakeValidString, QualifiedTable);

                        if (!string.IsNullOrEmpty(_definitionQuery))
                            sql += " WHERE " + DefinitionQuery;

                        using (var command = new SqlCommand(sql, conn))
                        {
                            var bx = new Envelope();
                            using (var dr = command.ExecuteReader())
                            {
                                while (dr.Read())
                                {
                                    var wkt = dr.GetString(0); //[GeometryColumn];   
                                    var g = Converters.WellKnownText.GeometryFromWKT.Parse(wkt);
                                    bx.ExpandToInclude(g.EnvelopeInternal);
                                }
                            }
                            return bx;
                        }

                    case SqlServer2008ExtentsMode.EnvelopeAggregate:
                        sql = String.Format("SELECT {3}::EnvelopeAggregate(g.{0}{1}).STAsText() FROM {2} g ",
                            GeometryColumn, MakeValidString, QualifiedTable, _spatialObject);

                        if (!string.IsNullOrEmpty(_definitionQuery))
                            sql += " WHERE " + DefinitionQuery;
                        using (var command = new SqlCommand(sql, conn))
                        {
                            using (var dr = command.ExecuteReader())
                            {
                                if (dr.Read())
                                {
                                    var wkt = dr.GetString(0);
                                    var g = Converters.WellKnownText.GeometryFromWKT.Parse(wkt);
                                    if (_spatialObjectType == SqlServerSpatialObjectType.Geography)
                                    {
                                        FlipXY(g);
                                        g.GeometryChanged();
                                    }
                                    return g.EnvelopeInternal;
                                }
                            }
                        }
                        break;
                }
            }
            throw new InvalidOperationException();
        }

        #endregion

        #region IProvider Members   

        /// <summary>   
        /// Returns all features with the view box   
        /// </summary>   
        /// <param name="bbox">view box</param>   
        /// <param name="ds">FeatureDataSet to fill data into</param>   
        public override void ExecuteIntersectionQuery(Envelope bbox, FeatureDataSet ds)
        {
            //List<Geometry> features = new List<Geometry>();   
            using (var conn = new SqlConnection(ConnectionString))
            {
                //Get bounding box string   
                string strBbox = GetBoxFilterStr(bbox);

                //string strSQL = "SELECT g.*, g." + GeometryColumn + ".STAsBinary() AS sharpmap_tempgeometry ";   
                string strSql = String.Format(
                    "SELECT g.*, g.{0}{1}.STAsBinary() AS sharpmap_tempgeometry FROM {2} g {3} WHERE ",
                    GeometryColumn, MakeValidString, QualifiedTable, BuildTableHints());

                if (!string.IsNullOrEmpty(_definitionQuery))
                    strSql += DefinitionQuery + " AND ";

                strSql += strBbox;

                string extraOptions = GetExtraOptions();
                if (!string.IsNullOrEmpty(extraOptions))
                    strSql += " " + extraOptions;


                using (var adapter = new SqlDataAdapter(strSql, conn))
                {
                    conn.Open();
                    var ds2 = new System.Data.DataSet();
                    adapter.Fill(ds2);
                    conn.Close();
                    if (ds2.Tables.Count > 0)
                    {
                        var fdt = new FeatureDataTable(ds2.Tables[0]);
                        foreach (System.Data.DataColumn col in ds2.Tables[0].Columns)
                            if (col.ColumnName != GeometryColumn && col.ColumnName != "sharpmap_tempgeometry")
                                fdt.Columns.Add(col.ColumnName, col.DataType, col.Expression);
                        foreach (System.Data.DataRow dr in ds2.Tables[0].Rows)
                        {
                            FeatureDataRow fdr = fdt.NewRow();
                            foreach (System.Data.DataColumn col in ds2.Tables[0].Columns)
                                if (col.ColumnName != GeometryColumn && col.ColumnName != "sharpmap_tempgeometry")
                                    fdr[col.ColumnName] = dr[col];
                            fdr.Geometry =
                                Converters.WellKnownBinary.GeometryFromWKB.Parse((byte[]) dr["sharpmap_tempgeometry"],
                                    Factory);
                            fdt.AddRow(fdr);
                        }
                        ds.Tables.Add(fdt);
                    }
                }
            }
        }

        #endregion

        private bool _forceSeekHint;

        /// <summary>
        /// When <code>true</code>, uses the FORCESEEK table hint.
        /// </summary>   
        public bool ForceSeekHint
        {
            get { return _forceSeekHint; }
            set { _forceSeekHint = value; }
        }

        private bool _noLockHint;

        /// <summary>
        /// When <code>true</code>, uses the NOLOCK table hint.
        /// </summary>   
        public bool NoLockHint
        {
            get { return _noLockHint; }
            set { _noLockHint = value; }
        }

        private string _forceIndex;

        /// <summary>
        /// When set, forces use of the specified index
        /// </summary>   
        public string ForceIndex
        {
            get { return _forceIndex; }
            set { _forceIndex = value; }
        }

        /// <summary>
        /// Builds the WITH clause containing all specified table hints
        /// </summary>
        /// <returns>The WITH clause</returns>
        protected string BuildTableHints()
        {
            if (ForceSeekHint || NoLockHint || !string.IsNullOrEmpty(ForceIndex))
            {
                var hints = new List<string>(3);
                if (!string.IsNullOrEmpty(ForceIndex))
                {
                    hints.Add("INDEX(" + ForceIndex + ")");
                }
                if (NoLockHint)
                {
                    hints.Add("NOLOCK");
                }
                if (ForceSeekHint)
                {
                    hints.Add("FORCESEEK");
                }
                return "WITH (" + string.Join(",", hints.ToArray()) + ")";
            }
            return string.Empty;
        }
    }
}