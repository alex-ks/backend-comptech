using Xunit;


namespace Comptech.Backend.Service.Test
{
    public abstract class GenericEqualityTest<T>
    {
        protected T a;
        protected T b;
        protected T c;

        [Fact]
        public void TestEqual()
        {
            Assert.Equal(a, b);
        }

        [Fact]
        public void TestEqualReflection()
        {
            Assert.Equal(a, a);
        }


        [Fact]
        public void TestEqualSymmetric()
        {
            Assert.Equal(a.Equals(b), b.Equals(a));
        }

        [Fact]
        public void TestEqualTransitive()
        {
            Assert.Equal(a.Equals(b) && b.Equals(c) && a.Equals(c), true);
        }

        [Fact]
        public void TestEqualNull()
        {
            Assert.Equal(a.Equals(null), false);
        }

        [Fact]
        public void TestGetHashCode()
        {
            int ahash = a.GetHashCode();
            int bhash = b.GetHashCode();
            Assert.Equal(a.Equals(b) && (a.GetHashCode() == b.GetHashCode()), true);
        }

    }
}