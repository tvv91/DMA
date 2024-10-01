namespace Web.Models
{
    public class CartrigeManufacturer
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public ICollection<Cartrige> Cartriges { get; set; }
    }
}
