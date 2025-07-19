using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BaseProject.Service.EmailCheckService.Common
{
    // EmailPatterns.cs
    public static class EmailPatterns
    {
        public static class Spam
        {
            public static class Basic
            {
                public static readonly Regex[] TitlePatterns = new[]
                {
                new Regex(@"GIẢM GIÁ.*[0-9]{2,}%", RegexOptions.IgnoreCase),
                new Regex(@"CHỈ.*HÔM NAY", RegexOptions.IgnoreCase),
                new Regex(@"KHUYẾN MÃI.*KHỦNG", RegexOptions.IgnoreCase),
                new Regex(@"💰|🎉|🔥|⭐|💯"),
                new Regex(@"!!!"),
                new Regex(@"\$\$\$"),
                new Regex(@"CLICK.*NGAY", RegexOptions.IgnoreCase),
                new Regex(@"FREE|MIỄN PHÍ.*100%", RegexOptions.IgnoreCase)
            };

                public static readonly Regex[] ContentPatterns = new[]
                {
                new Regex(@"giảm giá.*[789][0-9]%", RegexOptions.IgnoreCase),
                new Regex(@"chỉ còn.*[0-9]+.*giờ", RegexOptions.IgnoreCase),
                new Regex(@"click.*ngay.*link", RegexOptions.IgnoreCase),
                new Regex(@"bit\.ly|tinyurl|short\.link"),
                new Regex(@"!!!|💰💰💰")
            };

                public static readonly Regex[] FromDomainPatterns = new[]
                {
                new Regex(@"promo|deals|sale|offer|discount", RegexOptions.IgnoreCase),
                new Regex(@"\d{2,}\.net|\.tk|\.ml")
            };
            }

            public static class Advanced
            {
                public static readonly Regex[] TitlePatterns = new[]
                {
                new Regex(@"ưu đãi.*đặc biệt", RegexOptions.IgnoreCase),
                new Regex(@"thông báo.*khuyến mãi", RegexOptions.IgnoreCase),
                new Regex(@"cơ hội.*hiếm", RegexOptions.IgnoreCase)
            };

                public static readonly Regex[] ContentPatterns = new[]
                {
                new Regex(@"số lượng có hạn", RegexOptions.IgnoreCase),
                new Regex(@"đăng ký ngay để nhận", RegexOptions.IgnoreCase),
                new Regex(@"ưu đãi dành riêng cho bạn", RegexOptions.IgnoreCase)
            };

                public static readonly Regex[] FromDomainPatterns = new[]
                {
                new Regex(@"marketing@", RegexOptions.IgnoreCase),
                new Regex(@"newsletter@", RegexOptions.IgnoreCase)
            };
            }
        }

        public static class Phishing
        {
            public static class Basic
            {
                public static readonly Regex[] TitlePatterns = new[]
                {
                new Regex(@"bảo mật|security", RegexOptions.IgnoreCase),
                new Regex(@"tài khoản.*bị.*khóa", RegexOptions.IgnoreCase),
                new Regex(@"xác (minh|nhận|thực).*khẩn", RegexOptions.IgnoreCase),
                new Regex(@"cập nhật.*ngay", RegexOptions.IgnoreCase)
            };

                public static readonly Regex[] ContentPatterns = new[]
                {
                new Regex(@"tài khoản.*sẽ bị.*khóa", RegexOptions.IgnoreCase),
                new Regex(@"xác (minh|nhận).*trong.*[0-9]+.*giờ", RegexOptions.IgnoreCase),
                new Regex(@"click.*link.*xác (minh|nhận)", RegexOptions.IgnoreCase),
                new Regex(@"cập nhật.*thông tin.*bảo mật", RegexOptions.IgnoreCase)
            };

                public static readonly Regex[] FromDomainPatterns = new[]
                {
                new Regex(@"[0-9]"),
                new Regex(@"-verification|-security|-account", RegexOptions.IgnoreCase),
                new Regex(@"\.tk|\.ml|\.ga|\.cf")
            };

                public static readonly Regex[] BrandSpoofing = new[]
                {
                new Regex(@"amaz[0o]n", RegexOptions.IgnoreCase),
                new Regex(@"g[0o]{2}gle", RegexOptions.IgnoreCase),
                new Regex(@"micr[0o]soft", RegexOptions.IgnoreCase),
                new Regex(@"payp[a@]l", RegexOptions.IgnoreCase),
                new Regex(@"faceb[0o]{2}k", RegexOptions.IgnoreCase)
            };
            }

            public static class Advanced
            {
                public static readonly Regex[] TitlePatterns = new[]
                {
                new Regex(@"thông báo từ.*phòng.*kế toán", RegexOptions.IgnoreCase),
                new Regex(@"yêu cầu xác nhận.*thanh toán", RegexOptions.IgnoreCase)
            };

                public static readonly Regex[] ContentPatterns = new[]
                {
                new Regex(@"vui lòng kiểm tra.*đính kèm", RegexOptions.IgnoreCase),
                new Regex(@"xác nhận.*giao dịch", RegexOptions.IgnoreCase),
                new Regex(@"để tiếp tục.*vui lòng", RegexOptions.IgnoreCase)
            };

                public static readonly Regex[] FromDomainPatterns = new[]
                {
                new Regex(@"no-?reply@.*\.(info|online|site)", RegexOptions.IgnoreCase)
            };
            }
        }

        public static class Suspicious
        {
            public static class Basic
            {
                public static readonly Regex[] TitlePatterns = new[]
                {
                new Regex(@"khẩn|gấp|urgent", RegexOptions.IgnoreCase),
                new Regex(@"hạn chót|deadline", RegexOptions.IgnoreCase),
                new Regex(@"quan trọng.*cập nhật", RegexOptions.IgnoreCase)
            };

                public static readonly Regex[] ContentPatterns = new[]
                {
                new Regex(@"vui lòng.*cung cấp", RegexOptions.IgnoreCase),
                new Regex(@"xác nhận.*thông tin", RegexOptions.IgnoreCase),
                new Regex(@"truy cập.*link.*bên dưới", RegexOptions.IgnoreCase),
                new Regex(@"trong vòng.*[0-9]+.*giờ", RegexOptions.IgnoreCase)
            };

                public static readonly Regex[] FromDomainPatterns = new[]
                {
                new Regex(@"\.(info|click|site|online)$", RegexOptions.IgnoreCase),
                new Regex(@"-system|-admin", RegexOptions.IgnoreCase)
            };

                public static readonly Regex[] SpellingErrors = new[]
                {
                new Regex(@"recieve", RegexOptions.IgnoreCase),
                new Regex(@"occured", RegexOptions.IgnoreCase),
                new Regex(@"loose", RegexOptions.IgnoreCase),
                new Regex(@"there account", RegexOptions.IgnoreCase)
            };
            }

            public static class Advanced
            {
                public static readonly Regex[] SubtleIndicators = new[]
                {
                new Regex(@"vui lòng phản hồi sớm", RegexOptions.IgnoreCase),
                new Regex(@"thông tin này là bảo mật", RegexOptions.IgnoreCase),
                new Regex(@"không chia sẻ email này", RegexOptions.IgnoreCase)
            };
            }
        }

        public static class Safe
        {
            public static class RequiredPatterns
            {
                public static readonly Regex[] FromDomainPatterns = new[]
                {
                new Regex(@"@fpt\.edu\.vn$"),
                new Regex(@"@[a-z]+\.edu\.vn$"),
                new Regex(@"@(gmail|outlook|yahoo)\.com$"),
                new Regex(@"@[a-z]+(corp|company|university)\.(com|vn|edu)$")
            };

                public static readonly Regex[] ProfessionalGreetings = new[]
                {
                new Regex(@"^kính (gửi|chào)", RegexOptions.IgnoreCase),
                new Regex(@"^thân gửi", RegexOptions.IgnoreCase),
                new Regex(@"^dear", RegexOptions.IgnoreCase)
            };

                public static readonly Regex[] ProfessionalClosings = new[]
                {
                new Regex(@"trân trọng", RegexOptions.IgnoreCase),
                new Regex(@"best regards", RegexOptions.IgnoreCase),
                new Regex(@"thân ái", RegexOptions.IgnoreCase),
                new Regex(@"kính thư", RegexOptions.IgnoreCase)
            };
            }

            public static class MustNotHave
            {
                public static readonly Regex[] SuspiciousWords = new[]
                {
                new Regex(@"click*here|nhấp.*vào đây", RegexOptions.IgnoreCase),
                new Regex(@"verify.*account|xác minh.*tài khoản", RegexOptions.IgnoreCase),
                new Regex(@"suspended|bị treo", RegexOptions.IgnoreCase),
                new Regex(@"act now|hành động ngay", RegexOptions.IgnoreCase)
            };
            }
        }
    }
}
