using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseProject.Model.Entities;
using BaseProject.Repository.EmailCheckRepository;
using BaseProject.Service.Common;
using BaseProject.Service.EmailCheckService.Common;
using BaseProject.Service.EmailCheckService.Dto;
using BaseProject.Service.EmailCheckService.ViewModels;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using Repository.Common;
using Service.Common;

namespace BaseProject.Service.EmailCheckService
{
    public class EmailCheckService : Service<Incoming_emails>, IEmailCheckService
    {
        private readonly IEmailCheckRepository _emailCheckRepository;

        public EmailCheckService(IEmailCheckRepository emailCheckRepository) : base(emailCheckRepository)
        {
            _emailCheckRepository = emailCheckRepository;
        }

        public async Task<PagedList<EmailCheckDto>> GetData(EmailCheckSearch search)
        {
            var query = from q in GetQueryable()
                        select new EmailCheckDto()
                        {
                            category = q.category,
                            content = q.content,
                            created_at = q.created_at,
                            from_email = q.from_email,
                            id = q.id,
                            received_time = q.received_time,
                            suspicious_indicators = q.suspicious_indicators,
                            title = q.title,
                            to_email = q.to_email
                        };

            if (query != null)
            {

                if (!string.IsNullOrEmpty(search.from_email))
                    query = query.Where(x => x.from_email.ToLower().Contains(search.from_email.ToLower()));

                if (!string.IsNullOrEmpty(search.content))
                    query = query.Where(x => x.content.ToLower().Contains(search.content.ToLower()));
                if (!string.IsNullOrEmpty(search.from_email))
                    query = query.Where(x => x.from_email.ToLower().Contains(search.from_email.ToLower()));
                if (!string.IsNullOrEmpty(search.to_email))
                    query = query.Where(x => x.to_email.ToLower().Contains(search.to_email.ToLower()));
                if (!string.IsNullOrEmpty(search.suspicious_indicators))
                    query = query.Where(x => x.suspicious_indicators.ToLower().Contains(search.suspicious_indicators.ToLower()));
                if (!string.IsNullOrEmpty(search.title))
                    query = query.Where(x => x.title.ToLower().Contains(search.title.ToLower()));
                if (!string.IsNullOrEmpty(search.category))
                    query = query.Where(x => x.category.ToLower().Contains(search.category.ToLower()));

            }
            query = query.OrderByDescending(x => x.created_at);

            var result = await PagedList<EmailCheckDto>.CreateAsync(query, search);
            foreach (var item in result.Items)
            {
                if (string.IsNullOrEmpty(item.category) || string.IsNullOrEmpty(item.suspicious_indicators))
                {
                    var classification = EmailClassifier.ClassifyEmail(item);
                    item.category = classification.Category;
                    item.suspicious_indicators = string.Join("; ", classification.Indicators);
                }
            }

            return result;

        }

        public Task<Incoming_emails> GetDto(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<EmailCheckResponseImportExcel> ReadExcel(IFormFile fileExcel)
        {
            var response = new EmailCheckResponseImportExcel();

            // Ensure the file is not null or empty
            if (fileExcel == null || fileExcel.Length == 0)
            {
                response.ListEmailCheck.Add(new EmailCheckImportItemDto
                {
                    RowIndex = 0,
                    Errors = new List<string> { "No file uploaded or file is empty." }
                });
                response.SoLuongThatBai = 1;
                return response;
            }

            // Set EPPlus license context (required for non-commercial use)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var stream = new MemoryStream())
            {
                await fileExcel.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Assume data is in the first sheet
                    int rowCount = worksheet.Dimension?.Rows ?? 0;
                    int colCount = worksheet.Dimension?.Columns ?? 0;

                    // Validate headers
                    var expectedHeaders = new[] { "id", "title", "content", "from_email", "to_email", "received_time", "category", "suspicious_indicators", "created_at" };
                    if (colCount < expectedHeaders.Length)
                    {
                        response.ListEmailCheck.Add(new EmailCheckImportItemDto
                        {
                            RowIndex = 1,
                            Errors = new List<string> { $"Expected {expectedHeaders.Length} columns but found {colCount}." }
                        });
                        response.SoLuongThatBai = 1;
                        return response;
                    }

                    for (int col = 1; col <= expectedHeaders.Length; col++)
                    {
                        var header = worksheet.Cells[1, col].Text?.Trim().ToLower();
                        if (header != expectedHeaders[col - 1])
                        {
                            response.ListEmailCheck.Add(new EmailCheckImportItemDto
                            {
                                RowIndex = 1,
                                Errors = new List<string> { $"Expected header '{expectedHeaders[col - 1]}' but found '{header}' in column {col}." }
                            });
                            response.SoLuongThatBai++;
                        }
                    }

                    if (response.SoLuongThatBai > 0)
                    {
                        return response; // Return early if header validation fails
                    }

                    // Read data rows (starting from row 2 to skip headers)
                    for (int row = 2; row <= rowCount; row++)
                    {
                        var item = new EmailCheckImportItemDto
                        {
                            RowIndex = row,
                            Data = new EmailCheckDto()
                        };

                        try
                        {
                            // Validate ID
                            if (!int.TryParse(worksheet.Cells[row, 1].Text, out int id) || id <= 0)
                            {
                                item.Errors.Add("Invalid or missing ID in column 'id'.");
                            }
                            else
                            {
                                item.Data.id = id;
                            }

                            // Validate Title
                            var title = worksheet.Cells[row, 2].Text?.Trim();
                            if (string.IsNullOrEmpty(title))
                            {
                                item.Errors.Add("Title is missing in column 'title'.");
                            }
                            item.Data.title = title;

                            // Validate Content
                            var content = worksheet.Cells[row, 3].Text?.Trim();
                            if (string.IsNullOrEmpty(content))
                            {
                                item.Errors.Add("Content is missing in column 'content'.");
                            }
                            item.Data.content = content;

                            // Validate FromEmail
                            var fromEmail = worksheet.Cells[row, 4].Text?.Trim();
                            if (string.IsNullOrEmpty(fromEmail) || !IsValidEmail(fromEmail))
                            {
                                item.Errors.Add("Invalid or missing email in column 'from_email'.");
                            }
                            item.Data.from_email = fromEmail;

                            // Validate ToEmail
                            var toEmail = worksheet.Cells[row, 5].Text?.Trim();
                            if (string.IsNullOrEmpty(toEmail) || !IsValidEmail(toEmail))
                            {
                                item.Errors.Add("Invalid or missing email in column 'to_email'.");
                            }
                            item.Data.to_email = toEmail;

                            // Validate ReceivedTime
                            if (!DateTime.TryParse(worksheet.Cells[row, 6].Text, out DateTime receivedTime))
                            {
                                item.Errors.Add("Invalid or missing date in column 'received_time'.");
                            }
                            item.Data.received_time = receivedTime;

                            // Validate Category
                            var category = worksheet.Cells[row, 7].Text?.Trim();
                            if (string.IsNullOrEmpty(category))
                            {
                                item.Errors.Add("Category is missing in column 'category'.");
                            }
                            item.Data.category = category;

                            // Validate SuspiciousIndicators
                            var suspiciousIndicatorsText = worksheet.Cells[row, 8].Text?.Trim();
                          
                            item.Data.suspicious_indicators = suspiciousIndicatorsText;

                            // Validate CreatedAt
                            if (!DateTime.TryParse(worksheet.Cells[row, 9].Text, out DateTime createdAt))
                            {
                                item.Errors.Add("Invalid or missing date in column 'created_at'.");
                            }
                            item.Data.created_at = createdAt;

                            // Update counts
                            if (item.IsValid)
                            {
                                response.SoLuongThanhCong++;
                            }
                            else
                            {
                                response.SoLuongThatBai++;
                            }

                            response.ListEmailCheck.Add(item);
                        }
                        catch (Exception ex)
                        {
                            item.Errors.Add($"Error processing row {row}: {ex.Message}");
                            response.ListEmailCheck.Add(item);
                            response.SoLuongThatBai++;
                        }
                    }
                }
            }

            return response;
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
