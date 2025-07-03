namespace PopProject.Util
{
    // 예시: Return_Model에 적용할 공통 인터페이스
    public interface IReturnModel
    {
        int ResultCode { get; set; }
        string ResultMessage { get; set; }
    }
}