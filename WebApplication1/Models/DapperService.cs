using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Transactions;
using WebApplication1.Common;
using WebApplication1.Model;
using WebApplication1.Model.Dto;
namespace WebApplication1.Models
{
    public class DapperService
    {
        public static MySqlConnection MySqlConnection()
        {
            string mysqlConnectionStr = ConfigurationManager.ConnectionStrings["MySqlConnString"].ToString();
            var connection = new MySqlConnection(mysqlConnectionStr);
            new MySqlCommand().CommandTimeout = 0;
            connection.Open();
            return connection;
        }
        public static class SqlHelp
        {
             /// <summary>
            /// 获取登陆用户
            /// </summary>
            /// <param name="model"></param>
            /// <returns></returns>
            public static UserDto UserLogin(string username,string password)
             {

                 UserDto user;
                using (IDbConnection conn = MySqlConnection())
                {
                    var p = new DynamicParameters();
                    p.Add("@UserName", username);
                    p.Add("@PassWord",password);
                    //var user1= conn.Query("select user_name,password,userid from user where user_name=@UserName and password=@PassWord", p, commandType: CommandType.Text, commandTimeout: 0).ToList().FirstOrDefault();
                    user= conn.Query<UserDto>("select user_name,password,userid from user where user_name=@UserName and password=@PassWord", p, commandType: CommandType.Text, commandTimeout: 0).ToList().FirstOrDefault();

                }
                return user;
            }
            /// <summary>
            /// 总成查询
            /// </summary>
            /// <param name="model"></param>
            /// <returns></returns>
            public static List<AssembleSearchDto> AssemblySearch(AssembleSearchModel model)
            {

                List<AssembleSearchDto> list;
                using (IDbConnection conn = MySqlConnection())
                {

              
                    var p = new DynamicParameters();

                    p.Add("@_DateTimeStart", model.StarTime);
                    p.Add("@_DateTimeEnd", model.EndTime);
                    p.Add("@_Area", model.Area);
                   // var list1= conn.Query("SP_Data_All_Barcode_QueryByDate", p, commandType: CommandType.StoredProcedure, commandTimeout: 0).ToList();
                    list = conn.Query<AssembleSearchDto>("SP_Data_All_Barcode_QueryByDate", p, commandType: CommandType.StoredProcedure, commandTimeout: 0).ToList();
                }
                return list;
            }
            /// <summary>
            /// 零件基础数据
            /// </summary>
            /// <param name="model"></param>
            /// <returns></returns>
            public static List<PartDataDto> PartDataBase(PartDataModel model)
            {

                List<PartDataDto> list;
                using (IDbConnection conn = MySqlConnection())
                {


                    var p = new DynamicParameters();

                    p.Add("@_DateTimeStart", model.StartTime);
                    p.Add("@_DateTimeEnd", model.EndTime);
                    p.Add("@_Area", model.Line);
                    // var list1= conn.Query("SP_Data_All_Barcode_QueryByDate", p, commandType: CommandType.StoredProcedure, commandTimeout: 0).ToList();
                    list = conn.Query<PartDataDto>("SP_DataPart_ExportByDate", p, commandType: CommandType.StoredProcedure, commandTimeout: 0).ToList();
                }
                return list;
            }
            //扫描量
            public static List<ResScnRateDto> ScanRate(PartDataModel model)    
            {
                List<ResScnRateDto> listend = new List<ResScnRateDto>();
                ResScanRateDto list=new ResScanRateDto();
                if (model.EndTime.Month - model.StartTime.Month > 0)
                {
                    CommonHelp.StarMon = model.StartTime.ToString("yyyy MMMM dd") + "~" +
                                         model.EndTime.ToString("yyyy MMMM dd");
                    CommonHelp.EndMon = CommonHelp.list.Where(p => p.Area.Equals(model.Line)).FirstOrDefault()
                        .DesCribe;
                }
                else
                {
                    CommonHelp.StarMon = model.StartTime.Month + "月";
                    CommonHelp.EndMon = CommonHelp.list.Where(p => p.Area.Equals(model.Line)).FirstOrDefault()
                        .DesCribe;
                }
                using (IDbConnection conn = MySqlConnection())
                {
                    var p = new DynamicParameters();
                    p.Add("@_DateTimeStart", model.StartTime);
                    p.Add("@_DateTimeEnd", model.EndTime);
                    p.Add("@_Area", model.Line);
                    // var list1= conn.Query("SP_Data_All_Barcode_QueryByDate", p, commandType: CommandType.StoredProcedure, commandTimeout: 0).ToList();
                    list.assemlist = conn.Query<ResAssemblyRateDto>("SP_ScanerRatezc", p, commandType: CommandType.StoredProcedure, commandTimeout: 0).ToList();
                    list.partlist = conn.Query<ResPartDto>("SP_ScanerRatepart", p, commandType: CommandType.StoredProcedure, commandTimeout: 0).ToList();
                    var tt = from left in list.assemlist
                        join right in list.partlist
                            on left.Figure_No_up equals right.Figure_No 
                        select new ResScnRateDto 
                        {
                            Figure_No_up = left.Figure_No_up,
                            sum_up = left.sum_up,
                            upstation = left.upstation,
                            Figure_No_down = left.Figure_No_down,
                            sum_down = left.sum_down,
                            DOWNSTATION = left.DOWNSTATION,
                            DOWNRATE = Convert.ToString(left.DOWNRATE)+"%",
                            cartype = right.cartype,
                            Partname = right.Partname,
                            Part_Sum = right.Part_Sum,
                            PartFigureNo = right.PartFigureNo
                        };
                    listend = tt.ToList();

                    foreach (var itmes in listend)
                    {
                        itmes.Rate =Convert.ToString(itmes.sum_up / itmes.Part_Sum * 100)+"%";
                    }


                }
                return listend;
            }


            /// <summary>
            /// 直通率基础数据
            /// </summary>
            /// <param name="model"></param>
            /// <returns></returns>
            public static List<ThroughRateDto> ThroughRateData(ThroughRateModel model)
            {

                List<ThroughRateDto> list;
                using (IDbConnection conn = MySqlConnection())
                {


                    var p = new DynamicParameters();

                    p.Add("@_DateTimeStart", model.StartTime);
                    p.Add("@_DateTimeEnd", model.EndTime);
                    p.Add("@_Area", model.Line);
                    // var list1= conn.Query("SP_Data_All_Barcode_QueryByDate", p, commandType: CommandType.StoredProcedure, commandTimeout: 0).ToList();
                    list = conn.Query<ThroughRateDto>("SP_FirstPassYied", p, commandType: CommandType.StoredProcedure, commandTimeout: 0).ToList();
                }
                return list;
            }
            // 需求与押车
            public static ResNeedDeleyListDto NeedDeley(ThroughRateModel model)
            {

                ResNeedDeleyListDto list=new ResNeedDeleyListDto();
                using (IDbConnection conn = MySqlConnection())
                {

                    var p = new DynamicParameters();
                    p.Add("@_DateTimeStart", model.StartTime);
                    p.Add("@_DateTimeEnd", model.EndTime);
                    p.Add("@_Area", model.Line);
                  
                    list.ListNeed = conn.Query<ResNeedDeleyDto>("SP_RequireSum", p, commandType: CommandType.StoredProcedure, commandTimeout: 0).ToList();
                    list.ListDeley = conn.Query<ResNeedDeleyDto>("SP_YaCheSum", p, commandType: CommandType.StoredProcedure, commandTimeout: 0).ToList();

                }
                return list;
            }





            /// <summary>
            /// 总成模糊查询
            /// </summary>
            /// <param name="code"></param>
            /// <param name="area"></param>
            /// <returns></returns>
            public static List<AssembleSearchDto> DarkSearch(string code,string area)
            {

                List<AssembleSearchDto> list;
                using (IDbConnection conn = MySqlConnection())
                {


                    var p = new DynamicParameters();

                    p.Add("@_barcode_zc", code);
                    p.Add("@_Area",area);
                
                    // var list1= conn.Query("SP_Data_All_Barcode_QueryByDate", p, commandType: CommandType.StoredProcedure, commandTimeout: 0).ToList();
                    list = conn.Query<AssembleSearchDto>("SP_Data_All_Barcode_QueryByBarcodeZc", p, commandType: CommandType.StoredProcedure, commandTimeout: 0).ToList();
                }
                return list;
            }
            //精确查询
            public static List<AssembleSearchDto> DataSourceSearch(string code, string area)
            {

                List<AssembleSearchDto> list;
                using (IDbConnection conn = MySqlConnection())
                {


                    var p = new DynamicParameters();

                    p.Add("@_CSN", code);
                    p.Add("@_Area", area);

                    // var list1= conn.Query("SP_Data_All_Barcode_QueryByDate", p, commandType: CommandType.StoredProcedure, commandTimeout: 0).ToList();
                    list = conn.Query<AssembleSearchDto>("SP_Data_All_Barcode_QueryByCSN", p, commandType: CommandType.StoredProcedure, commandTimeout: 0).ToList();
                }
                return list;
            }
            /// <summary>
            /// QC查询
            /// </summary>
            /// <param name="Area"></param>
            /// <returns></returns>
            public static List<QCSelectDto> QCSelect(string Area)
            {

                List<QCSelectDto> list=null;
                using (IDbConnection conn = MySqlConnection())
                {
                    var p = new DynamicParameters();

                    p.Add("@_LINE", Area);
                    // TimeZone.CurrentTimeZone.ToLocalTime(model.StartDateTime);
                    try
                    {
                      
                        list = conn.Query<QCSelectDto>("SP_QC_PASS", p, commandType: CommandType.StoredProcedure, commandTimeout: 0).ToList();
                    }
                    catch (Exception ex)
                    {

                    }

                    //var p = new DynamicParameters();

                    //p.Add("@_LINE", Area);
                    //list = conn.Query<QCSelectDto>("SP_QC_PASS", p, commandType: CommandType.StoredProcedure,commandTimeout:0).ToList();
                }
                return list;

            }
            /// <summary>
            /// 根据时间段获取部件原始数据
            /// </summary>
            public static List<ByDataDto> PartByData(ByDataModel model)
            {

                List<ByDataDto> list;
                using (IDbConnection conn = MySqlConnection())
                {

                    // TimeZone.CurrentTimeZone.ToLocalTime(model.StartDateTime);
                    var p = new DynamicParameters();
                    p.Add("@_DateTimeStart", model.StartDateTime);
                    p.Add("@_DateTimeEnd", model.EndDateTime);
                    p.Add("@_Station", model.Station);
                    p.Add("@_Area", model.Area);
                    list = conn.Query<ByDataDto>("SP_DataPart_QueryByDate", p, commandType: CommandType.StoredProcedure).ToList();
                }
                return list;
            }
            /// <summary>
            /// 获得部件excel表原始数据
            /// </summary>
            /// <param name="model"></param>
            /// <returns></returns>
            public static List<PartToExcelDto> PartyByDataToExcel(AssembleSearchModel model)
            {

                List<PartToExcelDto> list;
                using (IDbConnection conn = MySqlConnection())
                {
                   // model.EndTime = model.StarTime.AddDays(1);
                    var p = new DynamicParameters(); 
                    p.Add("@_DateTimeStart", model.StarTime);
                    p.Add("@_DateTimeEnd", model.EndTime); 
                    p.Add("@_Area", model.Area);
                //var list2= conn.Query("SP_DataPart_ExportByDate", p, commandType: CommandType.StoredProcedure, commandTimeout: 240).ToList();
                    list = conn.Query<PartToExcelDto>("SP_DataPart_ExportByDate1", p, commandType: CommandType.StoredProcedure, commandTimeout: 240).ToList(); 
                }
                return list;
            }
            /// <summary>
            /// 获得扭矩excel表原始数据
            /// </summary>
            /// <param name="model"></param>
            /// <returns></returns>
            public static List<TorqToExcelDto> TorqByDataToExcel(AssembleSearchModel model)
            {
                List<TorqToExcelDto> list;
                using (IDbConnection conn = MySqlConnection())
                {

                    //model.EndTime = model.StarTime.AddDays(1);
                    var p = new DynamicParameters();
                    p.Add("@_DateTimeStart", model.StarTime);
                    p.Add("@_DateTimeEnd", model.EndTime);
                    p.Add("@_Area", model.Area);
                    //var list6= conn.Query("SP_DataTorq_ExportByDate", p, commandType: CommandType.StoredProcedure, commandTimeout: 240).ToList();
                    list = conn.Query<TorqToExcelDto>("SP_DataTorq_ExportByDate", p, commandType: CommandType.StoredProcedure, commandTimeout: 240).ToList();
                }
                return list;
            }
            /// <summary>
                /// 根据条码查询部件原始数据
                /// </summary>
                /// <param name="model"></param>
                /// <returns></returns>
            public static List<ByCodeDto> PartByCode(ByCodeModel model)
            {
                List<ByCodeDto> dataTable;
                using (IDbConnection conn = MySqlConnection())
                {
                    var p = new DynamicParameters();
                    p.Add("@_Barcode_zc", model.BarCode);
                    p.Add("@_Station", model.Station);
                    p.Add("@_Area", model.Area);
                    dataTable = conn.Query<ByCodeDto>("SP_DataPart_QueryByBarcodeZC", p, commandType: CommandType.StoredProcedure).ToList();
                }

                return dataTable;
            }
            public static List<PartDetailToExcelDto> PartToExcel(ByCodeModel model)
            {
                List<PartDetailToExcelDto> dataTable;
                using (IDbConnection conn = MySqlConnection())
                {
                    var p = new DynamicParameters();
                    p.Add("@_Barcode_zc", model.BarCode);
                    p.Add("@_Station", model.Station);
                    p.Add("@_Area", model.Area);
                    dataTable = conn.Query<PartDetailToExcelDto>("SP_DataPart_QueryByBarcodeZC", p, commandType: CommandType.StoredProcedure).ToList();
                }

                return dataTable;
            }
            /// <summary>
            /// 根据时间段查询扭矩原始数据
            /// </summary>
            /// <param name="model"></param>
            /// <returns></returns>
            public static List<ByDataDto> TorqByData(ByDataModel model)
            {
                List<ByDataDto> dataTable;
                using (IDbConnection conn = MySqlConnection())
                {
                    var p = new DynamicParameters();
                    p.Add("@_DateTimeStart", model.StartDateTime);
                    p.Add("@_DateTimeEnd", model.EndDateTime);
                    p.Add("@_Station", model.Station);
                    p.Add("@_Area", model.Area);

                    dataTable = conn.Query<ByDataDto>("SP_DataTorq_QueryByDate", p, commandType: CommandType.StoredProcedure).ToList();


                }

                return dataTable;
            }
            /// <summary>
            /// 根据部件编号得到总成信息原始数据
            /// </summary>
            /// <param name="model"></param>
            /// <returns></returns>
            public static List<ResAssemblyBarCode> GetAssemblyByPartCode(string partcode,string area)
            {
                List<ResAssemblyBarCode> dataTable;
                using (IDbConnection conn = MySqlConnection())
                {
                    var p = new DynamicParameters();
                    p.Add("@_barcode_part",partcode);
                    p.Add("@_Area", area);
                  //  var tt= conn.Query<ResAssemblyBarCode>("SP_PART_ZC", p, commandType: CommandType.StoredProcedure).ToList();
                  //  var tt= conn.Query("SP_PART_ZC", p, commandType: CommandType.StoredProcedure).ToList();
                    dataTable = conn.Query<ResAssemblyBarCode>("SP_Data_All_Barcode_QueryByBarcodePart", p, commandType: CommandType.StoredProcedure).ToList();
                }
                return dataTable;
            }
            /// <summary>
            /// 根据条码查询扭矩原始数据
            /// </summary>
            /// <param name="model"></param>
            /// <returns></returns>
            public static List<ByCodeDto> TorqByCode(ByCodeModel model)
            {

                List<ByCodeDto> dataTable;
                using (IDbConnection conn = MySqlConnection())
                {
                    var p = new DynamicParameters();
                    p.Add("@_Barcode_zc", model.BarCode);
                    p.Add("@_Station", model.Station);
                    p.Add("@_Area", model.Area);
                    dataTable = conn.Query<ByCodeDto>("SP_DataTorq_QueryByBarcodeZC", p, commandType: CommandType.StoredProcedure).ToList();
                }

                return dataTable;
            }

            /// <summary>
            /// 节拍查询
            /// </summary>
            /// <param name="model"></param>
            /// <returns></returns>
            public static List<BeatDto> BeatChart(ChartModel model)
            {

                List<BeatDto> dataTable;
                using (IDbConnection conn = MySqlConnection())
                {
                    var p = new DynamicParameters();
                    p.Add("@_DateTime", model.DateTime);
                    p.Add("@_Area", model.Area);
                    dataTable = conn.Query<BeatDto>("SP_GETCOUNTBYTIME", p, commandType: CommandType.StoredProcedure).ToList();
                }

                return dataTable;
            }
            /// <summary>
            /// 图标显示原始数据
            /// </summary>
            /// <param name="model"></param>
            /// <returns></returns>
            public static List<ChartDataDto> GetChart(ChartModel model)
            {
                List<ChartDataDto> ResList = new List<ChartDataDto>();
                using (IDbConnection conn = MySqlConnection())
                {
                    MySqlParameter[] p = new MySqlParameter[3] { new MySqlParameter("@_DateTime", model.DateTime),
                        new MySqlParameter("@_QueryKind", model.QueryKind),new MySqlParameter("@_Area", model.Area)};
                    // List<dynamic>
                    DataTable dataTable = ExecuteDataTable("SP_Web_TaktTimeCount", CommandType.StoredProcedure, p);
                    for (var i = 0; i < dataTable.Columns.Count; i++)
                    {
                        ResList.Add(new ChartDataDto()
                        {
                            Num = Convert.ToInt32(dataTable.Rows[0][dataTable.Columns[i].ColumnName]),
                            ChartDate = dataTable.Columns[i].ColumnName.Substring(0,10),
                            Light = Convert.ToInt32(dataTable.Rows[0][dataTable.Columns[i].ColumnName])+10,
                            Hight = Convert.ToInt32(dataTable.Rows[0][dataTable.Columns[i].ColumnName])+70
                        });
                    }
                }

                return ResList;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="sql"></param>
            /// <param name="commandtype"></param>
            /// <param name="parameters"></param>
            /// <returns></returns>
            public static DataTable ExecuteDataTable(string sql, CommandType commandtype, MySqlParameter[] parameters)
            {
                DataTable data = new DataTable();//实例化datatable，用于装载查询结果集  
                using (MySqlConnection con = MySqlConnection())
                {
                    using (MySqlCommand cmd = new MySqlCommand(sql, con))
                    {
                        cmd.CommandType = commandtype;//设置command的commandType为指定的Commandtype  
                                                      //如果同时传入了参数，则添加这些参数  
                        if (parameters != null)
                        {
                            foreach (MySqlParameter parameter in parameters)
                            {
                                cmd.Parameters.Add(parameter);
                            }
                        }

                        //通过包含查询sql的sqlcommand实例来实例化sqldataadapter  
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        adapter.Fill(data);//填充datatable  

                    }
                }
                return data;
            }
        }
    }
}