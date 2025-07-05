using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseProject.Service.Constant.BaoCao
{
    public class KieuTongHopConstant
    { 
        public const string MotNhiemVuHoanThanh = "MotNhiemVuHoanThanh"; 
        public const string TatCaNhiemVuHoanThanh = "TatCaNhiemVuHoanThanh";
    }
    public class KieuTongHopConstantV2
    {
        [DisplayName("Một nhiệm vụ hoàn thành")]
        public static string MotNhiemVuHoanThanh => "MotNhiemVuHoanThanh";
        [DisplayName("Tất nhiệm vụ hoàn thành")]
        public static string TatCaNhiemVuHoanThanh => "TatCaNhiemVuHoanThanh";
    }
}
