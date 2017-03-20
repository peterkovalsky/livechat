namespace Kookaburra.Domain.Common
{
    public class Pagination
    {
        public Pagination(int size, int page)
        {
            Size = size;
            Page = page;
        }

        public int Size { get; private set; }

        public int Page { get; private set; }

        public int Skip { get { return Size * (Page - 1); } }
    }
}