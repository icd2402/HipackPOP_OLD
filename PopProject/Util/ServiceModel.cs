using System;

namespace PopProject.Util
{
    internal class ServiceModel
    {
        public static string[] 접속정보 = new string[] { SettingJsonManager.corp_id, SettingJsonManager.UserID, SettingJsonManager.UserName };
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

        //(주)혜성 울산공장
        public static ServiceReference_hssbox.Service_CRUDClient hss_Service;
        public static ServiceReference_hssbox.DBHandle_Model hss_DB_Handle_Model;
        public static ServiceReference_hssbox.Access_Model hss_Access_Model;
        public static ServiceReference_hssbox.Return_Model hss_Return_Model;

        //지온
        public static ServiceReference_jobox.Service_CRUDClient jo_Service;
        public static ServiceReference_jobox.DBHandle_Model jo_DB_Handle_Model;
        public static ServiceReference_jobox.Access_Model jo_Access_Model;
        public static ServiceReference_jobox.Return_Model jo_Return_Model;

        //금화
        public static ServiceReference_khbox.Service_CRUDClient kh_Service;
        public static ServiceReference_khbox.DBHandle_Model kh_DB_Handle_Model;
        public static ServiceReference_khbox.Access_Model kh_Access_Model;
        public static ServiceReference_khbox.Return_Model kh_Return_Model;

        //내포
        public static ServiceReference_npbox.Service_CRUDClient np_Service;
        public static ServiceReference_npbox.DBHandle_Model np_DB_Handle_Model;
        public static ServiceReference_npbox.Access_Model np_Access_Model;
        public static ServiceReference_npbox.Return_Model np_Return_Model;

        //원팩
        public static ServiceReference_opbox.Service_CRUDClient op_Service;
        public static ServiceReference_opbox.DBHandle_Model op_DB_Handle_Model;
        public static ServiceReference_opbox.Access_Model op_Access_Model;
        public static ServiceReference_opbox.Return_Model op_Return_Model;

        //로컬
        public static ServiceReference_jobox.Service_CRUDClient local_Service;
        public static ServiceReference_jobox.DBHandle_Model local_DB_Handle_Model;
        public static ServiceReference_jobox.Access_Model local_Access_Model;
        public static ServiceReference_jobox.Return_Model local_Return_Model;

        public static void ServiceModelClean(string serverIndex, string dbName = "")
        {
            //Console.WriteLine("serverIndex : " + serverIndex);

            if (string.IsNullOrEmpty(dbName))
            {
                접속정보[0] = SettingJsonManager.corp_id;
            }
            else
            {
                접속정보[0] = dbName;
            }

            switch (serverIndex)
            {

                //원팩
                case "opbox":
                    op_Service = new ServiceReference_opbox.Service_CRUDClient();
                    op_Access_Model = new ServiceReference_opbox.Access_Model();
                    op_Return_Model = new ServiceReference_opbox.Return_Model();
                    op_DB_Handle_Model = new ServiceReference_opbox.DBHandle_Model
                    {
                        접속정보 = 접속정보,
                        BroadCasting = 0,
                        BroadCastingName = string.Empty,
                        inputPkey = "0",
                        inputContent = string.Empty,
                        inputProcessContent = string.Empty
                    };
                    break;

                //내포
                case "npbox":
                    np_Service = new ServiceReference_npbox.Service_CRUDClient();
                    np_Access_Model = new ServiceReference_npbox.Access_Model();
                    np_Return_Model = new ServiceReference_npbox.Return_Model();
                    np_DB_Handle_Model = new ServiceReference_npbox.DBHandle_Model
                    {
                        접속정보 = 접속정보,
                        BroadCasting = 0,
                        BroadCastingName = string.Empty,
                        inputPkey = "0",
                        inputContent = string.Empty,
                        inputProcessContent = string.Empty
                    };
                    break;

                //금화
                case "khbox":
                    kh_Service = new ServiceReference_khbox.Service_CRUDClient();
                    kh_Access_Model = new ServiceReference_khbox.Access_Model();
                    kh_Return_Model = new ServiceReference_khbox.Return_Model();
                    kh_DB_Handle_Model = new ServiceReference_khbox.DBHandle_Model
                    {
                        접속정보 = 접속정보,
                        BroadCasting = 0,
                        BroadCastingName = string.Empty,
                        inputPkey = "0",
                        inputContent = string.Empty,
                        inputProcessContent = string.Empty
                    };
                    break;

                //주)혜성
                case "hssbox":
                    hss_Service = new ServiceReference_hssbox.Service_CRUDClient();
                    hss_Access_Model = new ServiceReference_hssbox.Access_Model();
                    hss_Return_Model = new ServiceReference_hssbox.Return_Model();
                    hss_DB_Handle_Model = new ServiceReference_hssbox.DBHandle_Model
                    {
                        접속정보 = 접속정보,
                        BroadCasting = 0,
                        BroadCastingName = string.Empty,
                        inputPkey = "0",
                        inputContent = string.Empty,
                        inputProcessContent = string.Empty
                    };
                    break;

                    //구룡
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

                    //대세
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

                    //두성
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

                    //성주
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

                    //대양
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

                    //지온
                case "jobox":
                    jo_Service = new ServiceReference_jobox.Service_CRUDClient();
                    jo_Access_Model = new ServiceReference_jobox.Access_Model();
                    jo_Return_Model = new ServiceReference_jobox.Return_Model();
                    jo_DB_Handle_Model = new ServiceReference_jobox.DBHandle_Model
                    {
                        접속정보 = 접속정보,
                        BroadCasting = 0,
                        BroadCastingName = string.Empty,
                        inputPkey = "0",
                        inputContent = string.Empty,
                        inputProcessContent = string.Empty
                    };
                    break;

                    //우리
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

                    //누리
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

                    //신우
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

                    //혜성
                case "hsbox":
                case "hsbox1":
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

                    //오주
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

                    //미래
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

                    //광성
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

                    //하나
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

                    //선호
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

                    //광무
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

                    //유창
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

                //case "local":

                //    local_Service = new ServiceReference_ycbox.Service_CRUDClient();
                //    local_Access_Model = new ServiceReference_ycbox.Access_Model();
                //    local_Return_Model = new ServiceReference_ycbox.Return_Model();
                //    local_DB_Handle_Model = new ServiceReference_ycbox.DBHandle_Model
                //    {
                //        접속정보 = 접속정보,
                //        BroadCasting = 0,
                //        BroadCastingName = string.Empty,
                //        inputPkey = "0",
                //        inputContent = string.Empty,
                //        inputProcessContent = string.Empty
                //    };
                //    break;
                default:
                    break;
            }
        }

        public static ReturnModel GetData(string serverIndex, string SP명, string[] 파라미터, object[] 값, int 갱신)
        {
            ReturnModel returnModel = new ReturnModel();
            //if (값.Length >0)
            //{
            //    if (!SP명[0].Equals("[PopSearchMerc]") && 값[0].Equals(""))
            //    {
            //        //returnModel.결과 = 0;
            //        //returnModel.리턴값 = "";
            //        return returnModel;
            //    };
            //}

            try
            {
                switch (serverIndex)
                {
                    //원팩
                    case "opbox":
                        op_Access_Model.SP명 = SP명;
                        op_Access_Model.파라미터 = 파라미터;
                        op_Access_Model.값 = 값;
                        op_DB_Handle_Model.프로시저 = new ServiceReference_opbox.Access_Model[1] { op_Access_Model };
                        op_DB_Handle_Model.BroadCasting = 0;
                        op_Return_Model = 갱신 == 1 ? op_Service.갱신(op_DB_Handle_Model, 갱신) : op_Service.조회(op_DB_Handle_Model);

                        returnModel.결과 = op_Return_Model.결과;
                        returnModel.리턴값 = op_Return_Model.Retrun값;

                        //Console.WriteLine(SP명 + " 리턴결과 : " + kh_Return_Model.결과);
                        //Console.WriteLine(SP명 + " 리턴값 : " + kh_Return_Model.Retrun값);
                        break;

                    //내포
                    case "npbox":
                        np_Access_Model.SP명 = SP명;
                        np_Access_Model.파라미터 = 파라미터;
                        np_Access_Model.값 = 값;
                        np_DB_Handle_Model.프로시저 = new ServiceReference_npbox.Access_Model[1] { np_Access_Model };
                        np_DB_Handle_Model.BroadCasting = 0;
                        np_Return_Model = 갱신 == 1 ? np_Service.갱신(np_DB_Handle_Model, 갱신) : np_Service.조회(np_DB_Handle_Model);

                        returnModel.결과 = np_Return_Model.결과;
                        returnModel.리턴값 = np_Return_Model.Retrun값;

                        //Console.WriteLine(SP명 + " 리턴결과 : " + kh_Return_Model.결과);
                        //Console.WriteLine(SP명 + " 리턴값 : " + kh_Return_Model.Retrun값);
                        break;

                    //금화
                    case "khbox":
                        kh_Access_Model.SP명 = SP명;
                        kh_Access_Model.파라미터 = 파라미터;
                        kh_Access_Model.값 = 값;
                        kh_DB_Handle_Model.프로시저 = new ServiceReference_khbox.Access_Model[1] { kh_Access_Model };
                        kh_DB_Handle_Model.BroadCasting = 0;
                        kh_Return_Model = 갱신 == 1 ? kh_Service.갱신(kh_DB_Handle_Model, 갱신) : kh_Service.조회(kh_DB_Handle_Model);

                        returnModel.결과 = kh_Return_Model.결과;
                        returnModel.리턴값 = kh_Return_Model.Retrun값;

                        //Console.WriteLine(SP명 + " 리턴결과 : " + kh_Return_Model.결과);
                        //Console.WriteLine(SP명 + " 리턴값 : " + kh_Return_Model.Retrun값);
                        break;

                    //주)혜성
                    case "hssbox":
                        hss_Access_Model.SP명 = SP명;
                        hss_Access_Model.파라미터 = 파라미터;
                        hss_Access_Model.값 = 값;
                        hss_DB_Handle_Model.프로시저 = new ServiceReference_hssbox.Access_Model[1] { hss_Access_Model };
                        hss_DB_Handle_Model.BroadCasting = 0;
                        hss_Return_Model = 갱신 == 1 ? hss_Service.갱신(hss_DB_Handle_Model, 갱신) : hss_Service.조회(hss_DB_Handle_Model);

                        returnModel.결과 = hss_Return_Model.결과;
                        returnModel.리턴값 = hss_Return_Model.Retrun값;
                        
                        //Console.WriteLine(SP명 + " 리턴결과 : " + hss_Return_Model.결과);
                        //Console.WriteLine(SP명 + " 리턴값 : " + hss_Return_Model.Retrun값);
                        break;

                        //구룡
                    case "grbox":
                        gr_Access_Model.SP명 = SP명;
                        gr_Access_Model.파라미터 = 파라미터;
                        gr_Access_Model.값 = 값;
                        gr_DB_Handle_Model.프로시저 = new ServiceReference_grbox1.Access_Model[1] { gr_Access_Model };
                        gr_DB_Handle_Model.BroadCasting = 0;
                        gr_Return_Model = 갱신 == 1 ? gr_Service.갱신(gr_DB_Handle_Model, 갱신) : gr_Service.조회(gr_DB_Handle_Model);

                        returnModel.결과 = gr_Return_Model.결과;
                        returnModel.리턴값 = gr_Return_Model.Retrun값;
                        
                        //Console.WriteLine(SP명 + " 리턴결과 : " + gr_Return_Model.결과);
                        //Console.WriteLine(SP명 + " 리턴값 : " + gr_Return_Model.Retrun값);
                        break;

                        //대세
                    case "dsbox":
                        ds_Access_Model.SP명 = SP명;
                        ds_Access_Model.파라미터 = 파라미터;
                        ds_Access_Model.값 = 값;
                        ds_DB_Handle_Model.프로시저 = new ServiceReference_dsbox.Access_Model[1] { ds_Access_Model };
                        ds_DB_Handle_Model.BroadCasting = 0;
                        ds_Return_Model = 갱신 == 1 ? ds_Service.갱신(ds_DB_Handle_Model, 갱신) : ds_Service.조회(ds_DB_Handle_Model);

                        returnModel.결과 = ds_Return_Model.결과;
                        returnModel.리턴값 = ds_Return_Model.Retrun값;
                        
                        //Console.WriteLine(SP명 + " 리턴결과 : " + ds_Return_Model.결과);
                        //Console.WriteLine(SP명 + " 리턴값 : " + ds_Return_Model.Retrun값);
                        break;

                        //두성
                    case "dubox":
                        du_Access_Model.SP명 = SP명;
                        du_Access_Model.파라미터 = 파라미터;
                        du_Access_Model.값 = 값;
                        du_DB_Handle_Model.프로시저 = new ServiceReference_dubox.Access_Model[1] { du_Access_Model };
                        du_DB_Handle_Model.BroadCasting = 0;
                        du_Return_Model = 갱신 == 1 ? du_Service.갱신(du_DB_Handle_Model, 갱신) : du_Service.조회(du_DB_Handle_Model);

                        returnModel.결과 = du_Return_Model.결과;
                        returnModel.리턴값 = du_Return_Model.Retrun값;
                        
                        //Console.WriteLine(SP명 + " 리턴결과 : " + du_Return_Model.결과);
                        //Console.WriteLine(SP명 + " 리턴값 : " + du_Return_Model.Retrun값);
                        break;

                        //성주
                    case "sjbox":
                        sj_Access_Model.SP명 = SP명;
                        sj_Access_Model.파라미터 = 파라미터;
                        sj_Access_Model.값 = 값;
                        sj_DB_Handle_Model.프로시저 = new ServiceReference_sjbox.Access_Model[1] { sj_Access_Model };
                        sj_DB_Handle_Model.BroadCasting = 0;
                        sj_Return_Model = 갱신 == 1 ? sj_Service.갱신(sj_DB_Handle_Model, 갱신) : sj_Service.조회(sj_DB_Handle_Model);

                        returnModel.결과 = sj_Return_Model.결과;
                        returnModel.리턴값 = sj_Return_Model.Retrun값;
                        
                        //Console.WriteLine(SP명 + " 리턴결과 : " + sj_Return_Model.결과);
                        //Console.WriteLine(SP명 + " 리턴값 : " + sj_Return_Model.Retrun값);
                        break;

                        //대양
                    case "dybox":
                        dy_Access_Model.SP명 = SP명;
                        dy_Access_Model.파라미터 = 파라미터;
                        dy_Access_Model.값 = 값;
                        dy_DB_Handle_Model.프로시저 = new ServiceReference_dybox.Access_Model[1] { dy_Access_Model };
                        dy_DB_Handle_Model.BroadCasting = 0;
                        dy_Return_Model = 갱신 == 1 ? dy_Service.갱신(dy_DB_Handle_Model, 갱신) : dy_Service.조회(dy_DB_Handle_Model);

                        returnModel.결과 = dy_Return_Model.결과;
                        returnModel.리턴값 = dy_Return_Model.Retrun값;
                        
                        //Console.WriteLine(SP명 + " 리턴결과 : " + dy_Return_Model.결과);
                        //Console.WriteLine(SP명 + " 리턴값 : " + dy_Return_Model.Retrun값);
                        break;

                        //우리
                    case "wrbox":
                        wr_Access_Model.SP명 = SP명;
                        wr_Access_Model.파라미터 = 파라미터;
                        wr_Access_Model.값 = 값;
                        wr_DB_Handle_Model.프로시저 = new ServiceReference_wrbox.Access_Model[1] { wr_Access_Model };
                        wr_DB_Handle_Model.BroadCasting = 0;
                        wr_Return_Model = 갱신 == 1 ? wr_Service.갱신(wr_DB_Handle_Model, 갱신) : wr_Service.조회(wr_DB_Handle_Model);

                        returnModel.결과 = wr_Return_Model.결과;
                        returnModel.리턴값 = wr_Return_Model.Retrun값;
                        
                        //Console.WriteLine(SP명 + " 리턴결과 : " + wr_Return_Model.결과);
                        //Console.WriteLine(SP명 + " 리턴값 : " + wr_Return_Model.Retrun값);
                        break;

                        //지온
                    case "jobox":
                        jo_Access_Model.SP명 = SP명;
                        jo_Access_Model.파라미터 = 파라미터;
                        jo_Access_Model.값 = 값;
                        jo_DB_Handle_Model.프로시저 = new ServiceReference_jobox.Access_Model[1] { jo_Access_Model };
                        jo_DB_Handle_Model.BroadCasting = 0;
                        jo_Return_Model = 갱신 == 1 ? jo_Service.갱신(jo_DB_Handle_Model, 갱신) : jo_Service.조회(jo_DB_Handle_Model);

                        returnModel.결과 = jo_Return_Model.결과;
                        returnModel.리턴값 = jo_Return_Model.Retrun값;
                        
                        //Console.WriteLine(SP명 + " 리턴결과 : " + jo_Return_Model.결과);
                        //Console.WriteLine(SP명 + " 리턴값 : " + jo_Return_Model.Retrun값);
                        break;

                        //누리
                    case "nrbox":
                        nr_Access_Model.SP명 = SP명;
                        nr_Access_Model.파라미터 = 파라미터;
                        nr_Access_Model.값 = 값;
                        nr_DB_Handle_Model.프로시저 = new ServiceReference_nrbox.Access_Model[1] { nr_Access_Model };
                        nr_DB_Handle_Model.BroadCasting = 0;
                        nr_Return_Model = 갱신 == 1 ? nr_Service.갱신(nr_DB_Handle_Model, 갱신) : nr_Service.조회(nr_DB_Handle_Model);

                        returnModel.결과 = nr_Return_Model.결과;
                        returnModel.리턴값 = nr_Return_Model.Retrun값;

                        //Console.WriteLine(SP명 + " 리턴결과 : " + nr_Return_Model.결과);
                        //Console.WriteLine(SP명 + " 리턴값 : " + nr_Return_Model.Retrun값);
                        break;

                        //신우
                    case "swbox":
                        sw_Access_Model.SP명 = SP명;
                        sw_Access_Model.파라미터 = 파라미터;
                        sw_Access_Model.값 = 값;
                        sw_DB_Handle_Model.프로시저 = new ServiceReference_swbox.Access_Model[1] { sw_Access_Model };
                        sw_DB_Handle_Model.BroadCasting = 0;
                        sw_Return_Model = 갱신 == 1 ? sw_Service.갱신(sw_DB_Handle_Model, 갱신) : sw_Service.조회(sw_DB_Handle_Model);

                        returnModel.결과 = sw_Return_Model.결과;
                        returnModel.리턴값 = sw_Return_Model.Retrun값;

                        //Console.WriteLine(SP명 + " 리턴결과 : " + sw_Return_Model.결과);
                        //Console.WriteLine(SP명 + " 리턴값 : " + sw_Return_Model.Retrun값);
                        break;

                        //혜성
                    case "hsbox":
                    case "hsbox1":
                        hs_Access_Model.SP명 = SP명;
                        hs_Access_Model.파라미터 = 파라미터;
                        hs_Access_Model.값 = 값;
                        hs_DB_Handle_Model.프로시저 = new ServiceReference_hsbox.Access_Model[1] { hs_Access_Model };
                        hs_DB_Handle_Model.BroadCasting = 0;
                        hs_Return_Model = 갱신 == 1 ? hs_Service.갱신(hs_DB_Handle_Model, 갱신) : hs_Service.조회(hs_DB_Handle_Model);

                        returnModel.결과 = hs_Return_Model.결과;
                        returnModel.리턴값 = hs_Return_Model.Retrun값;
                       
                        //Console.WriteLine(SP명 + " 리턴결과 : " + hs_Return_Model.결과);
                        //Console.WriteLine(SP명 + " 리턴값 : " + hs_Return_Model.Retrun값);
                        break;

                        //오주
                    case "ojbox":
                        oj_Access_Model.SP명 = SP명;
                        oj_Access_Model.파라미터 = 파라미터;
                        oj_Access_Model.값 = 값;
                        oj_DB_Handle_Model.프로시저 = new ServiceReference_real_ojbox.Access_Model[1] { oj_Access_Model };
                        oj_DB_Handle_Model.BroadCasting = 0;
                        oj_Return_Model = 갱신 == 1 ? oj_Service.갱신(oj_DB_Handle_Model, 갱신) : oj_Service.조회(oj_DB_Handle_Model);

                        returnModel.결과 = oj_Return_Model.결과;
                        returnModel.리턴값 = oj_Return_Model.Retrun값;
                        
                        //Console.WriteLine(SP명 + " 리턴결과 : " + oj_Return_Model.결과);
                        //Console.WriteLine(SP명 + " 리턴값 : " + oj_Return_Model.Retrun값);
                        break;

                        //미래
                    case "miraebox":
                        mirae_Access_Model.SP명 = SP명;
                        mirae_Access_Model.파라미터 = 파라미터;
                        mirae_Access_Model.값 = 값;
                        mirae_DB_Handle_Model.프로시저 = new ServiceReference_mirae.Access_Model[1] { mirae_Access_Model };
                        mirae_DB_Handle_Model.BroadCasting = 0;
                        mirae_Return_Model = 갱신 == 1 ? mirae_Service.갱신(mirae_DB_Handle_Model, 갱신) : mirae_Service.조회(mirae_DB_Handle_Model);

                        returnModel.결과 = mirae_Return_Model.결과;
                        returnModel.리턴값 = mirae_Return_Model.Retrun값;
                        
                        //Console.WriteLine(SP명 + " 리턴결과 : " + mirae_Return_Model.결과);
                        //Console.WriteLine(SP명 + " 리턴값 : " + mirae_Return_Model.Retrun값);
                        break;

                        //광성
                    case "ksbox":
                        ks_Access_Model.SP명 = SP명;
                        ks_Access_Model.파라미터 = 파라미터;
                        ks_Access_Model.값 = 값;
                        ks_DB_Handle_Model.프로시저 = new ServiceReference_ksbox.Access_Model[1] { ks_Access_Model };
                        ks_DB_Handle_Model.BroadCasting = 0;
                        ks_Return_Model = 갱신 == 1 ? Ks_Service.갱신(ks_DB_Handle_Model, 갱신) : Ks_Service.조회(ks_DB_Handle_Model);

                        returnModel.결과 = ks_Return_Model.결과;
                        returnModel.리턴값 = ks_Return_Model.Retrun값;

                        //Console.WriteLine(SP명 + " 리턴결과 : " + ks_Return_Model.결과);
                        //Console.WriteLine(SP명 + " 리턴값 : " + ks_Return_Model.Retrun값);
                        break;
                        
                        //하나
                    case "hnbox":
                        hn_Access_Model.SP명 = SP명;
                        hn_Access_Model.파라미터 = 파라미터;
                        hn_Access_Model.값 = 값;
                        hn_DB_Handle_Model.프로시저 = new ServiceReference_hnbox.Access_Model[1] { hn_Access_Model };
                        hn_DB_Handle_Model.BroadCasting = 0;
                        hn_Return_Model = 갱신 == 1 ? hn_Service.갱신(hn_DB_Handle_Model, 갱신) : hn_Service.조회(hn_DB_Handle_Model);

                        returnModel.결과 = hn_Return_Model.결과;
                        returnModel.리턴값 = hn_Return_Model.Retrun값;

                        //Console.WriteLine(SP명 + " 리턴결과 : " + hn_Return_Model.결과);
                        //Console.WriteLine(SP명 + " 리턴값 : " + hn_Return_Model.Retrun값);
                        break;

                        //선호
                    case "shbox":
                        sh_Access_Model.SP명 = SP명;
                        sh_Access_Model.파라미터 = 파라미터;
                        sh_Access_Model.값 = 값;
                        sh_DB_Handle_Model.프로시저 = new ServiceReference_shbox.Access_Model[1] { sh_Access_Model };
                        sh_DB_Handle_Model.BroadCasting = 0;
                        sh_Return_Model = 갱신 == 1 ? sh_Service.갱신(sh_DB_Handle_Model, 갱신) : sh_Service.조회(sh_DB_Handle_Model);

                        returnModel.결과 = sh_Return_Model.결과;
                        returnModel.리턴값 = sh_Return_Model.Retrun값;

                        //Console.WriteLine(SP명 + " 리턴결과 : " + sh_Return_Model.결과);
                        //Console.WriteLine(SP명 + " 리턴값 : " + sh_Return_Model.Retrun값);
                        break;

                        //광무
                    case "kmbox":
                        km_Access_Model.SP명 = SP명;
                        km_Access_Model.파라미터 = 파라미터;
                        km_Access_Model.값 = 값;
                        km_DB_Handle_Model.프로시저 = new ServiceReference_kmbox.Access_Model[1] { km_Access_Model };
                        km_DB_Handle_Model.BroadCasting = 0;
                        km_Return_Model = 갱신 == 1 ? km_Service.갱신(km_DB_Handle_Model, 갱신) : km_Service.조회(km_DB_Handle_Model);
                        
                        returnModel.결과 = km_Return_Model.결과;
                        returnModel.리턴값 = km_Return_Model.Retrun값;

                        //Console.WriteLine(SP명 + " 리턴결과 : " + km_Return_Model.결과);
                        //Console.WriteLine(SP명 + " 리턴값 : " + km_Return_Model.Retrun값);
                        break;

                        //유창
                    case "ycbox":
                        yc_Access_Model.SP명 = SP명;
                        yc_Access_Model.파라미터 = 파라미터;
                        yc_Access_Model.값 = 값;
                        yc_DB_Handle_Model.프로시저 = new ServiceReference_ycbox.Access_Model[1] { yc_Access_Model };
                        yc_DB_Handle_Model.BroadCasting = 0;
                        yc_Return_Model = 갱신 == 1 ? yc_Service.갱신(yc_DB_Handle_Model, 갱신) : yc_Service.조회(yc_DB_Handle_Model);

                        returnModel.결과 = yc_Return_Model.결과;
                        returnModel.리턴값 = yc_Return_Model.Retrun값;

                        //Console.WriteLine(SP명 + " 리턴결과 : " + yc_Return_Model.결과);
                        //Console.WriteLine(SP명 + " 리턴값 : " + yc_Return_Model.Retrun값);
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
                //내포
                case "opbox":
                    op_Access_Model.SP명 = SP명;
                    op_Access_Model.파라미터 = 파라미터;
                    op_Access_Model.값 = 값;

                    op_DB_Handle_Model.프로시저 = new ServiceReference_opbox.Access_Model[1] { op_Access_Model };
                    op_DB_Handle_Model.BroadCasting = 1;
                    op_DB_Handle_Model.BroadCastingName = "생산입력완료";
                    op_DB_Handle_Model.inputPkey = "14201";
                    op_DB_Handle_Model.inputContent = parameter;
                    op_Return_Model = 갱신 == 1 ? op_Service.갱신(op_DB_Handle_Model, 갱신) : op_Service.조회(op_DB_Handle_Model);

                    returnModel.결과 = op_Return_Model.결과;
                    returnModel.리턴값 = op_Return_Model.Retrun값;
                    break;

                //내포
                case "npbox":
                    np_Access_Model.SP명 = SP명;
                    np_Access_Model.파라미터 = 파라미터;
                    np_Access_Model.값 = 값;

                    np_DB_Handle_Model.프로시저 = new ServiceReference_npbox.Access_Model[1] { np_Access_Model };
                    np_DB_Handle_Model.BroadCasting = 1;
                    np_DB_Handle_Model.BroadCastingName = "생산입력완료";
                    np_DB_Handle_Model.inputPkey = "14201";
                    np_DB_Handle_Model.inputContent = parameter;
                    np_Return_Model = 갱신 == 1 ? np_Service.갱신(np_DB_Handle_Model, 갱신) : np_Service.조회(np_DB_Handle_Model);

                    returnModel.결과 = np_Return_Model.결과;
                    returnModel.리턴값 = np_Return_Model.Retrun값;
                    break;

                //금화
                case "khbox":
                    kh_Access_Model.SP명 = SP명;
                    kh_Access_Model.파라미터 = 파라미터;
                    kh_Access_Model.값 = 값;

                    kh_DB_Handle_Model.프로시저 = new ServiceReference_khbox.Access_Model[1] { kh_Access_Model };
                    kh_DB_Handle_Model.BroadCasting = 1;
                    kh_DB_Handle_Model.BroadCastingName = "생산입력완료";
                    kh_DB_Handle_Model.inputPkey = "14201";
                    kh_DB_Handle_Model.inputContent = parameter;
                    kh_Return_Model = 갱신 == 1 ? kh_Service.갱신(kh_DB_Handle_Model, 갱신) : kh_Service.조회(kh_DB_Handle_Model);

                    returnModel.결과 = kh_Return_Model.결과;
                    returnModel.리턴값 = kh_Return_Model.Retrun값;
                    break;

                //주)혜성
                case "hssbox":
                    hss_Access_Model.SP명 = SP명;
                    hss_Access_Model.파라미터 = 파라미터;
                    hss_Access_Model.값 = 값;

                    hss_DB_Handle_Model.프로시저 = new ServiceReference_hssbox.Access_Model[1] { hss_Access_Model };
                    hss_DB_Handle_Model.BroadCasting = 1;
                    hss_DB_Handle_Model.BroadCastingName = "생산입력완료";
                    hss_DB_Handle_Model.inputPkey = "14201";
                    hss_DB_Handle_Model.inputContent = parameter;
                    hss_Return_Model = 갱신 == 1 ? hss_Service.갱신(hss_DB_Handle_Model, 갱신) : hss_Service.조회(hss_DB_Handle_Model);

                    returnModel.결과 = hss_Return_Model.결과;
                    returnModel.리턴값 = hss_Return_Model.Retrun값;
                    break;

                   //대세
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

                    //두성
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


                    //성주
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

                    //대양
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

                    //우리
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
                    break;

                    //지온
                case "jobox":
                    jo_Access_Model.SP명 = SP명;
                    jo_Access_Model.파라미터 = 파라미터;
                    jo_Access_Model.값 = 값;

                    jo_DB_Handle_Model.프로시저 = new ServiceReference_jobox.Access_Model[1] { jo_Access_Model };
                    jo_DB_Handle_Model.BroadCasting = 1;
                    jo_DB_Handle_Model.BroadCastingName = "생산입력완료";
                    jo_DB_Handle_Model.inputPkey = "14201";
                    jo_DB_Handle_Model.inputContent = parameter;
                    jo_Return_Model = 갱신 == 1 ? jo_Service.갱신(jo_DB_Handle_Model, 갱신) : jo_Service.조회(jo_DB_Handle_Model);

                    returnModel.결과 = jo_Return_Model.결과;
                    returnModel.리턴값 = jo_Return_Model.Retrun값;
                    break;

                    //누리
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
                    break;

                    //신우
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
                    break;

                    //혜성
                case "hsbox":
                case "hsbox1":
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
                    break;

                    //오주
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

                    break;

                    //미래
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
                    break;

                    //광성
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
                    break;

                    //하나
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
                    break;

                    //선호
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
                    break;

                    //광무
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
                    break;

                    //유창
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
                    break;

                    //구룡
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
                    break;
                
                default:
                    break;
            }
            return returnModel;
        }
    }
}
