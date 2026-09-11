using System.Collections.Generic;

namespace PopProject.Util
{
    public static class ServiceAddressManager
    {
        // 서비스별 주소를 Dictionary로 관리
        private static readonly Dictionary<(string, string), string> _serviceAddresses = new Dictionary<(string, string), string>
        {
            {( "ojbox", "ojbox"), "http://ojbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" }, //오주 1
            {( "miraebox", "mirae"), "http://newpackcombox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },   //미래 2
            {( "hnbox", "hnbox"), "http://hnbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },     //하나 3
            {( "ksbox", "ksbox"), "http://ksbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },     //광성 4
            {( "shbox", "shbox"), "http://shbox1.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },    //선호 5
            {( "ycbox", "ycbox"), "http://211.253.37.232:8088/WAS/Access_MiddleWare_CRUD.svc" },       //유창 6
            {( "kmbox", "kmbox"), "http://kmbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },     //광무 7
            {( "dybox", "dybox"), "http://dybox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },     //대양 8
            {( "swbox", "swbox"), "http://swbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },     //신우 9
            {( "erbox", "nrbox"), "http://erboxtest.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" }, //유램 10
            {( "wrbox", "wrbox"), "http://wrbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },     //우리 11
            {( "hsbox", "hsbox"), "http://hsbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },     //혜성 12
            {( "nrbox", "nrbox"), "http://erboxtest.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" }, //누리 13 
            {( "cwbox", "wrbox"), "http://wrbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },     //채움 14
            {( "sjbox", "sjbox"), "http://newpackcombox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },     //성주 15
            {( "jobox", "jobox"), "http://jobox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },     //지온 16
            {( "dubox", "dubox"), "http://dubox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },     //두성 17
            {( "dsbox", "dsbox"), "http://dsbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },     //대세 18
            {( "gybox", "grbox1"), "http://grbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },    //구룡 19 로그인시 계정
            {( "grbox", "grbox1"), "http://grbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },    //구룡 19 서비스추출 계정
            {( "hsbox1","hsbox"), "http://hsbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },    //혜성(주)20 로그인시 계정
            {( "hssbox","hsbox"), "http://hsbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },    //혜성(주)20 서비스추출 계정
            
            {( "sgbox","sgbox"), "http://sjbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },    //성지 21
            {( "khbox","khbox"), "http://khbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },     //금화 22
             
            {( "opbox" ,"opbox" ), "http://hnbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },     //원팩 23
            {( "kmsbox","kmsbox"), "http://kumsungbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },   //금성 24
            {( "npbox" ,"npbox"), "http://183.107.68.4:8088/WAS/Access_MiddleWare_CRUD.svc" },     //내포 25
            {( "sspbox","ssngsanpackbox"), "http://ssbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },    //승산 27
            {( "chsbox","chsbox"), "http://chams001.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },    //참스 28

            {( "djbox","dongju"), "http://djbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },    //동주 29
            {( "djboxtest","dongju"), "http://djbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },    //동주 29
            {( "dypbox", "dongyang"), "http://newpackcombox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },     //동양포장 30
            {( "dypboxtest", "dongyang"), "http://newpackcombox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },     //동양포장 30
            // 필요에 따라 추가
        };

        // 서비스 키로 주소 가져오기// 값 가져오기

        // serverKey로 typeKey, address 반환
        public static (string typeKey, string address) GetTypeKeyAndAddress(string serverKey)
        {
            foreach (var kv in _serviceAddresses)
            {
                if (kv.Key.Item1 == serverKey)
                    return (kv.Key.Item2, kv.Value);
            }
            return (null, null);
        }

        // 주소 동적 변경
        public static void SetAddress(string serverKey, string typeKey, string address)
        {
            _serviceAddresses[(serverKey, typeKey)] = address;
        }

    }
}