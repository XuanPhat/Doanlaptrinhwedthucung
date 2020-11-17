using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Xunit;

namespace OnlineShop.Areas.Admin.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage ="Mời bạn nhập username:")]
        public string UserName { set; get; }

        [Required(ErrorMessage ="Mời bạn nhập password:")]
        public string Password { set; get; }



        public bool RememberMe { set; get; }
        



    }
}