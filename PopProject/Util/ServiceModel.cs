using System;

namespace PopProject.Util
{
    internal class ServiceModel
    {
        public static string[] 접속정보 = new string[] { Properties.Settings.Default.corp_id, Properties.Settings.Default.UserID, Properties.Settings.Default.UserName };
        //두성
        public static ServiceReference_dubox.Service_CRUDClient du_Service;
        public static ServiceReference_dubox.DBHandle_Model du_DB_Handle_Model;
        public static ServiceReference_dubox.Access_Model du_Access_Model;
        public static ServiceReference_dubox.Return_Model du_Return_Model;

        //대양
        public static ServiceReference_dybox.Service_CRUDClient dy_Service;
        public static ServiceReference_dybox.DBHandle_Model dy_DB_Handle_Model;
        public static ServiceReference_dybox.Access_Model dy_Access_Model;
        public static ServiceReference_dybox.Return_Model dy_Return_Model;

        //오주
        public static ServiceReference_real_ojbox.Service_CRUDClient oj_Service;
        public static ServiceReference_real_ojbox.DBHandle_Model oj_DB_Handle_Model;
        public static ServiceReference_real_ojbox.Access_Model oj_Access_Model;
        public static ServiceReference_real_ojbox.Return_Model oj_Return_Model;

        //미래
        public static ServiceReference_mirae.Service_CRUDClient mirae_Service;
        public static ServiceReference_mirae.DBHandle_Model mirae_DB_Handle_Model;
        public static ServiceReference_mirae.Access_Model mirae_Access_Model;
        public static ServiceReference_mirae.Return_Model mirae_Return_Model;

        //광성
        public static ServiceReference_ksbox.Service_CRUDClient Ks_Service;
        public static ServiceReference_ksbox.DBHandle_Model ks_DB_Handle_Model;
        public static ServiceReference_ksbox.Access_Model ks_Access_Model;
        public static ServiceReference_ksbox.Return_Model ks_Return_Model;

        //하나
        public static ServiceReference_hnbox.Service_CRUDClient hn_Service;
        public static ServiceReference_hnbox.DBHandle_Model hn_DB_Handle_Model;
        public static ServiceReference_hnbox.Access_Model hn_Access_Model;
        public static ServiceReference_hnbox.Return_Model hn_Return_Model;

        //선호
        public static ServiceReference_shbox.Service_CRUDClient sh_Service;
        public static ServiceReference_shbox.DBHandle_Model sh_DB_Handle_Model;
        public static ServiceReference_shbox.Access_Model sh_Access_Model;
        public static ServiceReference_shbox.Return_Model sh_Return_Model;

        //광무
        public static ServiceReference_kmbox.Service_CRUDClient km_Service;
        public static ServiceReference_kmbox.DBHandle_Model km_DB_Handle_Model;
        public static ServiceReference_kmbox.Access_Model km_Access_Model;
        public static ServiceReference_kmbox.Return_Model km_Return_Model;

        //유창
        public static ServiceReference_ycbox.Service_CRUDClient yc_Service;
        public static ServiceReference_ycbox.DBHandle_Model yc_DB_Handle_Model;
        public static ServiceReference_ycbox.Access_Model yc_Access_Model;
        public static ServiceReference_ycbox.Return_Model yc_Return_Model;

        //(주)혜성
        public static ServiceReference_hsbox.Service_CRUDClient hs_Service;
        public static ServiceReference_hsbox.DBHandle_Model hs_DB_Handle_Model;
        public static ServiceReference_hsbox.Access_Model hs_Access_Model;
        public static ServiceReference_hsbox.Return_Model hs_Return_Model;

        //신우
        public static ServiceReference_swbox.Service_CRUDClient sw_Service;
        public static ServiceReference_swbox.DBHandle_Model sw_DB_Handle_Model;
        public static ServiceReference_swbox.Access_Model sw_Access_Model;
        public static ServiceReference_swbox.Return_Model sw_Return_Model;

        //누리
        public static ServiceReference_nrbox.Service_CRUDClient nr_Service;
        public static ServiceReference_nrbox.DBHandle_Model nr_DB_Handle_Model;
        public static ServiceReference_nrbox.Access_Model nr_Access_Model;
        public static ServiceReference_nrbox.Return_Model nr_Return_Model;

        //우리
        public static ServiceReference_wrbox.Service_CRUDClient wr_Service;
        public static ServiceReference_wrbox.DBHandle_Model wr_DB_Handle_Model;
        public static ServiceReference_wrbox.Access_Model wr_Access_Model;
        public static ServiceReference_wrbox.Return_Model wr_Return_Model;

        //성주
        public static ServiceReference_sjbox.Service_CRUDClient sj_Service;
        public static ServiceReference_sjbox.DBHandle_Model sj_DB_Handle_Model;
        public static ServiceReference_sjbox.Access_Model sj_Access_Model;
        public static ServiceReference_sjbox.Return_Model sj_Return_Model;


        //대세
        public static ServiceReference_dsbox.Service_CRUDClient ds_Service;
        public static ServiceReference_dsbox.DBHandle_Model ds_DB_Handle_Model;
        public static ServiceReference_dsbox.Access_Model ds_Access_Model;
        public static ServiceReference_dsbox.Return_Model ds_Return_Model;
        
        //구룡산업제2공장
        public static ServiceReference_grbox1.Service_CRUDClient gr_Service;
        public static ServiceReference_grbox1.DBHandle_Model gr_DB_Handle_Model;
        public static ServiceReference_grbox1.Access_Model gr_Access_Model;
        public static ServiceReference_grbox1.Return_Model gr_Return_Model;

        public static void ServiceModelClean(string serverIndex)
        {
            Console.WriteLine("serverIndex : " + serverIndex);
            switch (serverIndex)
            {
                case "grbox":
                    gr_Service = new ServiceReference_grbox1.Service_CRUDClient();
                    gr_Access_Model = new ServiceReference_grbox1.Access_Model();
                    gr_Return_Model = new ServiceReference_grbox1.Return_Model();
                    gr_DB_Handle_Model = new ServiceReference_grbox1.DBHandle_Model
                    {
                        접속정보 = 접속정보,
                        BroadCasting = 0,
                        BroadCastingName = string.Empty,
                        inputPkey = "0",
                        inputContent = string.Empty,
                        inputProcessContent = string.Empty
                    };
                    break;

                case "dsbox":
                    ds_Service = new ServiceReference_dsbox.Service_CRUDClient();
                    ds_Access_Model = new ServiceReference_dsbox.Access_Model();
                    ds_Return_Model = new ServiceReference_dsbox.Return_Model();
                    ds_DB_Handle_Model = new ServiceReference_dsbox.DBHandle_Model
                    {
                        접속정보 = 접속정보,
                        BroadCasting = 0,
                        BroadCastingName = string.Empty,
                        inputPkey = "0",
                        inputContent = string.Empty,
                        inputProcessContent = string.Empty
                    };
                    break;
                case "dubox":
                    du_Service = new ServiceReference_dubox.Service_CRUDClient();
                    du_Access_Model = new ServiceReference_dubox.Access_Model();
                    du_Return_Model = new ServiceReference_dubox.Return_Model();
                    du_DB_Handle_Model = new ServiceReference_dubox.DBHandle_Model
                    {
                        접속정보 = 접속정보,
                        BroadCasting = 0,
                        BroadCastingName = string.Empty,
                        inputPkey = "0",
                        inputContent = string.Empty,
                        inputProcessContent = string.Empty
                    };

                    break;
                case "sjbox":
                    sj_Service = new ServiceReference_sjbox.Service_CRUDClient();
                    sj_Access_Model = new ServiceReference_sjbox.Access_Model();
                    sj_Return_Model = new ServiceReference_sjbox.Return_Model();
                    sj_DB_Handle_Model = new ServiceReference_sjbox.DBHandle_Model
                    {
                        접속정보 = 접속정보,
                        BroadCasting = 0,
                        BroadCastingName = string.Empty,
                        inputPkey = "0",
                        inputContent = string.Empty,
                        inputProcessContent = string.Empty
                    };
                    break;

                case "dybox":

                    dy_Service = new ServiceReference_dybox.Service_CRUDClient();
                    dy_Access_Model = new ServiceReference_dybox.Access_Model();
                    dy_Return_Model = new ServiceReference_dybox.Return_Model();
                    dy_DB_Handle_Model = new ServiceReference_dybox.DBHandle_Model
                    {
                        접속정보 = 접속정보,
                        BroadCasting = 0,
                        BroadCastingName = string.Empty,
                        inputPkey = "0",
                        inputContent = string.Empty,
                        inputProcessContent = string.Empty
                    };
                    break;

                case "jobox":
                    wr_Service = new ServiceReference_wrbox.Service_CRUDClient();
                    wr_Access_Model = new ServiceReference_wrbox.Access_Model();
                    wr_Return_Model = new ServiceReference_wrbox.Return_Model();
                    wr_DB_Handle_Model = new ServiceReference_wrbox.DBHandle_Model
                    {
                        접속정보 = 접속정보,
                        BroadCasting = 0,
                        BroadCastingName = string.Empty,
                        inputPkey = "0",
                        inputContent = string.Empty,
                        inputProcessContent = string.Empty
                    };
                    break;

                case "wrbox":
                    wr_Service = new ServiceReference_wrbox.Service_CRUDClient();
                    wr_Access_Model = new ServiceReference_wrbox.Access_Model();
                    wr_Return_Model = new ServiceReference_wrbox.Return_Model();

                    wr_DB_Handle_Model = new ServiceReference_wrbox.DBHandle_Model
                    {
                        접속정보 = 접속정보,
                        BroadCasting = 0,
                        BroadCastingName = string.Empty,
                        inputPkey = "0",
                        inputContent = string.Empty,
                        inputProcessContent = string.Empty
                    };

                    break;

                case "nrbox":
                    nr_Service = new ServiceReference_nrbox.Service_CRUDClient();
                    nr_Access_Model = new ServiceReference_nrbox.Access_Model();
                    nr_Return_Model = new ServiceReference_nrbox.Return_Model();
                    nr_DB_Handle_Model = new ServiceReference_nrbox.DBHandle_Model
                    {
                        접속정보 = 접속정보,
                        BroadCasting = 0,
                        BroadCastingName = string.Empty,
                        inputPkey = "0",
                        inputContent = string.Empty,
                        inputProcessContent = string.Empty
                    };

                    break;

                case "swbox":
                    sw_Service = new ServiceReference_swbox.Service_CRUDClient();
                    sw_Access_Model = new ServiceReference_swbox.Access_Model();
                    sw_Return_Model = new ServiceReference_swbox.Return_Model();
                    sw_DB_Handle_Model = new ServiceReference_swbox.DBHandle_Model
                    {
                        접속정보 = 접속정보,
                        BroadCasting = 0,
                        BroadCastingName = string.Empty,
                        inputPkey = "0",
                        inputContent = string.Empty,
                        inputProcessContent = string.Empty
                    };
                    break;

                case "hsbox":
                    hs_Service = new ServiceReference_hsbox.Service_CRUDClient();
                    hs_Access_Model = new ServiceReference_hsbox.Access_Model();
                    hs_Return_Model = new ServiceReference_hsbox.Return_Model();
                    hs_DB_Handle_Model = new ServiceReference_hsbox.DBHandle_Model
                    {
                        접속정보 = 접속정보,
                        BroadCasting = 0,
                        BroadCastingName = string.Empty,
                        inputPkey = "0",
                        inputContent = string.Empty,
                        inputProcessContent = string.Empty
                    };
                    break;

                case "ojbox":
                    oj_Service = new ServiceReference_real_ojbox.Service_CRUDClient();
                    oj_Access_Model = new ServiceReference_real_ojbox.Access_Model();
                    oj_Return_Model = new ServiceReference_real_ojbox.Return_Model();
                    oj_DB_Handle_Model = new ServiceReference_real_ojbox.DBHandle_Model

                    {
                        접속정보 = 접속정보,
                        BroadCasting = 0,
                        BroadCastingName = string.Empty,
                        inputPkey = "0",
                        inputContent = string.Empty,
                        inputProcessContent = string.Empty
                    };
                    break;

                case "miraebox":

                    mirae_Service = new ServiceReference_mirae.Service_CRUDClient();
                    mirae_Access_Model = new ServiceReference_mirae.Access_Model();
                    mirae_Return_Model = new ServiceReference_mirae.Return_Model();
                    mirae_DB_Handle_Model = new ServiceReference_mirae.DBHandle_Model
                    {
                        접속정보 = 접속정보,
                        BroadCasting = 0,
                        BroadCastingName = string.Empty,
                        inputPkey = 0,
                        inputContent = string.Empty,
                        inputProcessContent = string.Empty
                    };

                    break;

                case "ksbox":

                    Ks_Service = new ServiceReference_ksbox.Service_CRUDClient();
                    ks_Access_Model = new ServiceReference_ksbox.Access_Model();
                    ks_Return_Model = new ServiceReference_ksbox.Return_Model();
                    ks_DB_Handle_Model = new ServiceReference_ksbox.DBHandle_Model
                    {
                        접속정보 = 접속정보,
                        BroadCasting = 0,
                        BroadCastingName = string.Empty,
                        inputPkey = "0",
                        inputContent = string.Empty,
                        inputProcessContent = string.Empty
                    };

                    break;

                case "hnbox":

                    hn_Service = new ServiceReference_hnbox.Service_CRUDClient();
                    hn_Access_Model = new ServiceReference_hnbox.Access_Model();
                    hn_Return_Model = new ServiceReference_hnbox.Return_Model();
                    hn_DB_Handle_Model = new ServiceReference_hnbox.DBHandle_Model
                    {
                        접속정보 = 접속정보,
                        BroadCasting = 0,
                        BroadCastingName = string.Empty,
                        inputPkey = "0",
                        inputContent = string.Empty,
                        inputProcessContent = string.Empty
                    };

                    break;

                case "shbox":
                    sh_Service = new ServiceReference_shbox.Service_CRUDClient();
                    sh_Access_Model = new ServiceReference_shbox.Access_Model();
                    sh_Return_Model = new ServiceReference_shbox.Return_Model();
                    sh_DB_Handle_Model = new ServiceReference_shbox.DBHandle_Model
                    {
                        접속정보 = 접속정보,
                        BroadCasting = 0,
                        BroadCastingName = string.Empty,
                        inputPkey = "0",
                        inputContent = string.Empty,
                        inputProcessContent = string.Empty
                    };
                    break;

                case "kmbox":

                    km_Service = new ServiceReference_kmbox.Service_CRUDClient();
                    km_Access_Model = new ServiceReference_kmbox.Access_Model();
                    km_Return_Model = new ServiceReference_kmbox.Return_Model();
                    km_DB_Handle_Model = new ServiceReference_kmbox.DBHandle_Model
                    {
                        접속정보 = 접속정보,
                        BroadCasting = 0,
                        BroadCastingName = string.Empty,
                        inputPkey = "0",
                        inputContent = string.Empty,
                        inputProcessContent = string.Empty
                    };

                    break;

                case "ycbox":

                    yc_Service = new ServiceReference_ycbox.Service_CRUDClient();
                    yc_Access_Model = new ServiceReference_ycbox.Access_Model();
                    yc_Return_Model = new ServiceReference_ycbox.Return_Model();
                    yc_DB_Handle_Model = new ServiceReference_ycbox.DBHandle_Model
                    {
                        접속정보 = 접속정보,
                        BroadCasting = 0,
                        BroadCastingName = string.Empty,
                        inputPkey = "0",
                        inputContent = string.Empty,
                        inputProcessContent = string.Empty
                    };

                    break;
                default:
                    break;
            }
        }

        public static ReturnModel GetData(string serverIndex, string SP명, string[] 파라미터, object[] 값, int 갱신)
        {
            ReturnModel returnModel = new ReturnModel();
            try
            {
                switch (serverIndex)
                {
                    case "grbox":
                        gr_Access_Model.SP명 = SP명;
                        gr_Access_Model.파라미터 = 파라미터;
                        gr_Access_Model.값 = 값;
                        gr_DB_Handle_Model.프로시저 = new ServiceReference_grbox1.Access_Model[1] { gr_Access_Model };
                        gr_DB_Handle_Model.BroadCasting = 0;

                        gr_Return_Model = 갱신 == 1 ? gr_Service.갱신(gr_DB_Handle_Model, 갱신) : gr_Service.조회(gr_DB_Handle_Model);

                        returnModel.결과 = gr_Return_Model.결과;
                        returnModel.리턴값 = gr_Return_Model.Retrun값;
                        Console.WriteLine(SP명 + " 리턴결과 : " + gr_Return_Model.결과);
                        Console.WriteLine(SP명 + " 리턴값 : " + gr_Return_Model.Retrun값);
                        break;

                    case "dsbox":
                        ds_Access_Model.SP명 = SP명;
                        ds_Access_Model.파라미터 = 파라미터;
                        ds_Access_Model.값 = 값;
                        ds_DB_Handle_Model.프로시저 = new ServiceReference_dsbox.Access_Model[1] { ds_Access_Model };
                        ds_DB_Handle_Model.BroadCasting = 0;

                        ds_Return_Model = 갱신 == 1 ? ds_Service.갱신(ds_DB_Handle_Model, 갱신) : ds_Service.조회(ds_DB_Handle_Model);

                        returnModel.결과 = ds_Return_Model.결과;
                        returnModel.리턴값 = ds_Return_Model.Retrun값;
                        Console.WriteLine(SP명 + " 리턴결과 : " + ds_Return_Model.결과);
                        Console.WriteLine(SP명 + " 리턴값 : " + ds_Return_Model.Retrun값);
                        break;

                    case "dubox":
                        du_Access_Model.SP명 = SP명;
                        du_Access_Model.파라미터 = 파라미터;
                        du_Access_Model.값 = 값;
                        du_DB_Handle_Model.프로시저 = new ServiceReference_dubox.Access_Model[1] { du_Access_Model };
                        du_DB_Handle_Model.BroadCasting = 0;

                        du_Return_Model = 갱신 == 1 ? du_Service.갱신(du_DB_Handle_Model, 갱신) : du_Service.조회(du_DB_Handle_Model);

                        returnModel.결과 = du_Return_Model.결과;
                        returnModel.리턴값 = du_Return_Model.Retrun값;
                        Console.WriteLine(SP명 + " 리턴결과 : " + du_Return_Model.결과);
                        Console.WriteLine(SP명 + " 리턴값 : " + du_Return_Model.Retrun값);
                        break;

                    case "sjbox":
                        sj_Access_Model.SP명 = SP명;
                        sj_Access_Model.파라미터 = 파라미터;
                        sj_Access_Model.값 = 값;
                        sj_DB_Handle_Model.프로시저 = new ServiceReference_sjbox.Access_Model[1] { sj_Access_Model };
                        sj_DB_Handle_Model.BroadCasting = 0;

                        sj_Return_Model = 갱신 == 1 ? sj_Service.갱신(sj_DB_Handle_Model, 갱신) : sj_Service.조회(sj_DB_Handle_Model);

                        returnModel.결과 = sj_Return_Model.결과;
                        returnModel.리턴값 = sj_Return_Model.Retrun값;
                        Console.WriteLine(SP명 + " 리턴결과 : " + sj_Return_Model.결과);
                        Console.WriteLine(SP명 + " 리턴값 : " + sj_Return_Model.Retrun값);
                        break;


                    case "dybox":
                        dy_Access_Model.SP명 = SP명;
                        dy_Access_Model.파라미터 = 파라미터;
                        dy_Access_Model.값 = 값;
                        dy_DB_Handle_Model.프로시저 = new ServiceReference_dybox.Access_Model[1] { dy_Access_Model };
                        dy_DB_Handle_Model.BroadCasting = 0;

                        dy_Return_Model = 갱신 == 1 ? dy_Service.갱신(dy_DB_Handle_Model, 갱신) : dy_Service.조회(dy_DB_Handle_Model);

                        returnModel.결과 = dy_Return_Model.결과;
                        returnModel.리턴값 = dy_Return_Model.Retrun값;
                        Console.WriteLine(SP명 + " 리턴결과 : " + dy_Return_Model.결과);
                        Console.WriteLine(SP명 + " 리턴값 : " + dy_Return_Model.Retrun값);
                        break;

                    case "wrbox":

                        wr_Access_Model.SP명 = SP명;
                        wr_Access_Model.파라미터 = 파라미터;
                        wr_Access_Model.값 = 값;
                        wr_DB_Handle_Model.프로시저 = new ServiceReference_wrbox.Access_Model[1] { wr_Access_Model };
                        wr_DB_Handle_Model.BroadCasting = 0;

                        wr_Return_Model = 갱신 == 1 ? wr_Service.갱신(wr_DB_Handle_Model, 갱신) : wr_Service.조회(wr_DB_Handle_Model);

                        returnModel.결과 = wr_Return_Model.결과;
                        returnModel.리턴값 = wr_Return_Model.Retrun값;
                        Console.WriteLine(SP명 + " 리턴결과 : " + wr_Return_Model.결과);
                        Console.WriteLine(SP명 + " 리턴값 : " + wr_Return_Model.Retrun값);
                        break;


                    case "jobox":

                        wr_Access_Model.SP명 = SP명;
                        wr_Access_Model.파라미터 = 파라미터;
                        wr_Access_Model.값 = 값;
                        wr_DB_Handle_Model.프로시저 = new ServiceReference_wrbox.Access_Model[1] { wr_Access_Model };
                        wr_DB_Handle_Model.BroadCasting = 0;

                        wr_Return_Model = 갱신 == 1 ? wr_Service.갱신(wr_DB_Handle_Model, 갱신) : wr_Service.조회(wr_DB_Handle_Model);

                        returnModel.결과 = wr_Return_Model.결과;
                        returnModel.리턴값 = wr_Return_Model.Retrun값;
                        Console.WriteLine(SP명 + " 리턴결과 : " + wr_Return_Model.결과);
                        Console.WriteLine(SP명 + " 리턴값 : " + wr_Return_Model.Retrun값);
                        break;

                    case "nrbox":

                        nr_Access_Model.SP명 = SP명;
                        nr_Access_Model.파라미터 = 파라미터;
                        nr_Access_Model.값 = 값;
                        nr_DB_Handle_Model.프로시저 = new ServiceReference_nrbox.Access_Model[1] { nr_Access_Model };
                        nr_DB_Handle_Model.BroadCasting = 0;

                        nr_Return_Model = 갱신 == 1 ? nr_Service.갱신(nr_DB_Handle_Model, 갱신) : nr_Service.조회(nr_DB_Handle_Model);

                        returnModel.결과 = nr_Return_Model.결과;
                        returnModel.리턴값 = nr_Return_Model.Retrun값;
                        Console.WriteLine(SP명 + " 리턴결과 : " + nr_Return_Model.결과);
                        Console.WriteLine(SP명 + " 리턴값 : " + nr_Return_Model.Retrun값);
                        break;

                    case "swbox":
                        sw_Access_Model.SP명 = SP명;
                        sw_Access_Model.파라미터 = 파라미터;
                        sw_Access_Model.값 = 값;
                        sw_DB_Handle_Model.프로시저 = new ServiceReference_swbox.Access_Model[1] { sw_Access_Model };
                        sw_DB_Handle_Model.BroadCasting = 0;

                        sw_Return_Model = 갱신 == 1 ? sw_Service.갱신(sw_DB_Handle_Model, 갱신) : sw_Service.조회(sw_DB_Handle_Model);

                        returnModel.결과 = sw_Return_Model.결과;
                        returnModel.리턴값 = sw_Return_Model.Retrun값;
                        Console.WriteLine(SP명 + " 리턴결과 : " + sw_Return_Model.결과);
                        Console.WriteLine(SP명 + " 리턴값 : " + sw_Return_Model.Retrun값);
                        break;

                    case "hsbox":
                        hs_Access_Model.SP명 = SP명;
                        hs_Access_Model.파라미터 = 파라미터;
                        hs_Access_Model.값 = 값;
                        hs_DB_Handle_Model.프로시저 = new ServiceReference_hsbox.Access_Model[1] { hs_Access_Model };
                        hs_DB_Handle_Model.BroadCasting = 0;

                        hs_Return_Model = 갱신 == 1 ? hs_Service.갱신(hs_DB_Handle_Model, 갱신) : hs_Service.조회(hs_DB_Handle_Model);

                        returnModel.결과 = hs_Return_Model.결과;
                        returnModel.리턴값 = hs_Return_Model.Retrun값;
                        Console.WriteLine(SP명 + " 리턴결과 : " + hs_Return_Model.결과);
                        Console.WriteLine(SP명 + " 리턴값 : " + hs_Return_Model.Retrun값);
                        break;
                    case "ojbox":
                        oj_Access_Model.SP명 = SP명;
                        oj_Access_Model.파라미터 = 파라미터;
                        oj_Access_Model.값 = 값;
                        oj_DB_Handle_Model.프로시저 = new ServiceReference_real_ojbox.Access_Model[1] { oj_Access_Model };
                        oj_DB_Handle_Model.BroadCasting = 0;

                        oj_Return_Model = 갱신 == 1 ? oj_Service.갱신(oj_DB_Handle_Model, 갱신) : oj_Service.조회(oj_DB_Handle_Model);

                        returnModel.결과 = oj_Return_Model.결과;
                        returnModel.리턴값 = oj_Return_Model.Retrun값;
                        Console.WriteLine(SP명 + " 리턴결과 : " + oj_Return_Model.결과);
                        Console.WriteLine(SP명 + " 리턴값 : " + oj_Return_Model.Retrun값);
                        break;
                    case "miraebox":

                        mirae_Access_Model.SP명 = SP명;
                        mirae_Access_Model.파라미터 = 파라미터;
                        mirae_Access_Model.값 = 값;
                        mirae_DB_Handle_Model.프로시저 = new ServiceReference_mirae.Access_Model[1] { mirae_Access_Model };
                        mirae_DB_Handle_Model.BroadCasting = 0;

                        mirae_Return_Model = 갱신 == 1 ? mirae_Service.갱신(mirae_DB_Handle_Model, 갱신) : mirae_Service.조회(mirae_DB_Handle_Model);

                        returnModel.결과 = mirae_Return_Model.결과;
                        returnModel.리턴값 = mirae_Return_Model.Retrun값;
                        Console.WriteLine(SP명 + " 리턴결과 : " + mirae_Return_Model.결과);
                         Console.WriteLine(SP명 + " 리턴값 : " + mirae_Return_Model.Retrun값);

                        break;

                    case "ksbox":

                        ks_Access_Model.SP명 = SP명;
                        ks_Access_Model.파라미터 = 파라미터;
                        ks_Access_Model.값 = 값;
                        ks_DB_Handle_Model.프로시저 = new ServiceReference_ksbox.Access_Model[1] { ks_Access_Model };
                        ks_DB_Handle_Model.BroadCasting = 0;

                        ks_Return_Model = 갱신 == 1 ? Ks_Service.갱신(ks_DB_Handle_Model, 갱신) : Ks_Service.조회(ks_DB_Handle_Model);

                        returnModel.결과 = ks_Return_Model.결과;
                        returnModel.리턴값 = ks_Return_Model.Retrun값;

                        Console.WriteLine(SP명 + " 리턴결과 : " + ks_Return_Model.결과);
                        Console.WriteLine(SP명 + " 리턴값 : " + ks_Return_Model.Retrun값);

                        break;

                    case "hnbox":

                        hn_Access_Model.SP명 = SP명;
                        hn_Access_Model.파라미터 = 파라미터;
                        hn_Access_Model.값 = 값;
                        hn_DB_Handle_Model.프로시저 = new ServiceReference_hnbox.Access_Model[1] { hn_Access_Model };
                        hn_DB_Handle_Model.BroadCasting = 0;

                        hn_Return_Model = 갱신 == 1 ? hn_Service.갱신(hn_DB_Handle_Model, 갱신) : hn_Service.조회(hn_DB_Handle_Model);

                        returnModel.결과 = hn_Return_Model.결과;
                        returnModel.리턴값 = hn_Return_Model.Retrun값;

                         Console.WriteLine(SP명 + " 리턴결과 : " + hn_Return_Model.결과);
                         Console.WriteLine(SP명 + " 리턴값 : " + hn_Return_Model.Retrun값);

                        break;

                    case "shbox":
                        sh_Access_Model.SP명 = SP명;
                        sh_Access_Model.파라미터 = 파라미터;
                        sh_Access_Model.값 = 값;
                        sh_DB_Handle_Model.프로시저 = new ServiceReference_shbox.Access_Model[1] { sh_Access_Model };
                        sh_DB_Handle_Model.BroadCasting = 0;

                        sh_Return_Model = 갱신 == 1 ? sh_Service.갱신(sh_DB_Handle_Model, 갱신) : sh_Service.조회(sh_DB_Handle_Model);

                        returnModel.결과 = sh_Return_Model.결과;
                        returnModel.리턴값 = sh_Return_Model.Retrun값;
                         Console.WriteLine(SP명 + " 리턴결과 : " + sh_Return_Model.결과);
                         Console.WriteLine(SP명 + " 리턴값 : " + sh_Return_Model.Retrun값);
                        break;
                    case "kmbox":
                        km_Access_Model.SP명 = SP명;
                        km_Access_Model.파라미터 = 파라미터;
                        km_Access_Model.값 = 값;
                        km_DB_Handle_Model.프로시저 = new ServiceReference_kmbox.Access_Model[1] { km_Access_Model };
                        km_DB_Handle_Model.BroadCasting = 0;
                        km_Return_Model = 갱신 == 1 ? km_Service.갱신(km_DB_Handle_Model, 갱신) : km_Service.조회(km_DB_Handle_Model);
                        returnModel.결과 = km_Return_Model.결과;
                        returnModel.리턴값 = km_Return_Model.Retrun값;
                        //   Console.WriteLine(SP명 + " 리턴결과 : " + km_Return_Model.결과);
                          Console.WriteLine(SP명 + " 리턴값 : " + km_Return_Model.Retrun값);
                        break;

                    case "ycbox":

                        yc_Access_Model.SP명 = SP명;
                        yc_Access_Model.파라미터 = 파라미터;
                        yc_Access_Model.값 = 값;
                        yc_DB_Handle_Model.프로시저 = new ServiceReference_ycbox.Access_Model[1] { yc_Access_Model };
                        yc_DB_Handle_Model.BroadCasting = 0;

                        yc_Return_Model = 갱신 == 1 ? yc_Service.갱신(yc_DB_Handle_Model, 갱신) : yc_Service.조회(yc_DB_Handle_Model);

                        returnModel.결과 = yc_Return_Model.결과;
                        returnModel.리턴값 = yc_Return_Model.Retrun값;

                        Console.WriteLine(SP명 + " 리턴결과 : " + yc_Return_Model.결과);
                        Console.WriteLine(SP명 + " 리턴값 : " + yc_Return_Model.Retrun값);
                        break;
                    default:
                        break;
                }

            }

            catch (Exception)
            {
                returnModel.결과 = 0;
                returnModel.리턴값 = "";
            }

            return returnModel;
        }


        public static ReturnModel GetData(string serverIndex, string SP명, string[] 파라미터, object[] 값, int 갱신, string parameter)
        {
            ReturnModel returnModel = new ReturnModel();
            switch (serverIndex)
            {
                //dsbox 이거 오타 때문인거 같은데
                case "dsbox":
                    ds_Access_Model.SP명 = SP명;
                    ds_Access_Model.파라미터 = 파라미터;
                    ds_Access_Model.값 = 값;

                    ds_DB_Handle_Model.프로시저 = new ServiceReference_dsbox.Access_Model[1] { ds_Access_Model };
                    ds_DB_Handle_Model.BroadCasting = 1;
                    ds_DB_Handle_Model.BroadCastingName = "생산입력완료";
                    ds_DB_Handle_Model.inputPkey = "14201";
                    ds_DB_Handle_Model.inputContent = parameter;
                    ds_Return_Model = 갱신 == 1 ? ds_Service.갱신(ds_DB_Handle_Model, 갱신) : ds_Service.조회(ds_DB_Handle_Model);

                    returnModel.결과 = ds_Return_Model.결과;
                    returnModel.리턴값 = ds_Return_Model.Retrun값;
                    break;



                case "dubox":
                    du_Access_Model.SP명 = SP명;
                    du_Access_Model.파라미터 = 파라미터;
                    du_Access_Model.값 = 값;

                    du_DB_Handle_Model.프로시저 = new ServiceReference_dubox.Access_Model[1] { du_Access_Model };
                    du_DB_Handle_Model.BroadCasting = 1;
                    du_DB_Handle_Model.BroadCastingName = "생산입력완료";
                    du_DB_Handle_Model.inputPkey = "14201";
                    du_DB_Handle_Model.inputContent = parameter;
                    du_Return_Model = 갱신 == 1 ? du_Service.갱신(du_DB_Handle_Model, 갱신) : du_Service.조회(du_DB_Handle_Model);

                    returnModel.결과 = du_Return_Model.결과;
                    returnModel.리턴값 = du_Return_Model.Retrun값;
                    break;


                case "sjbox":
                    sj_Access_Model.SP명 = SP명;
                    sj_Access_Model.파라미터 = 파라미터;
                    sj_Access_Model.값 = 값;

                    sj_DB_Handle_Model.프로시저 = new ServiceReference_sjbox.Access_Model[1] { sj_Access_Model };
                    sj_DB_Handle_Model.BroadCasting = 1;
                    sj_DB_Handle_Model.BroadCastingName = "생산입력완료";
                    sj_DB_Handle_Model.inputPkey = "14201";
                    sj_DB_Handle_Model.inputContent = parameter;
                    sj_Return_Model = 갱신 == 1 ? sj_Service.갱신(sj_DB_Handle_Model, 갱신) : sj_Service.조회(sj_DB_Handle_Model);

                    returnModel.결과 = sj_Return_Model.결과;
                    returnModel.리턴값 = sj_Return_Model.Retrun값;
                    break;


                case "dybox":
                    dy_Access_Model.SP명 = SP명;
                    dy_Access_Model.파라미터 = 파라미터;
                    dy_Access_Model.값 = 값;

                    dy_DB_Handle_Model.프로시저 = new ServiceReference_dybox.Access_Model[1] { dy_Access_Model };
                    dy_DB_Handle_Model.BroadCasting = 1;
                    dy_DB_Handle_Model.BroadCastingName = "생산입력완료";
                    dy_DB_Handle_Model.inputPkey = "14201";
                    dy_DB_Handle_Model.inputContent = parameter;
                    dy_Return_Model = 갱신 == 1 ? dy_Service.갱신(dy_DB_Handle_Model, 갱신) : dy_Service.조회(dy_DB_Handle_Model);

                    returnModel.결과 = dy_Return_Model.결과;
                    returnModel.리턴값 = dy_Return_Model.Retrun값;
                    break;

                case "wrbox":
                    wr_Access_Model.SP명 = SP명;
                    wr_Access_Model.파라미터 = 파라미터;
                    wr_Access_Model.값 = 값;

                    wr_DB_Handle_Model.프로시저 = new ServiceReference_wrbox.Access_Model[1] { wr_Access_Model };
                    wr_DB_Handle_Model.BroadCasting = 1;
                    wr_DB_Handle_Model.BroadCastingName = "생산입력완료";
                    wr_DB_Handle_Model.inputPkey = "14201";
                    wr_DB_Handle_Model.inputContent = parameter;
                    wr_Return_Model = 갱신 == 1 ? wr_Service.갱신(wr_DB_Handle_Model, 갱신) : wr_Service.조회(wr_DB_Handle_Model);
                    
                    returnModel.결과 = wr_Return_Model.결과;
                    returnModel.리턴값 = wr_Return_Model.Retrun값;
                    //  Console.WriteLine(SP명 + " 리턴결과 : " + mirae_Return_Model.결과);
                    //  Console.WriteLine(SP명 + " 리턴값 : " + mirae_Return_Model.Retrun값);
                    break;
                case "jobox":
                    wr_Access_Model.SP명 = SP명;
                    wr_Access_Model.파라미터 = 파라미터;
                    wr_Access_Model.값 = 값;

                    wr_DB_Handle_Model.프로시저 = new ServiceReference_wrbox.Access_Model[1] { wr_Access_Model };
                    wr_DB_Handle_Model.BroadCasting = 1;
                    wr_DB_Handle_Model.BroadCastingName = "생산입력완료";
                    wr_DB_Handle_Model.inputPkey = "14201";
                    wr_DB_Handle_Model.inputContent = parameter;
                    wr_Return_Model = 갱신 == 1 ? wr_Service.갱신(wr_DB_Handle_Model, 갱신) : wr_Service.조회(wr_DB_Handle_Model);

                    returnModel.결과 = wr_Return_Model.결과;
                    returnModel.리턴값 = wr_Return_Model.Retrun값;
                    //  Console.WriteLine(SP명 + " 리턴결과 : " + mirae_Return_Model.결과);
                    //  Console.WriteLine(SP명 + " 리턴값 : " + mirae_Return_Model.Retrun값);
                    break;

                case "nrbox":
                    nr_Access_Model.SP명 = SP명;
                    nr_Access_Model.파라미터 = 파라미터;
                    nr_Access_Model.값 = 값;

                    nr_DB_Handle_Model.프로시저 = new ServiceReference_nrbox.Access_Model[1] { nr_Access_Model };
                    nr_DB_Handle_Model.BroadCasting = 1;
                    nr_DB_Handle_Model.BroadCastingName = "생산입력완료";
                    nr_DB_Handle_Model.inputPkey = "14201";
                    nr_DB_Handle_Model.inputContent = parameter;
                    nr_Return_Model = 갱신 == 1 ? nr_Service.갱신(nr_DB_Handle_Model, 갱신) : nr_Service.조회(nr_DB_Handle_Model);

                    returnModel.결과 = nr_Return_Model.결과;
                    returnModel.리턴값 = nr_Return_Model.Retrun값;
                    //  Console.WriteLine(SP명 + " 리턴결과 : " + mirae_Return_Model.결과);
                    //  Console.WriteLine(SP명 + " 리턴값 : " + mirae_Return_Model.Retrun값);
                    break;
                case "swbox":
                    sw_Access_Model.SP명 = SP명;
                    sw_Access_Model.파라미터 = 파라미터;
                    sw_Access_Model.값 = 값;

                    sw_DB_Handle_Model.프로시저 = new ServiceReference_swbox.Access_Model[1] { sw_Access_Model };
                    sw_DB_Handle_Model.BroadCasting = 1;
                    sw_DB_Handle_Model.BroadCastingName = "생산입력완료";
                    sw_DB_Handle_Model.inputPkey = "14201";
                    sw_DB_Handle_Model.inputContent = parameter;
                    sw_Return_Model = 갱신 == 1 ? sw_Service.갱신(sw_DB_Handle_Model, 갱신) : sw_Service.조회(sw_DB_Handle_Model);

                    returnModel.결과 = sw_Return_Model.결과;
                    returnModel.리턴값 = sw_Return_Model.Retrun값;
                    //  Console.WriteLine(SP명 + " 리턴결과 : " + mirae_Return_Model.결과);
                    //  Console.WriteLine(SP명 + " 리턴값 : " + mirae_Return_Model.Retrun값);
                    break;

                case "hsbox":
                    hs_Access_Model.SP명 = SP명;
                    hs_Access_Model.파라미터 = 파라미터;
                    hs_Access_Model.값 = 값;

                    hs_DB_Handle_Model.프로시저 = new ServiceReference_hsbox.Access_Model[1] { hs_Access_Model };
                    hs_DB_Handle_Model.BroadCasting = 1;
                    hs_DB_Handle_Model.BroadCastingName = "생산입력완료";
                    hs_DB_Handle_Model.inputPkey = "14201";
                    hs_DB_Handle_Model.inputContent = parameter;
                    hs_Return_Model = 갱신 == 1 ? hs_Service.갱신(hs_DB_Handle_Model, 갱신) : hs_Service.조회(hs_DB_Handle_Model);

                    returnModel.결과 = hs_Return_Model.결과;
                    returnModel.리턴값 = hs_Return_Model.Retrun값;
                    //  Console.WriteLine(SP명 + " 리턴결과 : " + mirae_Return_Model.결과);
                    //  Console.WriteLine(SP명 + " 리턴값 : " + mirae_Return_Model.Retrun값);
                    break;
                case "ojbox":
                    oj_Access_Model.SP명 = SP명;
                    oj_Access_Model.파라미터 = 파라미터;
                    oj_Access_Model.값 = 값;

                    oj_DB_Handle_Model.프로시저 = new ServiceReference_real_ojbox.Access_Model[1] { oj_Access_Model };
                    oj_DB_Handle_Model.BroadCasting = 1;
                    oj_DB_Handle_Model.BroadCastingName = "생산입력완료";
                    oj_DB_Handle_Model.inputPkey = "14201";
                    oj_DB_Handle_Model.inputContent = parameter;
                    oj_Return_Model = 갱신 == 1 ? oj_Service.갱신(oj_DB_Handle_Model, 갱신) : oj_Service.조회(oj_DB_Handle_Model);

                    returnModel.결과 = oj_Return_Model.결과;
                    returnModel.리턴값 = oj_Return_Model.Retrun값;
                    //  Console.WriteLine(SP명 + " 리턴결과 : " + mirae_Return_Model.결과);
                    //  Console.WriteLine(SP명 + " 리턴값 : " + mirae_Return_Model.Retrun값);

                    break;
                case "miraebox":
                    mirae_Access_Model.SP명 = SP명;
                    mirae_Access_Model.파라미터 = 파라미터;
                    mirae_Access_Model.값 = 값;

                    mirae_DB_Handle_Model.프로시저 = new ServiceReference_mirae.Access_Model[1] { mirae_Access_Model };
                    mirae_DB_Handle_Model.BroadCasting = 1;
                    mirae_DB_Handle_Model.BroadCastingName = "생산입력완료";
                    mirae_DB_Handle_Model.inputPkey = 14201;
                    mirae_DB_Handle_Model.inputContent = parameter;
                    if (갱신 == 1)
                    {
                        try
                        {
                            mirae_Return_Model = mirae_Service.갱신(mirae_DB_Handle_Model, 갱신);
                        }
                        catch (Exception)
                        {
                            MessageBox.MessageBox_Dialog dialog = new MessageBox.MessageBox_Dialog("갱신 오류");
                            dialog.Show();
                        }
                    }

                    else
                    {
                        mirae_Return_Model = mirae_Service.조회(mirae_DB_Handle_Model);
                    }

                    returnModel.결과 = mirae_Return_Model.결과;
                    returnModel.리턴값 = mirae_Return_Model.Retrun값;
                    //  Console.WriteLine(SP명 + " 리턴결과 : " + mirae_Return_Model.결과);
                    //  Console.WriteLine(SP명 + " 리턴값 : " + mirae_Return_Model.Retrun값);
                    break;

                case "ksbox":

                    ks_Access_Model.SP명 = SP명;
                    ks_Access_Model.파라미터 = 파라미터;
                    ks_Access_Model.값 = 값;

                    ks_DB_Handle_Model.프로시저 = new ServiceReference_ksbox.Access_Model[1] { ks_Access_Model };
                    ks_DB_Handle_Model.BroadCasting = 1;
                    ks_DB_Handle_Model.BroadCastingName = "생산입력완료";
                    ks_DB_Handle_Model.inputPkey = "14201";
                    ks_DB_Handle_Model.inputContent = parameter;
                    ks_Return_Model = 갱신 == 1 ? Ks_Service.갱신(ks_DB_Handle_Model, 갱신) : Ks_Service.조회(ks_DB_Handle_Model);

                    returnModel.결과 = ks_Return_Model.결과;
                    returnModel.리턴값 = ks_Return_Model.Retrun값;
                    //  Console.WriteLine(SP명 + " 리턴결과 : " + ks_Return_Model.결과);
                    //  Console.WriteLine(SP명 + " 리턴값 : " + ks_Return_Model.Retrun값);

                    break;

                case "hnbox":

                    hn_Access_Model.SP명 = SP명;
                    hn_Access_Model.파라미터 = 파라미터;
                    hn_Access_Model.값 = 값;
                    hn_DB_Handle_Model.프로시저 = new ServiceReference_hnbox.Access_Model[1] { hn_Access_Model };
                    hn_DB_Handle_Model.BroadCasting = 1;
                    hn_DB_Handle_Model.BroadCastingName = "생산입력완료";
                    hn_DB_Handle_Model.inputPkey = "14201";
                    hn_DB_Handle_Model.inputContent = parameter;
                    hn_Return_Model = 갱신 == 1 ? hn_Service.갱신(hn_DB_Handle_Model, 갱신) : hn_Service.조회(hn_DB_Handle_Model);

                    returnModel.결과 = hn_Return_Model.결과;
                    returnModel.리턴값 = hn_Return_Model.Retrun값;

                    //  Console.WriteLine(SP명 + " 리턴결과 : " + hn_Return_Model.결과);
                    //  Console.WriteLine(SP명 + " 리턴값 : " + hn_Return_Model.Retrun값);

                    break;

                case "shbox":

                    sh_Access_Model.SP명 = SP명;
                    sh_Access_Model.파라미터 = 파라미터;
                    sh_Access_Model.값 = 값;
                    sh_DB_Handle_Model.프로시저 = new ServiceReference_shbox.Access_Model[1] { sh_Access_Model };
                    sh_DB_Handle_Model.BroadCasting = 1;
                    sh_DB_Handle_Model.inputPkey = "14201";
                    sh_DB_Handle_Model.BroadCastingName = "생산입력완료";
                    sh_DB_Handle_Model.inputContent = parameter;

                    sh_Return_Model = 갱신 == 1 ? sh_Service.갱신(sh_DB_Handle_Model, 갱신) : sh_Service.조회(sh_DB_Handle_Model);

                    returnModel.결과 = sh_Return_Model.결과;
                    returnModel.리턴값 = sh_Return_Model.Retrun값;

                    //  Console.WriteLine(SP명 + " 리턴결과 : " + sh_Return_Model.결과);
                    //   Console.WriteLine(SP명 + " 리턴값 : " + sh_Return_Model.Retrun값);

                    break;
                case "kmbox":
                    km_Access_Model.SP명 = SP명;
                    km_Access_Model.파라미터 = 파라미터;
                    km_Access_Model.값 = 값;
                    km_DB_Handle_Model.프로시저 = new ServiceReference_kmbox.Access_Model[1] { km_Access_Model };
                    km_DB_Handle_Model.BroadCasting = 1;
                    km_DB_Handle_Model.inputPkey = "14201";
                    km_DB_Handle_Model.BroadCastingName = "생산입력완료";
                    km_DB_Handle_Model.inputContent = parameter;

                    km_Return_Model = 갱신 == 1 ? km_Service.갱신(km_DB_Handle_Model, 갱신) : km_Service.조회(km_DB_Handle_Model);

                    returnModel.결과 = km_Return_Model.결과;
                    returnModel.리턴값 = km_Return_Model.Retrun값;

                    // Console.WriteLine(SP명 + " 리턴결과 : " + km_Return_Model.결과);
                      Console.WriteLine(SP명 + " 리턴값 : " + km_Return_Model.Retrun값);

                    break;

                case "ycbox":
                    yc_Access_Model.SP명 = SP명;
                    yc_Access_Model.파라미터 = 파라미터;
                    yc_Access_Model.값 = 값;
                    yc_DB_Handle_Model.프로시저 = new ServiceReference_ycbox.Access_Model[1] { yc_Access_Model };
                    yc_DB_Handle_Model.BroadCasting = 1;
                    yc_DB_Handle_Model.inputPkey = "14201";
                    yc_DB_Handle_Model.BroadCastingName = "생산입력완료";
                    yc_DB_Handle_Model.inputContent = parameter;

                    yc_Return_Model = 갱신 == 1 ? yc_Service.갱신(yc_DB_Handle_Model, 갱신) : yc_Service.조회(yc_DB_Handle_Model);

                    returnModel.결과 = yc_Return_Model.결과;
                    returnModel.리턴값 = yc_Return_Model.Retrun값;

                    //  Console.WriteLine(SP명 + " 리턴결과 : " + yc_Return_Model.결과);
                    //  Console.WriteLine(SP명 + " 리턴값 : " + yc_Return_Model.Retrun값);

                    break;

                case "grbox":
                    gr_Access_Model.SP명 = SP명;
                    gr_Access_Model.파라미터 = 파라미터;
                    gr_Access_Model.값 = 값;
                    gr_DB_Handle_Model.프로시저 = new ServiceReference_grbox1.Access_Model[1] { gr_Access_Model };
                    gr_DB_Handle_Model.BroadCasting = 1;
                    gr_DB_Handle_Model.inputPkey = "14201";
                    gr_DB_Handle_Model.BroadCastingName = "생산입력완료";
                    gr_DB_Handle_Model.inputContent = parameter;

                    gr_Return_Model = 갱신 == 1 ? gr_Service.갱신(gr_DB_Handle_Model, 갱신) : gr_Service.조회(gr_DB_Handle_Model);

                    returnModel.결과 = gr_Return_Model.결과;
                    returnModel.리턴값 = gr_Return_Model.Retrun값;

                    //  Console.WriteLine(SP명 + " 리턴결과 : " + yc_Return_Model.결과);
                    //  Console.WriteLine(SP명 + " 리턴값 : " + yc_Return_Model.Retrun값);

                    break;
                default:
                    break;
            }
            return returnModel;
        }


    }
}
