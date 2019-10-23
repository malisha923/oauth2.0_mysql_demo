using Newtonsoft.Json;

namespace oauth2._0_mysql_demo
{
    public class ApiResult<T>
    {
        /// <summary>
        /// 是否获取成果
        /// </summary>
        public bool Success { set; get; }

        /// <summary>
        /// 代码
        /// </summary>
        public int Code { set; get; }

        /// <summary>
        /// 结果
        /// </summary>
        // [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public T Result { set; get; }

        /// <summary>
        /// 错误信息，支持HTML
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Error { set; get; }

        public ApiResult(T result, bool success = true)
        {
            Result = result;
            Success = success;
        }

        public ApiResult() { }

    }

    public static class ApiResultEx
    {
        public static ApiResult<T> ToApiResult<T>(this T t, bool success = true, int code = 0, string error = null)
        {
            return new ApiResult<T>(t, success) { Code = code, Error = error };
        }
    }

}