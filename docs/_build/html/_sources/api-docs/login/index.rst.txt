

登录
========


接口描述
用于登录系统并获取accesstoken,目前token设定为每登录一次token即更新一次且不过期

请求说明
HTTP方法：POST
请求URL：http://www.writebug.site/api/login
URL参数：

Header
参数	值
Content-Type	application/json

Body中放置JSON包，其节点详情如下：

参数	是否必选	类型	说明
username	是	string	登录用户名
password	是	string	登陆密码

请求示例：
{
    	"username":"yuancong",
   	 	"password":"123456"
}


返回说明

返回类型：JSON

返回参数：

返回参数		类型	是否必须	说明
AccessToken		string	是	用户请求其他接口时的唯一身份认证标识

返回示例：
{
    	"AccessToken":"65266319-4f03-4f99-a194-5d1a29a34cfd"
}

错误码说明
Error_code	msg	说明
0001	JSON format error	发送的JSON包格式不符合要求
0002	Username contains dangerous characters	用户名包含危险字符
0003	Password contains dangerous characters	密码包含危险字符

返回示例：

{
    	"Error_code":"0003",
   	 	"msg":"Password contains dangerous characters"
}

