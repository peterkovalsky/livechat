namespace Kookaburra.Domain.Model
{
    public class Setting
    {
        public int AccountId { get; set; }

        public virtual Account Account { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}