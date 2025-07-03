using System;
using System.ServiceModel;

namespace PopProject.Util
{
    public static class ServiceFactory
    {
        // 동적으로 서비스 클라이언트 생성
        public static object CreateServiceClient(string serverKey)
        {
            string ns = $"PopProject.ServiceReference_{serverKey}";
            string typeName = $"{ns}.Service_CRUDClient";
            string address = ServiceAddressManager.GetAddress(serverKey);
            var endpoint = new EndpointAddress(address);

            var type = Type.GetType(typeName);
            if (type == null)
                throw new ArgumentException("지원하지 않는 서비스입니다.", nameof(serverKey));

            // 동적으로 바인딩 생성
            var binding = new BasicHttpBinding
            {
                MaxReceivedMessageSize = 2147483647
                // 필요시 추가 설정
            };

            // 생성자: (System.ServiceModel.Channels.Binding, EndpointAddress endpoint)
            return Activator.CreateInstance(type, binding, endpoint);
        }

        // 동적으로 Return_Model 생성 및 인터페이스 반환
        public static IReturnModel CreateReturnModel(string serverKey)
        {
            string ns = $"PopProject.ServiceReference_{serverKey}";
            string typeName = $"{ns}.Return_Model";
            var type = Type.GetType(typeName);
            if (type == null)
                throw new ArgumentException("지원하지 않는 서비스입니다.", nameof(serverKey));

            return Activator.CreateInstance(type) as IReturnModel;
        }


        public static object CreateAccessModel(string serverKey)
        {
            string ns = $"PopProject.ServiceReference_{serverKey}";
            string typeName = $"{ns}.Access_Model";
            var type = Type.GetType(typeName);
            if (type == null)
                throw new ArgumentException("지원하지 않는 서비스입니다.", nameof(serverKey));

            return Activator.CreateInstance(type);
        }


        public static object CreateDBHandleModel(string serverKey, string[] 접속정보)
        {
            // 네임스페이스 및 타입명 조합
            string ns = $"PopProject.ServiceReference_{serverKey}";
            string typeName = $"{ns}.DBHandle_Model";
            var type = Type.GetType(typeName);
            if (type == null)
                throw new ArgumentException("지원하지 않는 서비스입니다.", nameof(serverKey));

            // 인스턴스 생성
            var instance = Activator.CreateInstance(type);

            // 속성 설정 (리플렉션)
            SetProperty(instance, "접속정보", 접속정보);
            SetProperty(instance, "BroadCasting", 0);
            SetProperty(instance, "BroadCastingName", string.Empty);
            SetProperty(instance, "inputPkey", "0");
            SetProperty(instance, "inputContent", string.Empty);
            SetProperty(instance, "inputProcessContent", string.Empty);

            return instance;
        }
        // 리플렉션을 이용한 속성 설정 헬퍼
        private static void SetProperty(object obj, string propertyName, object value)
        {
            var prop = obj.GetType().GetProperty(propertyName);
            if (prop != null && prop.CanWrite)
                prop.SetValue(obj, value);
        }
    }
}