namespace mchomerEANProductFinder.Core.Models
{
    public class EANItem
    {
        public string MainCategory { get; set; }
        public string SubCategory { get; set; }
        public string Name { get; set; }
        public string NameInDetail { get; set; }
        public string Description { get; set; }
        public string Producer { get; set; }
        public string Origin { get; set; }
        public string Validation { get; set; }
        public string Ingredient { get; set; }
        public string Packing { get; set; }
        public string EANBarCode { get; set; }
    }
}
