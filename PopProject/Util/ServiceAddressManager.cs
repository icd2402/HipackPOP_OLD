using System.Collections.Generic;

namespace PopProject.Util
{
    public static class ServiceAddressManager
    {
        // 서비스별 주소를 Dictionary로 관리
        private static readonly Dictionary<string, string> _serviceAddresses = new Dictionary<string, string>
        {
            { "hnbox", "http://hnbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },
            { "dsbox", "http://dsbox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },
            { "dubox", "http://dubox.iptime.org:8088/WAS/Access_MiddleWare_CRUD.svc" },
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