using System;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1
{
    public class DerOctetString
        : Asn1OctetString
    {
        public static readonly DerOctetString Empty = new DerOctetString(EmptyOctets);

        public static DerOctetString FromContents(byte[] contents)
        {
            if (contents == null)
                throw new ArgumentNullException(nameof(contents));

            return contents.Length < 1 ? Empty : new DerOctetString(Arrays.Clone(contents));
        }

        public static DerOctetString FromContentsOptional(byte[] contents)
        {
            if (contents == null)
                return null;

            return contents.Length < 1 ? Empty : new DerOctetString(Arrays.Clone(contents));
        }

        internal static DerOctetString WithContents(byte[] contents)
        {
            return contents.Length < 1 ? Empty : new DerOctetString(contents);
        }

        /// <param name="contents">The octets making up the octet string.</param>
        public DerOctetString(byte[] contents)
			: base(contents)
        {
        }

        public DerOctetString(IAsn1Convertible obj)
            : this(obj.ToAsn1Object())
        {
        }

        public DerOctetString(Asn1Encodable obj)
            : base(obj.GetEncoded(Der))
        {
        }

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal DerOctetString(ReadOnlySpan<byte> contents)
            : base(contents)
        {
        }
#endif

        internal override IAsn1Encoding GetEncoding(int encoding)
        {
            return new PrimitiveEncoding(Asn1Tags.Universal, Asn1Tags.OctetString, contents);
        }

        internal override IAsn1Encoding GetEncodingImplicit(int encoding, int tagClass, int tagNo)
        {
            return new PrimitiveEncoding(tagClass, tagNo, contents);
        }

        internal sealed override DerEncoding GetEncodingDer()
        {
            return new PrimitiveDerEncoding(Asn1Tags.Universal, Asn1Tags.OctetString, contents);
        }

        internal sealed override DerEncoding GetEncodingDerImplicit(int tagClass, int tagNo)
        {
            return new PrimitiveDerEncoding(tagClass, tagNo, contents);
        }

        internal static void Encode(Asn1OutputStream asn1Out, byte[] buf, int off, int len)
		{
            asn1Out.WriteIdentifier(Asn1Tags.Universal, Asn1Tags.OctetString);
            asn1Out.WriteDL(len);
            asn1Out.Write(buf, off, len);
        }

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static void Encode(Asn1OutputStream asn1Out, ReadOnlySpan<byte> buf)
        {
            asn1Out.WriteIdentifier(Asn1Tags.Universal, Asn1Tags.OctetString);
            asn1Out.WriteDL(buf.Length);
            asn1Out.Write(buf);
        }

        internal static void Encode(Asn1OutputStream asn1Out, ReadOnlySpan<byte> buf1, ReadOnlySpan<byte> buf2)
        {
            asn1Out.WriteIdentifier(Asn1Tags.Universal, Asn1Tags.OctetString);
            asn1Out.WriteDL(buf1.Length + buf2.Length);
            asn1Out.Write(buf1);
            asn1Out.Write(buf2);
        }
#endif
    }
}
