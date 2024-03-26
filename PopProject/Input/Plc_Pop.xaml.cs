using Newtonsoft.Json.Linq;
using PopProject.Detail;
using PopProject.input;
using PopProject.KeyBoard;
using PopProject.KeyBoard2;
using PopProject.MessageBox;
using PopProject.UserControl_;
using PopProject.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace PopProject.Input
{
    /// <summary>
    /// Plc_Pop.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Plc_Pop : Window
    {
        string[] psize; //규격 배열
        MainModel mainModel;
        public InputModel inputModel;
        Socket mainSock;
        Win_KeyBoard win_KeyBoard; //일반 키보드
        Win_KeyBoard2 win_KeyBoard2; //소숫점 키보드
        private bool checkChangeButton = false;
        private bool CNS_Send = false;
        private int TextChangeState = 0;

        public Plc_Pop(MainModel mainModel)
        {
            InitializeComponent();
            this.mainModel = mainModel;
            SetPlcData();
            textChange(0);
        }

        private void textChange(int state)
        {
            //기본 텍스트
            if (state == 0)
            {
                if (SettingJsonManager.mercName.Equals("후렉소1호기"))
                {
                    작업모델_.Text = "작업모델";
                    작업지시_.Text = "생산매수";
                    장_.Text = "장";
                    폭_.Text = "폭";
                    고_.Text = "고";
                    d3909_.Text = "갭";
                    d3910_.Text = "앞날개";
                    d3911_.Text = "미미(F1)";
                    d3912_.Text = "컷팅(F2)";
                    d3913_.Text = "뒷날개";
                    d3914_.Text = "";
                    d3915_.Text = "";

                    tb1_.Text = "A 위치";
                    tb2_.Text = "B 위치";
                    tb3_.Text = "C 위치";
                    tb4_.Text = "D 위치";
                    tb5_.Text = "E 위치";
                    tb6_.Text = "F1 위치";
                    tb7_.Text = "F2 위치";
                    tb8_.Text = "G 위치";
                    tb9_.Text = "O 위치";
                    tb10_.Text = "P1 위치";
                    tb11_.Text = "P2 위치";
                    tb12_.Text = "P3 위치";
                    tb13_.Text = "P4 위치";
                    tb14_.Text = "뒷날개";
                    tb15_.Text = "Spare";

                    tb16_.Text = "슬로타      레지스타";
                    tb17_.Text = "다이캇타    레지스타";
                    tb18_.Text = "1도 프레스롤";
                    tb19_.Text = "1도 프레스";
                    tb20_.Text = "2도 프레스롤";
                    tb21_.Text = "2도 프레스";
                    tb22_.Text = "3도 프레스롤";
                    tb23_.Text = "3도 프레스";
                    tb24_.Text = "4도 프레스롤";
                    tb25_.Text = "4도 프레스";
                    tb26_.Text = "Spare";
                    tb27_.Text = "Spare";
                    tb28_.Text = "Spare";
                    tb29_.Text = "Spare";
                    tb30_.Text = "급지부 진입 갭";

                    tb31_.Text = "급지부 진입 롤갭";
                    tb32_.Text = "슬로타 진입 보조괘선 갭";
                    tb33_.Text = "슬로타 중간 본 괘선 갭";
                    tb34_.Text = "슬로타 배출 연결롤 갭";
                    tb35_.Text = "다이캇타진입연결롤 갭";
                    tb36_.Text = "글루어벨트 진입 갭F1";
                    tb37_.Text = "글루어벨트 진입 갭F2";
                    tb38_.Text = "글루어프레스 갭A";
                    tb39_.Text = "글루어프레스 갭B";
                    tb40_.Text = "Spare";
                    tb41_.Text = "Spare";
                    tb42_.Text = "Spare";
                    tb43_.Text = "Spare";
                    tb44_.Text = "Spare";
                    tb45_.Text = "Spare";

                    tb46_.Text = "Spare";
                    tb47_.Text = "Spare";
                    tb48_.Text = "Spare";
                    tb49_.Text = "Spare";
                    tb50_.Text = "Spare";
                    tb51_.Text = "Spare";
                    tb52_.Text = "Spare";
                    tb53_.Text = "Spare";
                    tb54_.Text = "Spare";
                    tb55_.Text = "Spare";
                    tb56_.Text = "Spare";
                    tb57_.Text = "Spare";
                    tb58_.Text = "Spare";
                    tb59_.Text = "Spare";
                    tb60_.Text = "Spare";
                }
                else if (SettingJsonManager.mercName.Equals("인라인2호기"))
                {
                    작업모델_.Text = "작업모델";
                    작업지시_.Text = "생산매수";
                    장_.Text = "장";
                    폭_.Text = "폭";
                    고_.Text = "고";
                    d3909_.Text = "갭";
                    d3910_.Text = "앞날개";
                    d3911_.Text = "미미(F1)";
                    d3912_.Text = "컷팅(F2)";
                    d3913_.Text = "뒷날개";
                    d3914_.Text = "";
                    d3915_.Text = "";

                    tb1_.Text = "A 위치";
                    tb2_.Text = "B 위치";
                    tb3_.Text = "C 위치";
                    tb4_.Text = "D 위치";
                    tb5_.Text = "E 위치";
                    tb6_.Text = "F1 위치";
                    tb7_.Text = "F2 위치";
                    tb8_.Text = "G 위치";
                    tb9_.Text = "O 위치";
                    tb10_.Text = "P1 위치";
                    tb11_.Text = "P2 위치";
                    tb12_.Text = "P3 위치";
                    tb13_.Text = "P4 위치";
                    tb14_.Text = "뒷날개";
                    tb15_.Text = "Spare";

                    tb16_.Text = "슬로타      레지스타";
                    tb17_.Text = "다이캇타    레지스타";
                    tb18_.Text = "1도 프레스롤";
                    tb19_.Text = "1도 프레스";
                    tb20_.Text = "2도 프레스롤";
                    tb21_.Text = "2도 프레스";
                    tb22_.Text = "3도 프레스롤";
                    tb23_.Text = "3도 프레스";
                    tb24_.Text = "4도 프레스롤";
                    tb25_.Text = "4도 프레스";
                    tb26_.Text = "Spare";
                    tb27_.Text = "Spare";
                    tb28_.Text = "Spare";
                    tb29_.Text = "Spare";
                    tb30_.Text = "급지부 진입 갭";

                    tb31_.Text = "급지부 진입 롤갭";
                    tb32_.Text = "슬로타 진입 보조괘선 갭";
                    tb33_.Text = "슬로타 중간 본 괘선 갭";
                    tb34_.Text = "슬로타 배출 연결롤 갭";
                    tb35_.Text = "다이캇타진입연결롤 갭";
                    tb36_.Text = "글루어벨트 진입 갭F1";
                    tb37_.Text = "글루어벨트 진입 갭F2";
                    tb38_.Text = "글루어프레스 갭A";
                    tb39_.Text = "글루어프레스 갭B";
                    tb40_.Text = "Spare";
                    tb41_.Text = "Spare";
                    tb42_.Text = "Spare";
                    tb43_.Text = "Spare";
                    tb44_.Text = "Spare";
                    tb45_.Text = "Spare";

                    tb46_.Text = "Spare";
                    tb47_.Text = "Spare";
                    tb48_.Text = "Spare";
                    tb49_.Text = "Spare";
                    tb50_.Text = "Spare";
                    tb51_.Text = "Spare";
                    tb52_.Text = "Spare";
                    tb53_.Text = "Spare";
                    tb54_.Text = "Spare";
                    tb55_.Text = "Spare";
                    tb56_.Text = "Spare";
                    tb57_.Text = "Spare";
                    tb58_.Text = "Spare";
                    tb59_.Text = "Spare";
                    tb60_.Text = "Spare";

                }
                else if (SettingJsonManager.mercName.Equals("인라인3호기"))
                {
                    작업모델_.Text = "작업모델";
                    작업지시_.Text = "생산매수";
                    장_.Text = "장";
                    폭_.Text = "폭";
                    고_.Text = "고";
                    d3909_.Text = "갭"; 
                    d3910_.Text = "앞날개";
                    d3911_.Text = "미미(F1)";
                    d3912_.Text = "컷팅(F2)";
                    d3913_.Text = "";
                    d3914_.Text = "";
                    d3915_.Text = "";

                    tb1_.Text  = "A 위치";
                    tb2_.Text  = "B 위치";
                    tb3_.Text  = "C 위치";
                    tb4_.Text  = "D 위치";
                    tb5_.Text  = "E 위치";
                    tb6_.Text  = "F1 위치";
                    tb7_.Text  = "F2 위치";
                    tb8_.Text  = "G 위치";
                    tb9_.Text  = "O 위치";
                    tb10_.Text = "P1 위치";
                    tb11_.Text = "P2 위치";
                    tb12_.Text = "P3 위치";
                    tb13_.Text = "P4 위치";
                    tb14_.Text = "Spare";
                    tb15_.Text = "Spare";
                    
                    tb16_.Text = "Spare";
                    tb17_.Text = "D레지스타";
                    tb18_.Text = "1도 프레스 롤";
                    tb19_.Text = "1도 프레스";
                    tb20_.Text = "2도 프레스 롤";
                    tb21_.Text = "2도 프레서";
                    tb22_.Text = "3도 프레스 롤";
                    tb23_.Text = "3도 프레스";
                    tb24_.Text = "4도 프레스 롤";
                    tb25_.Text = "4도 프레스";
                    tb26_.Text = "Spare";
                    tb27_.Text = "Spare";
                    tb28_.Text = "Spare";
                    tb29_.Text = "Spare";
                    tb30_.Text = "급지부 진입 갭";

                    tb31_.Text = "급지부 롤갭";
                    tb32_.Text = "슬로타      보조괘선 갭";
                    tb33_.Text = "슬로타      본괘선 갭";
                    tb34_.Text = "슬로타      연결 갭";
                    tb35_.Text = "다이캇타    롤 갭";
                    tb36_.Text = "다이캇타    연결 갭";
                    tb37_.Text = "Spare";
                    tb38_.Text = "글루어 벨트 진입 갭F1";
                    tb39_.Text = "글루어 벨트 진입 갭F2";
                    tb40_.Text = "글루어 배출 갭 프레스A";
                    tb41_.Text = "글루어 배출 갭 프레스B";
                    tb42_.Text = "PE가이드";
                    tb43_.Text = "Spare";
                    tb44_.Text = "보조괘선 갭(진입)";
                    tb45_.Text = "본괘선 갭   (중간)";
                    
                    tb46_.Text = "괘선 연결롤 갭(배출)";
                    tb47_.Text = "중간연결 갭 (중간)";
                    tb48_.Text = "Spare";
                    tb49_.Text = "Spare";
                    tb50_.Text = "Spare";
                    tb51_.Text = "Spare";
                    tb52_.Text = "Spare";
                    tb53_.Text = "Spare";
                    tb54_.Text = "Spare";
                    tb55_.Text = "Spare";
                    tb56_.Text = "Spare";
                    tb57_.Text = "Spare";
                    tb58_.Text = "Spare";
                    tb59_.Text = "Spare";
                    tb60_.Text = "Spare";
                }
            }
            //DB명
            else if (state == 1)
            {
                tb1_.Text = "Recipe1";
                tb2_.Text = "Recipe2";
                tb3_.Text = "Recipe3";
                tb4_.Text = "Recipe4";
                tb5_.Text = "Recipe5";
                tb6_.Text = "Recipe6";
                tb7_.Text = "Recipe7";
                tb8_.Text = "Recipe8";
                tb9_.Text = "Recipe9";
                tb10_.Text = "Recipe10";
                tb11_.Text = "Recipe11";
                tb12_.Text = "Recipe12";

                tb13_.Text = "Recipe12";
                tb14_.Text = "Recipe14";
                tb15_.Text = "Recipe15";
                tb16_.Text = "Recipe16";
                tb17_.Text = "Recipe17";
                tb18_.Text = "Recipe18";
                tb19_.Text = "Recipe19";
                tb20_.Text = "Recipe20";
                tb21_.Text = "Recipe21";
                tb22_.Text = "Recipe22";
                tb23_.Text = "Recipe23";
                tb24_.Text = "Recipe24";

                tb25_.Text = "Recipe25";
                tb26_.Text = "Recipe26";
                tb27_.Text = "Recipe27";
                tb28_.Text = "Recipe28";
                tb29_.Text = "Recipe29";
                tb30_.Text = "Recipe30";
                tb31_.Text = "Recipe31";
                tb32_.Text = "Recipe32";
                tb33_.Text = "Recipe33";
                tb34_.Text = "Recipe34";
                tb35_.Text = "Recipe35";
                tb36_.Text = "Recipe36";

                tb37_.Text = "Recipe37";
                tb38_.Text = "Recipe38";
                tb39_.Text = "Recipe39";
                tb40_.Text = "Recipe40";
                tb41_.Text = "Recipe41";
                tb42_.Text = "Recipe42";
                tb43_.Text = "Recipe43";
                tb44_.Text = "Recipe44";
                tb45_.Text = "Recipe45";
                tb46_.Text = "Recipe46";
                tb47_.Text = "Recipe47";
                tb48_.Text = "Recipe48";

                tb49_.Text = "Recipe49";
                tb50_.Text = "Recipe50";
                tb51_.Text = "Recipe51";
                tb52_.Text = "Recipe52";
                tb53_.Text = "Recipe53";
                tb54_.Text = "Recipe54";
                tb55_.Text = "Recipe55";
                tb56_.Text = "Recipe56";
                tb57_.Text = "Recipe57";
                tb58_.Text = "Recipe58";
                tb59_.Text = "Recipe59";
                tb60_.Text = "Recipe60";
            }
            //PLC 변수명
            else if (state == 2)
            {

                작업지시_.Text = "D3903~4";
                작업모델_.Text = "D3905";
                장_.Text = "D3906";
                폭_.Text = "D3907";
                고_.Text = "D3908";
                d3909_.Text = "D3909"; //갭
                d3910_.Text = "D3910"; //앞날개
                d3911_.Text = "D3911"; //미미
                d3912_.Text = "D3912"; //컷팅
                d3913_.Text = "D3913";
                d3914_.Text = "D3914";
                d3915_.Text = "D3915";

                tb1_.Text = "D3916";
                tb2_.Text = "D3917";
                tb3_.Text = "D3918";
                tb4_.Text = "D3919";
                tb5_.Text = "D3920";
                tb6_.Text = "D3921";
                tb7_.Text = "D3922";
                tb8_.Text = "D3923";
                tb9_.Text = "D3924";
                tb10_.Text = "D3925";
                tb11_.Text = "D3926";
                tb12_.Text = "D3927";

                tb13_.Text = "D3928";
                tb14_.Text = "D3929"; //뒷날개
                tb15_.Text = "D3930";
                tb16_.Text = "D3931";
                tb17_.Text = "D3932";
                tb18_.Text = "D3933";
                tb19_.Text = "D3934";
                tb20_.Text = "D3935";
                tb21_.Text = "D3936";
                tb22_.Text = "D3937";
                tb23_.Text = "D3938";
                tb24_.Text = "D3939";

                tb25_.Text = "D3940";
                tb26_.Text = "D3941";
                tb27_.Text = "D3942";
                tb28_.Text = "D3943";
                tb29_.Text = "D3944";
                tb30_.Text = "D3945";
                tb31_.Text = "D3946";
                tb32_.Text = "D3947";
                tb33_.Text = "D3948";
                tb34_.Text = "D3949";
                tb35_.Text = "D3950";
                tb36_.Text = "D3951";

                tb37_.Text = "D3952";
                tb38_.Text = "D3953";
                tb39_.Text = "D3954";
                tb40_.Text = "D3955";
                tb41_.Text = "D3956";
                tb42_.Text = "D3957";
                tb43_.Text = "D3958";
                tb44_.Text = "D3959";
                tb45_.Text = "D3960";
                tb46_.Text = "D3961";
                tb47_.Text = "D3962";
                tb48_.Text = "D3963";

                tb49_.Text = "D3964";
                tb50_.Text = "D3965";
                tb51_.Text = "D3966";
                tb52_.Text = "D3967";
                tb53_.Text = "D3968";
                tb54_.Text = "D3969";
                tb55_.Text = "D3970";
                tb56_.Text = "D3971";
                tb57_.Text = "D3972";
                tb58_.Text = "D3973";
                tb59_.Text = "D3974";
                tb60_.Text = "D3975";
            }
        }

        public static DependencyObject FindControl(DependencyObject root, string controlName)
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(root);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);
                if (child is FrameworkElement && ((FrameworkElement)child).Name == controlName)
                    return child;

                var foundChild = FindControl(child, controlName);
                if (foundChild != null)
                    return foundChild;
            }
            return null;
        }

        //초기화면 세팅
        private void SetPlcData()
        {
            ip.Text = SettingJsonManager.ip + " / " + SettingJsonManager.port; //IP
            거래처.Text = mainModel.customSName; //거래처
            품명.Text = mainModel.prodName; //품명
            //psize = mainModel.pSize.Split('*'); //규격
            //작업지시.Text = SettingJsonManager.setCount.ToString();
            //장.Text = psize[0] == null ? "" : psize[0].Trim();
            //폭.Text = psize[1] == null ? "" : psize[1].Trim();
            //고.Text = psize[2] == null ? "" : psize[2].Trim();
          
            //d3910.Text = SettingJsonManager.mimi.ToString(); 
            //switch (mainModel.paperGor.Replace("골", "").Trim())
            //{
            //    case "AB":
            //    case "BA":
            //        SettingJsonManager.Gap = 70;
            //        break;

            //    case "BB":
            //        SettingJsonManager.Gap = 60;
            //        break;

            //    case "A":
            //        SettingJsonManager.Gap = 50;
            //        break;

            //    case "EB":
            //        SettingJsonManager.Gap = 40;
            //        break;

            //    case "B":
            //        SettingJsonManager.Gap = 30;
            //        break;

            //    case "E":
            //        SettingJsonManager.Gap = 20;
            //        break;
            //    default:
            //        break;
            //}
            //d3909.Text = SettingJsonManager.Gap.ToString();

        }

        //PLC IP 세팅
        private void ip_input(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Ip_input ip_Input = new Ip_input();
            ip_Input.ShowDialog();
            ip.Text = SettingJsonManager.ip + " / " + SettingJsonManager.port;
        }

        //DB데이터 조회
        private void DB_Read2(object sender, MouseButtonEventArgs e)
        {
            LogManager.Info("PC조회 버튼 클릭", MethodBase.GetCurrentMethod().DeclaringType);

            Border clickedBorder = (Border)sender;
            clickedBorder.Background = Brushes.Gray;

            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
               SettingJsonManager.serverIndex,
               "[PopCNSSearch2]",
               new string[] { "prodID", "MachineName" },
               new object[] { this.mainModel.prodID, SettingJsonManager.mercName },
               0);

            if (returnData.결과 == 1)
            {
                string ProdID = "";
                string machineName = "";
                int modelNum = 0;
                int workPlan1 = 0;
                int workPlan2 = 0;
                int boxType = 0;
                int pock = 0;
                int jang = 0;
                int go = 0;
                float D3909    = 0; //갭
                float D3910    = 0; //앞날개
                float D3911    = 0; //미미
                float D3912    = 0; //뒷날개
                float D3913    = 0; //
                float D3914    = 0; //
                float D3915    = 0; //
                float spare1   = 0;
                float recipe1  = 0;
                float recipe2  = 0;
                float recipe3  = 0;
                float recipe4  = 0;
                float recipe5  = 0;
                float recipe6  = 0;
                float recipe7  = 0;
                float recipe8  = 0;
                float recipe9  = 0;
                float recipe10 = 0;
                float recipe11 = 0;
                float recipe12 = 0;
                float recipe13 = 0;
                float recipe14 = 0;
                float recipe15 = 0;
                float recipe16 = 0;
                float recipe17 = 0;
                float recipe18 = 0;
                float recipe19 = 0;
                float recipe20 = 0;
                float recipe21 = 0;
                float recipe22 = 0;
                float recipe23 = 0;
                float recipe24 = 0;
                float recipe25 = 0;
                float recipe26 = 0;
                float recipe27 = 0;
                float recipe28 = 0;
                float recipe29 = 0;
                float recipe30 = 0;
                float recipe31 = 0;
                float recipe32 = 0;
                float recipe33 = 0;
                float recipe34 = 0;
                float recipe35 = 0;
                float recipe36 = 0;
                float recipe37 = 0;
                float recipe38 = 0;
                float recipe39 = 0;
                float recipe40 = 0;
                float recipe41 = 0;
                float recipe42 = 0;
                float recipe43 = 0;
                float recipe44 = 0;
                float recipe45 = 0;
                float recipe46 = 0;
                float recipe47 = 0;
                float recipe48 = 0;
                float recipe49 = 0;
                float recipe50 = 0;
                float recipe51 = 0;
                float recipe52 = 0;
                float recipe53 = 0;
                float recipe54 = 0;
                float recipe55 = 0;
                float recipe56 = 0;
                float recipe57 = 0;
                float recipe58 = 0;
                float recipe59 = 0;
                float recipe60 = 0;

                JArray Data_Jarry = JArray.Parse(returnData.리턴값);
                foreach (JObject result in Data_Jarry)
                {
                    ProdID = result["prodID"].ToString();
                    machineName = result["MachineName"].ToString();

                    modelNum = Convert.ToInt32(result["ModelNum"]);//D3902
                    workPlan1 = Convert.ToInt32(result["WorkPlan1"]);//D3903
                    workPlan2 = Convert.ToInt32(result["WorkPlan2"]);//D3904
                    boxType = Convert.ToInt32(result["BoxType"]);//D3905

                    pock = Convert.ToInt32(result["Pock"]);//D3906
                    jang = Convert.ToInt32(result["Jang"]);//D3907
                    go = Convert.ToInt32(result["Go"]);//D3908

                    D3909 = Convert.ToSingle(result["Gap"]);//D3909
                    D3910 = Convert.ToSingle(result["Wing"]);//D3910
                    D3911 = Convert.ToSingle(result["Mimi"]);//D3911
                    D3912 = Convert.ToSingle(result["WonDan"]);//D3912
                    D3913 = Convert.ToSingle(result["WorkSpeed"]);//D3913
                    D3914 = Convert.ToSingle(result["SaveState"]);//D3914
                    D3915 = Convert.ToSingle(result["Spare1"]);//D3915

                    recipe1  = Convert.ToSingle(result["Recipe1"]);//D3916
                    recipe2  = Convert.ToSingle(result["Recipe2"]);//D3917
                    recipe3  = Convert.ToSingle(result["Recipe3"]);//D3918
                    recipe4  = Convert.ToSingle(result["Recipe4"]);//D3919
                    recipe5  = Convert.ToSingle(result["Recipe5"]);//D3920
                    recipe6  = Convert.ToSingle(result["Recipe6"]);//D3921
                    recipe7  = Convert.ToSingle(result["Recipe7"]);//D3922
                    recipe8  = Convert.ToSingle(result["Recipe8"]);//D3923
                    recipe9  = Convert.ToSingle(result["Recipe9"]);//D3924
                    recipe10 = Convert.ToSingle(result["Recipe10"]);//D3925
                    recipe11 = Convert.ToSingle(result["Recipe11"]);//D3926
                    recipe12 = Convert.ToSingle(result["Recipe12"]);//D3927
                    recipe13 = Convert.ToSingle(result["Recipe13"]);//D3928
                    recipe14 = Convert.ToSingle(result["Recipe14"]);//D3929 //2호기 뒷날개
                    recipe15 = Convert.ToSingle(result["Recipe15"]);//D3930
                    recipe16 = Convert.ToSingle(result["Recipe16"]);//D3931
                    recipe17 = Convert.ToSingle(result["Recipe17"]);//D3932
                    recipe18 = Convert.ToSingle(result["Recipe18"]);//D3933
                    recipe19 = Convert.ToSingle(result["Recipe19"]);//D3934
                    recipe20 = Convert.ToSingle(result["Recipe20"]);//D3935
                    recipe21 = Convert.ToSingle(result["Recipe21"]);//D3936
                    recipe22 = Convert.ToSingle(result["Recipe22"]);//D3937
                    recipe23 = Convert.ToSingle(result["Recipe23"]);//D3938
                    recipe24 = Convert.ToSingle(result["Recipe24"]);//D3939
                    recipe25 = Convert.ToSingle(result["Recipe25"]);//D3940
                    recipe26 = Convert.ToSingle(result["Recipe26"]);//D3941
                    recipe27 = Convert.ToSingle(result["Recipe27"]);//D3942
                    recipe28 = Convert.ToSingle(result["Recipe28"]);//D3943
                    recipe29 = Convert.ToSingle(result["Recipe29"]);//D3944
                    recipe30 = Convert.ToSingle(result["Recipe30"]);//D3945
                    recipe31 = Convert.ToSingle(result["Recipe31"]);//D3946
                    recipe32 = Convert.ToSingle(result["Recipe32"]);//D3947
                    recipe33 = Convert.ToSingle(result["Recipe33"]);//D3948
                    recipe34 = Convert.ToSingle(result["Recipe34"]);//D3949
                    recipe35 = Convert.ToSingle(result["Recipe35"]);//D3950
                    recipe36 = Convert.ToSingle(result["Recipe36"]);//D3951
                    recipe37 = Convert.ToSingle(result["Recipe37"]);//D3952
                    recipe38 = Convert.ToSingle(result["Recipe38"]);//D3953
                    recipe39 = Convert.ToSingle(result["Recipe39"]);//D3954
                    recipe40 = Convert.ToSingle(result["Recipe40"]);//D3955
                    recipe41 = Convert.ToSingle(result["Recipe41"]);//D3956
                    recipe42 = Convert.ToSingle(result["Recipe42"]);//D3957
                    recipe43 = Convert.ToSingle(result["Recipe43"]);//D3958
                    recipe44 = Convert.ToSingle(result["Recipe44"]);//D3959
                    recipe45 = Convert.ToSingle(result["Recipe45"]);//D3960
                    recipe46 = Convert.ToSingle(result["Recipe46"]);//D3961
                    recipe47 = Convert.ToSingle(result["Recipe47"]);//D3962
                    recipe48 = Convert.ToSingle(result["Recipe48"]);//D3963
                    recipe49 = Convert.ToSingle(result["Recipe49"]);//D3964
                    recipe50 = Convert.ToSingle(result["Recipe50"]);//D3965
                    recipe51 = Convert.ToSingle(result["Recipe51"]);//D3966
                    recipe52 = Convert.ToSingle(result["Recipe52"]);//D3967
                    recipe53 = Convert.ToSingle(result["Recipe53"]);//D3968
                    recipe54 = Convert.ToSingle(result["Recipe54"]);//D3969
                    recipe55 = Convert.ToSingle(result["Recipe55"]);//D3970
                    recipe56 = Convert.ToSingle(result["Recipe56"]);//D3971
                    recipe57 = Convert.ToSingle(result["Recipe57"]);//D3972
                    recipe58 = Convert.ToSingle(result["Recipe58"]);//D3973
                    recipe59 = Convert.ToSingle(result["Recipe59"]);//D3974
                    recipe60 = Convert.ToSingle(result["Recipe60"]);//D3975
                }

                //박스타입 선택
                int a = boxType;
                switch (a)
                {
                    case 1://1합
                        combo.SelectedIndex = 0; break;
                    case 3://2합
                        combo.SelectedIndex = 1; break;
                    case 5://인쇄물
                        combo.SelectedIndex = 2; break;
                    case 6://H형
                        combo.SelectedIndex = 3; break;
                    case 7://H-Cover형
                        combo.SelectedIndex = 4; break;
                }

                장.Text = jang.ToString();
                폭.Text = pock.ToString();
                고.Text = go.ToString();
                d3909.Text = D3909.ToString();
                d3910.Text = D3910.ToString();
                d3911.Text = D3911.ToString();
                d3912.Text = D3912.ToString();
                d3913.Text = D3913.ToString();
                d3914.Text = D3914.ToString();
                d3915.Text = D3915.ToString();

                //#:1000단위 콤마  ("#,0.0")

                tb1.Text  = recipe1.ToString(); //D3916
                tb2.Text  = recipe2.ToString(); //D3917
                tb3.Text  = recipe3.ToString(); //D3918
                tb4.Text  = recipe4.ToString(); //D3919
                tb5.Text  = recipe5.ToString(); //D3920
                tb6.Text  = recipe6.ToString(); //D3921
                tb7.Text  = recipe7.ToString(); //D3922
                tb8.Text  = recipe8.ToString(); //D3923
                tb9.Text  = recipe9.ToString(); //D3924
                tb10.Text = recipe10.ToString();//D3925
                tb11.Text = recipe11.ToString();//D3926
                tb12.Text = recipe12.ToString();//D3927
                tb13.Text = recipe13.ToString();//D3928
                tb14.Text = recipe14.ToString();//D3929
                tb15.Text = recipe15.ToString();//D3930
                tb16.Text = recipe16.ToString();//D3931
                tb17.Text = recipe17.ToString();//D3932
                tb18.Text = recipe18.ToString();//D3933
                tb19.Text = recipe19.ToString();//D3934
                tb20.Text = recipe20.ToString();//D3935
                tb21.Text = recipe21.ToString();//D3936
                tb22.Text = recipe22.ToString();//D3937
                tb23.Text = recipe23.ToString();//D3938
                tb24.Text = recipe24.ToString();//D3939
                tb25.Text = recipe25.ToString();//D3940
                tb26.Text = recipe26.ToString();//D3941
                tb27.Text = recipe27.ToString();//D3942
                tb28.Text = recipe28.ToString();//D3943
                tb29.Text = recipe29.ToString();//D3944
                tb30.Text = recipe30.ToString();//D3945
                tb31.Text = recipe31.ToString();//D3946
                tb32.Text = recipe32.ToString();//D3947
                tb33.Text = recipe33.ToString();//D3948
                tb34.Text = recipe34.ToString();//D3949
                tb35.Text = recipe35.ToString();//D3950
                tb36.Text = recipe36.ToString();//D3951
                tb37.Text = recipe37.ToString();//D3952
                tb38.Text = recipe38.ToString();//D3953
                tb39.Text = recipe39.ToString();//D3954
                tb40.Text = recipe40.ToString();//D3955
                tb41.Text = recipe41.ToString();//D3956
                tb42.Text = recipe42.ToString();//D3957
                tb43.Text = recipe43.ToString();//D3958
                tb44.Text = recipe44.ToString();//D3959
                tb45.Text = recipe45.ToString();//D3960
                tb46.Text = recipe46.ToString();//D3961
                tb47.Text = recipe47.ToString();//D3962
                tb48.Text = recipe48.ToString();//D3963
                tb49.Text = recipe49.ToString();//D3964
                tb50.Text = recipe50.ToString();//D3965
                tb51.Text = recipe51.ToString();//D3966
                tb52.Text = recipe52.ToString();//D3967
                tb53.Text = recipe53.ToString();//D3968
                tb54.Text = recipe54.ToString();//D3969
                tb55.Text = recipe55.ToString();//D3970
                tb56.Text = recipe56.ToString();//D3971
                tb57.Text = recipe57.ToString();//D3972
                tb58.Text = recipe58.ToString();//D3973
                tb59.Text = recipe59.ToString();//D3974
                tb60.Text = recipe60.ToString();//D3975
            }
            else
            {
            }
        }


        private void SingleWrite() //데이터를 읽기 위한 쓰기
        {
            CNS_Send = true;

            ConnectSocket(); //CNS 통신 연결
            Ls_Fenet wd = new Ls_Fenet(); //헤더
            byte[] bDts2 = wd.single_Write("%DW3814", 02, 1); //개별쓰기 (변수명, 데이터 타입, 쓸 데이터값) 
            mainSock.Send(bDts2); //데이터 쓰기
            mainSock.Close();

            CNS_Send = false;
        }


        //CNS 데이터 조회 버튼
        private void PLC_Read(object sender, MouseButtonEventArgs e)
        {
            LogManager.Info("CNS 상세창에서 CNS데이터 조회 버튼 클릭", MethodBase.GetCurrentMethod().DeclaringType);

            Border clickedBorder = (Border)sender;
            clickedBorder.Background = Brushes.Gray;

            try
            {
                SingleWrite(); //현재 설정을 읽기 위해서는 D3814번에 0이 아닌 수를 넣어줘야함
                ConnectSocket(); //CNS 통신 연결
                Ls_Fenet wd = new Ls_Fenet(); //헤더

                byte[] bDts = wd.Word_continuous_Read("%DB7804", 74); //연속읽기 D3902 ~ D3975 (74개)
                
                //송신한 데이터 값 보는용도
                string Send_Data = string.Empty;
                for (int i = 0; i < bDts.Length; i++)
                {
                    Send_Data += bDts[i].ToString("x2"); //Byte값을 16진수로 변환
                }
                Console.WriteLine("PLC송신 데이터(Plc_Pop) : " + Send_Data);

                mainSock.Send(bDts); //수신데이터 요청

                LogManager.Info("CNS 상세창에서 CNS데이터 조회 요청 내용 : " + Send_Data, MethodBase.GetCurrentMethod().DeclaringType);
            }
            catch (Exception)
            {
                CNS_Send = false;
                LogManager.Info("CNS 상세창에서 CNS데이터 조회 실패", MethodBase.GetCurrentMethod().DeclaringType);
            }
            CNS_Send = false;
        }



        private void ConnectSocket()
        {
            mainSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            if (!mainSock.Connected)
            {
                int port = int.Parse(SettingJsonManager.port); //포트 2004
                try
                {
                    IAsyncResult result = mainSock.BeginConnect(SettingJsonManager.ip, port, null, null);
                    bool success = result.AsyncWaitHandle.WaitOne(1500, true);

                    if (CNS_Send)
                    {
                    }
                    else

                    {
                        if (success)//연결 성공
                        {
                            mainSock.EndConnect(result);
                            AsyncObject obj = new AsyncObject(500);
                            obj.WorkingSocket = mainSock;
                            obj.ClearBuffer();
                            mainSock.BeginReceive(obj.Buffer, 0, obj.BufferSize, 0, DataReceived, obj);
                        }
                        else //연결 실패
                        {
                            mainSock.Close();
                        }
                    }
                }
                catch (Exception e)
                {
                    mainSock.Close();
                }
            }
        }

        // 데이터 수신
        public void DataReceived(IAsyncResult ar) 
        {
            try
            {
                AsyncObject obj = (AsyncObject)ar.AsyncState;

                // 수신한 데이터 16진수로 변환
                string Receive_Data = string.Empty;
                for (int i = 0; i < obj.Buffer.Length; i++)
                {
                    Receive_Data += obj.Buffer[i].ToString("x2"); //16진수
                }

                //Word 연속읽기
                string continuous_read = string.Empty;
                string RD = Receive_Data.Substring(64); //앞에 데이터 지우고 데이터 정보부터 읽음
                Console.WriteLine("PLC수신 데이터(Plc_Pop) : " + RD);
                Console.WriteLine("테스트 값 : " + RD);

                string dummyData = "";

                this.Dispatcher.Invoke(() =>
                {
                    for (int j = 0; j < RD.Length / 4; j++)
                    {
                        string data = RD.Substring(j * 4, 4);

                        switch (j)
                        {
                            case 0: //작업모델 번호
                                dummyData = ValueConvert(data, false); break;
                                int boxType = Convert.ToInt32(dummyData);
                                switch (boxType)
                                {
                                    case 1://1합
                                        combo.SelectedIndex = 0; break;
                                    case 3://2합
                                        combo.SelectedIndex = 1; break;
                                    case 5://인쇄물
                                        combo.SelectedIndex = 2; break;
                                    case 6://H형
                                        combo.SelectedIndex = 3; break;
                                    case 7://H-Cover형
                                        combo.SelectedIndex = 4; break;
                                }

                            case 1: //작업지시 매수
                                d3903.Text = ValueConvert(data, false); break;
                            case 2: //작업지시 매수
                                d3904.Text = ValueConvert(data, false); break;
                            case 3: //박스타입
                                int boxtype = Convert.ToInt32(ValueConvert(data, false));
                                if (boxtype == 1) {combo.SelectedIndex = 0;}
                                else if (boxtype == 3) {combo.SelectedIndex = 1;}
                                else if (boxtype == 5) {combo.SelectedIndex = 2;}
                                else if (boxtype == 6) {combo.SelectedIndex = 3;}
                                else if (boxtype == 7) {combo.SelectedIndex = 4;}
                                break;
                            case 4:  //폭
                                폭.Text = ValueConvert(data, false); break; 
                            case 5:  //장                            
                                장.Text = ValueConvert(data, false); break;
                            case 6:  //고                            
                                고.Text = ValueConvert(data, false); break;
                            case 7:  //갭                            
                                d3909.Text = ValueConvert(data, true); break;
                            case 8:  //3호:앞날개(ture) 2호:앞날개
                                d3910.Text = ValueConvert(data, true); break;
                            case 9:  //3호:미미컷팅(false) //2호: 미미
                                d3911.Text = ValueConvert(data, false); break;
                            case 10: //2호기: 컷팅
                                d3912.Text = ValueConvert(data, false); break;
                            case 11: //뒷날개
                                if (SettingJsonManager.mercName.Equals("후렉소1호기")|| SettingJsonManager.mercName.Equals("인라인2호기"))
                                {
                                    d3913.Text = dummyData;
                                }
                                else
                                {
                                    d3913.Text = ValueConvert(data, true); 
                                }
                                break;
                            case 12: //
                                d3914.Text = ValueConvert(data, false); break;
                            case 13: //
                                d3915.Text = ValueConvert(data, false); break;
                            case 14: //A
                                tb1.Text = ValueConvert(data, true); break;
                            case 15: //B
                                tb2.Text = ValueConvert(data, true); break;
                            case 16: //C
                                tb3.Text = ValueConvert(data, true); break;
                            case 17: //D
                                tb4.Text = ValueConvert(data, true); break;
                            case 18: //E
                                tb5.Text = ValueConvert(data, true); break;
                            case 19: //F1
                                tb6.Text = ValueConvert(data, true); break;
                            case 20: //F2
                                tb7.Text = ValueConvert(data, true); break;
                            case 21: //G
                                tb8.Text = ValueConvert(data, true); break;
                            case 22: //O
                                tb9.Text = ValueConvert(data, true); break;
                            case 23: //P1
                                tb10.Text = ValueConvert(data, true); break;
                            case 24: //P2                             
                                tb11.Text = ValueConvert(data, true); break;
                            case 25: //P3                             
                                tb12.Text = ValueConvert(data, true); break;
                            case 26: //P4                             
                                tb13.Text = ValueConvert(data, true); break;
                            case 27: //
                                if (SettingJsonManager.mercName.Equals("후렉소1호기")|| SettingJsonManager.mercName.Equals("인라인2호기"))
                                {
                                    d3913.Text = ValueConvert(data, true);
                                    tb14.Text = ValueConvert(data, true); 
                                }
                                else
                                {
                                    tb14.Text = ValueConvert(data, true); 
                                }
                                break;
                                tb15.Text = ValueConvert(data, true); break;

                            case 28: //                          
                                tb15.Text = ValueConvert(data, true); break;
                            case 29: //                        
                                tb16.Text = ValueConvert(data, true); break;
                            case 30: //                     
                                tb17.Text = ValueConvert(data, true); break;
                            case 31: //                    
                                tb18.Text = ValueConvert(data, true); break;
                            case 32: //                   
                                tb19.Text = ValueConvert(data, true); break;
                            case 33: //                    
                                tb20.Text = ValueConvert(data, true); break;
                            case 34: //             
                                tb21.Text = ValueConvert(data, true); break;
                            case 35: //                 
                                tb22.Text = ValueConvert(data, true); break;
                            case 36: //                    
                                tb23.Text = ValueConvert(data, true); break;
                            case 37: //                    
                                tb24.Text = ValueConvert(data, true); break;
                            case 38: //                      
                                tb25.Text = ValueConvert(data, true); break;
                            case 39: //                          
                                tb26.Text = ValueConvert(data, true); break;
                            case 40: //                          
                                tb27.Text = ValueConvert(data, true); break;
                            case 41: //                          
                                tb28.Text = ValueConvert(data, true); break;
                            case 42: //                          
                                tb29.Text = ValueConvert(data, true); break;
                            case 43: //                   
                                tb30.Text = ValueConvert(data, true); break;
                            case 44: //               
                                tb31.Text = ValueConvert(data, true); break;
                            case 45: //         
                                tb32.Text = ValueConvert(data, true); break;
                            case 46: //             
                                tb33.Text = ValueConvert(data, true); break;
                            case 47: //
                                tb34.Text = ValueConvert(data, true); break;
                            case 48: //               
                                tb35.Text = ValueConvert(data, true); break;
                            case 49: //             
                                tb36.Text = ValueConvert(data, true); break;
                            case 50: //                      
                                tb37.Text = ValueConvert(data, true); break;
                            case 51: //         
                                tb38.Text = ValueConvert(data, true); break;
                            case 52: //        
                                tb39.Text = ValueConvert(data, true); break;
                            case 53: //       
                                tb40.Text = ValueConvert(data, true); break;
                            case 54: //    
                                tb41.Text = ValueConvert(data, true); break;
                            case 55: //                       
                                tb42.Text = ValueConvert(data, true); break;
                            case 56: //                       
                                tb43.Text = ValueConvert(data, true); break;
                            case 57: //               
                                tb44.Text = ValueConvert(data, true); break;
                            case 58: //             
                                tb45.Text = ValueConvert(data, true); break;
                            case 59: //              
                                tb46.Text = ValueConvert(data, true); break;
                            case 60: //      
                                tb47.Text = ValueConvert(data, true); break;
                            case 61: //
                                tb48.Text = ValueConvert(data, true); break;
                            case 62: //
                                tb49.Text = ValueConvert(data, true); break;
                            case 63: //
                                tb50.Text = ValueConvert(data, true); break;
                            case 64: //
                                tb51.Text = ValueConvert(data, true); break;
                            case 65: //
                                tb52.Text = ValueConvert(data, true); break;
                            case 66: //
                                tb53.Text = ValueConvert(data, true); break;
                            case 67: //
                                tb54.Text = ValueConvert(data, true); break;
                            case 68: //
                                tb55.Text = ValueConvert(data, true); break;
                            case 69: //
                                tb56.Text = ValueConvert(data, true); break;
                            case 70: //
                                tb57.Text = ValueConvert(data, true); break;
                            case 71: //
                                tb58.Text = ValueConvert(data, true); break;
                            case 72: //
                                tb59.Text = ValueConvert(data, true); break;
                            case 73: //
                                tb60.Text = ValueConvert(data, true); break;

                            default:
                                break;
                        }
                        //continuous_read += Convert.ToInt32(low_data + high_data, 16) + "/"; //앞 뒤 자리바꾸고 10진수 변환
                    }
                });
                //Console.WriteLine("CNS PLC수신 데이터 확인(Plc_Pop) : " + continuous_read); //수신값 확인용
            }
            catch (Exception)
            {
                MessageBox_Dialog messageBox = new MessageBox_Dialog("레시피 읽기 실패");
                _ = messageBox.ShowDialog();
                mainSock.Close();
            }
            finally
            {
                mainSock.Close();
            }

        }

        private string ValueConvert(string data, bool intIsFalse)
        {
            //앞뒤 두자리 묶음 자리변환
            int value = Convert.ToInt32(data.Substring(2, 2) + data.Substring(0, 2), 16);

            if (value > 32767)
            {
                value -= 65536;
            }

            //-값 표현
            if (intIsFalse)
            {
                return (value / 10.0).ToString("N1"); //소숫점 첫번째까지 표현
            }

            return value.ToString();
        }

        //신규 생산 버튼 (일단 사용 안함)
        private void Done(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            LogManager.Info("CNS 상세창에서 신규생산 버튼 클릭", MethodBase.GetCurrentMethod().DeclaringType);

            Border clickedBorder = (Border)sender;
            clickedBorder.Background = Brushes.Gray;

            MessageBoxResult result = System.Windows.MessageBox.Show(" 신규 생산시 기본정보만 전송됩니다.\n ※장,폭,고,갭,미미 외 세팅값은 0으로 처리됩니다." , "CNS 데이터 저장", MessageBoxButton.YesNo);

            clickedBorder.Background = Brushes.White;

            if (result == MessageBoxResult.Yes) //확인
            {
                //박스타입 선택
                int a = combo.SelectedIndex;
                int boxType = 1;
                switch (a)
                {
                    case 0:
                        boxType = 1; //1합
                        break;
                    case 1:
                        boxType = 3; //2합
                        break;
                    case 2:
                        boxType = 5; //인쇄물
                        break;
                    case 3:
                        boxType = 6; //H형
                        break;
                    case 4:
                        boxType = 7; //H-Cover형
                        break;
                }

                SettingJsonManager.plcCheck = true;
                SettingJsonManager.setCount = int.Parse(d3903.Text.ToString());
                SettingJsonManager.boxType = int.Parse(boxType.ToString());
                SettingJsonManager.jang = int.Parse(장.Text.ToString());
                SettingJsonManager.pok = int.Parse(폭.Text.ToString());
                SettingJsonManager.go = int.Parse(고.Text.ToString());
                SettingJsonManager.Gap = int.Parse(d3909.Text.ToString());
                SettingJsonManager.mimi = int.Parse(d3910.Text.ToString());
                SettingJsonManager.SaveJsonFile();



                try
                {
                    ConnectSocket(); //CNS 통신연결
                    Ls_Fenet wd = new Ls_Fenet();
                    List<int> items = new List<int>();
                    items.Add(99);  //설정 변경요구 (99로 요청하면 1로바뀜)
                    items.Add(0);   //설정 byte[] bDts = wd.continuous_Read("%DB7800", 70); 요구 (0=> 신규설정요구, 1=>데이터 설정 변경요구
                    items.Add(1); //작업 모델 번호
                    items.Add(10000); //작업지시 매수(하위)
                    items.Add(0); //작업지시 매수(상위)
                    items.Add(SettingJsonManager.boxType); //BOX TYPE
                    items.Add(SettingJsonManager.pok);  //폭
                    items.Add(SettingJsonManager.jang); //장
                    items.Add(SettingJsonManager.go);   //고
                    items.Add(SettingJsonManager.Gap);  //갭
                    items.Add(0);                       //앞날개
                    items.Add(SettingJsonManager.mimi); //미미

                    int WriteCount = items.Count() * 2;
                    byte[] bDts = wd.continuous_Write("%DB7600", WriteCount, items);
                   
                    //송신한 데이터 값 보는용도
                    string Send_Data = string.Empty;
                    for (int i = 0; i < bDts.Length; i++)
                    {
                        Send_Data += bDts[i].ToString("x2"); //Byte값을 16진수로 변환
                    }
                    Console.WriteLine("신규생산 송신데이터(Plc_Pop) : " + Send_Data);

                    //데이터 쓰기
                    mainSock.Send(bDts);
                }
                catch (Exception)
                {
                }
                finally
                {
                    mainSock.Close();
                }

            }
            else //취소
            {
            }

            Close();

        }

        //값 넣기 이벤트
        private void Input_value(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender.GetType().Name.Equals("TextBox"))
            {
                TextBox context = (TextBox)sender;
                win_KeyBoard = new Win_KeyBoard(context)
                {
                    Left = SystemParameters.WorkArea.Width - Width - 10,
                    Top = (SystemParameters.WorkArea.Height / 2) - Top
                };
                _ = win_KeyBoard.ShowDialog();
                win_KeyBoard = null;
            }
        }

        //PLC세팅 단축키 F1
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key.ToString())
            {
                case "F1":
                    Ip_input ip_Input = new Ip_input();
                    ip_Input.ShowDialog();
                    ip.Text = SettingJsonManager.ip + " / " + SettingJsonManager.port;
                    break;
                default:
                    break;
            }
        }

        //확장창 값 넣기 이벤트
        private void Input_value2(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender.GetType().Name.Equals("TextBox"))
            {
                TextBox context = (TextBox)sender;
                win_KeyBoard2 = new Win_KeyBoard2(context)
                {
                    Left = SystemParameters.WorkArea.Width - Width - 400,
                    Top = (SystemParameters.WorkArea.Height / 2) - Top
                };
                _ = win_KeyBoard2.Opacity = 1;
                _ = win_KeyBoard2.ShowDialog();
                win_KeyBoard2 = null;
            }
        }

        //화장 창 버튼
        private void ChangeButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (checkChangeButton)
            {
                changeButtonIn.Visibility = Visibility.Collapsed;
                changeButtonOut.Visibility = Visibility.Visible;
                checkChangeButton = false;
                Width = 320;
                Left = (SystemParameters.WorkArea.Width / 2) - (Width / 2);
            }
            else
            {
                changeButtonIn.Visibility = Visibility.Visible;
                changeButtonOut.Visibility = Visibility.Collapsed;
                checkChangeButton = true;
                Width = 1000;
                Left = (SystemParameters.WorkArea.Width / 2) - (Width / 2);
            }
        }

        private void Image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Border_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Border clickedBorder = (Border)sender;
            clickedBorder.Background = Brushes.White;
        }

        //닫기 버튼
        private void exit(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            LogManager.Info("CNS 상세창 종료", MethodBase.GetCurrentMethod().DeclaringType);

            SettingJsonManager.plcCheck = false;
            SettingJsonManager.SaveJsonFile();
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            changeButtonIn.Visibility = Visibility.Visible;
            changeButtonOut.Visibility = Visibility.Collapsed;
            checkChangeButton = true;
            Width = 1000;
            //Height = 650;
            Left = (SystemParameters.WorkArea.Width / 2) - (Width / 2);
        }

        private void IP_setting(object sender, MouseButtonEventArgs e)
        {
            Ip_input ip = new Ip_input();
            ip.Show();
        }

        private void TextChange_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            switch (TextChangeState)
            {
                case 0:
                    TextChangeState = 1;
                    TextChangeName.Text = "DB";
                    break;
                case 1:
                    TextChangeState = 2;
                    TextChangeName.Text = "PLC";
                    break;
                case 2:
                    TextChangeState = 0;
                    TextChangeName.Text = "명칭";
                    break;
                default:
                    TextChangeState = 0; // 예상치 못한 값에 대한 기본 처리
                    TextChangeName.Text = "명칭";
                    break;
            }
         
            textChange(TextChangeState);
            Console.WriteLine(TextChangeState);
            //TextChangeState = !TextChangeState;
        }
    }
}
