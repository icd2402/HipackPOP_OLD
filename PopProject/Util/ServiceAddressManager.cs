using System.Collections.Generic;

namespace PopProject.Util
{
    public static class ServiceAddressManager
    {
        // 서비스별 주소를 Dictionary로 관리
        private static readonly Dictionary<string, string> _serviceAddresses = new Dictionary<string, string>
        {
            { "ojbox", "http://ojbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },     //오주 1
            { "mirae", "http://miraebox.float-zone.com/WAS/Access_MiddleWare_CRUD.svc" },   //미래 2
            { "hnbox", "http://hnbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },     //하나 3
            { "ksbox", "http://ksbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },     //광성 4
            { "shbox", "http://shbox1.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },    //선호 5
            { "ycbox", "http://211.253.37.232:8088/WAS/Access_MiddleWare_CRUD.svc" },       //유창 6
            { "kmbox", "http://kmbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },     //광무 7
            { "dybox", "http://dybox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },     //대양 8
            { "swbox", "http://swbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },     //신우 9
            { "erbox", "http://erboxtest.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" }, //유램 10
            { "wrbox", "http://wrbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },     //우리 11
            { "hsbox", "http://hsbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },     //혜성 12
            { "nrbox", "http://erboxtest.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" }, //누리 13 
            { "cwbox", "http://wrbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },     //채움 14
            { "sjbox", "http://newpackcombox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },     //성주 15
            { "jobox", "http://jobox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },     //지온 16
            { "dubox", "http://dubox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },     //두성 17
            { "dsbox", "http://dsbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },     //대세 18
            { "grbox1", "http://grbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },    //구룡 19
            { "hssbox", "http://hsbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },    //혜성(주)20

            { "sgbox", "http://newpackcombox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },    //성지 21
            { "khbox", "http://khbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },     //금화 22

            { "opbox", "http://hnbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },     //원팩 23
            { "kmsbox", "http://kmsbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },   //금성 24
            { "npbox", "http://npbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },     //내포 25
            { "sspbox", "http://newpackcombox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },    //승산 27
            { "chsbox", "http://newpackcombox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },    //참스 28
            // 필요에 따라 추가
        };

        // 서비스 키로 주소 가져오기
        public static string GetAddress(string serviceKey)
        {
            return _serviceAddresses.TryGetValue(serviceKey, out var address) ? address : null;
        }

        // 주소 동적 변경
        public static void SetAddress(string serviceKey, string address)
        {
            _serviceAddresses[serviceKey] = address;
        }
    }
}