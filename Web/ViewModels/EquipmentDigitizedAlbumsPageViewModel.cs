namespace Web.ViewModels
{
    public class EquipmentDigitizedAlbumsPageViewModel
    {
        public List<EquipmentAlbumRowViewModel> Albums { get; set; } = [];
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public bool HasResults { get; set; }
        public string CategorySegment { get; set; } = string.Empty;
        public int EquipmentId { get; set; }
    }
}
