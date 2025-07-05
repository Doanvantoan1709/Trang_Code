using BaseProject.Service.Dto;

namespace BaseProject.Service.GioiHanDiaChiMangService.Dto
{
    public class GioiHanDiaChiMangSearch : SearchBase
    {
        public string? IPAddress {get; set; }
		public bool? Allowed {get; set; }
    }
}
