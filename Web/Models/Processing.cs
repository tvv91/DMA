namespace Web.Models
{
    /// <summary>
    /// Information about processing (declicking, etc.)
    /// </summary>
    public class Processing
    {
        public int Id { get; set; }
        public string Data { get; set; }
        /// <summary>
        /// As data is JSON where we save list of any processing operations,
        /// OperationCount shows count of operations, for example
        /// if Data = { declicking, vacuum cleaning }, OperationCount = 2
        /// </summary>
        public int OpeationCount { get; set; }
        public ICollection<TechnicalInfo> TechnicalInfos { get; set; }
    }
}
