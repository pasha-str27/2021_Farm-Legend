// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("LU3qgLQZcl9FlAaUYzmCkk2dk1LE4gcceojJnXeb1DEZMyKpwghPnjE5AcR4di8tkJsa1qg8GQIeIV9abN5dfmxRWlV22hTaq1FdXV1ZXF/9kav3Me4eQUOdF/frK7mB14p/J/I8s/50m6fDCEF8PPFZYpA0Uq7VDecgbaBBAf0rZHxudK6wRfB+TaEvuUebWjgU89wHnKnQfptq1kXyckRGyRgHwzkRFKy1kI7VrwOvETCc3l1TXGzeXVZe3l1dXPrytPrvPdP0naOuKQ8/pwn10Pq9DnaUhHgYnnUpoE/vjNb8zhwlhWFVDXq2rzhVIX0+BfM3KXiPgrhr3gfaNLX17YrXCTVW3Cn5k7/N/Cfzv4r8smbwE0z1PoGLTcO7n15fXVxd");
        private static int[] order = new int[] { 7,3,13,7,11,8,10,8,10,10,11,11,12,13,14 };
        private static int key = 92;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
