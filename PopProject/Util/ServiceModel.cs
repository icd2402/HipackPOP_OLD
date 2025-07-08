using System;

namespace PopProject.Util
{
    internal class ServiceModel
    {
        public static string[] 접속정보 = new string[] { SettingJsonManager.corp_id, SettingJsonManager.UserID, SettingJsonManager.UserName };

        public static object serviceClient;
        public static object accessModel;
        public static object returnModel;
        public static object dbHandleModel;

        public static object service;
        public static object returnModelObj;

        //로컬
#pragma warning disable CS0649 // 'ServiceModel.local_Service' 필드에는 할당되지 않으므로 항상 null 기본값을 사용합니다.
        public static ServiceReference_jobox.Service_CRUDClient local_Service;
#pragma warning restore CS0649 // 'ServiceModel.local_Service' 필드에는 할당되지 않으므로 항상 null 기본값을 사용합니다.
#pragma warning disable CS0649 // 'ServiceModel.local_DB_Handle_Model' 필드에는 할당되지 않으므로 항상 null 기본값을 사용합니다.
        public static ServiceReference_jobox.DBHandle_Model local_DB_Handle_Model;
#pragma warning restore CS0649 // 'ServiceModel.local_DB_Handle_Model' 필드에는 할당되지 않으므로 항상 null 기본값을 사용합니다.
#pragma warning disable CS0649 // 'ServiceModel.local_Access_Model' 필드에는 할당되지 않으므로 항상 null 기본값을 사용합니다.
        public static ServiceReference_jobox.Access_Model local_Access_Model;
#pragma warning restore CS0649 // 'ServiceModel.local_Access_Model' 필드에는 할당되지 않으므로 항상 null 기본값을 사용합니다.
#pragma warning disable CS0649 // 'ServiceModel.local_Return_Model' 필드에는 할당되지 않으므로 항상 null 기본값을 사용합니다.
        public static ServiceReference_jobox.Return_Model local_Return_Model;
#pragma warning restore CS0649 // 'ServiceModel.local_Return_Model' 필드에는 할당되지 않으므로 항상 null 기본값을 사용합니다.

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
            serverIndex = serverIndex.Equals("cwbox") ? "wrbox" : serverIndex;
            //서비스 클라이언트 생성
            serviceClient = ServiceFactory.CreateServiceClient(serverIndex);
            accessModel = ServiceFactory.CreateAccessModel(serverIndex);
            returnModel = ServiceFactory.CreateReturnModel(serverIndex);
            dbHandleModel = ServiceFactory.CreateDBHandleModel(serverIndex, 접속정보);

        }

        private static void SetProperty(object obj, string propertyName, object value)
        {
            var prop = obj.GetType().GetProperty(propertyName);
            if (prop != null && prop.CanWrite)
                prop.SetValue(obj, value);
        }

        public static ReturnModel GetData(string serverIndex, string SP명, string[] 파라미터, object[] 값, int 갱신)
        {
            ReturnModel returnModel = new ReturnModel();

            // 네임스페이스 조합
            string ns = $"PopProject.ServiceReference_{serverIndex}";

            // 1. 동적으로 인스턴스 생성
            service = ServiceFactory.CreateServiceClient(serverIndex);
            accessModel = ServiceFactory.CreateAccessModel(serverIndex);
            returnModelObj = ServiceFactory.CreateReturnModel(serverIndex);
            dbHandleModel = ServiceFactory.CreateDBHandleModel(serverIndex, 접속정보);

            // 2. Access_Model 속성 세팅
            SetProperty(accessModel, "SP명", SP명);
            SetProperty(accessModel, "파라미터", 파라미터);
            SetProperty(accessModel, "값", 값);

            // 3. DBHandle_Model의 프로시저 속성에 Access_Model 배열 할당
            var accessModelArray = Array.CreateInstance(accessModel.GetType(), 1);
            accessModelArray.SetValue(accessModel, 0);
            SetProperty(dbHandleModel, "프로시저", accessModelArray);
            SetProperty(dbHandleModel, "BroadCasting", 0);

            // 4. 서비스의 "갱신" 또는 "조회" 메서드 호출
            var methodName = (갱신 == 1) ? "갱신" : "조회";
            var method = service.GetType().GetMethod(methodName);
            if (method == null)
                throw new InvalidOperationException($"{methodName} 메서드를 찾을 수 없습니다.");

            var result = (dynamic)method.Invoke(service, (갱신 == 1) ? new object[] { dbHandleModel, 갱신 } : new object[] { dbHandleModel });

            // 5. 결과값 매핑
            returnModel.결과 = result.결과;
            returnModel.리턴값 = result.Retrun값;

            return returnModel;


        }


        public static ReturnModel GetData(string serverIndex, string SP명, string[] 파라미터, object[] 값, int 갱신, string parameter)
        {
            ReturnModel returnModel = new ReturnModel();

            try
            {
                // 1. 동적으로 인스턴스 생성
                service = ServiceFactory.CreateServiceClient(serverIndex);
                accessModel = ServiceFactory.CreateAccessModel(serverIndex);
                dbHandleModel = ServiceFactory.CreateDBHandleModel(serverIndex, 접속정보);

                // 2. Access_Model 속성 세팅
                SetProperty(accessModel, "SP명", SP명);
                SetProperty(accessModel, "파라미터", 파라미터);
                SetProperty(accessModel, "값", 값);

                // 3. DBHandle_Model의 프로시저 속성에 Access_Model 배열 할당
                var accessModelArray = Array.CreateInstance(accessModel.GetType(), 1);
                accessModelArray.SetValue(accessModel, 0);
                SetProperty(dbHandleModel, "프로시저", accessModelArray);

                // 4. BroadCasting 관련 속성 세팅
                SetProperty(dbHandleModel, "BroadCasting", 1);
                SetProperty(dbHandleModel, "BroadCastingName", "생산입력완료");
                SetProperty(dbHandleModel, "inputPkey", "14201");
                SetProperty(dbHandleModel, "inputContent", parameter);

                // 5. 서비스의 "갱신" 또는 "조회" 메서드 호출
                var methodName = (갱신 == 1) ? "갱신" : "조회";
                var method = service.GetType().GetMethod(methodName);
                if (method == null)
                    throw new InvalidOperationException($"{methodName} 메서드를 찾을 수 없습니다.");

                var result = (dynamic)method.Invoke(service, (갱신 == 1) ? new object[] { dbHandleModel, 갱신 } : new object[] { dbHandleModel });

                // 6. 결과값 매핑
                returnModel.결과 = result.결과;
                returnModel.리턴값 = result.Retrun값;
            }
            catch (Exception)
            {
                returnModel.결과 = 0;
                returnModel.리턴값 = "";
            }
            return returnModel;

        }
    }
}
