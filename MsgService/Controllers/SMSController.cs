using System;
using Microsoft.AspNetCore.Mvc;

namespace MsgService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SMSController : ControllerBase
    {
        //发请求，报文体为{phoneNum:"110",msg:"aaaaaaaaaaaaa"}，
        [HttpPost(nameof(Send_MI))]
        public string Send_MI(dynamic model)
        {
            Console.WriteLine($"通过小米短信接口向{model.phoneNum}发送短信{model.msg}");
            return "success";
        }
        [HttpPost(nameof(Send_LX))]
        public void Send_LX(SendSMSRequest model)
        {
            Console.WriteLine($"通过联想短信接口向{model.PhoneNum}发送短信{model.Msg}");
        }
        [HttpPost(nameof(Send_HW))]
        public void Send_HW(SendSMSRequest model)
        {
            Console.WriteLine($"通过华为短信接口向{model.PhoneNum}发送短信{model.Msg}");
        }
    }
    public class SendSMSRequest
    {
        public string PhoneNum { get; set; }
        public string Msg { get; set; }
    }
}