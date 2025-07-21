// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("ebuIB4aiQN44ADVQe+ghuDE5l0Myq/4x2zjeAVIKat6lPa3FOnQs1hQ8Na/ypgu7BhdRUYkaG8fUuthAC/4L0IpDrcsstsG5Ilmu02zQCND24CktmA8Mb+nWPGyBDm/RDwb4W2MNz+k0DgpZz4vdIlnAT00bAnfc41HS8ePe1dr5VZtVJN7S0tLW09D1LUFi7t9isb5acQyJioVAZfjkEIFLLZWzMNruaFtvglq1xuRxobMACLghqaZRqBamAHBK5FdB6q8JDvIG9ZhimCcbtWM5Mp1I5HseBrKalJGi6RsW5UIJMI/e5S49yGOM1zE0velEYH6QEiQu2Nipv9SJ9K0LKHZR0tzT41HS2dFR0tLTQTY7J8wzHncxPk7JJwEKMNHQ0tPS");
        private static int[] order = new int[] { 8,11,13,3,7,12,8,7,13,10,12,13,12,13,14 };
        private static int key = 211;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
