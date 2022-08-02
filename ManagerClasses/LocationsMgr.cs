using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Oracle.DataAccess.Client;
using System.Data;


namespace Proof3.Core.ManagerClasses
{
    public class RegionsMgr
    {
        private readonly Proof3Entities db = new Proof3Entities();

        public RegionsMgr() { }

        public IList<string> GetFMACitiesByRegion(string regionName)
        {
            System.Text.StringBuilder SQLQuery = new System.Text.StringBuilder();

            List<string> itemsList = new List<string>();

            if (regionName == "undefined")
            {
                return itemsList;
            }

            switch (regionName)
            {
                case "WNE":
                    {
                        SQLQuery.Append("SELECT DISTINCT (GEO_LEVEL3) FROM ACP.CSG_MAPPING ");
                        SQLQuery.Append("WHERE GEO_LEVEL3 IS NOT NULL AND GEO_LEVEL1 = " + "\'" + "WNE" + "\'");
                        SQLQuery.Append(" ORDER BY GEO_LEVEL3");
                        break;
                    }
                case "GBR":
                    {
                        SQLQuery.Append("SELECT DISTINCT (GEO_LEVEL3) FROM ACP.CSG_MAPPING ");
                        SQLQuery.Append("WHERE GEO_LEVEL3 IS NOT NULL AND GEO_LEVEL1 = " + "\'" + "GBR" + "\'");
                        SQLQuery.Append(" ORDER BY GEO_LEVEL3");
                        break;
                    }
                case "Freedom":
                    {
                        SQLQuery.Append("SELECT DISTINCT (GEO_LEVEL3) FROM FRE.CSG_MAPPING ");
                        SQLQuery.Append("WHERE GEO_LEVEL3 IS NOT NULL AND GEO_LEVEL1 = " + "\'" + "FREEDOM" + "\'");
                        SQLQuery.Append(" ORDER BY GEO_LEVEL3");
                        break;
                    }
                case "KEYSTONE":
                case "Keystone":
                    {
                        SQLQuery.Append("SELECT DISTINCT (AREA_FMA) FROM PIT.CSG_MAPPING ");
                        SQLQuery.Append("WHERE AREA_FMA IS NOT NULL AND REGION = " + "\'" + "KEYSTONE" + "\'");
                        SQLQuery.Append(" ORDER BY AREA_FMA");
                        break;
                    }
                case "Beltway":
                    {
                        SQLQuery.Append("SELECT DISTINCT (GEO_LEVEL3) FROM BLT.CSG_MAPPING ");
                        SQLQuery.Append("WHERE GEO_LEVEL3 IS NOT NULL AND GEO_LEVEL1 = " + "\'" + "BELTWAY" + "\'");
                        SQLQuery.Append(" ORDER BY GEO_LEVEL3");
                        break;
                    }
            }

            string queryString = SQLQuery.ToString();

            try
            {
                OracleConnection conn = new OracleConnection(ConfigurationManager.AppSettings["OracleConn1"].ToString());
                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = queryString;
                cmd.CommandType = CommandType.Text;
                OracleDataReader dr = cmd.ExecuteReader();
                dr.Read();

                if (dr.HasRows)
                {
                    do
                    {
                        itemsList.Add(dr.GetString(0));

                    } while (dr.Read());
                }

                conn.Dispose();
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Error fetching FMAs by Region");
            }

            return itemsList;
        }

        public string GetFMAByRegionStateCity(string regionCSGName, string stateName, string cityName)
        {
            string FmaName = string.Empty;

            System.Text.StringBuilder SQLQuery = new System.Text.StringBuilder();

            switch (regionCSGName.ToUpperInvariant())
            {
                case "WNE":
                case "GBR":
                    {   // GEO_LEVEL3
                        SQLQuery.Append("SELECT DISTINCT (GEO_LEVEL3) FROM ACP.CSG_MAPPING ");
                        SQLQuery.Append("WHERE AGENT_NAME = :city AND STATE = :state");
                        break;
                    }
                //case "FRE":
                case "FREEDOM":
                    {
                        SQLQuery.Append("SELECT DISTINCT (GEO_LEVEL3) FROM FRE.CSG_MAPPING ");
                        SQLQuery.Append("WHERE AGENT_NAME = :city AND STATE = :state");
                        break;
                    }
                //case "KEY":
                case "KEYSTONE":
                    {
                        SQLQuery.Append("SELECT DISTINCT (AREA_FMA) FROM PIT.CSG_MAPPING ");
                        SQLQuery.Append("WHERE AGENT_NAME = :city AND STATE = :state");
                        break;
                    }
                //case "BLT":
                case "BELTWAY":
                    {
                        SQLQuery.Append("SELECT DISTINCT (GEO_LEVEL3) FROM BLT.CSG_MAPPING ");
                        SQLQuery.Append("WHERE AGENT_NAME = :city AND STATE = :state");
                        break;
                    }
            }

            string queryString = SQLQuery.ToString();

            try
            {
                OracleConnection conn = new OracleConnection(ConfigurationManager.AppSettings["OracleConn1"].ToString());
                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Parameters.Add(new OracleParameter(":city", cityName));
                cmd.Parameters.Add(new OracleParameter(":state", stateName));
                cmd.Connection = conn;
                cmd.CommandText = queryString;
                cmd.CommandType = CommandType.Text;

                FmaName = (string)cmd.ExecuteScalar();
                conn.Dispose();

                return FmaName;

            }
            catch (Exception)
            {
                throw new InvalidOperationException("Error fetching FMA's by City");
            }
        }

        public IList<string> GetCitiesByStatesAndRegionID(string state, string regionID)
        {

            System.Text.StringBuilder SQLQuery = new System.Text.StringBuilder();

            RegionsMgr regionsManager = new RegionsMgr();

            string regionCSGName = regionsManager.GetRegion(Int32.Parse(regionID)).CSG_Name.ToUpperInvariant();

            List<string> itemsList = new List<string>();

            switch (regionCSGName.ToUpperInvariant())
            {
                case "WNE":
                case "GBR":
                    {
                        SQLQuery.Append("SELECT DISTINCT (AGENT_NAME) FROM ACP.CSG_MAPPING ");
                        SQLQuery.Append("WHERE AGENT_NAME IS NOT NULL AND State = :state AND GEO_LEVEL1 = :region");
                        SQLQuery.Append(" AND GEO_LEVEL2 IS NOT NULL");
                        SQLQuery.Append(" ORDER BY AGENT_NAME");
                        break;
                    }
                case "FREEDOM":
                    {
                        SQLQuery.Append("SELECT DISTINCT (AGENT_NAME) FROM FRE.CSG_MAPPING ");
                        SQLQuery.Append("WHERE AGENT_NAME IS NOT NULL AND State = :state AND GEO_LEVEL1 = :region");
                        SQLQuery.Append(" AND GEO_LEVEL2 NOT in (" + "\'DEFAULT\'" + ")");
                        SQLQuery.Append(" ORDER BY AGENT_NAME");
                        break;
                    }
                case "KEYSTONE":
                    {
                        SQLQuery.Append("SELECT DISTINCT (AGENT_NAME) FROM PIT.CSG_MAPPING ");
                        SQLQuery.Append("WHERE AGENT_NAME IS NOT NULL AND State = :state AND REGION = :region");
                        SQLQuery.Append(" ORDER BY AGENT_NAME");
                        break;
                    }
                case "BELTWAY":
                    {
                        SQLQuery.Append("SELECT DISTINCT (AGENT_NAME) FROM BLT.CSG_MAPPING ");
                        SQLQuery.Append("WHERE AGENT_NAME IS NOT NULL AND State = :state AND GEO_LEVEL1 = :region");
                        SQLQuery.Append(" ORDER BY AGENT_NAME");
                        break;
                    }
            }

            string queryString = SQLQuery.ToString();

            try
            {
                OracleConnection conn = new OracleConnection(ConfigurationManager.AppSettings["OracleConn1"].ToString());
                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Parameters.Add(new OracleParameter(":state", state));
                cmd.Parameters.Add(new OracleParameter(":region", regionCSGName));
                cmd.Connection = conn;
                cmd.CommandText = queryString;
                cmd.CommandType = CommandType.Text;
                OracleDataReader dr = cmd.ExecuteReader();
                dr.Read();

                if (dr.HasRows)
                {

                    do
                    {
                        itemsList.Add(dr.GetString(0));
                    } while (dr.Read());
                }

                conn.Dispose();
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Error fetching Cities by State");
            }

            return itemsList;

        }

        public IList<string> GetStatesByRegion(string region)
        {
            string strRegionUpperCase = region.ToUpperInvariant();

            System.Text.StringBuilder SQLQuery = new System.Text.StringBuilder();

            List<string> itemsList = new List<string>();
            switch (region.ToUpperInvariant())
            {
                case "WNE":
                case "GBR":
                    {
                        SQLQuery.Append("SELECT DISTINCT (State) FROM ACP.CSG_MAPPING ");
                        SQLQuery.Append("WHERE State IS NOT NULL AND Geo_Level1 = :region ");
                        SQLQuery.Append(" AND GEO_LEVEL2 IS NOT NULL");
                        SQLQuery.Append(" ORDER BY State");
                        break;
                    }
                case "FREEDOM":
                    {
                        SQLQuery.Append("SELECT DISTINCT (State) FROM FRE.CSG_MAPPING ");
                        SQLQuery.Append("WHERE State IS NOT NULL AND Geo_Level1 = :strRegionUpperCase ");
                        SQLQuery.Append("AND GEO_LEVEL2 NOT in (" + "\'DEFAULT\'" + ")");
                        SQLQuery.Append(" ORDER BY State");
                        break;
                    }
                case "KEYSTONE":
                    {
                        SQLQuery.Append("SELECT DISTINCT (State) FROM PIT.CSG_MAPPING ");
                        SQLQuery.Append("WHERE State IS NOT NULL AND Region = :region ");
                        SQLQuery.Append(" ORDER BY State");
                        break;
                    }
                case "BELTWAY":
                    {
                        SQLQuery.Append("SELECT DISTINCT (State) FROM BLT.CSG_MAPPING ");
                        SQLQuery.Append("WHERE State IS NOT NULL AND Geo_Level1 = :region ");
                        SQLQuery.Append(" ORDER BY State");
                        break;
                    }
            }


            string queryString = SQLQuery.ToString();

            try
            {
                OracleConnection conn = new OracleConnection(ConfigurationManager.AppSettings["OracleConn1"].ToString());
                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Parameters.Add(new OracleParameter(":region", strRegionUpperCase));
                cmd.Connection = conn;
                cmd.CommandText = queryString;
                cmd.CommandType = CommandType.Text;
                OracleDataReader dr = cmd.ExecuteReader();
                dr.Read();

                if (dr.HasRows)
                {
                    do
                    {
                        itemsList.Add(dr.GetString(0));

                    } while (dr.Read());
                }

                conn.Dispose();
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Error fetching States by Region");
            }

            return itemsList;
        }

        public IList<string> GetRegions()
        {
            List<string> List_RegionStatePair = new List<string>();

            System.Text.StringBuilder SQLQuery = new System.Text.StringBuilder();

            SQLQuery.Append("SELECT DISTINCT (Geo_Level1) AS Region FROM ACP.CSG_MAPPING ");
            SQLQuery.Append("WHERE Geo_Level1 IS NOT NULL ");
            SQLQuery.Append("UNION ");
            SQLQuery.Append("SELECT DISTINCT (Geo_Level1) AS Region FROM FRE.CSG_MAPPING ");
            SQLQuery.Append("WHERE Geo_Level1 IS NOT NULL ");
            SQLQuery.Append("UNION ");
            SQLQuery.Append("SELECT DISTINCT (Geo_Level1) AS Region FROM BLT.CSG_MAPPING ");
            SQLQuery.Append("WHERE Geo_Level1 IS NOT NULL ");
            SQLQuery.Append("UNION ");
            SQLQuery.Append("SELECT DISTINCT (Region) FROM PIT.CSG_MAPPING ");
            SQLQuery.Append("WHERE Region IS NOT NULL ORDER BY 1 ");

            string queryString = SQLQuery.ToString();

            try
            {
                OracleConnection conn = new OracleConnection(ConfigurationManager.AppSettings["OracleConn1"].ToString());
                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = queryString;
                cmd.CommandType = CommandType.Text;
                OracleDataReader dr = cmd.ExecuteReader();
                dr.Read();

                if (dr.HasRows)
                {

                    do
                    {
                        List_RegionStatePair.Add(dr.GetString(0));

                    } while (dr.Read());

                }

                conn.Dispose();
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Error fetching Regions");
            }

            return List_RegionStatePair;

        }

        public IList<string> GetRegionsCSGNames()
        {
            List<string> itemsList = new List<string>();

            System.Text.StringBuilder SQLQuery = new System.Text.StringBuilder();

            SQLQuery.Append("SELECT DISTINCT (Geo_Level1) AS Region FROM ACP.CSG_MAPPING ");
            SQLQuery.Append("WHERE Geo_Level1 IS NOT NULL ");
            SQLQuery.Append("UNION ");
            SQLQuery.Append("SELECT DISTINCT (Geo_Level1) AS Region FROM FRE.CSG_MAPPING ");
            SQLQuery.Append("WHERE Geo_Level1 IS NOT NULL ");
            SQLQuery.Append("UNION ");
            SQLQuery.Append("SELECT DISTINCT (Geo_Level1) AS Region FROM BLT.CSG_MAPPING ");
            SQLQuery.Append("WHERE Geo_Level1 IS NOT NULL ");
            SQLQuery.Append("UNION ");
            SQLQuery.Append("SELECT DISTINCT (Region) FROM PIT.CSG_MAPPING ");
            SQLQuery.Append("WHERE Region IS NOT NULL ORDER BY 1 ");

            string queryString = SQLQuery.ToString();

            try
            {
                OracleConnection conn = new OracleConnection(ConfigurationManager.AppSettings["OracleConn1"].ToString());
                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = queryString;
                cmd.CommandType = CommandType.Text;
                OracleDataReader dr = cmd.ExecuteReader();
                dr.Read();

                string regionAbbreviation = string.Empty;

                if (dr.HasRows)
                {
                    do
                    {
                        itemsList.Add(dr.GetString(0));

                    } while (dr.Read());
                }

                conn.Dispose();
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Error fetching Regions");
            }

            return itemsList;
        }

        public XrState GetState(int id)
        {
            return db.XrStates.Find(id);
        }

        public XrState GetStateByAbbr(string stateAbbr)
        {
            return db.XrStates.Where(a => a.Abreviation == stateAbbr).FirstOrDefault();
        }

        public IEnumerable<XrState> GetAllStates()
        {
            return db.XrStates;
        }

    }
}
