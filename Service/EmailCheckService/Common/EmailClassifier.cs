using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BaseProject.Service.EmailCheckService.Dto;

namespace BaseProject.Service.EmailCheckService.Common
{
    // EmailClassifier.cs
    public class EmailClassifier
    {
        public class ClassificationResult
        {
            public string Category { get; set; }
            public double Confidence { get; set; }
            public List<string> Indicators { get; set; }
            public string Level { get; set; }
        }

        public class PhishingCheckResult
        {
            public bool IsPhishing { get; set; }
            public double Confidence { get; set; }
            public List<string> Indicators { get; set; }
            public string Level { get; set; }
        }

        public class SpamCheckResult
        {
            public bool IsSpam { get; set; }
            public double Confidence { get; set; }
            public List<string> Indicators { get; set; }
            public string Level { get; set; }
        }

        public class SuspiciousCheckResult
        {
            public bool IsSuspicious { get; set; }
            public double Confidence { get; set; }
            public List<string> Indicators { get; set; }
            public string Level { get; set; }
        }

        public class SafeCheckResult
        {
            public bool IsSafe { get; set; }
            public double Confidence { get; set; }
        }

        public static ClassificationResult ClassifyEmail(EmailCheckDto email)
        {
            var result = new ClassificationResult
            {
                Category = "An toàn",
                Confidence = 0,
                Indicators = new List<string>(),
                Level = "basic"
            };

            var phishingCheck = CheckPhishing(email.title, email.content, email.from_email);
            if (phishingCheck.IsPhishing)
            {
                return new ClassificationResult
                {
                    Category = "Giả mạo",
                    Confidence = phishingCheck.Confidence,
                    Indicators = phishingCheck.Indicators,
                    Level = phishingCheck.Level
                };
            }

            var spamCheck = CheckSpam(email.title, email.content, email.from_email);
            if (spamCheck.IsSpam)
            {
                return new ClassificationResult
                {
                    Category = "Spam",
                    Confidence = spamCheck.Confidence,
                    Indicators = spamCheck.Indicators,
                    Level = spamCheck.Level
                };
            }

            var suspiciousCheck = CheckSuspicious(email.title, email.content, email.from_email);
            if (suspiciousCheck.IsSuspicious)
            {
                return new ClassificationResult
                {
                    Category = "Nghi ngờ",
                    Confidence = suspiciousCheck.Confidence,
                    Indicators = suspiciousCheck.Indicators,
                    Level = suspiciousCheck.Level
                };
            }

            var safeCheck = CheckSafe(email.title, email.content, email.from_email);
            if (safeCheck.IsSafe)
            {
                return new ClassificationResult
                {
                    Category = "An toàn",
                    Confidence = safeCheck.Confidence,
                    Indicators = new List<string> { "Email từ nguồn tin cậy", "Không có dấu hiệu đáng ngờ" },
                    Level = "basic"
                };
            }

            return new ClassificationResult
            {
                Category = "Nghi ngờ",
                Confidence = 0.3,
                Indicators = new List<string> { "Không thể xác định rõ ràng" },
                Level = "basic"
            };
        }

        private static PhishingCheckResult CheckPhishing(string title, string content, string fromEmail)
        {
            var indicators = new List<string>();
            var matchCount = 0;
            var level = "basic";
            var domain = fromEmail.Contains("@") ? fromEmail.Split('@')[1] : "";

            foreach (var pattern in EmailPatterns.Phishing.Basic.BrandSpoofing)
            {
                if (pattern.IsMatch(fromEmail) || pattern.IsMatch(content))
                {
                    indicators.Add("Giả mạo thương hiệu với ký tự số thay chữ");
                    matchCount += 2;
                }
            }

            foreach (var pattern in EmailPatterns.Phishing.Basic.FromDomainPatterns)
            {
                if (pattern.IsMatch(domain))
                {
                    indicators.Add($"Domain đáng ngờ: {domain}");
                    matchCount += 2;
                }
            }

            foreach (var pattern in EmailPatterns.Phishing.Basic.TitlePatterns)
            {
                if (pattern.IsMatch(title))
                {
                    indicators.Add("Tiêu đề có dấu hiệu phishing");
                    matchCount++;
                }
            }

            foreach (var pattern in EmailPatterns.Phishing.Basic.ContentPatterns)
            {
                if (pattern.IsMatch(content))
                {
                    indicators.Add("Nội dung yêu cầu xác minh khẩn cấp");
                    matchCount++;
                }
            }

            if (matchCount < 3)
            {
                level = "advanced";
                var accountingPattern = new Regex(@"phòng.*kế.*toán|accounting", RegexOptions.IgnoreCase);
                if (accountingPattern.IsMatch(fromEmail))
                {
                    indicators.Add("Giả danh phòng ban nội bộ");
                    matchCount++;
                }
            }

            var confidence = Math.Min(matchCount * 0.25, 1.0);
            return new PhishingCheckResult
            {
                IsPhishing = matchCount >= 2,
                Confidence = confidence,
                Indicators = indicators,
                Level = level
            };
        }

        private static SpamCheckResult CheckSpam(string title, string content, string fromEmail)
        {
            var indicators = new List<string>();
            var matchCount = 0;
            var level = "basic";
            var domain = fromEmail.Contains("@") ? fromEmail.Split('@')[1] : "";

            foreach (var pattern in EmailPatterns.Spam.Basic.TitlePatterns)
            {
                if (pattern.IsMatch(title))
                {
                    if (new Regex(@"[0-9]{2,}%", RegexOptions.IgnoreCase).IsMatch(title))
                        indicators.Add("Quảng cáo giảm giá lớn");
                    else if (new Regex(@"!!!", RegexOptions.IgnoreCase).IsMatch(title))
                        indicators.Add("Sử dụng nhiều dấu chấm than");
                    else if (new Regex(@"💰|🎉|🔥").IsMatch(title))
                        indicators.Add("Sử dụng emoji spam");
                    matchCount++;
                }
            }

            foreach (var pattern in EmailPatterns.Spam.Basic.ContentPatterns)
            {
                if (pattern.IsMatch(content))
                {
                    if (new Regex(@"bit\.ly|tinyurl").IsMatch(content))
                    {
                        indicators.Add("Chứa link rút gọn đáng ngờ");
                        matchCount += 2;
                    }
                    else
                    {
                        indicators.Add("Nội dung spam điển hình");
                        matchCount++;
                    }
                }
            }

            foreach (var pattern in EmailPatterns.Spam.Basic.FromDomainPatterns)
            {
                if (pattern.IsMatch(domain))
                {
                    indicators.Add("Domain spam thương mại");
                    matchCount++;
                }
            }

            if (matchCount < 2)
            {
                level = "advanced";
                foreach (var pattern in EmailPatterns.Spam.Advanced.ContentPatterns)
                {
                    if (pattern.IsMatch(content))
                    {
                        indicators.Add("Marketing email với trigger tâm lý");
                        matchCount++;
                    }
                }
            }

            var confidence = Math.Min(matchCount * 0.3, 1.0);
            return new SpamCheckResult
            {
                IsSpam = matchCount >= 2,
                Confidence = confidence,
                Indicators = indicators,
                Level = level
            };
        }

        private static SuspiciousCheckResult CheckSuspicious(string title, string content, string fromEmail)
        {
            var indicators = new List<string>();
            var matchCount = 0;
            var level = "basic";
            var domain = fromEmail.Contains("@") ? fromEmail.Split('@')[1] : "";

            foreach (var pattern in EmailPatterns.Suspicious.Basic.TitlePatterns)
            {
                if (pattern.IsMatch(title))
                {
                    indicators.Add("Tạo áp lực thời gian trong tiêu đề");
                    matchCount++;
                }
            }

            foreach (var pattern in EmailPatterns.Suspicious.Basic.ContentPatterns)
            {
                if (pattern.IsMatch(content))
                {
                    if (new Regex(@"trong vòng.*[0-9]+.*giờ", RegexOptions.IgnoreCase).IsMatch(content))
                        indicators.Add("Yêu cầu hành động trong thời gian ngắn");
                    else if (new Regex(@"vui lòng.*cung cấp", RegexOptions.IgnoreCase).IsMatch(content))
                        indicators.Add("Yêu cầu cung cấp thông tin");
                    else
                        indicators.Add("Nội dung có dấu hiệu đáng ngờ");
                    matchCount++;
                }
            }

            foreach (var pattern in EmailPatterns.Suspicious.Basic.FromDomainPatterns)
            {
                if (pattern.IsMatch(domain))
                {
                    indicators.Add($"Domain không chính thức: {domain}");
                    matchCount++;
                }
            }

            var fullText = title + " " + content;
            foreach (var pattern in EmailPatterns.Suspicious.Basic.SpellingErrors)
            {
                if (pattern.IsMatch(fullText))
                {
                    indicators.Add("Có lỗi chính tả đáng ngờ");
                    matchCount++;
                    break;
                }
            }

            var confidence = Math.Min(matchCount * 0.35, 1.0);
            return new SuspiciousCheckResult
            {
                IsSuspicious = matchCount >= 2,
                Confidence = confidence,
                Indicators = indicators,
                Level = level
            };
        }

        private static SafeCheckResult CheckSafe(string title, string content, string fromEmail)
        {
            var safeScore = 0;
            var domain = fromEmail.Contains("@") ? fromEmail.Split('@')[1] : "";

            foreach (var pattern in EmailPatterns.Safe.RequiredPatterns.FromDomainPatterns)
            {
                if (pattern.IsMatch(fromEmail))
                {
                    safeScore += 2;
                    break;
                }
            }

            foreach (var pattern in EmailPatterns.Safe.RequiredPatterns.ProfessionalGreetings)
            {
                if (pattern.IsMatch(content))
                {
                    safeScore++;
                    break;
                }
            }

            foreach (var pattern in EmailPatterns.Safe.RequiredPatterns.ProfessionalClosings)
            {
                if (pattern.IsMatch(content))
                {
                    safeScore++;
                    break;
                }
            }

            var hasSuspiciousWords = false;
            foreach (var pattern in EmailPatterns.Safe.MustNotHave.SuspiciousWords)
            {
                if (pattern.IsMatch(content) || pattern.IsMatch(title))
                {
                    hasSuspiciousWords = true;
                    break;
                }
            }

            var isSafe = safeScore >= 3 && !hasSuspiciousWords;
            var confidence = isSafe ? Math.Min(safeScore * 0.25, 1.0) : 0;
            return new SafeCheckResult
            {
                IsSafe = isSafe,
                Confidence = confidence
            };
        }
    }
}
