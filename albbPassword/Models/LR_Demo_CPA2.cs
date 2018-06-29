using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace albbPassword.Models
{
    public class LR_Demo_CPA2
    {
        public LR_Demo_CPA2() {
            F_CPAId = "";
            F_ProductName = "";
            F_QueryUrl = "";
            F_QueryName = "";
            F_QueryPwd = "";
            F_QueryPwd2 = "";
        }

        public string F_CPAId { get; set; }
        public string F_ProductName { get; set; }
        public string F_QueryUrl { get; set; }
        public string F_QueryName { get; set; }
        public string F_QueryPwd { get; set; }
        public string F_QueryPwd2 { get; set; }
    }
}